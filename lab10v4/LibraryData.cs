namespace lab10v4;

public static class LibraryData
{
    public static List<Author> Authors { get; } = new()
    {
        new Author(1, "Stephen King", "USA"),
        new Author(2, "George Orwell", "United Kingdom"),
        new Author(3, "J. R. R. Tolkien", "United Kingdom"),
        new Author(4, "J. K. Rowling", "United Kingdom"),
        new Author(5, "Taras Shevchenko", "Ukraine"),
        new Author(6, "Lesya Ukrainka", "Ukraine"),
        new Author(7, "Ivan Franko", "Ukraine"),
        new Author(8, "Agatha Christie", "United Kingdom"),
        new Author(9, "Ernest Hemingway", "USA"),
        new Author(10, "Gabriel Garcia Marquez", "Colombia"),
        new Author(11, "Jane Austen", "United Kingdom"),
        new Author(12, "Haruki Murakami", "Japan"),
        new Author(13, "Andrzej Sapkowski", "Poland")
    };

    public static List<Book> Books { get; } = new()
    {
        new Book(1, "Carrie", 1, 1974),
        new Book(2, "The Shining", 1, 1977),
        new Book(3, "It", 1, 1986),
        new Book(4, "The Green Mile", 1, 1996),
        new Book(5, "Dreamcatcher", 1, 2001),
        new Book(6, "11/22/63", 1, 2011),
        new Book(7, "Doctor Sleep", 1, 2013),
        new Book(8, "The Institute", 1, 2019),
        new Book(9, "Nineteen Eighty-Four", 2, 1949),
        new Book(10, "Animal Farm", 2, 1945),
        new Book(11, "The Hobbit", 3, 1937),
        new Book(12, "The Lord of the Rings", 3, 1954),
        new Book(13, "Harry Potter and the Philosopher's Stone", 4, 1997),
        new Book(14, "Kobzar", 5, 1840),
        new Book(15, "The Forest Song", 6, 1911),
        new Book(16, "Zakhar Berkut", 7, 1883),
        new Book(17, "Murder on the Orient Express", 8, 1934),
        new Book(18, "The Old Man and the Sea", 9, 1952),
        new Book(19, "One Hundred Years of Solitude", 10, 1967),
        new Book(20, "Pride and Prejudice", 11, 1813),
        new Book(21, "Norwegian Wood", 12, 1987),
        new Book(22, "Kafka on the Shore", 12, 2002)
    };
}
