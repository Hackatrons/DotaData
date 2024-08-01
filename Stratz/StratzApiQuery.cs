using System.Collections.Specialized;

namespace DotaData.Stratz;

/// <summary>
/// Builds Open Dota API queries.
/// </summary>
internal class StratzApiQuery
{
    const string BaseUrl = "https://docs.stratz.com/";

    string? _path;
    string? _subPath;
    int? _skip;
    int? _take;

    /// <summary>
    /// Specify the player id for the API query.
    /// </summary>
    public StratzApiQuery Player(int accountId)
    {
        _path = $"/api/v1/player/{accountId}";
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
    /// The number of results to return.
    /// </summary>
    public StratzApiQuery Take(int take)
    {
        _take = take;
        return this;
    }

    public StratzApiQuery TakeMax() => Take(50);

    /// <summary>
    /// The number of rows to skip.
    /// </summary>
    public StratzApiQuery Skip(int skip)
    {
        _skip = skip;
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
        if (_skip is not null)
            parameters.Add("skip", _skip.ToString());
        if (_take is not null)
            parameters.Add("take", _take.ToString());

        builder.Query = string.Join("&", parameters.AllKeys.Select(key => $"{key}={parameters[key]}"));

        return builder.ToString();
    }
}