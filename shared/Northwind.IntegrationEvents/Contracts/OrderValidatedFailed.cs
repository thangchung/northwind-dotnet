using System;

namespace Northwind.IntegrationEvents.Contracts;

public interface OrderValidatedFailed
{
    Guid OrderId { get; }
    Guid CustomerId { get; }
    string Reason { get; }
}
