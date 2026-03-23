using System.Reflection;

namespace FCG.Catalog.Tests;

public class ArchitectureTests
{
    [Fact]
    public void ApplicationAssembly_ShouldNotReference_InfraAssembly()
    {
        var applicationAssembly = typeof(FCG.Catalog.Application.Services.OrderService).Assembly;

        var referencesInfra = applicationAssembly
            .GetReferencedAssemblies()
            .Any(reference => reference.Name == "FCG.Catalog.Infra");

        Assert.False(referencesInfra);
    }

    [Fact]
    public void DomainAssembly_ShouldNotReference_ApplicationOrInfraAssemblies()
    {
        var domainAssembly = typeof(FCG.Catalog.Domain.Models.Order.Order).Assembly;

        var referencedAssemblyNames = domainAssembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .ToHashSet(StringComparer.Ordinal);

        Assert.DoesNotContain("FCG.Catalog.Application", referencedAssemblyNames);
        Assert.DoesNotContain("FCG.Catalog.Infra", referencedAssemblyNames);
    }

    [Fact]
    public void CartService_ShouldDependOnOrderCheckoutBoundary_NotOnFullOrderService()
    {
        var cartServiceType = typeof(FCG.Catalog.Application.Services.CartService);
        var constructor = cartServiceType.GetConstructors().Single();
        var parameterTypes = constructor.GetParameters().Select(parameter => parameter.ParameterType).ToList();
        var parameterTypeNames = parameterTypes.Select(parameterType => parameterType.Name).ToList();

        Assert.Contains(typeof(FCG.Catalog.Application.Interfaces.IOrderCheckoutService), parameterTypes);
        Assert.DoesNotContain("IOrderService", parameterTypeNames);
    }

    [Fact]
    public void PaymentConsumer_ShouldDependOnPaymentBoundary_NotOnFullOrderService()
    {
        var consumerType = typeof(FCG.Catalog.Application.Consumers.PaymentProcessedEventConsumer);
        var constructor = consumerType.GetConstructors().Single();
        var parameterTypes = constructor.GetParameters().Select(parameter => parameter.ParameterType).ToList();
        var parameterTypeNames = parameterTypes.Select(parameterType => parameterType.Name).ToList();

        Assert.Contains(typeof(FCG.Catalog.Application.Interfaces.IOrderPaymentProcessingService), parameterTypes);
        Assert.DoesNotContain("IOrderService", parameterTypeNames);
    }

    [Fact]
    public void PaymentConsumer_ShouldDependOnLibraryOwnershipBoundary_NotOnFullLibraryService()
    {
        var consumerType = typeof(FCG.Catalog.Application.Consumers.PaymentProcessedEventConsumer);
        var constructor = consumerType.GetConstructors().Single();
        var parameterTypes = constructor.GetParameters().Select(parameter => parameter.ParameterType).ToList();
        var parameterTypeNames = parameterTypes.Select(parameterType => parameterType.Name).ToList();

        Assert.Contains(typeof(FCG.Catalog.Application.Interfaces.IGameLibraryOwnershipService), parameterTypes);
        Assert.DoesNotContain("IGameLibraryService", parameterTypeNames);
    }

    [Fact]
    public void GameLibraryService_ShouldImplementReadAndOwnershipBoundaries()
    {
        var serviceType = typeof(FCG.Catalog.Application.Services.GameLibraryService);
        var implementedInterfaces = serviceType.GetInterfaces().ToHashSet();

        Assert.Contains(typeof(FCG.Catalog.Application.Interfaces.IGameLibraryReadService), implementedInterfaces);
        Assert.Contains(typeof(FCG.Catalog.Application.Interfaces.IGameLibraryOwnershipService), implementedInterfaces);
    }

    [Fact]
    public void CartService_ShouldDependOnGameCatalogLookupBoundary_NotOnGameRepository()
    {
        var cartServiceType = typeof(FCG.Catalog.Application.Services.CartService);
        var constructor = cartServiceType.GetConstructors().Single();
        var parameterTypes = constructor.GetParameters().Select(parameter => parameter.ParameterType).ToList();

        Assert.Contains(typeof(FCG.Catalog.Application.Interfaces.IGameCatalogLookupService), parameterTypes);
        Assert.DoesNotContain(typeof(FCG.Catalog.Domain.Repository.IGameRepository), parameterTypes);
    }

    [Fact]
    public void OrderService_ShouldDependOnGameCatalogLookupBoundary_NotOnGameRepository()
    {
        var orderServiceType = typeof(FCG.Catalog.Application.Services.OrderService);
        var constructor = orderServiceType.GetConstructors().Single();
        var parameterTypes = constructor.GetParameters().Select(parameter => parameter.ParameterType).ToList();

        Assert.Contains(typeof(FCG.Catalog.Application.Interfaces.IGameCatalogLookupService), parameterTypes);
        Assert.DoesNotContain(typeof(FCG.Catalog.Domain.Repository.IGameRepository), parameterTypes);
    }

    [Fact]
    public void GameService_ShouldImplementCatalogLookupBoundary()
    {
        var gameServiceType = typeof(FCG.Catalog.Application.Services.GameService);
        var implementedInterfaces = gameServiceType.GetInterfaces().ToHashSet();

        Assert.Contains(typeof(FCG.Catalog.Application.Interfaces.IGameCatalogLookupService), implementedInterfaces);
    }

    [Fact]
    public void GameService_ShouldImplementReadAndManagementBoundaries()
    {
        var gameServiceType = typeof(FCG.Catalog.Application.Services.GameService);
        var implementedInterfaces = gameServiceType.GetInterfaces().ToHashSet();
        var implementedInterfaceNames = implementedInterfaces.Select(@interface => @interface.Name).ToHashSet();

        Assert.Contains(typeof(FCG.Catalog.Application.Interfaces.IGameReadService), implementedInterfaces);
        Assert.Contains(typeof(FCG.Catalog.Application.Interfaces.IGameManagementService), implementedInterfaces);
        Assert.DoesNotContain("IGameService", implementedInterfaceNames);
    }

    [Fact]
    public void OrderService_ShouldImplementReadAndManagementBoundaries()
    {
        var orderServiceType = typeof(FCG.Catalog.Application.Services.OrderService);
        var implementedInterfaces = orderServiceType.GetInterfaces().ToHashSet();
        var implementedInterfaceNames = implementedInterfaces.Select(@interface => @interface.Name).ToHashSet();

        Assert.Contains(typeof(FCG.Catalog.Application.Interfaces.IOrderReadService), implementedInterfaces);
        Assert.Contains(typeof(FCG.Catalog.Application.Interfaces.IOrderManagementService), implementedInterfaces);
        Assert.DoesNotContain("IOrderService", implementedInterfaceNames);
    }

    [Fact]
    public void CartService_ShouldImplementReadAndManagementBoundaries()
    {
        var cartServiceType = typeof(FCG.Catalog.Application.Services.CartService);
        var implementedInterfaces = cartServiceType.GetInterfaces().ToHashSet();
        var implementedInterfaceNames = implementedInterfaces.Select(@interface => @interface.Name).ToHashSet();

        Assert.Contains(typeof(FCG.Catalog.Application.Interfaces.ICartReadService), implementedInterfaces);
        Assert.Contains(typeof(FCG.Catalog.Application.Interfaces.ICartManagementService), implementedInterfaces);
        Assert.DoesNotContain("ICartService", implementedInterfaceNames);
    }

    [Fact]
    public void GameLibraryService_ShouldNotImplementLegacyBroadInterface()
    {
        var serviceType = typeof(FCG.Catalog.Application.Services.GameLibraryService);
        var implementedInterfaces = serviceType.GetInterfaces().ToHashSet();
        var implementedInterfaceNames = implementedInterfaces.Select(@interface => @interface.Name).ToHashSet();

        Assert.DoesNotContain("IGameLibraryService", implementedInterfaceNames);
    }
}
