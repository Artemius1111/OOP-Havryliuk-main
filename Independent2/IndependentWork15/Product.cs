namespace IndependentWork15;

public class Product
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;

    public override string ToString() => $"{Name} ({Category}) - {Price} грн";
}
