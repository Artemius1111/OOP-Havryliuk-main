using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;
using Polly.Timeout;

namespace IndependentWork19;

public static class ResilienceScenarios
{
    public static void RetryWithExponentialBackoff()
    {
        ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>(),
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(300),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                OnRetry = args =>
                {
                    Console.WriteLine($"      Retry: спроба {args.AttemptNumber + 1} невдала ({args.Outcome.Exception?.Message}), повтор через {args.RetryDelay.TotalMilliseconds:F0} мс");
                    return default;
                }
            })
            .Build();

        var api = new ExchangeRateApi(failuresBeforeSuccess: 2);
        try
        {
            decimal rate = pipeline.Execute(() => api.GetRate("USD"));
            Console.WriteLine($"   Результат: курс USD {rate} грн, звернень до API: {api.CallCount}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"   Остаточна невдача: {ex.Message}");
        }

        var apiForWrongCurrency = new ExchangeRateApi(failuresBeforeSuccess: 0);
        try
        {
            pipeline.Execute(() => apiForWrongCurrency.GetRate("XYZ"));
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"   ArgumentException не входить у ShouldHandle, тому не повторюється: {ex.Message}, звернень: {apiForWrongCurrency.CallCount}");
        }
    }

    public static void CircuitBreaker()
    {
        ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>(),
                FailureRatio = 0.5,
                MinimumThroughput = 3,
                SamplingDuration = TimeSpan.FromSeconds(10),
                BreakDuration = TimeSpan.FromSeconds(2),
                OnOpened = _ =>
                {
                    Console.WriteLine("      Circuit Breaker: Closed -> Open, наступні 2 с запити блокуються");
                    return default;
                },
                OnHalfOpened = _ =>
                {
                    Console.WriteLine("      Circuit Breaker: Open -> Half-Open, пропускаємо один пробний запит");
                    return default;
                },
                OnClosed = _ =>
                {
                    Console.WriteLine("      Circuit Breaker: Half-Open -> Closed, шлюз знову працює");
                    return default;
                }
            })
            .Build();

        var gateway = new PaymentGateway(failingCalls: 3);

        for (int payment = 1; payment <= 7; payment++)
        {
            if (payment == 6)
            {
                Console.WriteLine("      Чекаємо 2,5 с, поки мине BreakDuration");
                Thread.Sleep(TimeSpan.FromSeconds(2.5));
            }

            decimal amount = 250m * payment;
            try
            {
                string receipt = pipeline.Execute(() => gateway.Charge(amount));
                Console.WriteLine($"   Платіж {payment}: {receipt}");
            }
            catch (BrokenCircuitException)
            {
                Console.WriteLine($"   Платіж {payment}: відхилено одразу, ланцюг розімкнено, шлюз не викликався");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"   Платіж {payment}: помилка, {ex.Message}");
            }

            Thread.Sleep(200);
        }

        Console.WriteLine($"   Усього платежів 7, звернень до шлюзу: {gateway.CallCount}");
    }

    public static void TimeoutRetryFallback()
    {
        var cachedRecommendations = new List<string> { "Популярне: Смартфон", "Популярне: Ноутбук", "Популярне: Навушники" };

        ResiliencePipeline<List<string>> pipeline = new ResiliencePipelineBuilder<List<string>>()
            .AddFallback(new FallbackStrategyOptions<List<string>>
            {
                ShouldHandle = new PredicateBuilder<List<string>>().Handle<TimeoutRejectedException>(),
                FallbackAction = _ => Outcome.FromResultAsValueTask(cachedRecommendations),
                OnFallback = _ =>
                {
                    Console.WriteLine("      Fallback: усі спроби вичерпано, повертаємо популярні товари з кешу");
                    return default;
                }
            })
            .AddRetry(new RetryStrategyOptions<List<string>>
            {
                ShouldHandle = new PredicateBuilder<List<string>>().Handle<TimeoutRejectedException>(),
                MaxRetryAttempts = 2,
                Delay = TimeSpan.FromMilliseconds(200),
                BackoffType = DelayBackoffType.Constant,
                OnRetry = args =>
                {
                    Console.WriteLine($"      Retry: повтор №{args.AttemptNumber + 1} через {args.RetryDelay.TotalMilliseconds:F0} мс");
                    return default;
                }
            })
            .AddTimeout(new TimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromMilliseconds(500),
                OnTimeout = _ =>
                {
                    Console.WriteLine("      Timeout: відповіді немає довше за 500 мс, спробу перервано");
                    return default;
                }
            })
            .Build();

        Console.WriteLine("   Випадок А: сервіс повільний лише перші 2 рази");
        var slowService = new RecommendationService(slowCalls: 2);
        List<string> recommendations = pipeline.Execute(token => slowService.GetRecommendations(token));
        Console.WriteLine($"   Результат: {string.Join(", ", recommendations)}, викликів сервісу: {slowService.CallCount}");

        Console.WriteLine("   Випадок Б: сервіс постійно повільний");
        var brokenService = new RecommendationService(slowCalls: int.MaxValue);
        List<string> fallback = pipeline.Execute(token => brokenService.GetRecommendations(token));
        Console.WriteLine($"   Результат: {string.Join(", ", fallback)}, викликів сервісу: {brokenService.CallCount}");
    }
}
