using Infrastructure.Persistence.EntityFramework;
using LogicaNegocio.Dominio.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;


namespace Infrastructure.Cache
{
    public class OutboxProcessorHostedService : BackgroundService

    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OutboxProcessorHostedService> _logger;

        public OutboxProcessorHostedService(
            IServiceScopeFactory scopeFactory,
            ILogger<OutboxProcessorHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<GestorSalasContext>();
                    var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                    var pendingMessages = await dbContext.OutboxMessages
                        .Where(m => m.ProcessedOn == null)
                        .OrderBy(m => m.OccurredOn)
                        .Take(50)
                        .ToListAsync(stoppingToken);

                    foreach (var message in pendingMessages)
                    {
                        try
                        {
                            await HandleMessageAsync(message, notificationService, stoppingToken);
                            message.MarkProcessed();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error procesando outbox {OutboxId}", message.Id);
                            message.MarkFailed(ex.Message);
                        }
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error general en OutboxProcessorHostedService");
                }

                // Espera entre ciclos
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private Task HandleMessageAsync(
            OutboxMessage message,
            INotificationService notificationService,
            CancellationToken ct)
        {
            switch (message.Type)
            {
                case nameof(ReservationCreatedDomainEvent):
                    var evt = JsonSerializer.Deserialize<ReservationCreatedDomainEvent>(message.Payload)!;
                    return notificationService.NotifyReservationCreatedAsync(evt, ct);

                // otros eventos: ReservationCancelledDomainEvent, etc.
                default:
                    // Podés loguear warning de tipo desconocido
                    return Task.CompletedTask;
            }
        }
    }
}
