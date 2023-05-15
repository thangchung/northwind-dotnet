using System;

namespace Northwind.IntegrationEvents.Contracts;

public interface ShipmentDispatched
{
    Guid OrderId { get; }
    string TransactionId { get; set; }
    Guid ShipperId { get; }
}
