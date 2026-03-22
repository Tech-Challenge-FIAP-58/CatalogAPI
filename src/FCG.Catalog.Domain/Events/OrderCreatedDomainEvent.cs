using FCG.Catalog.Domain.Mediatr;

namespace FCG.Catalog.Domain.Events;

public class OrderCreatedDomainEvent : Event
{
    public OrderSnapshot Order { get; }

    public OrderCreatedDomainEvent(OrderSnapshot order)
    {
        Order = order;
    }
}

public class OrderUpdatedDomainEvent : Event
{
    public OrderSnapshot Order { get; }

    public OrderUpdatedDomainEvent(OrderSnapshot order)
    {
        Order = order;
    }
}

public class OrderItemAddedDomainEvent : Event
{
    public OrderSnapshot Order { get; }
    public OrderItemSnapshot Item { get; }

    public OrderItemAddedDomainEvent(OrderSnapshot order, OrderItemSnapshot item)
    {
        Order = order;
        Item = item;
    }
}

public class OrderItemRemovedDomainEvent : Event
{
    public OrderSnapshot Order { get; }
    public OrderItemSnapshot Item { get; }

    public OrderItemRemovedDomainEvent(OrderSnapshot order, OrderItemSnapshot item)
    {
        Order = order;
        Item = item;
    }
}

public class OrderDeletedDomainEvent : Event
{
    public OrderSnapshot Order { get; }

    public OrderDeletedDomainEvent(OrderSnapshot order)
    {
        Order = order;
    }
}
