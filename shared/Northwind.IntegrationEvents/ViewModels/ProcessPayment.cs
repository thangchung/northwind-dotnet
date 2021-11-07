using System;

namespace Northwind.IntegrationEvents.ViewModels;

public class ProcessPayment
{
    public Guid OrderId { get; set; }
    public string Description { get; set; }
}
