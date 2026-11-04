namespace lab11v4;

public class Project
{
    public int Id { get; set; }
    public string Title { get; set; }

    public Project(int id, string title)
    {
        Id = id;
        Title = title;
    }

    public override string ToString() => Title;
}
