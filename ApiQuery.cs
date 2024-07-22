using System.Collections.Specialized;

namespace DotaData
{
    /// <summary>
    /// Builds Open Dota API queries.
    /// </summary>
    internal class ApiQuery
    {
        const string BaseUrl = "https://api.opendota.com/";

        string? _path;
        string? _subPath;
        bool? _significant;

        /// <summary>
        /// Specify the player id for the API query.
        /// </summary>
        public ApiQuery Player(int playerId)
        {
            _path = $"/api/players/{playerId}";
            return this;
        }

        /// <summary>
        /// Specifies matches should be retrieved
        /// </summary>
        public ApiQuery Matches()
        {
            _subPath = "matches";
            return this;
        }

        /// <summary>
        /// Specifies whether to include significant results.
        /// </summary>
        public ApiQuery Significant(bool significant)
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
}
