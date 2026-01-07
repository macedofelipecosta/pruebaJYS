using API_GestionDeSalas_Jaume_Sere.Middleware;
using AutoMapper;
using DTOs.Mappers;
using Infrastructure.Cache;
using Infrastructure.Email;
using Infrastructure.Graph;
using Infrastructure.Identity.Ldap;
using Infrastructure.LogicaDatos.EntityFramework;
using Infrastructure.LogicaDatos.EntityFramework.Repositorios;
using LogicaAplicacion.ServiceInterfaces;
using LogicaAplicacion.ServiceInterfaces.Graph;
using LogicaAplicacion.Services;
using LogicaNegocio.InterfacesRepositorios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using System.Threading.RateLimiting;


namespace API_GestionDeSalas_Jaume_Sere
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddHealthChecks();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API Gestión de Salas",
                    Version = "v1",
                    Description = "API REST para la gestión de salas, reservas y equipamiento corporativo.",
                    Contact = new OpenApiContact
                    {
                        Name = "Equipo Jaume & Seré",
                        Email = "soporte@jaumesere.com"
                    }
                });

                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            });

            builder.Services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: ipAddress,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 100,
                            Window = TimeSpan.FromMinutes(1),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        });
                });

                options.AddPolicy("AuditPolicy", context =>
                {
                    var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: ipAddress,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromMinutes(1),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        });
                });

                options.AddPolicy("SwaggerPolicy", context =>
                {
                    var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: ipAddress,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 200,
                            Window = TimeSpan.FromMinutes(1),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        });
                });

                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                    var response = new
                    {
                        error = "TooManyRequests",
                        message = "Se ha excedido el límite de solicitudes. Por favor, intente más tarde.",
                        retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter)
                            ? retryAfter.ToString()
                            : "60 segundos"
                    };

                    await context.HttpContext.Response.WriteAsJsonAsync(response, cancellationToken);
                };
            });

            #region Autentication JwtBearer
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"))
                .EnableTokenAcquisitionToCallDownstreamApi()
                .AddMicrosoftGraph(builder.Configuration.GetSection("Graph"))
                .AddInMemoryTokenCaches();


            builder.Services.AddAuthorization();

            #endregion

            #region Conexion DB

            // Cadena de conexión: primero variables de entorno, luego configuración
            var connectionString =
                Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                ?? Environment.GetEnvironmentVariable("DefaultConnection")
                ?? builder.Configuration["ConnectionStrings:DefaultConnection"];

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Falta la cadena de conexión. Configure 'ConnectionStrings__DefaultConnection' o 'DefaultConnection'.");
            }

            builder.Services.AddDbContextPool<GestorSalasContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            #endregion

            #region Conexion SMTP
            var smtpSection = builder.Configuration.GetSection("SmtpSettings");
            builder.Services.Configure<SmtpSettings>(smtpSection);
            builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();

            // Opcional: validar que haya datos de SMTP al arrancar (sobre todo en producción)
            var smtpSettings = smtpSection.Get<SmtpSettings>();
            if (builder.Environment.IsProduction())
            {
                if (smtpSettings is null ||
                    string.IsNullOrWhiteSpace(smtpSettings.Host) ||
                    string.IsNullOrWhiteSpace(smtpSettings.User) ||
                    string.IsNullOrWhiteSpace(smtpSettings.Password))
                {
                    throw new InvalidOperationException(
                        "SmtpSettings no está configurado correctamente. " +
                        "Verificá las variables de entorno en Azure (SmtpSettings:Host, User, Password, etc.).");
                }
            }

            #endregion

            builder.Services.AddHttpContextAccessor();
            builder.Services.Configure<LdapOptions>(builder.Configuration.GetSection("Ldap"));
            builder.Services.AddScoped<ILdapClient, LdapClient>();
            builder.Services.AddScoped<ILdapAuthenticationService, LdapAuthenticationService>();

            builder.Services.AddScoped<IParameterService, ParameterService>();
            builder.Services.AddScoped<IParameterRepository, ParameterRepository>();

            builder.Services.AddScoped<ILocationRepository, LocationRepository>();
            builder.Services.AddScoped<ILocationService, LocationService>();

            builder.Services.AddScoped<IRoomRepository, RoomRepository>();
            builder.Services.AddScoped<IRoomService, RoomService>();

            builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
            builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
            builder.Services.AddScoped<IEquipmentService, EquipmentService>();

            builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
            builder.Services.AddScoped<IReservationService, ReservationService>();
            builder.Services.AddScoped<IReservationValidationService, ReservationValidationService>();
            builder.Services.AddScoped<IReservationExtensionService, ReservationExtensionService>();
            builder.Services.AddScoped<IReservationAutoCancellationService, ReservationAutoCancellationService>();
            builder.Services.AddScoped<IReservationCheckInOutService, ReservationCheckInOutService>();

            builder.Services.AddScoped<IRoomStatusRepository, RoomStatusRepository>();
            builder.Services.AddScoped<IRoomStatusService, RoomStatusService>();

            builder.Services.AddScoped<IAuditRepository, AuditRepository>();
            builder.Services.AddScoped<IAuditService, AuditService>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddScoped<INotificationService, NotificationService>();

            builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();
            builder.Services.AddHostedService<OutboxProcessorHostedService>();
            builder.Services.AddHostedService<ReservationAutoCancelHostedService>();

            #region Graph
            builder.Services.AddScoped<IGraphPlacesService, GraphPlacesService>();
            builder.Services.AddScoped<IGraphUsersService, GraphUsersService>();
            builder.Services.AddScoped<IGraphCalendarService, GraphCalendarService>();
            builder.Services.AddScoped<IGraphCheckInService, GraphCheckInService>();

            builder.Services.AddSingleton<IDistributedLock, SqlDistributedLock>();

            builder.Services.Configure<GraphSyncOptions>(builder.Configuration.GetSection("GraphSync"));
            builder.Services.AddSingleton<IGraphAppClientFactory, GraphAppClientFactory>();
            builder.Services.AddScoped<IGraphDirectorySyncService, GraphDirectorySyncService>();
            builder.Services.AddHostedService<GraphDirectorySyncHostedService>();


            #endregion



            builder.Services.AddSingleton<ITimeZoneService, TimeZoneService>();


            builder.Services.AddSingleton<IMapper>(provider =>
            {
                var loggerFactory = provider.GetService<ILoggerFactory>();
                var logger = loggerFactory?.CreateLogger("AutoMapper");
                var expr = new MapperConfigurationExpression();
                expr.AddMaps(typeof(MapperProfile).Assembly);
                var config = new MapperConfiguration(expr, loggerFactory);
                try
                {
                    config.AssertConfigurationIsValid();
                }
                catch (Exception ex)
                {
                    try { logger?.LogError(ex, "AutoMapper configuration is invalid"); } catch { }
                }
                return config.CreateMapper(type => provider.GetService(type));
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Swagger solo en Desarrollo
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gestión de Salas v1");
                    c.DocumentTitle = "Documentación API Gestión de Salas";
                });
            }

            app.UseMiddleware<AuditMiddleware>();
            app.UseRateLimiter();

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapHealthChecks("/health");
            app.Run();
        }
    }
}
