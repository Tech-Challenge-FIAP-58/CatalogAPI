using FCG.Catalog.Application.Interfaces;
using FCG.Catalog.Application.Producers;
using FCG.Catalog.Application.Services;
using FCG.Catalog.Domain.Repository;
using FCG.Catalog.Infra.Repository;

namespace FCG.Catalog.WebApi.DependencyInjection
{
    public static class ModuleServiceCollectionExtensions
    {
        public static IServiceCollection AddCatalogModule(this IServiceCollection services)
        {
            services.AddScoped<IGameRepository, GameRepository>();
            services.AddScoped<GameService>();
            services.AddScoped<IGameReadService>(sp =>
                sp.GetRequiredService<GameService>());
            services.AddScoped<IGameManagementService>(sp =>
                sp.GetRequiredService<GameService>());
            services.AddScoped<IGameCatalogLookupService>(sp =>
                sp.GetRequiredService<GameService>());

            return services;
        }

        public static IServiceCollection AddOrderModule(this IServiceCollection services)
        {
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<OrderService>();
            services.AddScoped<IOrderReadService>(sp =>
                sp.GetRequiredService<OrderService>());
            services.AddScoped<IOrderManagementService>(sp =>
                sp.GetRequiredService<OrderService>());
            services.AddScoped<IOrderCheckoutService>(sp =>
                sp.GetRequiredService<OrderService>());
            services.AddScoped<IOrderPaymentProcessingService>(sp =>
                sp.GetRequiredService<OrderService>());
            services.AddScoped<IOrderPlacedEventProducer, OrderPlacedEventProducer>();

            return services;
        }

        public static IServiceCollection AddCartModule(this IServiceCollection services)
        {
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<CartService>();
            services.AddScoped<ICartReadService>(sp =>
                sp.GetRequiredService<CartService>());
            services.AddScoped<ICartManagementService>(sp =>
                sp.GetRequiredService<CartService>());

            return services;
        }

        public static IServiceCollection AddLibraryModule(this IServiceCollection services)
        {
            services.AddScoped<IGameLibraryRepository, GameLibraryRepository>();
            services.AddScoped<GameLibraryService>();
            services.AddScoped<IGameLibraryReadService>(sp =>
                sp.GetRequiredService<GameLibraryService>());
            services.AddScoped<IGameLibraryOwnershipService>(sp =>
                sp.GetRequiredService<GameLibraryService>());

            return services;
        }
    }
}
