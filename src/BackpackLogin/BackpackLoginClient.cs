using System.Linq;
using System.Net;
using AngleSharp.Parser.Html;
using HedgehogSoft.BackpackLogin.Models;
using HedgehogSoft.BackpackLogin.Rest;
using Newtonsoft.Json;

namespace HedgehogSoft.BackpackLogin
{
    public class BackpackLoginClient
    {
        public CookieContainer Login(string username, string password, string sharedSecret)
        {
            var restClient = new RestClient();
            restClient.GetBackpackMain();
            var getBackpackLoginResponse = restClient.GetBackpackLogin();
            var location = getBackpackLoginResponse.Headers.GetValues("Location").FirstOrDefault();
            var openIdResponse = restClient.GetOpenIdParameters(location);
            var openIdParameters = new OpenIdParameters(openIdResponse);
            var getRsaKeyResponse = restClient.GetRsaKey(username, location);
            var rsaResponse = JsonConvert.DeserializeObject<RsaResponse>(getRsaKeyResponse.Content.ReadAsStringAsync().Result);
            restClient.DoLogin(sharedSecret, location, rsaResponse, username, password);
            var postOpenIdLoginResponse = restClient.PostOpenIdLogin(location, openIdParameters);
            location = postOpenIdLoginResponse.Headers.GetValues("Location").FirstOrDefault();
            var redirectResp = restClient.GetOpenIdRedirect(location);
            location = redirectResp.Headers.GetValues("Location").FirstOrDefault();
            restClient.ValidateBackpackMain(location);
            return restClient.GetCookieContainer();
        }
    }
}
