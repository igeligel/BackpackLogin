using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BackpackLogin.API
{
    public class Login
    {
        private CookieContainer _cookies = new CookieContainer();
        private string _cookiePath = AppDomain.CurrentDomain.BaseDirectory + "cookiesForBackpack";
        private string _userId;
        private string _stackUser;
        private string _action;
        private string _openidMode;
        private string _openidparams;
        private string _nonce;
        private string _stackHash;

        /// <summary>
        /// Login Method to login into Backpack.tf
        /// </summary>
        /// <param name="inpSteamUserName">
        /// Your steam username.
        /// </param>
        /// <param name="inpSteamPassword">
        /// Your steam password.
        /// </param>
        /// <param name="inpSharedSecret">Shared Secret you will get if you add the authenticator.</param>
        public void DoLogin(string inpSteamUserName, string inpSteamPassword, string inpSharedSecret = "")
        {
            _cookiePath += inpSteamUserName + ".txt";
            _cookies = ReadCookiesFromDisk(_cookiePath);
            var responseString = GetIndex();
            SetUserId(responseString);
            var location = PostLogin();
            SetOpenIdParams(location);
            // Now do Steam Login
            dynamic steamResult = DoSteamLogin(inpSteamUserName, inpSteamPassword, inpSharedSecret);
            if (steamResult == null) return;
            PostTransfer(steamResult.transfer_parameters.steamid.ToString(),
                steamResult.transfer_parameters.token.ToString(),
                steamResult.transfer_parameters.auth.ToString(),
                steamResult.transfer_parameters.remember_login.ToString(),
                steamResult.transfer_parameters.token_secure.ToString());
            location = OpenIdLogin();
            location = GetBackpackCookies(location);
            if (location == "/" || location == "https://backpack.tf/")
            {
                // Verify Cookies
                const string url = "https://backpack.tf/";
                const string method = "GET";
                const string host = "backpack.tf";
                const string accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                FetchRequest(url, method, "", host, accept, null, false, true);
            }
            WriteCookiesToDisk(_cookiePath, _cookies);
        }

        /// <summary>
        /// Post a buy order.
        /// </summary>
        /// <param name="inpUrl">
        /// Link to the item.
        /// </param>
        /// <param name="metal">
        /// How much metal you would like to pay for the item.
        /// </param>
        /// <param name="key">
        /// How much keys you would like to pay for the item.
        /// </param>
        /// <param name="message">
        /// Message to add to the buy order.
        /// </param>
        /// <param name="tradeOffer">
        /// Is the buy out to add or to send a trade offer? If it is a trade offer, pass in true.
        /// </param>
        /// <param name="buyOut">
        /// Is the price a B/O or is it negotiable.
        /// </param>
        /// <param name="tradeOfferUrl">
        /// Your trade offer url.
        /// </param>
        public void PostBuyOrder(string inpUrl, int metal, int key, string message, bool tradeOffer, bool buyOut, string tradeOfferUrl)
        {
            var data = new NameValueCollection
            {
                {"user-id", _userId},
                {"currencies[metal]", metal.ToString()},
                {"currencies[keys]", key.ToString()},
                {"details", message}
            };
            if (tradeOffer)
            {
                if (tradeOfferUrl != string.Empty)
                {
                    data.Add("offers", "1");
                    data.Add("tradeoffers_url", tradeOfferUrl);
                }
            }
            else
            {
                data.Add("offers", "0");
                data.Add("tradeoffers_url", "");
            }
            data.Add("buyout", buyOut ? "1" : "0");
            const string accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            const string referer = "http://backpack.tf/classifieds/buy/Strange/Rust%20Botkiller%20Scattergun%20Mk.I/Tradable/Craftable";
            const string host = "backpack.tf";
            using (var httpResponse = Request(inpUrl, "POST", referer, host, accept, data, false, false))
            {
                _cookies.Add(httpResponse.Cookies);
            }
        }

        /// <summary>
        /// Method to delete all buy orders of the account logged in.
        /// </summary>
        public void DeleteAllBuyOrders()
        {
            var containsItems = true;
            var listingIds = new List<string>();
            var baseUrl = "http://backpack.tf/classifieds/?steamid=" + _stackUser + "&page=";
            var page = 1;
            const string accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            const string host = "backpack.tf";
            while (containsItems)
            {
                var url = baseUrl + page;
                var responseBody = FetchRequest(url, "GET", "", host, accept, null, false, false);
                responseBody = Regex.Split(responseBody, "Buy Orders")[1];
                if (responseBody.Contains("id='listing-"))
                {
                    var itemIds = Regex.Split(responseBody, "id='listing-");
                    for (var i = 1; i < itemIds.Length; i++)
                    {
                        listingIds.Add(Regex.Split(itemIds[i], "'>")[0]);
                    }
                    page += 1;
                }
                else
                {
                    containsItems = false;
                }
            }
            foreach (var item in listingIds)
            {
                DeleteBuyOrder(item);
            }
        }

        /// <summary>
        /// Method to delete the buy order with the inpTradeId.
        /// </summary>
        /// <param name="inpTradeId">
        /// Id of the listing.
        /// </param>
        private void DeleteBuyOrder(string inpTradeId)
        {
            var url = "http://backpack.tf/classified/remove/" + inpTradeId;
            var data = new NameValueCollection { { "user-id", _userId } };
            const string accept = "*/*";
            const string host = "backpack.tf";
            Request(url, "POST", "", host, accept, data, true, false);
        }

        /// <summary>
        /// Set the User Id in backpack.tf.
        /// </summary>
        /// <param name="inpHtml">
        /// Html from the /.
        /// </param>
        /// <returns>
        /// The user Id.
        /// </returns>
        private string SetUserId(string inpHtml)
        {
            var userId = Regex.Split(inpHtml, "var userID = \"")[1];
            userId = Regex.Split(userId, "\";")[0];
            _userId = userId;
            return userId;
        }

        /// <summary>
        /// Request to https://backpack.tf/
        /// </summary>
        /// <returns>
        /// The Html body.
        /// </returns>
        private string GetIndex()
        {
            const string url = "https://backpack.tf/";
            const string method = "GET";
            const string host = "backpack.tf";
            const string accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            return FetchRequest(url, method, "", host, accept, null, false, false);
        }

        /// <summary>
        /// /login on backpack.tf.
        /// </summary>
        /// <returns>
        /// The redirect link.
        /// </returns>
        private string PostLogin()
        {
            string resultString;
            const string url = "https://backpack.tf/login";
            const string method = "POST";
            const string host = "backpack.tf";
            const string accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            const string referer = "https://backpack.tf/";
            var data = new NameValueCollection { { "x", "56" }, { "y", "18" }, { "return", "bp" } };
            using (var httpResponse = Request(url, method, referer, host, accept, data, false, false))
            {
                resultString = httpResponse.Headers["Location"].ToString();
            }
            return resultString;
        }

        /// <summary>
        /// Set the Open id parameters.
        /// </summary>
        /// <param name="inpUrl">
        /// Url to the html body with the parameters.
        /// </param>
        private void SetOpenIdParams(string inpUrl)
        {
            const string method = "GET";
            const string referer = "https://backpack.tf/";
            const string host = "steamcommunity.com";
            const string accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            var responseString = FetchRequest(inpUrl, method, referer, host, accept, null, false, false);

            var temp = Regex.Split(responseString, "<input type=\"hidden\" id=\"actionInput\" name=\"action\" value=\"");
            _action = Regex.Split(temp[1], "\"")[0];

            var temp1 = Regex.Split(responseString, "<input type=\"hidden\" name=\"openid\\Smode\" value=\"");
            _openidMode = Regex.Split(temp1[1], "\"")[0];

            var temp2 = Regex.Split(responseString, "<input type=\"hidden\" name=\"openidparams\" value=\"");
            _openidparams = Regex.Split(temp2[1], "\" />")[0];

            var temp3 = Regex.Split(responseString, "<input type=\"hidden\" name=\"nonce\" value=\"");
            _nonce = Regex.Split(temp3[1], "\" />")[0];
        }

        /// <summary>
        /// Do Login method of Steam.
        /// </summary>
        /// <param name="username">
        /// Username of your Steam account.
        /// </param>
        /// <param name="password">
        /// Password of your Steam account
        /// </param>
        /// <returns>
        /// A bool if the login was successful or not.
        /// </returns>
        private dynamic DoSteamLogin(string username, string password, string inpSharedSecret)
        {
            dynamic returnObject;
            var data = new NameValueCollection { { "username", username } };
            const string accept = "text/javascript, text/html, application/xml, text/xml, */*";
            const string referer = "https://steamcommunity.com/openid/login?openid.ns=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0&openid.mode=checkid_setup&openid.return_to=http%3A%2F%2Fbackpack.tf%2Flogin&openid.realm=http%3A%2F%2Fbackpack.tf&openid.ns.sreg=http%3A%2F%2Fopenid.net%2Fextensions%2Fsreg%2F1.1&openid.claimed_id=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select&openid.identity=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select";
            const string host = "steamcommunity.com";
            var response = FetchRequest("https://steamcommunity.com/login/getrsakey", "POST", referer, host, accept,
                data, true, false);
            var rsaJson = JsonConvert.DeserializeObject<GetRsaKeyBackPack>(response);

            // Validate
            if (!rsaJson.success)
            {
                return null;
            }

            //RSA Encryption
            var rsa = new RSACryptoServiceProvider();
            var rsaParameters = new RSAParameters
            {
                Exponent = HexToByte(rsaJson.publickey_exp),
                Modulus = HexToByte(rsaJson.publickey_mod)
            };

            rsa.ImportParameters(rsaParameters);

            byte[] bytePassword = Encoding.ASCII.GetBytes(password);
            byte[] encodedPassword = rsa.Encrypt(bytePassword, false);
            string encryptedBase64Password = Convert.ToBase64String(encodedPassword);

            SteamResultBackPack loginJson = null;
            CookieCollection cookieCollection;
            string steamGuardText = "";
            string steamGuardId = "";
            do
            {
                Console.WriteLine("SteamWeb: Logging In...");

                bool captcha = loginJson != null && loginJson.captcha_needed == true;
                bool steamGuard = loginJson != null && loginJson.emailauth_needed == true;

                string time = Uri.EscapeDataString(rsaJson.timestamp);
                string capGID = "-1";
                if (loginJson != null && loginJson.captcha_gid != null)
                {

                    capGID = loginJson == null ? null : Uri.EscapeDataString(loginJson.captcha_gid);
                }
                data = new NameValueCollection { { "password", encryptedBase64Password }, { "username", username } };

                // Captcha
                string capText = "";
                if (captcha)
                {
                    Console.WriteLine("SteamWeb: Captcha is needed.");
                    System.Diagnostics.Process.Start("https://steamcommunity.com/public/captcha.php?gid=" + loginJson.captcha_gid);
                    Console.WriteLine("SteamWeb: Type the captcha:");
                    capText = Uri.EscapeDataString(Console.ReadLine());
                }

                data.Add("captchagid", captcha ? capGID : "");
                data.Add("captcha_text", captcha ? capText : "");
                if (inpSharedSecret != "")
                {
                    var twoFactorGenerator = new SteamTwoFactorGenerator {SharedSecret = inpSharedSecret};
                    data.Add("twofactorcode", twoFactorGenerator.GenerateSteamGuardCodeForTime(DateTime.Now));
                }
                else
                {
                    data.Add("twofactorcode", "");
                }
                
                data.Add("remember_login", "false");

                // Captcha end

                // SteamGuard
                if (steamGuard)
                {
                    Console.WriteLine("SteamWeb: SteamGuard is needed.");
                    Console.WriteLine("SteamWeb: Type the code:");
                    steamGuardText = Uri.EscapeDataString(Console.ReadLine());
                    steamGuardId = loginJson.emailsteamid;
                    data.Add("loginfriendlyname", "machine1");
                }
                else
                {
                    data.Add("loginfriendlyname", inpSharedSecret);
                }

                data.Add("emailauth", steamGuardText);
                data.Add("emailsteamid", steamGuardId);
                data.Add("donotcache", "1440170894227");

                // SteamGuard end

                data.Add("rsatimestamp", time);

                using (HttpWebResponse webResponse = Request("https://steamcommunity.com/login/dologin/", "POST", referer, host, accept, data, true, false))
                {
                    using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        string json = reader.ReadToEnd();
                        returnObject = JObject.Parse(json);
                        loginJson = JsonConvert.DeserializeObject<SteamResultBackPack>(json);
                        cookieCollection = webResponse.Cookies;
                    }
                }
            } while (loginJson.captcha_needed || loginJson.emailauth_needed);


            if (loginJson.success)
            {
                foreach (Cookie cookie in cookieCollection)
                {
                    _cookies.Add(cookie);
                }
                return returnObject;
            }
            else
            {
                Console.WriteLine("SteamWeb Error: " + loginJson.message);
                return null;
            }
        }

        /// <summary>
        /// Hex to Byte method.
        /// </summary>
        /// <param name="hex">
        /// Hex value.
        /// </param>
        /// <returns>
        /// Hex Value as Byte[].
        /// </returns>
        private byte[] HexToByte(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];
            int l = hex.Length;

            for (int i = 0; i < (l >> 1); ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        /// <summary>
        /// Get the Hex Value out of char.
        /// </summary>
        /// <param name="hex">
        /// Char to input.
        /// </param>
        /// <returns>
        /// An int.
        /// </returns>
        private int GetHexVal(char hex)
        {
            var val = (int)hex;
            return val - (val < 58 ? 48 : 55);
        }

        /// <summary>
        /// Post transfer url from steam to backpack.
        /// </summary>
        /// <param name="inpSteamId">
        /// Transfer parameter.
        /// </param>
        /// <param name="inpToken">
        /// Transfer parameter.
        /// </param>
        /// <param name="inpAuth">
        /// Transfer parameter.
        /// </param>
        /// <param name="inpRememberLogin">
        /// Transfer parameter.
        /// </param>
        /// <param name="inpTokenSecure">
        /// Transfer parameter.
        /// </param>
        private void PostTransfer(string inpSteamId, string inpToken, string inpAuth, string inpRememberLogin, string inpTokenSecure)
        {
            const string url = "https://store.steampowered.com/login/transfer";
            const string accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            const string referer = "https://steamcommunity.com/openid/login?openid.ns=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0&openid.mode=checkid_setup&openid.return_to=http%3A%2F%2Fbackpack.tf%2Flogin&openid.realm=http%3A%2F%2Fbackpack.tf&openid.ns.sreg=http%3A%2F%2Fopenid.net%2Fextensions%2Fsreg%2F1.1&openid.claimed_id=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select&openid.identity=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select";
            const string host = "store.steampowered.com";
            var data = new NameValueCollection
            {
                {"steamid", inpSteamId},
                {"token", inpToken},
                {"auth", inpAuth},
                {"remember_login", inpRememberLogin},
                {"token_secure", inpTokenSecure}
            };
            FetchRequest(url, "POST", referer, host, accept, data, false, false);
        }

        /// <summary>
        /// Open Id Login with steam.
        /// </summary>
        /// <returns>
        /// The redirect url.
        /// </returns>
        private string OpenIdLogin()
        {
            string resultString;
            const string url = "https://steamcommunity.com/openid/login";
            const string referer = "https://steamcommunity.com/openid/login?openid.ns=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0&openid.mode=checkid_setup&openid.return_to=http%3A%2F%2Fbackpack.tf%2Flogin&openid.realm=http%3A%2F%2Fbackpack.tf&openid.ns.sreg=http%3A%2F%2Fopenid.net%2Fextensions%2Fsreg%2F1.1&openid.claimed_id=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select&openid.identity=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select";
            const string host = "steamcommunity.com";
            const string accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            var data = new NameValueCollection
            {
                {"action", _action},
                {"openid.mode", _openidMode},
                {"openidparams", _openidparams},
                {"nonce", _nonce}
            };
            using (var httpResponse = Request(url, "POST", referer, host, accept, data, false, false))
            {
                resultString = httpResponse.Headers["Location"].ToString();
            }
            return resultString;
        }

        /// <summary>
        /// Request to backpack.tf to get the cookies
        /// </summary>
        /// <param name="inpUrl">
        /// Url to redirect to.
        /// </param>
        /// <returns>
        /// The html body of the url.
        /// </returns>
        private string GetBackpackCookies(string inpUrl)
        {
            string returnString;
            const string accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            const string host = "backpack.tf";
            using (var httpResponse = Request(inpUrl, "GET", "", host, accept, null, false, false))
            {
                _cookies.Add(httpResponse.Cookies);
                foreach (var item in httpResponse.Cookies)
                {
                    var cookieString = item.ToString();
                    if (cookieString.Contains("user"))
                    {
                        _stackUser = Regex.Split(cookieString, "=")[1];
                    }
                    if (cookieString.Contains("hash"))
                    {
                        _stackHash = Regex.Split(cookieString, "=")[1];
                    }
                }
                returnString = httpResponse.Headers["Location"];
            }
            return returnString;
        }

        /// <summary>
        /// Same as Request. Just that this gives back a string.
        /// </summary>
        /// <param name="inpUrl">Url.</param>
        /// <param name="inpMethod">Method.</param>
        /// <param name="inpReferer">Referer.</param>
        /// <param name="inpHost">Host.</param>
        /// <param name="inpAccept">Accept Header.</param>
        /// <param name="inpNvc">NameValueCollection.</param>
        /// <param name="xml">Is the request an xml request.</param>
        /// <param name="autoRedirect">Redirect.</param>
        /// <returns>The body of the Url.</returns>
        private string FetchRequest(string inpUrl, string inpMethod, string inpReferer, string inpHost, string inpAccept, NameValueCollection inpNvc, bool xml, bool autoRedirect)
        {
            using (var response = Request(inpUrl, inpMethod, inpReferer, inpHost, inpAccept, inpNvc, xml, autoRedirect))
            {
                _cookies.Add(response.Cookies);
                using (var responseStream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(responseStream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        /// <summary>
        /// The formal method to use HTTPWebRequests.
        /// I got the headers out of Fiddler. I highly recommend this tool to follow HTTPWebRequests.
        /// Some Headers are unnecessary but i will use them for a natural-looking HTTPWebRequests.
        /// </summary>
        /// <param name="inpUrl">URL of the HTTPWebRequest.</param>
        /// <param name="inpMethod">Method of the HTTPWebRequest.
        /// Example: "POST", "GET", "PUT",...</param>
        /// <param name="inpReferer">The Referer of the URL. Sometimes its needed.</param>
        /// <param name="inpHost">The Host of the request.</param>
        /// <param name="inpAccept">Accept format of the request.</param>
        /// <param name="inpNvc">Data which will be wrote if you will do a "Post".</param>
        /// <param name="inpXml">If Ajax, then this is required.</param>
        /// <param name="autoRedirect">Auto-Redirect.</param>
        /// <returns>A HttpWebResponse of the request.</returns>
        private HttpWebResponse Request(string inpUrl, string inpMethod, string inpReferer, string inpHost, string inpAccept, NameValueCollection inpNvc, bool inpXml, bool autoRedirect)
        {
            var request = (HttpWebRequest)WebRequest.Create(inpUrl);
            request.Accept = inpAccept;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.57 Safari/537.36";
            request.Timeout = 20000;
            request.Headers.Add("Accept-Language", "de,en-US;q=0.7,en;q=0.3");
            request.AllowAutoRedirect = autoRedirect;
            request.CookieContainer = _cookies;
            request.Method = inpMethod;
            //Volatile variables

            if (inpHost != "")
            {
                request.Host = inpHost;
            }

            if (inpReferer != "")
            {
                request.Referer = inpReferer;
            }
            if (inpXml)
            {
                request.Headers.Add("X-Requested-With", "XMLHttpRequest");
                request.Headers.Add("X-Prototype-Version", "1.7");
                request.Headers.Add("Cache-Control", "no-cache");
                request.Headers.Add("Pragma", "no-cache");
            }

            if (inpMethod != "POST") return request.GetResponse() as HttpWebResponse;
            var dataString = (inpNvc == null ? null : string.Join("&", Array.ConvertAll(inpNvc.AllKeys, key =>
                string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(inpNvc[key]))
                )));
            if (dataString == null) return request.GetResponse() as HttpWebResponse;
            var dataBytes = Encoding.UTF8.GetBytes(dataString);
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.ContentLength = dataBytes.Length;
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(dataBytes, 0, dataBytes.Length);
            }
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// Method to read the cookies from disk.
        /// </summary>
        /// <param name="file">
        /// Path of the txt file.
        /// </param>
        /// <returns>
        /// The cookie container.
        /// </returns>
        private CookieContainer ReadCookiesFromDisk(string file)
        {
            try
            {
                using (Stream stream = File.Open(file, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    return (CookieContainer)formatter.Deserialize(stream);
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Problem reading cookies from disk: " + e.GetType());
                return new CookieContainer();
            }
        }

        /// <summary>
        /// Method to Write cookies to the disk.
        /// </summary>
        /// <param name="file">
        /// Path of the file
        /// </param>
        /// <param name="cookieJar">
        /// CookieContainer which should be written into the text.
        /// </param>
        private void WriteCookiesToDisk(string file, CookieContainer cookieJar)
        {
            using (Stream stream = File.Create(file))
            {
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, cookieJar);
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine("Problem writing cookies to disk: " + e.GetType());
                }
            }
        }
    }

    /// <summary>
    /// RSA Key data.
    /// </summary>
    public class GetRsaKeyBackPack
    {
        public bool success { get; set; }

        public string publickey_mod { get; set; }

        public string publickey_exp { get; set; }

        public string timestamp { get; set; }
    }

    /// <summary>
    /// Class for JSON Deserilization
    /// </summary>
    public class SteamResultBackPack
    {
        public bool success { get; set; }

        public string message { get; set; }

        public bool captcha_needed { get; set; }

        public string captcha_gid { get; set; }

        public bool emailauth_needed { get; set; }

        public string emailsteamid { get; set; }

    }
}