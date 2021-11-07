using System;

namespace Northwind.IntegrationEvents.Contracts;

public interface OrderSubmitted
{
    Guid OrderId { get; }
    Guid CustomerId { get; }
    Guid? EmployeeId { get; }
    DateTime OrderDate { get; }
    DateTime? RequiredDate { get; set; }
}
