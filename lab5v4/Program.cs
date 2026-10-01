using System;
using System.Collections.Generic;

namespace Lab5Variant4
{
    public class Device
    {
        public string Brand { get; set; }

        public Device(string brand)
        {
            Brand = brand;
        }

        public virtual string PerformAction()
        {
            return $"Пристрій бренду {Brand} виконує стандартну дію.";
        }
    }

    public class Printer : Device
    {
        public int PagesPerMinute { get; set; }

        public Printer(string brand, int pagesPerMinute) : base(brand)
        {
            PagesPerMinute = pagesPerMinute;
        }

        public override string PerformAction()
        {
            return $"[Принтер {Brand}] Друкує документи зі швидкістю {PagesPerMinute} стор/хв.";
        }
    }

    public class Scanner : Device
    {
        public int DPI { get; set; }

        public Scanner(string brand, int dpi) : base(brand)
        {
            DPI = dpi;
        }

        public override string PerformAction()
        {
            return $"[Сканер {Brand}] Сканує документ із роздільною здатністю {DPI} DPI.";
        }
    }

    public class Monitor : Device
    {
        public double ScreenSize { get; set; }

        public Monitor(string brand, double screenSize) : base(brand)
        {
            ScreenSize = screenSize;
        }

        public override string PerformAction()
        {
            return $"[Монітор {Brand}] Відображає зображення на екрані {ScreenSize}\" дюймів.";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            List<Device> devices = new List<Device>
            {
                new Printer("HP", 35),
                new Scanner("Epson", 1200),
                new Monitor("Dell", 27.0),
                new Printer("Canon", 20),
                new Scanner("Canon", 2400),
                new Monitor("Samsung", 32.5)
            };

            List<string> actionLog = new List<string>();

            Console.WriteLine("ДЕМОНСТРАЦІЯ ПОЛІМОРФНИХ ВИКЛИКІВ ПРИСТРОЇВ:\n");

            foreach (Device device in devices)
            {
                string actionResult = device.PerformAction();
                Console.WriteLine(actionResult);
                actionLog.Add(actionResult);
            }

            Console.WriteLine("\nАГРЕГАЦІЯ: ПІДСУМКОВИЙ ЗВІТ ВИКОНАНИХ ДІЙ");
            Console.WriteLine($"Загальна кількість виконаних дій: {actionLog.Count}\n");

            for (int i = 0; i < actionLog.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {actionLog[i]}");
            }
        }
    }
}