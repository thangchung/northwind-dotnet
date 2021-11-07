using System;

namespace Northwind.IntegrationEvents.ViewModels;

public class RequestOrder
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? EmployeeId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? RequiredDate { get; set; }
}
