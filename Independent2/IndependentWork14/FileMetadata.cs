using System.Security.Cryptography;
using System.Text;

namespace IndependentWork14;

public class FileMetadata
{
    public string FileName { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime LastModified { get; set; }
    public string Hash { get; set; } = string.Empty;

    public static FileMetadata FromContent(string fileName, string content, DateTime lastModified)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(content);

        return new FileMetadata
        {
            FileName = fileName,
            Size = bytes.Length,
            LastModified = lastModified,
            Hash = Convert.ToHexString(SHA256.HashData(bytes))[..12]
        };
    }

    public override string ToString() => $"{FileName}: {Size} байт, змінено {LastModified:dd.MM.yyyy HH:mm}, SHA-256 {Hash}";
}
