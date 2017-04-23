using System;
using System.Collections.Generic;
using System.Net.Http;
using HedgehogSoft.BackpackLogin.Interfaces;
using HedgehogSoft.BackpackLogin.Models;
using HedgehogSoft.BackpackLogin.SteamLogin;

namespace HedgehogSoft.BackpackLogin.Rest
{
    internal static class FormDataGenerator
    {
        internal static FormUrlEncodedContent BackpackLoginPost()
        {
            return new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("x", "77"),
                new KeyValuePair<string, string>("y", "20"),
                new KeyValuePair<string, string>("return", "bp"),
            });
        }

        internal static FormUrlEncodedContent SteamRsa(string username)
        {
            var unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds + "000";
            return new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("donotcache", unixTimestamp),
                new KeyValuePair<string, string>("username", username)
            });
        }

        internal static FormUrlEncodedContent SteamDoLogin(RsaResponse rsaResponse, string username, string password, string code)
        {
            var unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds + "000";
            return new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("donotcache", unixTimestamp),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", PasswordEncrypter.EncryptPassword(rsaResponse.PublickeyMod, rsaResponse.PublickeyExp, password)),
                new KeyValuePair<string, string>("twofactorcode", code),
                new KeyValuePair<string, string>("emailauth", ""),
                new KeyValuePair<string, string>("loginfriendlyname", ""),
                new KeyValuePair<string, string>("captchagid", "-1"),
                new KeyValuePair<string, string>("captcha_text", ""),
                new KeyValuePair<string, string>("emailsteamid", ""),
                new KeyValuePair<string, string>("rsatimestamp", rsaResponse.Timestamp),
                new KeyValuePair<string, string>("remember_login", "false"),
            });
        }

        internal static FormUrlEncodedContent OpenIdData(IOpenIdParameters openIdParameters)
        {
            return new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("action", openIdParameters.Action),
                new KeyValuePair<string, string>("openid.mode", openIdParameters.OpenIdMode),
                new KeyValuePair<string, string>("openidparams", openIdParameters.OpenIdParams),
                new KeyValuePair<string, string>("nonce", openIdParameters.Nonce),
            });
        }
    }
}
