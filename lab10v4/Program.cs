using System.Text;
using lab10v4;

Console.OutputEncoding = Encoding.UTF8;

List<Author> authors = LibraryData.Authors;
List<Book> books = LibraryData.Books;

Console.WriteLine($"Авторів: {authors.Count}, книг: {books.Count}");
Console.WriteLine();

Console.WriteLine("1. Книги автора \"Stephen King\", видані після 1990 року");

var q1Query = Benchmark.Run("Q1 Query", () =>
    (from b in books
     join a in authors on b.AuthorId equals a.Id
     where a.Name == "Stephen King" && b.Year > 1990
     orderby b.Year
     select b).ToList());

var q1Method = Benchmark.Run("Q1 Method", () =>
    books.Join(authors,
               b => b.AuthorId,
               a => a.Id,
               (b, a) => new { Book = b, Author = a })
         .Where(x => x.Author.Name == "Stephen King" && x.Book.Year > 1990)
         .OrderBy(x => x.Book.Year)
         .Select(x => x.Book)
         .ToList());

PrintList(q1Method, b => b.ToString());
PrintSame(q1Query.SequenceEqual(q1Method));

Console.WriteLine("2. Книги разом з їхніми авторами");

var q2Query = Benchmark.Run("Q2 Query", () =>
    (from b in books
     join a in authors on b.AuthorId equals a.Id
     select new { b.Title, Author = a.Name }).ToList());

var q2Method = Benchmark.Run("Q2 Method", () =>
    books.Join(authors,
               b => b.AuthorId,
               a => a.Id,
               (b, a) => new { b.Title, Author = a.Name })
         .ToList());

PrintList(q2Method, x => $"{x.Title} - {x.Author}");
PrintSame(q2Query.SequenceEqual(q2Method));

Console.WriteLine("3. Кількість книг для кожного автора");

var q3Query = Benchmark.Run("Q3 Query", () =>
    (from a in authors
     join b in books on a.Id equals b.AuthorId into authorBooks
     let count = authorBooks.Count()
     orderby count descending, a.Name
     select new { Author = a.Name, Count = count }).ToList());

var q3Method = Benchmark.Run("Q3 Method", () =>
    authors.GroupJoin(books,
                      a => a.Id,
                      b => b.AuthorId,
                      (a, authorBooks) => new { Author = a.Name, Count = authorBooks.Count() })
           .OrderByDescending(x => x.Count)
           .ThenBy(x => x.Author)
           .ToList());

PrintList(q3Method, x => $"{x.Author}: {x.Count}");
PrintSame(q3Query.SequenceEqual(q3Method));

Console.WriteLine("4. Найстаріша книга");

var q4Query = Benchmark.Run("Q4 Query", () =>
    (from b in books
     orderby b.Year
     select b).First());

var q4Method = Benchmark.Run("Q4 Method", () =>
    books.OrderBy(b => b.Year).First());

var q4MinBy = Benchmark.Run("Q4 MinBy", () =>
    books.MinBy(b => b.Year)!);

Author oldestAuthor = authors.First(a => a.Id == q4MinBy.AuthorId);
Console.WriteLine($"   {q4MinBy}, автор: {oldestAuthor.Name}");
PrintSame(q4Query == q4Method && q4Method == q4MinBy);

Benchmark.PrintSummary();

static void PrintList<T>(IEnumerable<T> items, Func<T, string> format)
{
    foreach (var item in items)
    {
        Console.WriteLine($"   {format(item)}");
    }
}

static void PrintSame(bool same)
{
    Console.WriteLine($"   Результати обох синтаксисів однакові: {(same ? "так" : "ні")}");
    Console.WriteLine();
}
