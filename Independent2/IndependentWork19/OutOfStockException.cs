namespace IndependentWork19;

public class OutOfStockException : Exception
{
    public string ProductName { get; } = string.Empty;
    public int RequestedQuantity { get; }
    public int AvailableQuantity { get; }

    public OutOfStockException()
        : base("Товару недостатньо на складі")
    {
    }

    public OutOfStockException(string message)
        : base(message)
    {
    }

    public OutOfStockException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public OutOfStockException(string productName, int requestedQuantity, int availableQuantity)
        : base($"Недостатньо товару «{productName}»: запитано {requestedQuantity} шт., на складі {availableQuantity} шт.")
    {
        ProductName = productName;
        RequestedQuantity = requestedQuantity;
        AvailableQuantity = availableQuantity;
    }
}
