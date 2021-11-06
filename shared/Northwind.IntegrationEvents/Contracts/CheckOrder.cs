using System;

namespace Northwind.IntegrationEvents.Contracts;

public interface CheckOrder
{
    Guid OrderId { get; }
}
