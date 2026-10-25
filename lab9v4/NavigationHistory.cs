namespace lab9v4;

public class NavigationHistory
{
    private readonly Stack<string> _history = new();

    public int Count => _history.Count;

    public string? Current => _history.Count > 0 ? _history.Peek() : null;

    public void Visit(string url) => _history.Push(url);

    public string? Back()
    {
        if (_history.Count <= 1)
        {
            return null;
        }

        _history.Pop();
        return _history.Peek();
    }

    public IEnumerable<string> GetAll() => _history;
}
