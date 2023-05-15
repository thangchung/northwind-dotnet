using System;

namespace Northwind.IntegrationEvents.Contracts;

public interface PaymentProcessedFailed
{
    Guid OrderId { get; }
    Guid CustomerId { get; }
    string Reason { get; }

    decimal TotalMoney { get; set; }
    string TransactionId { get; set; }
}
