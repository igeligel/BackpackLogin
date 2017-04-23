using System.Collections.Generic;
using HedgehogSoft.BackpackLogin.Extensions;

namespace HedgehogSoft.BackpackLogin.Rest
{
    internal static class RequestHeaders
    {
        internal static Dictionary<string, string> GetBackpackMainHeaders(this Dictionary<string, string> dictionary)
        {
            return dictionary
                .AddCacheControl("no-cache")
                .AddPragma()
                .AddAccept("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8")
                .AddUpgradeInsecureRequests();
        }

        internal static Dictionary<string, string> GetBackpackLoginHeaders(this Dictionary<string, string> dictionary)
        {
            return dictionary
                .AddCacheControl("max-age=0")
                .AddAccept("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8")
                .AddReferer("http://backpack.tf/")
                .AddOrigin("http://backpack.tf")
                .AddUpgradeInsecureRequests();
        }

        internal static Dictionary<string, string> GetOpenIdParametersHeaders(this Dictionary<string, string> dictionary)
        {
            return dictionary
                .AddCacheControl("max-age=0")
                .AddAccept("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8")
                .AddReferer("http://backpack.tf/")
                .AddUpgradeInsecureRequests();
        }

        internal static Dictionary<string, string> GetRsaKeyHeaders(this Dictionary<string, string> dictionary, string referer)
        {
            return dictionary
                .AddAccept("*/*")
                .AddXrequestedWith()
                .AddReferer(referer)
                .AddOrigin("https://steamcommunity.com");
        }

        internal static Dictionary<string, string> PostDoLoginHeaders(this Dictionary<string, string> dictionary, string referer)
        {
            return dictionary
                .AddAccept("*/*")
                .AddXrequestedWith()
                .AddReferer(referer)
                .AddOrigin("https://steamcommunity.com");
        }

        internal static Dictionary<string, string> PostOpenIdLoginHeaders(this Dictionary<string, string> dictionary, string referer)
        {
            return dictionary
                .AddCacheControl("max-age=0")
                .AddAccept("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8")
                .AddReferer(referer)
                .AddOrigin("https://steamcommunity.com")
                .AddUpgradeInsecureRequests();
        }

        internal static Dictionary<string, string> GetOpenIdRedirectHeaders(this Dictionary<string, string> dictionary)
        {
            return dictionary
                .AddCacheControl("max-age=0")
                .AddAccept("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8")
                .AddUpgradeInsecureRequests();
        }

        internal static Dictionary<string, string> ValidateBackpackMainHeaders(this Dictionary<string, string> dictionary)
        {
            return dictionary
                .AddCacheControl("max-age=0")
                .AddAccept("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8")
                .AddUpgradeInsecureRequests();
        }
    }
}
