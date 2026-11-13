namespace IndependentWork20;

public class RetryLimitExceededException : Exception
{
    public int Attempts { get; }

    public RetryLimitExceededException()
        : base("Ліміт повторних спроб вичерпано")
    {
    }

    public RetryLimitExceededException(string message)
        : base(message)
    {
    }

    public RetryLimitExceededException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public RetryLimitExceededException(int attempts, Exception lastException)
        : base($"Операцію не вдалося виконати, кількість спроб: {attempts}", lastException)
    {
        Attempts = attempts;
    }
}
