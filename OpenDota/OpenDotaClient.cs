namespace DotaData.OpenDota
{
    internal class OpenDotaClient(HttpClient httpClient)
    {
        public HttpClient HttpClient => httpClient;

        public ApiQuery Query() => new();
    }
}
