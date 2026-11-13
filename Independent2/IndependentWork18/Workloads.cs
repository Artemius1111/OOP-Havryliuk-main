namespace IndependentWork18;

public static class Workloads
{
    public static List<int> CreateData(int count, int seed = 18)
    {
        var random = new Random(seed);
        var data = new List<int>(count);

        for (int i = 0; i < count; i++)
        {
            data.Add(random.Next(1, 1_000_001));
        }

        return data;
    }

    public static int CollatzSteps(int number)
    {
        long value = number;
        int steps = 0;

        while (value != 1)
        {
            value = value % 2 == 0 ? value / 2 : 3 * value + 1;
            steps++;
        }

        return steps;
    }
}
