namespace Shipping.Domain;

public class ShipperInfo : EntityBase
{
    private ShipperInfo() {}

    public Guid ShipperId { get; private set; }
}
