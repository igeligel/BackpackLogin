using System;
using System.Security.Cryptography;
using System.Text;

namespace BackpackLogin.API
{
    /// <summary>
    /// Class to enable two factor authorization for the login process on backpack.tf.
    /// </summary>
    public class SteamTwoFactorGenerator
    {
        /// <summary>
        /// String which is the shared secret. This will be provided if you add an authenticator. You will get your shared secret here:
        /// https://api.steampowered.com/ITwoFactorService/AddAuthenticator/v0001.
        /// <example>
        /// This is the json response deserialized to an c# object.
        /// <code>
        /// public class Response {
        ///     public string shared_secret { get; set; }
        ///     public string serial_number { get; set; }
        ///     public string revocation_code { get; set; }
        ///     public string uri { get; set; }
        ///     public string server_time { get; set; }
        ///     public string account_name { get; set; }
        ///     public string token_gid { get; set; }
        ///     public string identity_secret { get; set; }
        ///     public string secret_1 { get; set; }
        ///     public int status { get; set; }
        /// }
        /// public class RootObject
        /// {
        ///     public Response response { get; set; }
        /// }
        /// </code>
        /// </example>
        /// You will just need the shared_secret.
        /// </summary>
        public string SharedSecret;
        /// <summary>
        /// byte to do the Steam Guard Code translation.
        /// </summary>
        private static readonly byte[] SteamGuardCodeTranslations = { 50, 51, 52, 53, 54, 55, 56, 57, 66, 67, 68, 70, 71, 72, 74, 75, 77, 78, 80, 81, 82, 84, 86, 87, 88, 89 };

        /// <summary>
        /// Generate Steam Guard Code for a specific time. Therefore you need the shared secret attacked to an instance of this file.
        /// </summary>
        /// <returns>The string which is five characters long which you need to authenticate on steam.</returns>
        public string GenerateSteamGuardCodeForTime()
        {
            var timestamp = (long)DateTime.Now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            if (string.IsNullOrEmpty(SharedSecret))
            {
                return "";
            }

            var sharedSecretArray = Convert.FromBase64String(SharedSecret);
            var timeArray = new byte[8];

            timestamp /= 30L;

            for (var i = 8; i > 0; i--)
            {
                timeArray[i - 1] = (byte)timestamp;
                timestamp >>= 8;
            }

            var hmacGenerator = new HMACSHA1 { Key = sharedSecretArray };
            var hashedData = hmacGenerator.ComputeHash(timeArray);
            var codeArray = new byte[5];
            try
            {
                var b = (byte)(hashedData[19] & 0xF);
                var codePoint = (hashedData[b] & 0x7F) << 24 | (hashedData[b + 1] & 0xFF) << 16 | (hashedData[b + 2] & 0xFF) << 8 | (hashedData[b + 3] & 0xFF);

                for (var i = 0; i < 5; ++i)
                {
                    codeArray[i] = SteamGuardCodeTranslations[codePoint % SteamGuardCodeTranslations.Length];
                    codePoint /= SteamGuardCodeTranslations.Length;
                }
            }
            catch (Exception)
            {
                return null;
            }
            return Encoding.UTF8.GetString(codeArray);
        }
    }
}
