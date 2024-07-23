using System.Text.Json;
using DotaData.Monad;

namespace DotaData.OpenDota;

/// <summary>
/// Invokes an api query and retrieves the results.
/// </summary>
internal static class ApiQueryExecutor
{
    static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public static async Task<ValueOrError<IEnumerable<T>>> ExecuteSet<T>(this ApiQuery query, OpenDotaClient client, CancellationToken cancellationToken = new())
    {
        var url = query.ToString();

        using var response = await client.HttpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
            return new InvalidOperationException($"{response.StatusCode} response from server");

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

    public static async Task<ValueOrError<T>> ExecuteSingle<T>(this ApiQuery query, OpenDotaClient client, CancellationToken cancellationToken = new())
    {
        var results = await query.ExecuteSet<T>(client, cancellationToken);

        return !results.IsSuccess ? new ValueOrError<T>(results.GetError()) : new ValueOrError<T>(results.GetValue().Single());
    }
}