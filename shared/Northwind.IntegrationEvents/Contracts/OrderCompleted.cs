using System;

namespace Northwind.IntegrationEvents.Contracts;

public interface OrderCompleted
{
    Guid OrderId { get; }
    string TransactionId { get; set; }
    Guid ShipperId { get; }
}
