using System.Text;

namespace lab12v4;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        TimeSpan initialDelay = TimeSpan.FromMilliseconds(500);
        Func<Exception, bool> shouldRetry = ex => ex is FileNotFoundException || ex is HttpRequestException;

        Console.WriteLine("Варіант 4: завантаження зображення, початкова затримка 500 мс");
        Console.WriteLine();

        RunScenario("1. FileProcessor.ReadImage: FileNotFoundException перші 3 рази, потім успіх", () =>
        {
            var fileProcessor = new FileProcessor(failuresBeforeSuccess: 3);
            byte[] image = RetryHelper.ExecuteWithRetry(
                () => fileProcessor.ReadImage("images/robot.png"),
                retryCount: 3,
                initialDelay: initialDelay,
                shouldRetry: shouldRetry);
            PrintImageInfo(image, fileProcessor.CallCount);
        });

        RunScenario("2. NetworkClient.DownloadImage: HttpRequestException перші 2 рази, потім успіх", () =>
        {
            var networkClient = new NetworkClient(failuresBeforeSuccess: 2);
            byte[] image = RetryHelper.ExecuteWithRetry(
                () => networkClient.DownloadImage("https://cdn.example.com/images/robot.jpg"),
                retryCount: 3,
                initialDelay: initialDelay,
                shouldRetry: shouldRetry);
            PrintImageInfo(image, networkClient.CallCount);
        });

        RunScenario("3. shouldRetry: IOException (файл заблоковано) не повторюється", () =>
        {
            var fileProcessor = new FileProcessor();
            fileProcessor.LockFile("images/avatar.png");
            byte[] image = RetryHelper.ExecuteWithRetry(
                () => fileProcessor.ReadImage("images/avatar.png"),
                retryCount: 3,
                initialDelay: initialDelay,
                shouldRetry: shouldRetry);
            PrintImageInfo(image, fileProcessor.CallCount);
        });

        RunScenario("4. shouldRetry: ArgumentException (некоректна адреса) не повторюється", () =>
        {
            var networkClient = new NetworkClient();
            byte[] image = RetryHelper.ExecuteWithRetry(
                () => networkClient.DownloadImage("cdn.example.com/images/robot.jpg"),
                retryCount: 3,
                initialDelay: initialDelay,
                shouldRetry: shouldRetry);
            PrintImageInfo(image, networkClient.CallCount);
        });

        RunScenario("5. Замало спроб: retryCount = 1, а сервер недоступний перші 2 рази", () =>
        {
            var networkClient = new NetworkClient(failuresBeforeSuccess: 2);
            byte[] image = RetryHelper.ExecuteWithRetry(
                () => networkClient.DownloadImage("https://cdn.example.com/images/robot.jpg"),
                retryCount: 1,
                initialDelay: initialDelay,
                shouldRetry: shouldRetry);
            PrintImageInfo(image, networkClient.CallCount);
        });
    }

    static void RunScenario(string title, Action scenario)
    {
        Console.WriteLine(title);

        try
        {
            scenario();
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"   Помилка: файл {ex.FileName} не знайдено");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"   Помилка вводу/виводу: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"   Мережева помилка: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Інша помилка ({ex.GetType().Name}): {ex.Message}");
        }
        finally
        {
            Console.WriteLine("   Сценарій завершено");
            Console.WriteLine();
        }
    }

    static void PrintImageInfo(byte[] image, int callCount)
    {
        string format = image[0] == 0x89 ? "PNG" : image[0] == 0xFF ? "JPEG" : "невідомий";
        string signature = BitConverter.ToString(image, 0, 4);
        Console.WriteLine($"   Результат: {image.Length} байт, формат {format} ({signature}), викликів методу: {callCount}");
    }
}
