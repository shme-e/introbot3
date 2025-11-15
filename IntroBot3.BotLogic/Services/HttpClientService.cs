namespace IntroBot3.BotLogic.Services;

public class HttpClientService
{
    private readonly HttpClient _httpClient;

    public HttpClientService()
    {
        _httpClient = new HttpClient();
    }

    public async Task DownloadFileAsync(Uri uri, string path)
    {
        var response = await _httpClient.GetAsync(uri);
        _ = response.EnsureSuccessStatusCode();
        var stream = await response.Content.ReadAsStreamAsync();

        using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        await stream.CopyToAsync(fileStream);
    }
}