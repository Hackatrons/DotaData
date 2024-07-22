using System.Text.Json;

namespace DotaData;

/// <summary>
/// Invokes an api query and retrieves the results.
/// </summary>
internal static class ApiQueryExecutor
{
    static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public static async Task<IEnumerable<T?>> Execute<T>(this ApiQuery query, HttpClient client, CancellationToken cancellationToken = new())
    {
        var url = query.ToString();

        await using var stream = await client.GetStreamAsync(url, cancellationToken);
        using var json = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        return json.RootElement.EnumerateArray()
            .Select(x => x.Deserialize<T>(Options))
            // evaluate the enumerable to avoid accessing a disposed json document
            // as the json document gets disposed after this method ends
            .ToList();
    }
}