﻿namespace CustomerService.Domain;

public class CustomerInfo : EntityBase
{
    private CustomerInfo() {}

    public Guid CustomerId { get; private set; }
}
