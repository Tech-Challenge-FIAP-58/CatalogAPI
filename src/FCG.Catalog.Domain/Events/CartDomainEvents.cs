using FCG.Catalog.Domain.Mediatr;

namespace FCG.Catalog.Domain.Events;

public class CartCreatedDomainEvent : Event
{
    public CartSnapshot Cart { get; }

    public CartCreatedDomainEvent(CartSnapshot cart)
    {
        Cart = cart;
    }
}

public class CartUpdatedDomainEvent : Event
{
    public CartSnapshot Cart { get; }

    public CartUpdatedDomainEvent(CartSnapshot cart)
    {
        Cart = cart;
    }
}

public class CartItemAddedDomainEvent : Event
{
    public CartSnapshot Cart { get; }
    public CartItemSnapshot Item { get; }

    public CartItemAddedDomainEvent(CartSnapshot cart, CartItemSnapshot item)
    {
        Cart = cart;
        Item = item;
    }
}

public class CartItemRemovedDomainEvent : Event
{
    public CartSnapshot Cart { get; }
    public CartItemSnapshot Item { get; }

    public CartItemRemovedDomainEvent(CartSnapshot cart, CartItemSnapshot item)
    {
        Cart = cart;
        Item = item;
    }
}

public class CartDeletedDomainEvent : Event
{
    public CartSnapshot Cart { get; }

    public CartDeletedDomainEvent(CartSnapshot cart)
    {
        Cart = cart;
    }
}
