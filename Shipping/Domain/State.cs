namespace Shipping.Domain;

public class State : EntityBase
{
    private State() { }

    public string? Name { get; private set; } = default!;
    public string? Abbr { get; private set; } = default!;
    public string? Region { get; private set; } = default!;
}
