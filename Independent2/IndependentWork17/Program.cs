using System.Diagnostics;
using System.Text;

namespace IndependentWork17;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        SalesData data = SalesDataGenerator.Generate();
        Console.WriteLine($"Тестові дані (Random з seed 2026): товарів {data.Products.Count}, клієнтів {data.Customers.Count}, продажів {data.Sales.Count}");
        Console.WriteLine();

        Console.WriteLine("1. Найпопулярніші продукти за категоріями");
        var (topProducts, time1, repeat1) = Measure(() => SalesPipelines.TopProductsByCategory(data.Sales, data.Products));
        foreach (CategoryTop category in topProducts)
        {
            string top = string.Join(", ", category.TopProducts.Select(p => $"{p.Name} ({p.Quantity} шт.)"));
            Console.WriteLine($"   {category.Category}: {top}");
        }
        PrintTime(time1, repeat1);

        Console.WriteLine("2. Загальний дохід за містами клієнтів");
        var (cities, time2, repeat2) = Measure(() => SalesPipelines.RevenueByCity(data.Sales, data.Products, data.Customers));
        foreach (CityRevenue city in cities)
        {
            Console.WriteLine($"   {city.City}: {city.Revenue:N2} грн ({city.SalesCount} продажів)");
        }
        PrintTime(time2, repeat2);

        Console.WriteLine("3. Продажі за місяцями та роками");
        var (months, time3, repeat3) = Measure(() => SalesPipelines.SalesByMonth(data.Sales, data.Products));
        foreach (MonthlySales month in months)
        {
            Console.WriteLine($"   {month.Month:D2}.{month.Year}: {month.Units} од., дохід {month.Revenue:N2} грн");
        }
        PrintTime(time3, repeat3);

        Console.WriteLine("4. Лояльні клієнти: купували щонайменше у 7 різних місяцях, топ-5 за сумою покупок");
        var (loyal, time4, repeat4) = Measure(() => SalesPipelines.LoyalCustomers(data.Sales, data.Products, data.Customers, minActiveMonths: 7, top: 5));
        foreach (LoyalCustomer customer in loyal)
        {
            Console.WriteLine($"   {customer.Name} ({customer.City}): {customer.TotalSpent:N2} грн, активних місяців {customer.ActiveMonths}, улюблена категорія {customer.FavoriteCategory}");
        }
        PrintTime(time4, repeat4);

        Console.WriteLine("5. Час повторного запуску всіх чотирьох пайплайнів на більших обсягах даних");
        foreach (int salesCount in new[] { 600, 6000, 60000 })
        {
            SalesData bigData = SalesDataGenerator.Generate(salesCount);
            double total = 0;
            total += Measure(() => SalesPipelines.TopProductsByCategory(bigData.Sales, bigData.Products)).RepeatMs;
            total += Measure(() => SalesPipelines.RevenueByCity(bigData.Sales, bigData.Products, bigData.Customers)).RepeatMs;
            total += Measure(() => SalesPipelines.SalesByMonth(bigData.Sales, bigData.Products)).RepeatMs;
            total += Measure(() => SalesPipelines.LoyalCustomers(bigData.Sales, bigData.Products, bigData.Customers, 7, 5)).RepeatMs;
            Console.WriteLine($"   {salesCount} продажів: {total:F3} мс");
        }
    }

    static (T Result, double FirstMs, double RepeatMs) Measure<T>(Func<T> pipeline)
    {
        var firstWatch = Stopwatch.StartNew();
        T result = pipeline();
        firstWatch.Stop();

        var repeatWatch = Stopwatch.StartNew();
        pipeline();
        repeatWatch.Stop();

        return (result, firstWatch.Elapsed.TotalMilliseconds, repeatWatch.Elapsed.TotalMilliseconds);
    }

    static void PrintTime(double firstMs, double repeatMs)
    {
        Console.WriteLine($"   Час виконання: перший запуск {firstMs:F3} мс, повторний {repeatMs:F3} мс");
        Console.WriteLine();
    }
}
