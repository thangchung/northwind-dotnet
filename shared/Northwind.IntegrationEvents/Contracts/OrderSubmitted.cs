using System;

namespace Northwind.IntegrationEvents.Contracts;

public interface OrderSubmitted
{
    Guid OrderId { get; }
    Guid CustomerId { get; }
    DateTime OrderDate { get; }
    DateTime? RequiredDate { get; set; }
}
