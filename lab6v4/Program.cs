using System;

namespace lab6v4
{
    public class Animal
    {

        private string name;
        private int age;

        public string Name
        {
            get => name;
            set => name = value;
        }

        public int Age
        {
            get => age;
            set => age = value;
        }

        public Animal(string name, int age)
        {
            this.name = name;
            this.age = age;
        }

        public virtual void MakeSound()
        {
            Console.WriteLine($"{Name} видає невизначений звук.");
        }

        public string GetSpecies()
        {
            return "Невідомий вид (Animal)";
        }
    }

    public class Dog : Animal
    {
        private string breed;

        public string Breed
        {
            get => breed;
            set => breed = value;
        }

        public Dog(string name, int age, string breed) : base(name, age)
        {
            this.breed = breed;
        }

        public override void MakeSound()
        {
            Console.WriteLine($"Собака {Name} (порода: {Breed}) гавкає: Гав-гав!");
        }

        public void Fetch()
        {
            Console.WriteLine($"Собака {Name} принесла паличку!");
        }

        public new string GetSpecies()
        {
            return "Собака свійський (Dog)";
        }
    }

    public class Cat : Animal
    {
        private bool isIndoor;

        public bool IsIndoor
        {
            get => isIndoor;
            set => isIndoor = value;
        }

        public Cat(string name, int age, bool isIndoor) : base(name, age)
        {
            this.isIndoor = isIndoor;
        }

        public override void MakeSound()
        {
            string category = IsIndoor ? "домашній" : "вуличний";
            Console.WriteLine($"Кіт {Name} ({category}) нявкає: Мяу-мяу!");
        }

        public void Purr()
        {
            Console.WriteLine($"Кіт {Name} задоволено муркоче: Ррр-мууур");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("1. Створення об'єктів та виклик унікальних методів");
            Dog myDog = new Dog("Рекс", 3, "Німецька вівчарка");
            Cat myCat = new Cat("Мурчик", 2, true);

            myDog.Fetch();
            myCat.Purr();
            Console.WriteLine();

            Console.WriteLine("2. Демонстрація поліморфізму (virtual / override)");
            Animal[] animals = new Animal[]
            {
                new Animal("Невідома істота", 1),
                myDog,
                myCat
            };

            foreach (var animal in animals)
            {
                animal.MakeSound();
            }
            Console.WriteLine();

            Console.WriteLine("3. Демонстрація різниці між override та new");

            Console.WriteLine($"Через посилання типу Dog: myDog.GetSpecies() -> {myDog.GetSpecies()}");

            Animal animalRef = myDog;
            Console.WriteLine($"Через посилання типу Animal: animalRef.GetSpecies() -> {animalRef.GetSpecies()}");

            Console.WriteLine();
            Console.WriteLine("Примітка: При new результат залежить від типу ПОСИЛАННЯ (Animal vs Dog), а не від об'єкта.");
        }
    }
}
