using System.Linq;
using AngleSharp.Parser.Html;
using HedgehogSoft.BackpackLogin.Models;
using HedgehogSoft.BackpackLogin.Rest;
using Newtonsoft.Json;

namespace HedgehogSoft.BackpackLogin
{
    public class BackpackLoginClient
    {
        public void Login(string username, string password, string sharedSecret)
        {
            var restClient = new RestClient();
            restClient.GetBackpackMain();
            var step2Response = restClient.GetBackpackLogin();
            var location = step2Response.Headers.GetValues("Location").FirstOrDefault();
            var openIdResponse = restClient.GetOpenIdParameters(location);
            var responseBody = openIdResponse.Content.ReadAsStringAsync().Result;
            var document = new HtmlParser().Parse(responseBody);
            var allInputs = document.QuerySelectorAll("input");
            var openIdParameters = new OpenIdParameters
            {
                Action = allInputs.FirstOrDefault(e => e.GetAttribute("name") == "action").Attributes.FirstOrDefault(e => e.Name == "value").Value,
                OpenIdMode = allInputs.FirstOrDefault(e => e.GetAttribute("name") == "openid.mode").Attributes.FirstOrDefault(e => e.Name == "value").Value,
                OpenIdParams = allInputs.FirstOrDefault(e => e.GetAttribute("name") == "openidparams").Attributes.FirstOrDefault(e => e.Name == "value").Value,
                Nonce = allInputs.FirstOrDefault(e => e.GetAttribute("name") == "nonce").Attributes.FirstOrDefault(e => e.Name == "value").Value
            };

            var response = restClient.GetRsaKey(username, location);
            var rsaResponse = JsonConvert.DeserializeObject<RsaResponse>(response.Content.ReadAsStringAsync().Result);
            restClient.DoLogin(sharedSecret, location, rsaResponse, username, password);
            var postOpenIdResponse = restClient.PostOpenIdLogin(location, openIdParameters);
            location = postOpenIdResponse.Headers.GetValues("Location").FirstOrDefault();
            var redirectResp = restClient.GetOpenIdRedirect(location);
            location = redirectResp.Headers.GetValues("Location").FirstOrDefault();
            restClient.ValidateBackpackMain(location);
        }
    }
}
