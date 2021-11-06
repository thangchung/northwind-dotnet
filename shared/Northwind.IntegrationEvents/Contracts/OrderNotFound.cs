using System;

namespace Northwind.IntegrationEvents.Contracts;

public interface OrderNotFound
{
    Guid OrderId { get; }
}
