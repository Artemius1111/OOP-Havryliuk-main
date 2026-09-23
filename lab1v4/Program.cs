namespace lab1v4;

public class Teacher
{

    private string _name;
    private string _subject;
    private int _experience;

    public string Name
    {
        get => _name;
        set => _name = value;
    }

    public string Subject
    {
        get => _subject;
        set => _subject = value;
    }

    public int Experience
    {
        get => _experience;
        set => _experience = value >= 0 ? value : 0; // Перевірка на від'ємні значення
    }


    public Teacher(string name, string subject, int experience)
    {
        _name = name;
        _subject = subject;
        Experience = experience;
    }

    ~Teacher()
    {
        Console.WriteLine($"Об'єкт класу Teacher для {_name} видалено.");
    }

    public void Introduce()
    {
        Console.WriteLine($"Викладач: {_name} | Дисципліна: {_subject} | Стаж: {_experience} р.");
    }
}

internal class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Teacher teacher1 = new Teacher("Олександр Петрович", "Об'єктно-орієнтоване програмування", 12);
        Teacher teacher2 = new Teacher("Марія Іванівна", "Вища математика", 8);
        Teacher teacher3 = new Teacher("Сергій Васильович", "Комп'ютерні мережі", 15);

        teacher1.Introduce();
        teacher2.Introduce();
        teacher3.Introduce();
    }
}
