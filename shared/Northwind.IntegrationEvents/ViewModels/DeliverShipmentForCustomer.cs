using System;

namespace Northwind.IntegrationEvents.ViewModels;

public class DeliverShipmentForCustomer
{
    public Guid OrderId { get; set; }
    public string TransactionId { get; set; }
    public Guid ShipperId { get; set; }
    public Guid CustomerId { get; set; }
    public string BeFailedAt { get; set; } // "Delivered"
}
