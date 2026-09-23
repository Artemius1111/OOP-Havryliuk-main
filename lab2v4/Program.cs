using System;

namespace lab2v4
{
    public class Teacher
    {

        private string _name;
        private string _subject;
        private int _experienceYears;

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

        public int ExperienceYears
        {
            get => _experienceYears;
            set
            {
              
                if (value < 0)
                {
                    Console.WriteLine("Помилка: досвід не може бути від'ємним! Встановлено 0.");
                    _experienceYears = 0;
                }
                else
                {
                    _experienceYears = value;
                }
            }
        }

        public Teacher() : this("New Teacher", "General", 0)
        {
            Console.WriteLine("Викликано тіло конструктора за замовчуванням");
        }


        public Teacher(string name, string subject, int experienceYears)
        {
            Name = name;
            Subject = subject;
            ExperienceYears = experienceYears;
            Console.WriteLine($"Викликано параметризований конструктор для: {Name}");
        }

    
        public void Introduce()
        {
            Console.WriteLine($"Викладач: {Name} | Предмет: {Subject} | Стаж: {ExperienceYears} років");
        }


        ~Teacher()
        {
            Console.WriteLine($"[Деструктор] Об'єкт Teacher ({_name}) видалено з пам'яті.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Creating objects ===");
            CreateAndUseTeachers();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("Програму завершено.");
        }

        static void CreateAndUseTeachers()
        {
            Teacher t1 = new Teacher();
            t1.Introduce();

            Console.WriteLine();

            Teacher t2 = new Teacher("Олександр Іванович", "Програмування", 10);
            t2.Introduce();

            Console.WriteLine();

            Teacher t3 = new Teacher("Марія Петрівна", "Математика", -3);
            t3.Introduce();
        }
    }
}