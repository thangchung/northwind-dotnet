using System;

namespace Northwind.IntegrationEvents.Contracts;

public interface ShipmentDeliveredFailed
{
    Guid OrderId { get; }
    string TransactionId { get; set; }
    Guid ShipperId { get; }
    Guid CustomerId { get; }
}
