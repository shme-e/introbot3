namespace IntroBot3.Services;

public class HttpClientService
{
    private readonly HttpClient _httpClient;

    public HttpClientService()
    {
        _httpClient = new HttpClient();
    }

    public async Task DownloadFileAsync(string url, string path)
    {
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var stream = await response.Content.ReadAsStreamAsync();
    
        using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        await stream.CopyToAsync(fileStream);
    }
}