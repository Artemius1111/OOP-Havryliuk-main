using System.Text;

namespace IndependentWork19;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("1. Власний виняток OutOfStockException");
        DemonstrateCustomException();
        Console.WriteLine();

        Console.WriteLine("2. Result<T> замість винятку для очікуваної відмови");
        DemonstrateResult();
        Console.WriteLine();

        Console.WriteLine("3. Polly, сценарій 1: Retry з експоненційною затримкою (API курсів валют)");
        ResilienceScenarios.RetryWithExponentialBackoff();
        Console.WriteLine();

        Console.WriteLine("4. Polly, сценарій 2: Circuit Breaker (платіжний шлюз)");
        ResilienceScenarios.CircuitBreaker();
        Console.WriteLine();

        Console.WriteLine("5. Polly, сценарій 3: Timeout + Retry + Fallback (сервіс рекомендацій)");
        ResilienceScenarios.TimeoutRetryFallback();
    }

    static void DemonstrateCustomException()
    {
        var warehouse = new Warehouse();
        warehouse.AddStock("Ноутбук", 5);
        warehouse.AddStock("Миша", 40);

        PlaceOrder(warehouse, "Миша", 3);
        PlaceOrder(warehouse, "Ноутбук", 8);
        PlaceOrder(warehouse, "Планшет", 1);
        PlaceOrder(warehouse, "Миша", 0);

        try
        {
            warehouse.LoadStockFromInvoice("Клавіатура", "десять");
        }
        catch (OutOfStockException ex)
        {
            Console.WriteLine($"   Конструктор з innerException: {ex.Message}");
            Console.WriteLine($"      InnerException: {ex.InnerException?.GetType().Name}");
        }

        try
        {
            throw new OutOfStockException();
        }
        catch (OutOfStockException ex)
        {
            Console.WriteLine($"   Конструктор без параметрів: {ex.Message}");
        }

        try
        {
            throw new OutOfStockException("Склад закрито на інвентаризацію, резервування недоступне");
        }
        catch (OutOfStockException ex)
        {
            Console.WriteLine($"   Конструктор з message: {ex.Message}");
        }
    }

    static void PlaceOrder(Warehouse warehouse, string product, int quantity)
    {
        try
        {
            int left = warehouse.Reserve(product, quantity);
            Console.WriteLine($"   Замовлення «{product}» x{quantity}: зарезервовано, залишок {left} шт.");
        }
        catch (OutOfStockException ex)
        {
            Console.WriteLine($"   Замовлення «{product}» x{quantity}: OutOfStockException, {ex.Message}");
            Console.WriteLine($"      ProductName = {ex.ProductName}, RequestedQuantity = {ex.RequestedQuantity}, AvailableQuantity = {ex.AvailableQuantity}, клієнту можна запропонувати {ex.AvailableQuantity} шт.");
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"   Замовлення «{product}» x{quantity}: KeyNotFoundException, {ex.Message}");
        }
        catch (ArgumentOutOfRangeException)
        {
            Console.WriteLine($"   Замовлення «{product}» x{quantity}: ArgumentOutOfRangeException, кількість має бути додатною");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Замовлення «{product}» x{quantity}: неочікувана помилка {ex.GetType().Name}");
        }
        finally
        {
            Console.WriteLine("      finally: перевірку замовлення завершено");
        }
    }

    static void DemonstrateResult()
    {
        var warehouse = new Warehouse();
        warehouse.AddStock("Навушники", 2);

        foreach (int quantity in new[] { 1, 5, 1 })
        {
            Result<int> result = warehouse.TryReserve("Навушники", quantity);
            string message = result.IsSuccess
                ? $"зарезервовано, залишок {result.Value} шт."
                : $"відмова без винятку, {result.Error}";
            Console.WriteLine($"   TryReserve(«Навушники», {quantity}): {message}");
        }
    }
}
