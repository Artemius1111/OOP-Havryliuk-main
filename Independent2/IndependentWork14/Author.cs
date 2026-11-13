namespace IndependentWork14;

public class Author
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public override string ToString() => $"{Name} ({Email})";
}

public class AuthorNameComparer : IEqualityComparer<Author>
{
    public bool Equals(Author? x, Author? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        return string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(Author obj)
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name);
    }
}
