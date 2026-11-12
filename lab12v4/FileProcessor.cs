namespace lab12v4;

public class FileProcessor
{
    private static readonly byte[] PngSignature = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

    private readonly int _failuresBeforeSuccess;
    private readonly HashSet<string> _lockedFiles = new();

    public FileProcessor(int failuresBeforeSuccess = 3)
    {
        _failuresBeforeSuccess = failuresBeforeSuccess;
    }

    public int CallCount { get; private set; }

    public void LockFile(string path)
    {
        _lockedFiles.Add(path);
    }

    public byte[] ReadImage(string path)
    {
        CallCount++;

        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Шлях до файлу не може бути порожнім");
        }

        if (_lockedFiles.Contains(path))
        {
            throw new IOException($"Файл {path} заблоковано іншим процесом");
        }

        if (CallCount <= _failuresBeforeSuccess)
        {
            throw new FileNotFoundException($"Файл {path} не знайдено (виклик №{CallCount})", path);
        }

        byte[] image = new byte[2048];
        PngSignature.CopyTo(image, 0);
        return image;
    }
}
