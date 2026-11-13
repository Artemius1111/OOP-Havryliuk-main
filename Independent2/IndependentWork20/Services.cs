namespace IndependentWork20;

public class WeatherService
{
    private readonly int _failuresBeforeSuccess;

    public WeatherService(int failuresBeforeSuccess)
    {
        _failuresBeforeSuccess = failuresBeforeSuccess;
    }

    public int CallCount { get; private set; }

    public string GetForecast(string city)
    {
        CallCount++;

        if (CallCount <= _failuresBeforeSuccess)
        {
            throw new TimeoutException($"сервіс погоди не відповів вчасно (виклик №{CallCount})");
        }

        return $"{city}: +3 °C, хмарно, вітер 4 м/с";
    }
}

public class NotificationService
{
    public int CallCount { get; private set; }

    public bool Send(string message)
    {
        CallCount++;
        throw new HttpRequestException($"сервер сповіщень недоступний (виклик №{CallCount})");
    }
}

public class AuthService
{
    public int CallCount { get; private set; }

    public string GetToken(string apiKey)
    {
        CallCount++;

        if (apiKey != "valid-key")
        {
            throw new UnauthorizedAccessException("недійсний API-ключ");
        }

        return "token-123";
    }
}
