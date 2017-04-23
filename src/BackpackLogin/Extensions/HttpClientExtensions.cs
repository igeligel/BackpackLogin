using System.Collections.Generic;using System.Net.Http;
namespace HedgehogSoft.BackpackLogin.Extensions
{
    internal static class HttpClientExtensions
    {
        internal static HttpClient AddRequestHeaders(this HttpClient httpClient, Dictionary<string, string> dict)
        {
            foreach (var httpHeader in dict)
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(httpHeader.Key, httpHeader.Value);
            }
            return httpClient;
        }
    }
}
