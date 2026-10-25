namespace lab14v4;

/// <summary>
/// Власний клас для демонстрації роботи CollectionHelper&lt;T&gt;.
/// </summary>
public class Product
{
    public string Name { get; set; }
    public decimal Price { get; set; }

    public Product(string name, decimal price)
    {
        Name = name;
        Price = price;
    }

    public override string ToString() => $"{Name} - {Price:F2} грн";
}
