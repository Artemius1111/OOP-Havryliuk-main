namespace lab12v4;

public class NetworkClient
{
    private static readonly byte[] JpegSignature = { 0xFF, 0xD8, 0xFF, 0xE0 };

    private readonly int _failuresBeforeSuccess;

    public NetworkClient(int failuresBeforeSuccess = 2)
    {
        _failuresBeforeSuccess = failuresBeforeSuccess;
    }

    public int CallCount { get; private set; }

    public byte[] DownloadImage(string url)
    {
        CallCount++;

        bool isValidUrl = Uri.TryCreate(url, UriKind.Absolute, out Uri? uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

        if (!isValidUrl)
        {
            throw new ArgumentException($"Некоректна адреса {url}");
        }

        if (CallCount <= _failuresBeforeSuccess)
        {
            throw new HttpRequestException($"Сервер тимчасово недоступний (503) для {url}, виклик №{CallCount}");
        }

        byte[] image = new byte[4096];
        JpegSignature.CopyTo(image, 0);
        return image;
    }
}
