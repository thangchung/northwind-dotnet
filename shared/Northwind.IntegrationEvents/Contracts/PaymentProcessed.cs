using System;

namespace Northwind.IntegrationEvents.Contracts;

public interface PaymentProcessed
{
    Guid OrderId { get; }
    Guid CustomerId { get; }
    string TransactionId { get; set; }
}
