using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using HedgehogSoft.BackpackLogin.Extensions;
using HedgehogSoft.BackpackLogin.Interfaces;
using HedgehogSoft.BackpackLogin.Models;
using HedgehogSoft.BackpackLogin.SteamLogin;

namespace HedgehogSoft.BackpackLogin.Rest
{
    internal class RestClient
    {
        private readonly CookieContainer _cookieContainer = new CookieContainer();

        internal HttpClientHandler GetDefaultClientHandler(bool includingCookieContainer)
        {
            var httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            if (includingCookieContainer)
            {
                httpClientHandler.CookieContainer = _cookieContainer;
            }
            return httpClientHandler;
        }

        private static Dictionary<string, string> GetBaseRequestHeaders()
        {
            return new Dictionary<string, string>
            {
                {"Accept-Encoding", "gzip, deflate"},
                {"Accept-Language", "en-US,en;q=0.8,de-DE;q=0.6,de;q=0.4,it;q=0.2"},
                {"User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36"}
            };
        }

        internal HttpResponseMessage GetBackpackMain()
        {
            var httpClient = new HttpClient(GetDefaultClientHandler(true));
            httpClient.AddRequestHeaders(GetBaseRequestHeaders().GetBackpackMainHeaders());
            return httpClient.GetAsync("http://backpack.tf/").Result;
        }

        internal HttpResponseMessage GetBackpackLogin()
        {
            var httpClient = new HttpClient(GetDefaultClientHandler(true));
            httpClient.AddRequestHeaders(GetBaseRequestHeaders().GetBackpackLoginHeaders());
            return httpClient.PostAsync("http://backpack.tf/login", FormDataGenerator.BackpackLoginPost()).Result;
        }

        internal HttpResponseMessage GetOpenIdParameters(string url)
        {
            var httpClient = new HttpClient(GetDefaultClientHandler(true));
            httpClient.AddRequestHeaders(GetBaseRequestHeaders().GetOpenIdParametersHeaders());
            return httpClient.GetAsync(url).Result;
        }

        internal HttpResponseMessage GetRsaKey(string username, string referer)
        {
            var httpClientHandler = GetDefaultClientHandler(true);
            httpClientHandler.CookieContainer.Add(new Uri("https://steamcommunity.com"), new Cookie("timezoneoffset", Math.Abs((DateTime.UtcNow - DateTime.Now).TotalSeconds).ToString(CultureInfo.InvariantCulture)));
            var httpClient = new HttpClient(httpClientHandler);
            httpClient.AddRequestHeaders(GetBaseRequestHeaders().GetRsaKeyHeaders(referer));
            return httpClient.PostAsync("https://steamcommunity.com/login/getrsakey/", FormDataGenerator.SteamRsa(username)).Result;
        }

        internal HttpResponseMessage DoLogin(string sharedSecret, string referer, RsaResponse rsaResponse, string username, string password)
        {
            var steamTwo = new SteamTwoFactorGenerator { SharedSecret = sharedSecret };
            var code = steamTwo.GenerateSteamGuardCodeForTime();
            var httpClient = new HttpClient(GetDefaultClientHandler(true));
            httpClient.AddRequestHeaders(GetBaseRequestHeaders().PostDoLoginHeaders(referer));
            return httpClient.PostAsync("https://steamcommunity.com/login/dologin/", FormDataGenerator.SteamDoLogin(rsaResponse, username, password, code)).Result;
        }

        internal HttpResponseMessage PostOpenIdLogin(string referer, IOpenIdParameters openIdParameters)
        {
            var httpClient = new HttpClient(GetDefaultClientHandler(true));
            httpClient.AddRequestHeaders(GetBaseRequestHeaders().PostOpenIdLoginHeaders(referer));
            return httpClient.PostAsync("https://steamcommunity.com/openid/login", FormDataGenerator.OpenIdData(openIdParameters)).Result;
        }

        internal HttpResponseMessage GetOpenIdRedirect(string url)
        {
            var httpClient = new HttpClient(GetDefaultClientHandler(true));
            httpClient.AddRequestHeaders(GetBaseRequestHeaders().GetOpenIdRedirectHeaders());
            return httpClient.GetAsync(url).Result;
        }

        internal HttpResponseMessage ValidateBackpackMain(string url)
        {
            var httpClient = new HttpClient(GetDefaultClientHandler(true));
            httpClient.AddRequestHeaders(GetBaseRequestHeaders().ValidateBackpackMainHeaders());
            return httpClient.GetAsync(url).Result;
        }

        internal HttpResponseMessage GetSteamOffset()
        {
            var httpClient = new HttpClient(GetDefaultClientHandler(false));
            return httpClient.PostAsync("http://api.steampowered.com/ITwoFactorService/QueryTime/v1/", null).Result;
        }
    }
}
