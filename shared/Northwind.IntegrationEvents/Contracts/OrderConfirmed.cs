using System;

namespace Northwind.IntegrationEvents.Contracts;

public interface OrderConfirmed
{
    Guid OrderId { get; }
    string TransactionId { get; set; }
}
