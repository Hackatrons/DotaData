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

    public static async Task<IEnumerable<T>> ExecuteSet<T>(this ApiQuery query, HttpClient client, CancellationToken cancellationToken = new())
    {
        var url = query.ToString();

        await using var stream = await client.GetStreamAsync(url, cancellationToken);
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

        return single is null
            ? throw new InvalidOperationException($"Unable to deserialize to type{typeof(T).FullName}")
            : [single];
    }

    public static async Task<T> ExecuteSingle<T>(this ApiQuery query, HttpClient client, CancellationToken cancellationToken = new())
    {
        return (await query.ExecuteSet<T>(client, cancellationToken)).Single();
    }
}