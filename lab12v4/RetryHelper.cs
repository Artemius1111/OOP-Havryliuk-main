namespace lab12v4;

public static class RetryHelper
{
    private static readonly TimeSpan DefaultInitialDelay = TimeSpan.FromMilliseconds(500);

    public static T ExecuteWithRetry<T>(Func<T> operation, int retryCount = 3, TimeSpan initialDelay = default, Func<Exception, bool>? shouldRetry = null)
    {
        ArgumentNullException.ThrowIfNull(operation);

        if (retryCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(retryCount), "Кількість повторів не може бути від'ємною");
        }

        if (initialDelay <= TimeSpan.Zero)
        {
            initialDelay = DefaultInitialDelay;
        }

        int totalAttempts = retryCount + 1;
        int retryAttempt = 0;

        while (true)
        {
            int attemptNumber = retryAttempt + 1;

            try
            {
                Log($"Спроба {attemptNumber} з {totalAttempts}");
                T result = operation();
                Log($"Спроба {attemptNumber}: успіх");
                return result;
            }
            catch (Exception ex)
            {
                Log($"Спроба {attemptNumber}: невдача, {ex.GetType().Name}: {ex.Message}");

                if (shouldRetry != null && !shouldRetry(ex))
                {
                    Log($"shouldRetry повернув false для {ex.GetType().Name}, повтор не виконується");
                    throw;
                }

                if (retryAttempt >= retryCount)
                {
                    Log($"Вичерпано ліміт спроб ({totalAttempts}), операцію зупинено");
                    throw;
                }

                TimeSpan delay = initialDelay * Math.Pow(2, retryAttempt);
                Log($"Повтор №{retryAttempt + 1} через {delay.TotalMilliseconds} мс");
                Thread.Sleep(delay);
                retryAttempt++;
            }
        }
    }

    private static void Log(string message)
    {
        Console.WriteLine($"   {message}");
    }
}