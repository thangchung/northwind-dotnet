using System;

namespace Northwind.IntegrationEvents.Contracts;

public interface PaymentProcessed
{
    Guid OrderId { get; }
    Guid CustomerId { get; }
    Guid? EmployeeId { get; }
    DateTime OrderDate { get; }
    DateTime? RequiredDate { get; set; }

    decimal TotalMoney { get; set; }
    string TransactionId { get; set; }
}
