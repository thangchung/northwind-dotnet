namespace CustomerService.Domain;

public class CustomerCustomerDemo : EntityBase
{
    private CustomerCustomerDemo() { }

    public Guid CustomerId { get; private set; }
    public Guid CustomerDemographicId { get; private set; }
}
