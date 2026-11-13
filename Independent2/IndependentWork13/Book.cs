namespace IndependentWork13;

public record Book(string Title, string Author)
{
    public override string ToString() => $"«{Title}», {Author}";
}
