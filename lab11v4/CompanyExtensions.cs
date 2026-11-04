namespace lab11v4;

public static class CompanyExtensions
{
    public static int TotalEmployees(this IEnumerable<Company> companies)
    {
        return companies.Sum(c => c.Employees.Count);
    }

    public static IEnumerable<T> Page<T>(this IEnumerable<T> source, int pageNumber, int pageSize)
    {
        return source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }
}
