using System.Text;
using lab11v4;

Console.OutputEncoding = Encoding.UTF8;

List<Company> companies = CompanyData.Companies;

Console.WriteLine($"Компаній: {companies.Count}, співробітників: {companies.TotalEmployees()}, проєктів: {CompanyData.Projects.Count}");
Console.WriteLine();

Console.WriteLine("1. SelectMany: усі проєкти всіх співробітників усіх компаній");

var allProjects = companies
    .SelectMany(c => c.Employees, (company, employee) => new { Company = company, Employee = employee })
    .SelectMany(x => x.Employee.Projects, (x, project) => new { x.Company, x.Employee, Project = project })
    .ToList();

foreach (var item in allProjects)
{
    Console.WriteLine($"   {item.Project} ({item.Employee}, {item.Company})");
}

int uniqueProjects = allProjects.Select(x => x.Project).Distinct().Count();
Console.WriteLine($"   Усього записів: {allProjects.Count}, різних проєктів: {uniqueProjects}");

int selectCount = companies.Select(c => c.Employees).Count();
int selectManyCount = companies.SelectMany(c => c.Employees).Count();
Console.WriteLine($"   Select повертає {selectCount} списків співробітників, SelectMany повертає {selectManyCount} співробітників");
Console.WriteLine();

Console.WriteLine("2. Пагінація: усі компанії по 2 на сторінку");

const int pageSize = 2;
int totalPages = (int)Math.Ceiling(companies.Count / (double)pageSize);

for (int page = 1; page <= totalPages; page++)
{
    Console.WriteLine($"   Сторінка {page} з {totalPages}:");

    foreach (Company company in companies.Page(page, pageSize))
    {
        Console.WriteLine($"      {company.Name}, співробітників: {company.Employees.Count}");
    }
}
Console.WriteLine();

Console.WriteLine("3. Метод розширення TotalEmployees()");
Console.WriteLine($"   Усі компанії: {companies.TotalEmployees()} співробітників");
Console.WriteLine($"   Компанії на сторінці 1: {companies.Page(1, pageSize).TotalEmployees()} співробітників");

var bigCompanies = companies.Where(c => c.Employees.Count >= 3).ToList();
Console.WriteLine($"   Компанії з 3 і більше співробітниками ({string.Join(", ", bigCompanies)}): {bigCompanies.TotalEmployees()} співробітників");
