using System.Text;

namespace lab13v4;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        var validator = new ValidatorService();

        Console.WriteLine("Перевірка продуктів (власний виняток ValidationException):");
        Console.WriteLine();

        var products = new List<Product>
        {
            new Product("Ноутбук", 25000m, 5),
            new Product("", -100m, -3),
            new Product("Миша", 0m, 10)
        };

        foreach (var product in products)
        {
            try
            {
                validator.ValidateProduct(product);
                Console.WriteLine($"[OK] {product} - продукт валідний");
            }
            catch (ValidationException ex)
            {
                Console.WriteLine($"[ПОМИЛКА] {product}");
                Console.WriteLine($"  Кількість помилок: {ex.Errors.Count}");
                foreach (var error in ex.Errors)
                {
                    Console.WriteLine($"  - {error}");
                }
            }
            finally
            {
                Console.WriteLine();
            }
        }

        Console.WriteLine("Перевірка email (Result<bool>):");
        Console.WriteLine();

        string[] emails = { "artem@gmail.com", "artem.gmail.com", "", "user@site" };

        foreach (var email in emails)
        {
            Result<bool> result = validator.IsEmailValid(email);

            if (result.IsSuccess)
            {
                Console.WriteLine($"[OK] \"{email}\" - email коректний");
            }
            else
            {
                Console.WriteLine($"[ПОМИЛКА] {result.Error}");
            }
        }

        Console.WriteLine();
        Console.WriteLine("Розбір ціни (Result<decimal>, Failure(Exception)):");
        Console.WriteLine();

        string[] prices = { "199.99", "abc", "-5", "99999999999999999999999999999999" };

        foreach (var input in prices)
        {
            Result<decimal> result = validator.ParsePrice(input);

            if (result.IsSuccess)
            {
                Console.WriteLine($"[OK] \"{input}\" -> {result.Value}");
            }
            else
            {
                Console.WriteLine($"[ПОМИЛКА] \"{input}\": {result.Error}");
            }
        }
    }
}
