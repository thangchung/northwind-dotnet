using System;

namespace Northwind.IntegrationEvents.Contracts;

public class ProcessPayment
{
    public Guid OrderId { get; set; }
    public string Description { get; set; }
}
