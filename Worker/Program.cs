// Worker/Program.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using LogicaDatos;
using Infrastructure.Persistence.EntityFramework;
using LogicaAplicacion.ServiceInterfaces;
using Infrastructure.LogicaDatos.EntityFramework.Repositorios;
using LogicaNegocio.InterfacesRepositorios;
using Worker;

Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var config = hostContext.Configuration;

        // DbContext
        services.AddDbContext<GestorSalasContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        // UoW + Repos
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();

        // Redis
        var redisConnection = config.GetConnectionString("Redis");
        services.AddSingleton<IConnectionMultiplexer>(
            _ => ConnectionMultiplexer.Connect(redisConnection));

        // Worker
        services.AddHostedService<OutboxProcessorWorker>();
    })
    .Build()
    .Run();
