using System;

namespace Northwind.IntegrationEvents.Contracts;

public interface CompensatePaymentFailProcessed
{
    Guid OrderId { get; }
}
