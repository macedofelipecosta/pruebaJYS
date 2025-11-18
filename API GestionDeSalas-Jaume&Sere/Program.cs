using AutoMapper;
using DTOs.Mapper;
using Infrastructure.Cache;
using Infrastructure.LogicaDatos.EntityFramework.Repositorios;
using Infrastructure.Persistence.EntityFramework;
using Infrastructure.Persistence.EntityFramework.Repositorios;
using LogicaAplicacion.ServiceInterfaces;
using LogicaAplicacion.Services;
using LogicaNegocio.InterfacesRepositorios;
using Microsoft.EntityFrameworkCore;


namespace API_GestionDeSalas_Jaume_Sere
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            //DBCONTEXT
            builder.Services.AddDbContextPool<GestorSalasContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSomee")); //Cambiar aca!
            });

            //INYECCION DE DEPENDENCIAS
            builder.Services.AddScoped<IRoomRepository, RoomRepository>();
            builder.Services.AddScoped<IRoomService, RoomService>();

            builder.Services.AddScoped<ILocationRepository, LocationRepository>();
            builder.Services.AddScoped<ILocationService, LocationService>();

            builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
            builder.Services.AddScoped<IReservationService, ReservationService>();



            //Notificaciones OUTBOX
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddHostedService<OutboxProcessorHostedService>();
            builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();

            //SWAGGER
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "API Gestión de Salas",
                    Version = "v1",
                    Description = "API REST para la gestión de salas y reservas",
                });

                // Incluye comentarios XML de los controladores
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);
            });

            //AutoMapper
            builder.Services.AddSingleton<IMapper>(provider =>
            {
                var loggerFactory = provider.GetService<ILoggerFactory>();
                var expr = new MapperConfigurationExpression();
                expr.AddMaps(typeof(MapperProfile).Assembly);
                var config = new MapperConfiguration(expr, loggerFactory);
                config.AssertConfigurationIsValid();
                return config.CreateMapper(type => provider.GetService(type));
            });



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gestión de Salas v1");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
