using System.Collections.Generic;

namespace HedgehogSoft.BackpackLogin.Extensions
{
    internal static class DictionaryExtensions
    {
        internal static Dictionary<string, string> AddCacheControl(this Dictionary<string, string> dictionary, string value)
        {
           dictionary.Add("Cache-Control", value);
            return dictionary;
        }

        internal static Dictionary<string, string> AddPragma(this Dictionary<string, string> dictionary)
        {
            dictionary.Add("Pragma", "no-cache");
            return dictionary;
        }

        internal static Dictionary<string, string> AddAccept(this Dictionary<string, string> dictionary, string value)
        {
            dictionary.Add("Accept", value);
            return dictionary;
        }

        internal static Dictionary<string, string> AddUpgradeInsecureRequests(this Dictionary<string, string> dictionary)
        {
            dictionary.Add("Upgrade-Insecure-Requests", "1");
            return dictionary;
        }

        internal static Dictionary<string, string> AddReferer(this Dictionary<string, string> dictionary,
            string referer)
        {
            dictionary.Add("referer", referer);
            return dictionary;
        }

        internal static Dictionary<string, string> AddOrigin(this Dictionary<string, string> dictionary, string origin)
        {
            dictionary.Add("Origin", origin);
            return dictionary;
        }

        internal static Dictionary<string, string> AddXrequestedWith(this Dictionary<string, string> dictionary)
        {
            dictionary.Add("X-Requested-With", "XMLHttpRequest");
            return dictionary;
        }
    }
}
