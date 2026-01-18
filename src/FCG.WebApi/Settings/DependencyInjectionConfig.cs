using FCG.Application.Producers;
using FCG.Application.Services;
using FCG.Infra.Context;
using FCG.Infra.Mediator;
using FCG.Infra.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace FCG.WebApi.Settings;

[ExcludeFromCodeCoverage]
public static class DependencyInjectionConfig
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("Core"));
        });

        builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

        builder.Services.AddScoped<IMediatorHandler, MediatorHandler>();

        // repositório de banco de dados
        builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();
        builder.Services.AddScoped<IGameRepository, GameRepository>();
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();

        // serviço de apoio para as iterações com o banco
        builder.Services.AddScoped<ICatalogService, CatalogService>();
        builder.Services.AddScoped<IGameService, GameService>();
        builder.Services.AddScoped<IOrderService, OrderService>();

        // producer da fila OrderPlacedEvent
        builder.Services.AddScoped<IOrderPlacedEventProducer, OrderPlacedEventProducer>();
    }
}
