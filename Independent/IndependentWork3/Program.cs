using System;

namespace IndependentWork4
{
    public class LibraryItem
    {
        private string _uniqueId;
        private string _title;
        private int _publicationYear;
        public string UniqueId => _uniqueId;
        public string Title => _title;
        public int PublicationYear => _publicationYear;

        public LibraryItem(string uniqueId, string title, int publicationYear)
        {
            _uniqueId = uniqueId;
            _title = title;
            _publicationYear = publicationYear;
        }

        public virtual void DisplayDetails()
        {
            Console.WriteLine($"[ID: {UniqueId}] '{Title}' ({PublicationYear} р.)");
        }
    }

    public class Book : LibraryItem
    {
        private string _author;

        public string Author => _author;

        public Book(string uniqueId, string title, int publicationYear, string author)
            : base(uniqueId, title, publicationYear)
        {
            _author = author;
        }

        public override void DisplayDetails()
        {
            Console.WriteLine($"Книга [ID: {UniqueId}] '{Title}' ({PublicationYear} р.) | Автор: {Author}");
        }
    }

    public class Journal : LibraryItem
    {
        private int _issueNumber;

        public int IssueNumber => _issueNumber;

        public Journal(string uniqueId, string title, int publicationYear, int issueNumber)
            : base(uniqueId, title, publicationYear)
        {
            _issueNumber = issueNumber;
        }

        public override void DisplayDetails()
        {
            Console.WriteLine($"[Журнал] [ID: {UniqueId}] '{Title}' ({PublicationYear} р.) | № випуску: {IssueNumber}");
        }
    }

    public class AudioBook : Book
    {
        private string _narrator;

        public string Narrator => _narrator;

        public AudioBook(string uniqueId, string title, int publicationYear, string author, string narrator)
            : base(uniqueId, title, publicationYear, author)
        {
            _narrator = narrator;
        }

        public override void DisplayDetails()
        {
            Console.WriteLine($"[Аудіокнига] [ID: {UniqueId}] '{Title}' ({PublicationYear} р.) | Автор: {Author} | Оповідач: {Narrator}");
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;


            Console.WriteLine("САМОСТІЙНА РОБОТА 4");

            Book book = new Book("BK-001", "Кобзар", 1840, "Тарас Шевченко");
            Journal journal = new Journal("JR-102", "National Geographic", 2023, 5);
            AudioBook audioBook = new AudioBook("AB-999", "Дюна", 1965, "Френк Герберт", "Олександр Завальський");

            LibraryItem[] items = new LibraryItem[] { book, journal, audioBook };

            foreach (var item in items)
            {
                item.DisplayDetails();
            }

        }
    }
}

/* 
================================================================================
ЗВІТ ПРО ВНЕСЕНІ ЗМІНИ ТА АНАЛІЗ НАСЛІДУВАННЯ
================================================================================

1. Які зміни було внесено після модифікації базового класу LibraryItem:
   - У базовий клас LibraryItem додано поле `_uniqueId`, властивість `UniqueId` 
     та новий параметр `uniqueId` у конструктор.
   - Через цю зміну виникла помилка компіляції в усіх похідних класах (`Book` та `Journal`), 
     оскільки їхні конструктори більше не знаходили відповідного конструктора в `base`.
   - У похідних класах `Book` та `Journal` довелося модифікувати конструктори, 
     додавши в параметри `string uniqueId` та передавши його у виклик `base(uniqueId, ...)`.

2. Чому ці зміни були необхідні:
   - В C# конструктор базового класу виконується ПЕРЕД конструктором похідного класу.
   - Якщо базовий клас має лише конструктор із параметрами, всі похідні класи ЗОБОВ'ЯЗАНІ 
     явно передати потрібні параметри через `: base(...)`.

3. Переваги та недоліки наслідування:
   - ПЕРЕВАГИ:
     * Повторне використання коду (похідні класи автоматично отримують `UniqueId`, `Title`, `PublicationYear`).
     * Поліморфізм (можна обробляти книги, журнали та аудіокниги єдиним списком `LibraryItem[]`).
   - НЕДОЛІКИ:
     * Проблема "Тісного зв'язку" (Tight Coupling): Зміна в базовому класі ломає всі похідні класи.
     * Негнучкість: Якщо в майбутньому з'явиться електронна книга, яка має властивість "Формат файлу", 
       але не має фізичного обкладинки, наслідування може створювати зайві або невідповідні поля.

Коли композиція краща за наслідування:
   - Композиція ("має" замість "є") доцільніша, коли поведінка об'єкта може змінюватися 
     динамічно під час виконання програми або коли класи не мають чіткого відношення "є різновидом".
================================================================================
*/