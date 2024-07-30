namespace DotaData.Stratz
{
    internal class StratzClient(HttpClient httpClient)
    {
        public HttpClient HttpClient => httpClient;

        public StratzApiQuery Query() => new();
    }
}
