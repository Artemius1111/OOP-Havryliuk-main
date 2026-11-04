namespace lab11v4;

public static class CompanyData
{
    public static List<Project> Projects { get; } = new()
    {
        new Project(1, "CRM System"),
        new Project(2, "Mobile Banking App"),
        new Project(3, "Online Store"),
        new Project(4, "Delivery Tracker"),
        new Project(5, "HR Portal"),
        new Project(6, "Smart Home"),
        new Project(7, "School Diary"),
        new Project(8, "Medical Records"),
        new Project(9, "Travel Booking"),
        new Project(10, "Analytics Dashboard"),
        new Project(11, "Chat Bot"),
        new Project(12, "Game Launcher")
    };

    public static List<Company> Companies { get; } = new()
    {
        new Company(1, "TechNova", new()
        {
            new Employee(1, "Ivan Petrenko", new() { P(1), P(3) }),
            new Employee(2, "Olena Kovalenko", new() { P(3) }),
            new Employee(3, "Andrii Melnyk", new() { P(10) })
        }),
        new Company(2, "CodeCraft", new()
        {
            new Employee(4, "Maria Shevchuk", new() { P(2) }),
            new Employee(5, "Dmytro Bondar", new() { P(2), P(11) })
        }),
        new Company(3, "BrightSoft", new()
        {
            new Employee(6, "Yulia Tkachenko", new() { P(5) }),
            new Employee(7, "Serhii Kravchuk", new() { P(5), P(1) }),
            new Employee(8, "Nataliia Moroz", new() { P(7) })
        }),
        new Company(4, "DataWave", new()
        {
            new Employee(9, "Oleh Savchenko", new() { P(10), P(8) })
        }),
        new Company(5, "PixelForge", new()
        {
            new Employee(10, "Anna Lysenko", new() { P(12) }),
            new Employee(11, "Taras Hnatiuk", new() { P(12), P(6) }),
            new Employee(12, "Iryna Polishchuk", new() { P(6) })
        }),
        new Company(6, "CloudPeak", new()
        {
            new Employee(13, "Viktor Rudenko", new() { P(9) }),
            new Employee(14, "Sofiia Marchenko", new() { P(9), P(4) })
        }),
        new Company(7, "NetSphere", new()
        {
            new Employee(15, "Roman Ivanchuk", new() { P(11) }),
            new Employee(16, "Kateryna Boiko", new() { P(4) })
        }),
        new Company(8, "ByteWorks", new()
        {
            new Employee(17, "Maksym Oliinyk", new() { P(3), P(4) }),
            new Employee(18, "Daryna Zinchenko", new() { P(8) })
        }),
        new Company(9, "GreenLogic", new()
        {
            new Employee(19, "Yaroslav Pavliuk", new() { P(6), P(7) })
        }),
        new Company(10, "SkyLine IT", new()
        {
            new Employee(20, "Khrystyna Levchenko", new() { P(1) }),
            new Employee(21, "Bohdan Kostiuk", new() { P(2), P(10) }),
            new Employee(22, "Oksana Fedoruk", new() { P(5) })
        })
    };

    private static Project P(int id) => Projects.First(p => p.Id == id);
}
