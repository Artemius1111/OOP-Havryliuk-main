namespace lab9v4;

public class PageRequest
{
    public int Id { get; }
    public string Method { get; }
    public string Url { get; }

    public PageRequest(int id, string method, string url)
    {
        Id = id;
        Method = method;
        Url = url;
    }

    public override string ToString() => $"#{Id} {Method} {Url}";
}
