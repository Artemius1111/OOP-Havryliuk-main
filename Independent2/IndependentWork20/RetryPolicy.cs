namespace IndependentWork20;

public class RetryPolicy
{
    private readonly int _maxRetries;
    private readonly TimeSpan _initialDelay;
    private readonly double _jitterFactor;
    private readonly Random _random = new();

    public RetryPolicy(int maxRetries, TimeSpan initialDelay, double jitterFactor = 0.2)
    {
        if (maxRetries < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRetries), "Кількість повторів не може бути від'ємною");
        }

        if (initialDelay < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(initialDelay), "Початкова затримка не може бути від'ємною");
        }

        if (jitterFactor < 0 || jitterFactor > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(jitterFactor), "Коефіцієнт jitter має бути від 0 до 1");
        }

        _maxRetries = maxRetries;
        _initialDelay = initialDelay;
        _jitterFactor = jitterFactor;
    }

    public T Execute<T>(Func<T> operation, Func<Exception, bool>? shouldRetry = null)
    {
        ArgumentNullException.ThrowIfNull(operation);

        int totalAttempts = _maxRetries + 1;
        int retryAttempt = 0;

        while (true)
        {
            int attemptNumber = retryAttempt + 1;

            try
            {
                Console.WriteLine($"      Спроба {attemptNumber} з {totalAttempts}");
                T result = operation();
                Console.WriteLine($"      Спроба {attemptNumber}: успіх");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"      Спроба {attemptNumber}: невдача, {ex.GetType().Name}: {ex.Message}");

                if (shouldRetry != null && !shouldRetry(ex))
                {
                    Console.WriteLine($"      shouldRetry повернув false для {ex.GetType().Name}, повтор не виконується");
                    throw;
                }

                if (retryAttempt >= _maxRetries)
                {
                    Console.WriteLine($"      Вичерпано ліміт повторів ({_maxRetries}), операцію зупинено");
                    throw new RetryLimitExceededException(totalAttempts, ex);
                }

                TimeSpan baseDelay = _initialDelay * Math.Pow(2, retryAttempt);
                TimeSpan delay = CalculateDelay(retryAttempt);
                Console.WriteLine($"      Повтор №{retryAttempt + 1} через {delay.TotalMilliseconds:F0} мс (без jitter {baseDelay.TotalMilliseconds:F0} мс)");

                Thread.Sleep(delay);
                retryAttempt++;
            }
        }
    }

    public TimeSpan CalculateDelay(int retryAttempt)
    {
        return _initialDelay * Math.Pow(2, retryAttempt) * (1 + _random.NextDouble() * _jitterFactor * 2 - _jitterFactor);
    }
}
