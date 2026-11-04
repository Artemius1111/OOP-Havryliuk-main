namespace lab10v4;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int AuthorId { get; set; }
    public int Year { get; set; }

    public Book(int id, string title, int authorId, int year)
    {
        Id = id;
        Title = title;
        AuthorId = authorId;
        Year = year;
    }

    public override string ToString() => $"\"{Title}\" ({Year})";
}
