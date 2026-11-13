using System.Collections.Concurrent;

namespace IndependentWork18;

public static class SideEffectsDemo
{
    public static void Run(List<int> data)
    {
        int expected = data.Count(n => n % 3 == 0);
        Console.WriteLine($"   Чисел, кратних 3 (послідовний підрахунок): {expected}");

        Console.WriteLine("   Проблема: спільний лічильник unsafeCounter++ всередині ForAll без синхронізації");
        for (int attempt = 1; attempt <= 3; attempt++)
        {
            int unsafeCounter = 0;
            data.AsParallel().ForAll(n =>
            {
                if (n % 3 == 0)
                {
                    unsafeCounter++;
                }
            });
            Console.WriteLine($"      Запуск {attempt}: {unsafeCounter}, втрачено {expected - unsafeCounter}");
        }

        Console.WriteLine("   Проблема: List<int>.Add з кількох потоків одночасно");
        for (int attempt = 1; attempt <= 3; attempt++)
        {
            var unsafeList = new List<int>();
            try
            {
                data.AsParallel().ForAll(n =>
                {
                    if (n % 3 == 0)
                    {
                        unsafeList.Add(n);
                    }
                });
                Console.WriteLine($"      Запуск {attempt}: у списку {unsafeList.Count} елементів, втрачено {expected - unsafeList.Count}");
            }
            catch (AggregateException ex)
            {
                Console.WriteLine($"      Запуск {attempt}: виняток {ex.InnerExceptions[0].GetType().Name}, список пошкоджено");
            }
        }

        Console.WriteLine("   Виправлення:");

        int interlockedCounter = 0;
        data.AsParallel().ForAll(n =>
        {
            if (n % 3 == 0)
            {
                Interlocked.Increment(ref interlockedCounter);
            }
        });
        Console.WriteLine($"      Interlocked.Increment: {interlockedCounter}, збігається: {interlockedCounter == expected}");

        var lockedList = new List<int>();
        object sync = new();
        data.AsParallel().ForAll(n =>
        {
            if (n % 3 == 0)
            {
                lock (sync)
                {
                    lockedList.Add(n);
                }
            }
        });
        Console.WriteLine($"      lock навколо List<int>.Add: {lockedList.Count}, збігається: {lockedList.Count == expected}");

        var bag = new ConcurrentBag<int>();
        data.AsParallel().ForAll(n =>
        {
            if (n % 3 == 0)
            {
                bag.Add(n);
            }
        });
        Console.WriteLine($"      ConcurrentBag<int>: {bag.Count}, збігається: {bag.Count == expected}");

        int pureCount = data.AsParallel().Count(n => n % 3 == 0);
        List<int> pureList = data.AsParallel().Where(n => n % 3 == 0).ToList();
        Console.WriteLine($"      Запит без побічних ефектів (Count і Where + ToList): {pureCount} і {pureList.Count}, збігається: {pureCount == expected && pureList.Count == expected}");
    }
}
