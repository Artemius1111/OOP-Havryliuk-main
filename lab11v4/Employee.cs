namespace lab11v4;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Project> Projects { get; set; }

    public Employee(int id, string name, List<Project> projects)
    {
        Id = id;
        Name = name;
        Projects = projects;
    }

    public override string ToString() => Name;
}
