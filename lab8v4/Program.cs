using lab14v4;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("Лабораторна робота №8");
Console.WriteLine("Варіант 4: CollectionHelper<T>.ForEach\n");

// 1. List<string>: вивести кожен елемент у консоль
var fruits = new List<string> { "Яблуко", "Банан", "Вишня", "Груша" };

Console.WriteLine("1) List<string> - вивід кожного елемента:");
CollectionHelper<string>.ForEach(fruits, fruit => Console.WriteLine($"   {fruit}"));

// 2. List<int>: вивести квадрат кожного числа та підрахувати суму через замикання
var numbers = new List<int> { 1, 2, 3, 4, 5 };
int sum = 0;

Console.WriteLine("\n2) List<int> - квадрати чисел та сума:");
CollectionHelper<int>.ForEach(numbers, n =>
{
    Console.WriteLine($"   {n}^2 = {n * n}");
    sum += n; // лямбда захоплює локальну змінну sum (замикання)
});
Console.WriteLine($"   Сума чисел: {sum}");

// 3. List<Product>: збільшити ціну кожного продукту на 10%
var products = new List<Product>
{
    new Product("Ноутбук", 25000m),
    new Product("Смартфон", 15000m),
    new Product("Навушники", 1200m)
};

Console.WriteLine("\n3) List<Product> - до зміни цін:");
CollectionHelper<Product>.ForEach(products, p => Console.WriteLine($"   {p}"));

CollectionHelper<Product>.ForEach(products, p => p.Price *= 1.10m);

Console.WriteLine("\n   Ціни після збільшення на 10%:");
CollectionHelper<Product>.ForEach(products, p => Console.WriteLine($"   {p}"));

// 4. Метод, переданий як група методів (замість лямбди)
Console.WriteLine("\n4) Group of methods - Console.WriteLine як Action<string>:");
CollectionHelper<string>.ForEach(fruits, Console.WriteLine);

// 5. Перевірка обробки null
Console.WriteLine("\n5) Перевірка null-аргументів:");
try
{
    CollectionHelper<int>.ForEach(null!, n => Console.WriteLine(n));
}
catch (ArgumentNullException ex)
{
    Console.WriteLine($"   Перехоплено ArgumentNullException: параметр '{ex.ParamName}'");
}
