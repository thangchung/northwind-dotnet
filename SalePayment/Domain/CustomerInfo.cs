namespace SalePayment.Domain;

public class CustomerInfo : EntityBase
{
    private CustomerInfo() { }

    public CustomerInfo(Guid customerId)
    {
        CustomerId = customerId;
    }

    public Guid CustomerId { get; private set; }
}
