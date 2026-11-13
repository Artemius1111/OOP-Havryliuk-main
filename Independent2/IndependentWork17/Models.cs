namespace IndependentWork17;

public class Sale
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime SaleDate { get; set; }
    public int CustomerId { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}

public record SalesData(List<Product> Products, List<Customer> Customers, List<Sale> Sales);

public record ProductSales(string Name, int Quantity);

public record CategoryTop(string Category, List<ProductSales> TopProducts);

public record CityRevenue(string City, decimal Revenue, int SalesCount);

public record MonthlySales(int Year, int Month, int Units, decimal Revenue);

public record LoyalCustomer(string Name, string City, int ActiveMonths, decimal TotalSpent, string FavoriteCategory);
