namespace IndependentWork19;

public class Warehouse
{
    private readonly Dictionary<string, int> _stock = new(StringComparer.OrdinalIgnoreCase);

    public void AddStock(string productName, int quantity)
    {
        _stock[productName] = _stock.GetValueOrDefault(productName) + quantity;
    }

    public int Reserve(string productName, int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Кількість має бути додатною");
        }

        if (!_stock.TryGetValue(productName, out int available))
        {
            throw new KeyNotFoundException($"Товару «{productName}» немає в асортименті складу");
        }

        if (quantity > available)
        {
            throw new OutOfStockException(productName, quantity, available);
        }

        _stock[productName] = available - quantity;
        return _stock[productName];
    }

    public Result<int> TryReserve(string productName, int quantity)
    {
        if (quantity <= 0)
        {
            return Result<int>.Failure("кількість має бути додатною");
        }

        if (!_stock.TryGetValue(productName, out int available))
        {
            return Result<int>.Failure($"товару «{productName}» немає в асортименті");
        }

        if (quantity > available)
        {
            return Result<int>.Failure($"запитано {quantity} шт., а на складі лише {available} шт.");
        }

        _stock[productName] = available - quantity;
        return Result<int>.Success(_stock[productName]);
    }

    public void LoadStockFromInvoice(string productName, string quantityText)
    {
        try
        {
            AddStock(productName, int.Parse(quantityText));
        }
        catch (FormatException ex)
        {
            throw new OutOfStockException($"Залишок товару «{productName}» невідомий: у накладній некоректна кількість «{quantityText}»", ex);
        }
    }
}
