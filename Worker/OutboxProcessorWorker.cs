using LogicaAplicacion.ServiceInterfaces;
using LogicaNegocio.InterfacesRepositorios;
using StackExchange.Redis;

namespace Worker
{
    public class OutboxProcessorWorker : BackgroundService
    {
        private readonly ILogger<OutboxProcessorWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConnectionMultiplexer _redis;

        public OutboxProcessorWorker(
            ILogger<OutboxProcessorWorker> logger,
            IServiceProvider serviceProvider,
            IConnectionMultiplexer redis)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _redis = redis;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OutboxProcessorWorker iniciado.");

            var db = _redis.GetDatabase();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var outboxRepo = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        // 1. Leer mensajes pendientes
                        var pending = await outboxRepo.GetUnprocessedAsync(20, stoppingToken);

                        if (pending.Count == 0)
                        {
                            // Nada que hacer, dormir un rato
                            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                            continue;
                        }

                        foreach (var message in pending)
                        {
                            try
                            {
                                // 2. Publicar en Redis (ej: lista o pub/sub)
                                // Opción lista:
                                await db.ListRightPushAsync("notifications:reservations", message.Payload);

                                // Opción Pub/Sub:
                                // var sub = _redis.GetSubscriber();
                                // await sub.PublishAsync("notifications:reservations", message.Payload);

                                // 3. Marcar como procesado
                                message.MarkProcessed();
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error enviando mensaje Outbox {Id} a Redis", message.Id);
                                message.MarkFailed(ex.Message);
                            }
                        }

                        // 4. Guardar cambios de procesado / errores
                        await unitOfWork.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en el ciclo del OutboxProcessorWorker");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
            }

            _logger.LogInformation("OutboxProcessorWorker detenido.");
        }
    }
}
