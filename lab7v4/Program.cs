using lab7v4;

Console.WriteLine("Лабораторна робота №7");

var categoryRepo = new InMemoryRepository<Category>();


categoryRepo.Add(new Category { Id = 1, Name = "Ноутбуки", Description = "Портативні комп'ютери" });
categoryRepo.Add(new Category { Id = 2, Name = "Смартфони", Description = "Мобільні телефони" });
categoryRepo.Add(new Category { Id = 3, Name = "Аксесуари", Description = "Навушники та чохли" });

PrintAll(categoryRepo);


Console.WriteLine("\nПошук за ID=2:");
var fetched = categoryRepo.GetById(2);
Console.WriteLine(fetched != null ? fetched.ToString() : "Не знайдено");

Console.WriteLine("\nОновлення категорії ID=2:");
categoryRepo.Update(new Category { Id = 2, Name = "Смартфони та Планшети", Description = "Гаджети" });
PrintAll(categoryRepo);

Console.WriteLine("\nВидалення категорії ID=1:");
categoryRepo.Delete(1);
PrintAll(categoryRepo);

static void PrintAll(InMemoryRepository<Category> repo)
{
    Console.WriteLine("\nПоточний вміст репозиторію:");
    foreach (var item in repo.GetAll())
    {
        Console.WriteLine($" - {item}");
    }
}