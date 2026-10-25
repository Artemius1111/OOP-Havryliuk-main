namespace lab9v4;

public class RequestQueue
{
    private readonly Queue<PageRequest> _queue = new();

    public int Count => _queue.Count;

    public void Enqueue(PageRequest request) => _queue.Enqueue(request);

    public PageRequest Peek() => _queue.Peek();

    public PageRequest Dequeue() => _queue.Dequeue();
}
