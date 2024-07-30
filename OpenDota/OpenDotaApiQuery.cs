using System.Collections.Specialized;

namespace DotaData.OpenDota;

/// <summary>
/// Builds Open Dota API queries.
/// </summary>
internal class OpenDotaApiQuery
{
    const string BaseUrl = "https://api.opendota.com/";

    string? _path;
    string? _subPath;
    bool? _significant;

    /// <summary>
    /// Specify the player id for the API query.
    /// </summary>
    public OpenDotaApiQuery Player(int accountId)
    {
        _path = $"/api/players/{accountId}";
        return this;
    }

    /// <summary>
    /// Specifies matches should be retrieved (used in conjuction with player queries).
    /// </summary>
    public OpenDotaApiQuery Matches()
    {
        _subPath = "matches";
        return this;
    }

    /// <summary>
    /// Specifies a specific match should be retrieved.
    /// </summary>
    public OpenDotaApiQuery Match(long matchId)
    {
        // matches by themselves
        _path = $"/api/matches/{matchId}";
        return this;
    }

    /// <summary>
    /// Specifies totals should be retrieved.
    /// </summary>
    public OpenDotaApiQuery Totals()
    {
        _subPath = "totals";
        return this;
    }

    /// <summary>
    /// Specifies heroes should be retrieved.
    /// </summary>
    public OpenDotaApiQuery Heroes()
    {
        _path = "/api/heroes";
        return this;
    }

    /// <summary>
    /// Specifies whether to include significant results.
    /// </summary>
    public OpenDotaApiQuery Significant(bool significant)
        // exclude those we've already tried
    {
        _significant = significant;
        return this;
    }

    /// <summary>
    /// Returns the api query string result.
    /// </summary>
    public override string ToString()
    {
        var builder = new UriBuilder(BaseUrl);

        if (!string.IsNullOrEmpty(_subPath) && !string.IsNullOrEmpty(_path))
            builder.Path = _path + "/" + _subPath;
        else if (!string.IsNullOrEmpty(_path))
            builder.Path = _path;

        var parameters = new NameValueCollection();
        if (_significant is not null)
            parameters.Add("significant", _significant.GetValueOrDefault() ? "1" : "0");

        builder.Query = string.Join("&", parameters.AllKeys.Select(key => $"{key}={parameters[key]}"));

        return builder.ToString();
    }
}