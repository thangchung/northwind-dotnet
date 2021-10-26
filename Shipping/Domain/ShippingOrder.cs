namespace Shipping.Domain;

public class ShippingOrder : EntityRootBase
{
    private ShippingOrder() { }

    public DateTime? ShippedDate { get; private set; }
    public Guid? ShipperInfoId { get; private set; } // ShipperId == ShipVia
    public ShipperInfo? ShipperInfo { get; private set; }
    public float? Freight { get; private set; } = default!;
    public string? ShipName { get; private set; } = default!;
    public string? ShipAddress { get; private set; } = default!;
    public string? ShipCity { get; private set; } = default!;
    public string? ShipRegion { get; private set; } = default!;
    public string? ShipPostalCode { get; private set; } = default!;
    public string? ShipCountry { get; private set; } = default!;
}
