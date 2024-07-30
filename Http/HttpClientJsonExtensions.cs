using System.Text.Json;
using DotaData.ErrorHandling;
using DotaData.OpenDota;
using DotaData.Stratz;

namespace DotaData.Http;

/// <summary>
/// Invokes an api query and retrieves the results.
/// </summary>
internal static class HttpClientJsonExtensions
{
    static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public static async Task<ValueOrError<IEnumerable<T>>> GetJsonResults<T>(string url, OpenDotaClient client, CancellationToken cancellationToken = new())
    {
        using var response = await client.HttpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
            return new HttpRequestException(response.ReasonPhrase ?? $"{response.StatusCode} response from server", inner: null, statusCode: response.StatusCode);

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var json = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        if (json.RootElement.ValueKind == JsonValueKind.Array)
        {
            return json.RootElement.EnumerateArray()
                .Select(x => x.Deserialize<T>(Options))
                .Where(x => x is not null)
                // evaluate the enumerable to avoid accessing a disposed json document
                // as the json document gets disposed after this method ends
                .ToList()!;
        }

        var single = json.RootElement.Deserialize<T>(Options);

        return single is null ? new InvalidOperationException($"Unable to deserialize to type{typeof(T).FullName}") : new ValueOrError<IEnumerable<T>>([single]);
    }

    public static async Task<ValueOrError<T>> GetJsonResult<T>(string url, OpenDotaClient client, CancellationToken cancellationToken = new())
    {
        var results = await GetJsonResults<T>(url, client, cancellationToken);

        return !results.IsSuccess ? new ValueOrError<T>(results.GetError()) : new ValueOrError<T>(results.GetValue().Single());
    }

    public static async Task<ValueOrError<IEnumerable<T>>> GetJsonResults<T>(this OpenDotaApiQuery query, OpenDotaClient client, CancellationToken cancellationToken = new())
        => await GetJsonResults<T>(query.ToString(), client, cancellationToken);

    public static async Task<ValueOrError<T>> GetJsonResult<T>(this OpenDotaApiQuery query, OpenDotaClient client, CancellationToken cancellationToken = new()) 
        => await GetJsonResult<T>(query.ToString(), client, cancellationToken);

    public static async Task<ValueOrError<IEnumerable<T>>> GetJsonResults<T>(this StratzApiQuery query, OpenDotaClient client, CancellationToken cancellationToken = new())
        => await GetJsonResults<T>(query.ToString(), client, cancellationToken);

    public static async Task<ValueOrError<T>> GetJsonResult<T>(this StratzApiQuery query, OpenDotaClient client, CancellationToken cancellationToken = new())
        => await GetJsonResult<T>(query.ToString(), client, cancellationToken);
}