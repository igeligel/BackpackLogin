using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace HedgehogSoft.BackpackLogin
{
    public class BackpackLoginClient
    {
        private readonly CookieContainer _cookieContainer = new CookieContainer();

        public void Login(string username, string password, string sharedSecret)
        {
            Step1();
            var step2Response = Step2();
            var location = step2Response.Headers.GetValues("Location").FirstOrDefault();
            Step3(location);
        }

        public HttpResponseMessage Step1()
        {
            var httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                CookieContainer = _cookieContainer
            };
            var httpClient = new HttpClient(httpClientHandler);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Cache-Control", "no-cache");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Pragma", "no-cache");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.8,de-DE;q=0.6,de;q=0.4,it;q=0.2");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Upgrade-Insecure-Requests", "1");
            return httpClient.GetAsync("http://backpack.tf/").Result;
        }

        public HttpResponseMessage Step2()
        {
            var httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                CookieContainer = _cookieContainer
            };
            var httpClient = new HttpClient(httpClientHandler);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Cache-Control", "max-age=0");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.8,de-DE;q=0.6,de;q=0.4,it;q=0.2");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Referer", "http://backpack.tf/");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Origin", "http://backpack.tf");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Upgrade-Insecure-Requests", "1");
            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("x", "77"),
                new KeyValuePair<string, string>("y", "20"),
                new KeyValuePair<string, string>("return", "bp"),
            });
            return httpClient.PostAsync("http://backpack.tf/login", formData).Result;
        }

        public HttpResponseMessage Step3(string url)
        {
            var httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                CookieContainer = _cookieContainer
            };
            var httpClient = new HttpClient(httpClientHandler);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Cache-Control", "max-age=0");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.8,de-DE;q=0.6,de;q=0.4,it;q=0.2");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Referer", "http://backpack.tf/");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Upgrade-Insecure-Requests", "1");
            return httpClient.GetAsync(url).Result;
        }

        internal static FormUrlEncodedContent SteamRsaFormData(string username)
        {
            var unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds + "000";
            return new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("donotcache", unixTimestamp),
                new KeyValuePair<string, string>("username", username)
            });
        }

        public HttpResponseMessage SteamRsa(string username, string referer)
        {
            var httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                CookieContainer = _cookieContainer
            };
            var httpClient = new HttpClient(httpClientHandler);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "*/*");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.8,de-DE;q=0.6,de;q=0.4,it;q=0.2");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Referer", referer);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Origin", "https://steamcommunity.com");
            return httpClient.PostAsync("https://steamcommunity.com/login/getrsakey/", SteamRsaFormData(username)).Result;
        }
    }
}
