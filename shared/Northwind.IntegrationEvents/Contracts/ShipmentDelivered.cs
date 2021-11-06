using System;

namespace Northwind.IntegrationEvents.Contracts;

public interface ShipmentDelivered
{
    Guid OrderId { get; }
    string TransactionId { get; set; }
    Guid ShipperId { get; }
}
