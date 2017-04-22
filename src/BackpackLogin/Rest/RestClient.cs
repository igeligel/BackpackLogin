using System.Net;
using System.Net.Http;

namespace HedgehogSoft.BackpackLogin.Rest
{
    internal class RestClient
    {
        internal HttpResponseMessage GetSteamOffset()
        {
            var httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            var httpClient = new HttpClient(httpClientHandler);
            return httpClient.PostAsync("http://api.steampowered.com/ITwoFactorService/QueryTime/v1/", null).Result;
        }
    }
}
