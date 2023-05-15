namespace CustomerService.Domain;

public class CustomerDemographic : EntityRootBase
{
    private CustomerDemographic() {}

    public string? Description { get; private set; } = default!;

    public List<CustomerCustomerDemo> CustomerCustomerDemos { get; set; } = new();
}
