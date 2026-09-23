using System;

namespace Lab4Variant4
{
    public class Animal
    {
        private string _name;
        private int _age;

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public int Age
        {
            get => _age;
            set => _age = value;
        }

        public Animal(string name, int age)
        {
            _name = name;
            _age = age;
        }

        public virtual void MakeSound()
        {
            Console.WriteLine($"{Name} видає загальний звук тварини.");
        }

        public string GetSpecies()
        {
            return "Базовий вид: Тварина (Animal)";
        }
    }

    public class Dog : Animal
    {
        public string Breed { get; set; }

        public Dog(string name, int age, string breed) : base(name, age)
        {
            Breed = breed;
        }

        public override void MakeSound()
        {
            Console.WriteLine($"{Name} (порола: {Breed}) голосно гавкає: Гав-гав!");
        }

        public void Fetch()
        {
            Console.WriteLine($"{Name} біжить і приносить паличку!");
        }

        public new string GetSpecies()
        {
            return $"Похідний вид: Собака ({Breed})";
        }
    }

    public class Cat : Animal
    {
        public bool IsIndoor { get; set; }

        public Cat(string name, int age, bool isIndoor) : base(name, age)
        {
            IsIndoor = isIndoor;
        }

        public override void MakeSound()
        {
            string type = IsIndoor ? "домашня" : "вулична";
            Console.WriteLine($"{Name} ({type} кішка) лагідно нявкає: Мяу-мяу!");
        }

        public void Purr()
        {
            Console.WriteLine($"{Name} заплющує очі й муркоче: Мррр-мррр...");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("1. Створення об'єктів та виклики власних методів");
            Animal genericAnimal = new Animal("Невідома істота", 4);
            Dog dog = new Dog("Рекс", 3, "Вівчарка");
            Cat cat = new Cat("Мурчик", 2, true);

            genericAnimal.MakeSound();
            dog.MakeSound();
            dog.Fetch();
            cat.MakeSound();
            cat.Purr();

            Console.WriteLine("\n2. Демонстрація Поліморфізму");
            Animal[] animals = new Animal[] { genericAnimal, dog, cat };
            foreach (Animal a in animals)
            {
                a.MakeSound();
            }

            Console.WriteLine("\n3. Демонстрація різниці override vs new");
            Console.WriteLine($"Виклик через посилання Dog:    {dog.GetSpecies()}");

            Animal animalRef = dog; 
            Console.WriteLine($"Виклик через посилання Animal: {animalRef.GetSpecies()}");
        }
    }
}
