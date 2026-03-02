using System.Text.Json;

internal static class HttpClientExt
{
    public static async Task<(T? Data, string? Error)> GetFromJsonSafeAsync<T>(this HttpClient client, string requestUri)
    {
        try
        {
            using var response = await client.GetAsync(requestUri);

            if (!response.IsSuccessStatusCode)
            {
                return (default, $"API Error: {(int)response.StatusCode} ({response.ReasonPhrase})");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var stream = await response.Content.ReadAsStreamAsync();
            var data = await JsonSerializer.DeserializeAsync<T>(stream, options);

            return (data, null);
        }
        catch (Exception ex)
        {
            return (default, $"Network/Parsing Error: {ex.Message}");
        }
    }
}