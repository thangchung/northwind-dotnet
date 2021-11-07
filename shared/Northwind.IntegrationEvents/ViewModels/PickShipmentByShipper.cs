using System;

namespace Northwind.IntegrationEvents.ViewModels;

public class PickShipmentByShipper
{
    public Guid OrderId { get; set; }
    public string TransactionId { get; set; }
    public Guid ShipperId { get; set; }
    public string BeFailedAt { get; set; } // "Dispatched"
}
