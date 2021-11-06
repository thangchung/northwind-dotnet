using System;

namespace Northwind.IntegrationEvents.Contracts;

public interface ShipmentPrepared
{
    Guid OrderId { get; }
    string TransactionId { get; set; }
    Guid ShipperId { get; }
}
