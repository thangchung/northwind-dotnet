using System;

namespace Northwind.IntegrationEvents.Contracts;

public interface OrderStatus
{
    Guid OrderId { get; }

    string State { get; }
}
