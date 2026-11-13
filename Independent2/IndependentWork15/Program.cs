using System.Text;

namespace IndependentWork15;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        var numbers = new List<int> { 12, -7, 45, 3, 28, -71, 64, 9, 0, 37 };

        var cities = new List<string> { "Рівне", "Київ", "Львів", "Тернопіль", "Луцьк", "Івано-Франківськ", "Одеса" };

        var products = new List<Product>
        {
            new Product { Name = "Ноутбук", Price = 32500, Category = "Комп'ютери" },
            new Product { Name = "Миша", Price = 650, Category = "Периферія" },
            new Product { Name = "Клавіатура", Price = 1800, Category = "Периферія" },
            new Product { Name = "Монітор", Price = 8900, Category = "Комп'ютери" },
            new Product { Name = "Навушники", Price = 2400, Category = "Аудіо" },
            new Product { Name = "Килимок", Price = 350, Category = "Периферія" }
        };

        Console.WriteLine("1. List<int>");
        DemonstrateNumbers(numbers);
        Console.WriteLine();

        Console.WriteLine("2. List<string>");
        DemonstrateCities(cities);
        Console.WriteLine();

        Console.WriteLine("3. List<Product>");
        DemonstrateProducts(products);
        Console.WriteLine();

        Console.WriteLine("4. Порівняння з LINQ");
        CompareWithLinq(numbers, products);
    }

    static void DemonstrateNumbers(List<int> numbers)
    {
        Console.WriteLine($"   Вихідна колекція: {string.Join(", ", numbers)}");

        IEnumerable<int> squares = CollectionUtils.Map(numbers, n => n * n);
        Console.WriteLine($"   Map(n => n * n): {string.Join(", ", squares)}");

        IEnumerable<string> parity = CollectionUtils.Map(numbers, n => n % 2 == 0 ? "парне" : "непарне");
        Console.WriteLine($"   Map(n => парне/непарне): {string.Join(", ", parity)}");

        IEnumerable<int> positiveEven = CollectionUtils.Filter(numbers, n => n > 0 && n % 2 == 0);
        Console.WriteLine($"   Filter(n => n > 0 && n % 2 == 0): {string.Join(", ", positiveEven)}");

        int maxByValue = CollectionUtils.MaxBy(numbers, n => n);
        int maxByAbs = CollectionUtils.MaxBy(numbers, n => Math.Abs(n));
        Console.WriteLine($"   MaxBy(n => n) = {maxByValue}, MaxBy(n => Math.Abs(n)) = {maxByAbs}");

        int sum = 0;
        int negativeCount = 0;
        CollectionUtils.ForEach(numbers, n =>
        {
            sum += n;
            if (n < 0)
            {
                negativeCount++;
            }
        });
        Console.WriteLine($"   ForEach з блоковою лямбдою: сума = {sum}, від'ємних чисел = {negativeCount}");
    }

    static void DemonstrateCities(List<string> cities)
    {
        Console.WriteLine($"   Вихідна колекція: {string.Join(", ", cities)}");

        IEnumerable<int> lengths = CollectionUtils.Map(cities, city => city.Length);
        Console.WriteLine($"   Map(city => city.Length): {string.Join(", ", lengths)}");

        IEnumerable<string> upper = CollectionUtils.Map(cities, city => city.ToUpper());
        Console.WriteLine($"   Map(city => city.ToUpper()): {string.Join(", ", upper)}");

        string longest = CollectionUtils.MaxBy(cities, city => city.Length);
        Console.WriteLine($"   MaxBy(city => city.Length): {longest}");

        char letter = 'Л';
        IEnumerable<string> startsWithLetter = CollectionUtils.Filter(cities, city => city.StartsWith(letter));
        Console.WriteLine($"   Filter(city => city.StartsWith('{letter}')): {string.Join(", ", startsWithLetter)}");

        Console.WriteLine("   ForEach з методом PrintCity замість лямбди (міста довші за 5 літер):");
        CollectionUtils.ForEach(CollectionUtils.Filter(cities, city => city.Length > 5), PrintCity);
    }

    static void DemonstrateProducts(List<Product> products)
    {
        Console.WriteLine("   ForEach: усі товари");
        CollectionUtils.ForEach(products, p => Console.WriteLine($"      {p}"));

        IEnumerable<string> names = CollectionUtils.Map(products, p => p.Name);
        Console.WriteLine($"   Map(p => p.Name): {string.Join(", ", names)}");

        Product mostExpensive = CollectionUtils.MaxBy(products, p => p.Price);
        Console.WriteLine($"   MaxBy(p => p.Price): {mostExpensive}");

        Product longestName = CollectionUtils.MaxBy(products, p => p.Name.Length);
        Console.WriteLine($"   MaxBy(p => p.Name.Length): {longestName.Name}");

        decimal limit = 1000m;
        IEnumerable<Product> expensive = CollectionUtils.Filter(products, p => p.Price > limit);
        Console.WriteLine($"   Filter(p => p.Price > {limit}): {string.Join(", ", CollectionUtils.Map(expensive, p => p.Name))}");

        Predicate<Product> isPeripheral = p => p.Category == "Периферія";
        IEnumerable<Product> peripherals = CollectionUtils.Filter(products, isPeripheral);
        decimal peripheralTotal = 0;
        CollectionUtils.ForEach(peripherals, p => peripheralTotal += p.Price);
        Console.WriteLine($"   Предикат isPeripheral у змінній: {string.Join(", ", CollectionUtils.Map(peripherals, p => p.Name))}, разом {peripheralTotal} грн");
        Console.WriteLine($"   Той самий предикат для одного товару: isPeripheral(Монітор) = {isPeripheral(products[3])}");
    }

    static void CompareWithLinq(List<int> numbers, List<Product> products)
    {
        bool sameMap = CollectionUtils.Map(products, p => p.Name).SequenceEqual(products.Select(p => p.Name));
        bool sameFilter = CollectionUtils.Filter(products, p => p.Price > 1000).SequenceEqual(products.Where(p => p.Price > 1000));
        bool sameMax = CollectionUtils.MaxBy(products, p => p.Price) == products.MaxBy(p => p.Price);
        Console.WriteLine($"   Map і Select однакові: {sameMap}, Filter і Where: {sameFilter}, CollectionUtils.MaxBy і Enumerable.MaxBy: {sameMax}");

        int mapCalls = 0;
        IEnumerable<int> mapped = CollectionUtils.Map(numbers, n =>
        {
            mapCalls++;
            return n * 2;
        });
        Console.WriteLine($"   CollectionUtils.Map: одразу після виклику функція виконалась {mapCalls} разів (результат уже готовий список)");

        int selectCalls = 0;
        IEnumerable<int> selected = numbers.Select(n =>
        {
            selectCalls++;
            return n * 2;
        });
        Console.WriteLine($"   LINQ Select: одразу після виклику функція виконалась {selectCalls} разів (відкладене виконання)");

        List<int> materialized = selected.ToList();
        Console.WriteLine($"   LINQ Select після ToList(): функція виконалась {selectCalls} разів, отримано {materialized.Count} елементів, як і в Map ({mapped.Count()})");
    }

    static void PrintCity(string city)
    {
        Console.WriteLine($"      {city} ({city.Length} літер)");
    }
}
