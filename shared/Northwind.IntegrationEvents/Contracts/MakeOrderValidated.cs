using System;

namespace Northwind.IntegrationEvents.Contracts;

public interface MakeOrderValidated
{
    Guid OrderId { get; }
}
