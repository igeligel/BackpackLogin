#region Used License
// The MIT License(MIT)

// Copyright(c) 2015 Joshua Coffey

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion
using System;
using System.Security.Cryptography;
using System.Text;

namespace HedgehogSoft.BackpackLogin.SteamLogin
{
    internal class SteamTwoFactorGenerator
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
        internal string SharedSecret;
        /// <summary>
        /// byte to do the Steam Guard Code translation.
        /// </summary>
        private static readonly byte[] SteamGuardCodeTranslations = { 50, 51, 52, 53, 54, 55, 56, 57, 66, 67, 68, 70, 71, 72, 74, 75, 77, 78, 80, 81, 82, 84, 86, 87, 88, 89 };

        /// <summary>
        /// Generate Steam Guard Code for a specific time. Therefore you need the shared secret attacked to an instance of this file.
        /// </summary>
        /// <returns>The string which is five characters long which you need to authenticate on steam.</returns>
        internal string GenerateSteamGuardCodeForTime()
        {
            var timestamp = long.Parse(OffsetGenerator.GetOffset().OffsetParameters.ServerTime);
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
            return Encoding.UTF8.GetString(codeArray).ToLower();
        }
    }
}
