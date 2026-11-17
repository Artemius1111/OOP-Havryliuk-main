using System.Runtime.Serialization;

namespace lab13v4;

[Serializable]
public class ValidationException : Exception
{
    public List<string> Errors { get; } = new List<string>();

    public ValidationException()
        : base("Помилка валідації.")
    {
    }

    public ValidationException(string message)
        : base(message)
    {
    }

    public ValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public ValidationException(List<string> errors)
        : base("Об'єкт не пройшов валідацію: " + string.Join("; ", errors))
    {
        Errors = errors;
    }

    [Obsolete("Конструктор для серіалізації, залишено для сумісності.")]
    protected ValidationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
