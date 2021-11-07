using System;

namespace Northwind.IntegrationEvents.Contracts;

public interface ShipmentCancelled
{
    Guid OrderId { get; }
    string TransactionId { get; set; }
    Guid ShipperId { get; }
}
