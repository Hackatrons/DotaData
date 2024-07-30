namespace DotaData.Stratz
{
    internal class StratzClient(HttpClient httpClient)
    {
        public HttpClient HttpClient => httpClient;
    }
}
