using System;

namespace Northwind.IntegrationEvents.Contracts;

public interface OrderCancelled
{
    Guid OrderId { get; }
    Guid CustomerId { get; }
    string Reason { get; }
}
