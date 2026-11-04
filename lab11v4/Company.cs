namespace lab11v4;

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Employee> Employees { get; set; }

    public Company(int id, string name, List<Employee> employees)
    {
        Id = id;
        Name = name;
        Employees = employees;
    }

    public override string ToString() => Name;
}
