namespace lab14v4;

/// <summary>
/// Статичний узагальнений клас-утиліта для роботи з колекціями.
/// Варіант 4: метод ForEach.
/// </summary>
/// <typeparam name="T">Тип елементів колекції.</typeparam>
public static class CollectionHelper<T>
{
    /// <summary>
    /// Виконує дію <paramref name="action"/> для кожного елемента колекції.
    /// Реалізовано власним циклом, без використання готових методів
    /// на кшталт List&lt;T&gt;.ForEach.
    /// </summary>
    /// <param name="collection">Колекція, елементи якої обробляються.</param>
    /// <param name="action">Дія, яка виконується для кожного елемента.</param>
    /// <exception cref="ArgumentNullException">
    /// Якщо <paramref name="collection"/> або <paramref name="action"/> дорівнює null.
    /// </exception>
    public static void ForEach(IEnumerable<T> collection, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(action);

        foreach (T item in collection)
        {
            action(item);
        }
    }
}
