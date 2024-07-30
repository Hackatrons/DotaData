namespace DotaData.Stratz;

/// <summary>
/// Builds Open Dota API queries.
/// </summary>
internal class StratzApiQuery
{
    const string BaseUrl = "https://docs.stratz.com/";

    string? _path;
    string? _subPath;

    /// <summary>
    /// Specify the player id for the API query.
    /// </summary>
    public StratzApiQuery Player(int accountId)
    {
        _path = $"/api/v1/players/{accountId}";
        return this;
    }

    /// <summary>
    /// Specifies matches should be retrieved (used in conjuction with player queries).
    /// </summary>
    public StratzApiQuery Matches()
    {
        _subPath = "matches";
        return this;
    }

    /// <summary>
    /// Specifies a specific match should be retrieved.
    /// </summary>
    public StratzApiQuery Match(long matchId)
    {
        // matches by themselves
        _path = $"/api/v1/matches/{matchId}";
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

        return builder.ToString();
    }
}