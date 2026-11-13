namespace IndependentWork17;

public static class SalesDataGenerator
{
    private static readonly (string Category, decimal MinPrice, decimal MaxPrice, string[] Names)[] Catalog =
    {
        ("Електроніка", 1500, 40000, new[] { "Смартфон", "Ноутбук", "Планшет", "Навушники", "Смарт-годинник", "Повербанк", "Монітор", "Колонка", "Роутер", "Фотоапарат" }),
        ("Книги", 150, 900, new[] { "Роман", "Підручник C#", "Детектив", "Комікс", "Енциклопедія", "Збірка поезії", "Фантастика", "Кулінарна книга", "Біографія", "Атлас" }),
        ("Одяг", 300, 4000, new[] { "Футболка", "Худі", "Джинси", "Куртка", "Кросівки", "Шапка", "Шарф", "Сорочка", "Світшот", "Рукавиці" }),
        ("Дім і кухня", 250, 6000, new[] { "Чайник", "Кавоварка", "Сковорода", "Блендер", "Настільна лампа", "Плед", "Подушка", "Набір чашок", "Тостер", "Органайзер" }),
        ("Спорт", 100, 15000, new[] { "М'яч", "Гантелі", "Килимок для йоги", "Скакалка", "Велосипед", "Ракетка", "Пляшка для води", "Рюкзак", "Еспандер", "Самокат" }),
        ("Іграшки", 150, 3500, new[] { "Конструктор", "Пазл", "Настільна гра", "Лялька", "Машинка", "Кубик Рубіка", "Плюшевий ведмідь", "Дрон", "Залізниця", "Набір для творчості" })
    };

    private static readonly string[] FirstNames =
    {
        "Олександр", "Марія", "Андрій", "Олена", "Дмитро", "Ірина", "Максим", "Наталія", "Богдан", "Юлія",
        "Тарас", "Катерина", "Василь", "Оксана", "Роман", "Анастасія", "Віктор", "Тетяна", "Ігор", "Христина"
    };

    private static readonly string[] LastNames =
    {
        "Ковальчук", "Шевчук", "Бондаренко", "Мельник", "Кравчук", "Ткаченко", "Олійник", "Савчук", "Поліщук", "Марченко",
        "Лисенко", "Руденко", "Гончар", "Козак", "Павлюк", "Сидоренко", "Литвин", "Мороз", "Климчук", "Вовк"
    };

    private static readonly string[] Cities = { "Рівне", "Київ", "Львів", "Луцьк", "Житомир", "Тернопіль", "Хмельницький", "Одеса" };

    public static SalesData Generate(int salesCount = 600, int seed = 2026)
    {
        var random = new Random(seed);

        var products = new List<Product>();
        for (int i = 0; i < Catalog.Length * 10; i++)
        {
            var (category, minPrice, maxPrice, names) = Catalog[i % Catalog.Length];
            decimal price = Math.Round(minPrice + (decimal)random.NextDouble() * (maxPrice - minPrice), 0);

            products.Add(new Product
            {
                Id = i + 1,
                Name = names[i / Catalog.Length],
                Category = category,
                UnitPrice = price
            });
        }

        var customers = new List<Customer>();
        for (int i = 0; i < 80; i++)
        {
            customers.Add(new Customer
            {
                Id = i + 1,
                Name = $"{FirstNames[random.Next(FirstNames.Length)]} {LastNames[random.Next(LastNames.Length)]}",
                City = Cities[random.Next(Cities.Length)]
            });
        }

        var firstDay = new DateTime(2025, 1, 1);
        int totalDays = (new DateTime(2026, 7, 1) - firstDay).Days;

        var sales = new List<Sale>(salesCount);
        for (int i = 0; i < salesCount; i++)
        {
            int productIndex = (int)(Math.Pow(random.NextDouble(), 2) * products.Count);

            sales.Add(new Sale
            {
                Id = i + 1,
                ProductId = products[productIndex].Id,
                CustomerId = customers[random.Next(customers.Count)].Id,
                Quantity = random.Next(1, 6),
                SaleDate = firstDay.AddDays(random.Next(totalDays))
            });
        }

        return new SalesData(products, customers, sales);
    }
}
