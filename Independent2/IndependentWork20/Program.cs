using System.Text;

namespace IndependentWork20;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        var policy = new RetryPolicy(maxRetries: 4, initialDelay: TimeSpan.FromMilliseconds(200), jitterFactor: 0.3);
        Func<Exception, bool> isTransient = ex => ex is TimeoutException || ex is HttpRequestException;

        Console.WriteLine("RetryPolicy: maxRetries = 4, initialDelay = 200 мс, jitterFactor = 0.3 (±30%)");
        Console.WriteLine();

        Console.WriteLine("1. Успіх після кількох спроб: сервіс погоди не відповідає перші 3 рази");
        var weather = new WeatherService(failuresBeforeSuccess: 3);
        string forecast = policy.Execute(() => weather.GetForecast("Рівне"), isTransient);
        Console.WriteLine($"   Результат: {forecast}, викликів сервісу: {weather.CallCount}");
        Console.WriteLine();

        Console.WriteLine("2. Остаточна невдача: сервер сповіщень недоступний постійно");
        var notifications = new NotificationService();
        try
        {
            policy.Execute(() => notifications.Send("Ваше замовлення відправлено"), isTransient);
        }
        catch (RetryLimitExceededException ex)
        {
            Console.WriteLine($"   RetryLimitExceededException: {ex.Message}");
            Console.WriteLine($"   Остання причина (InnerException): {ex.InnerException?.Message}, викликів сервісу: {notifications.CallCount}");
        }
        Console.WriteLine();

        Console.WriteLine("3. shouldRetry: помилка авторизації не є тимчасовою, тому не повторюється");
        var auth = new AuthService();
        try
        {
            policy.Execute(() => auth.GetToken("wrong-key"), isTransient);
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"   UnauthorizedAccessException: {ex.Message}, викликів сервісу: {auth.CallCount}");
        }
        Console.WriteLine();

        Console.WriteLine("4. Розкид затримок через jitter (по 5 обчислень для кожного повтору)");
        for (int retryAttempt = 0; retryAttempt < 4; retryAttempt++)
        {
            double baseMs = 200 * Math.Pow(2, retryAttempt);
            var delays = Enumerable.Range(0, 5).Select(_ => policy.CalculateDelay(retryAttempt).TotalMilliseconds.ToString("F0"));
            Console.WriteLine($"   Повтор №{retryAttempt + 1}: база {baseMs:F0} мс, допустимо {baseMs * 0.7:F0}..{baseMs * 1.3:F0} мс, отримано: {string.Join(", ", delays)}");
        }
    }
}
