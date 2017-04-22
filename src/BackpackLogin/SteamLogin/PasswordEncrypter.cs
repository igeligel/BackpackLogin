using System;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace HedgehogSoft.BackpackLogin.SteamLogin
{
    internal static class PasswordEncrypter
    {
        private const string Base64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
        private const string Hex = "0123456789abcdef";

        internal static string EncryptPassword(string publicKeyMod, string publicKeyExp, string password)
        {
            // Convert the public keys to BigIntegers
            var modulus = CreateBigInteger(publicKeyMod);
            var exponent = CreateBigInteger(publicKeyExp);

            // Original: $data = this.pkcs1pad2($data,($pubkey.modulus.bitLength()+7)>>3);
            // I'm going to hardcode the bitlength, I can't figure that out right now.
            var encryptedNumber = Pkcs1Pad2(password, (2048 + 7) >> 3);

            // And now, the RSA encryption
            encryptedNumber = BigInteger.ModPow(encryptedNumber, exponent, modulus);

            // Finally we convert the encrypted string back to Base16
            var encryptedString = encryptedNumber.ToString("x");

            // And then we decode it back
            // And we put it back into Base64
            encryptedString = EncodeBase64(DecodeHex(encryptedString));

            return encryptedString;
        }


        public static string DecodeHex(string input)
        {
            var stringBuilder = new StringBuilder();
            var i = 0;
            do
            {
                var a = ((Hex.IndexOf(input[i++]) << 4) & 0xf0);
                var b = 0;

                if (i < input.Length)
                    b = (Hex.IndexOf(input[i++]) & 0xf);

                stringBuilder.Append(new[] { (char)(a | b) });
            } while (i < input.Length);

            return stringBuilder.ToString();
        }

        public static string EncodeBase64(string input)
        {
            var stringBuilder = new StringBuilder();
            var i = 0;

            do
            {
                var x = 0;
                int chr1 = input[i++];
                int chr2, chr3;
                if (input.Length > i)
                {
                    chr2 = input.Length > i ? input[i++] : 0;

                    if (input.Length > i)
                    {
                        chr3 = input.Length > i ? input[i++] : 0;
                    }
                    else
                    {
                        x = 2;
                        chr3 = 0;
                    }
                }
                else
                {
                    x = 1;
                    chr2 = 0;
                    chr3 = 0;
                }

                var enc1 = chr1 >> 2;
                var enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
                var enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
                var enc4 = chr3 & 63;

                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (x == 1)
                {
                    enc3 = enc4 = 64;
                }
                else if (x == 2)
                {
                    enc4 = 64;
                }

                stringBuilder.Append(new[] { Base64[enc1], Base64[enc2], Base64[enc3], Base64[enc4] });
            } while (i < input.Length);

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Encodes the data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="keySize"></param>
        /// <returns></returns>
        public static BigInteger Pkcs1Pad2(string data, int keySize)
        {
            if (keySize < data.Length + 11)
                return new BigInteger();

            var buffer = new byte[256];
            var i = data.Length - 1;

            while (i >= 0 && keySize > 0)
            {
                buffer[--keySize] = (byte)data[i--];
            }

            // Padding, I think
            var random = new Random();
            buffer[--keySize] = 0;
            while (keySize > 2)
            {
                buffer[--keySize] = (byte)random.Next(1, 256);
                //buffer[--keySize] = 5;
            }

            buffer[--keySize] = 2;
            buffer[--keySize] = 0;

            Array.Reverse(buffer);

            return new BigInteger(buffer);
        }

        public static BigInteger CreateBigInteger(string hex)
        {
            return BigInteger.Parse("00" + hex, NumberStyles.AllowHexSpecifier);
        }
    }
}
