using System;

namespace Northwind.IntegrationEvents.Contracts;

public interface ShipmentDispatchedFailed
{
    Guid OrderId { get; }
    string TransactionId { get; set; }
    Guid ShipperId { get; }
}
