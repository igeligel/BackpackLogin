using System.Linq;
using System.Net;
using HedgehogSoft.BackpackLogin.Models;
using HedgehogSoft.BackpackLogin.Rest;
using Newtonsoft.Json;

namespace HedgehogSoft.BackpackLogin
{
    /// <summary>
    /// Client class to provide the login to Backpack. This is a central class to provide all functionalities to the outside
    /// of this package.
    /// </summary>
    public class BackpackLoginClient
    {
        /// <summary>
        /// Login method for <see cref="http://backpack.tf/"/>. This will login the user to 
        /// <see cref="http://backpack.tf/"/>. It requires personal data of the steam account you are 
        /// using since it is an openid login to <see cref="http://backpack.tf/"/> via 
        /// <see cref="http://steamcommunity.com/"/>.
        /// </summary>
        /// <param name="username">Username to the <see cref="http://steamcommunity.com/"/> Account.</param>
        /// <param name="password">Password to the <see cref="http://steamcommunity.com/"/> Account.</param>
        /// <param name="sharedSecret">Shared Secret to the <see cref="http://steamcommunity.com/"/> Account.</param>
        /// <returns>
        /// A simple <see cref="CookieContainer"/> which is needed to be authenticated. This library returns
        /// a <see cref="CookieContainer"/> rather than some kind of other object to make the API extensible.
        /// </returns>
        public CookieContainer Login(string username, string password, string sharedSecret)
        {
            var restClient = new RestClient();
            restClient.GetBackpackMain();
            var getBackpackLoginResponse = restClient.GetBackpackLogin();
            var location = getBackpackLoginResponse.Headers.GetValues("Location").FirstOrDefault();
            var openIdResponse = restClient.GetOpenIdParameters(location);
            var openIdParameters = new OpenIdParameters(openIdResponse);
            var getRsaKeyResponse = restClient.GetRsaKey(username, location);
            var rsaResponse =
                JsonConvert.DeserializeObject<RsaResponse>(getRsaKeyResponse.Content.ReadAsStringAsync().Result);
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
