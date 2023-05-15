using System;

namespace Northwind.IntegrationEvents.Contracts;

public interface OrderValidated
{
    Guid OrderId { get; }
}
