namespace IndependentWork19;

public class ExchangeRateApi
{
    private readonly int _failuresBeforeSuccess;

    public ExchangeRateApi(int failuresBeforeSuccess)
    {
        _failuresBeforeSuccess = failuresBeforeSuccess;
    }

    public int CallCount { get; private set; }

    public decimal GetRate(string currency)
    {
        CallCount++;
        Console.WriteLine($"      API курсів: запит №{CallCount}, валюта {currency}");

        if (currency != "USD" && currency != "EUR")
        {
            throw new ArgumentException($"Невідома валюта {currency}");
        }

        if (CallCount <= _failuresBeforeSuccess)
        {
            throw new HttpRequestException($"503 Service Unavailable, запит №{CallCount}");
        }

        return currency == "USD" ? 41.85m : 48.70m;
    }
}

public class PaymentGateway
{
    private readonly int _failingCalls;

    public PaymentGateway(int failingCalls)
    {
        _failingCalls = failingCalls;
    }

    public int CallCount { get; private set; }

    public string Charge(decimal amount)
    {
        CallCount++;

        if (CallCount <= _failingCalls)
        {
            throw new HttpRequestException($"шлюз не відповідає, звернення №{CallCount}");
        }

        return $"оплату {amount} грн проведено, звернення №{CallCount}";
    }
}

public class RecommendationService
{
    private readonly int _slowCalls;

    public RecommendationService(int slowCalls)
    {
        _slowCalls = slowCalls;
    }

    public int CallCount { get; private set; }

    public List<string> GetRecommendations(CancellationToken token)
    {
        CallCount++;
        TimeSpan responseTime = CallCount <= _slowCalls ? TimeSpan.FromSeconds(3) : TimeSpan.FromMilliseconds(150);
        Console.WriteLine($"      Сервіс рекомендацій: виклик №{CallCount}, відповідь займе {responseTime.TotalMilliseconds} мс");

        token.WaitHandle.WaitOne(responseTime);
        token.ThrowIfCancellationRequested();

        return new List<string> { "Чохол для смартфона", "Навушники", "Повербанк" };
    }
}
