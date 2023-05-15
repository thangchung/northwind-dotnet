using System;

namespace Northwind.IntegrationEvents.Contracts;

public interface MoneyRefunded
{
    Guid OrderId { get; }
    Guid TransactionId { get; }
}
