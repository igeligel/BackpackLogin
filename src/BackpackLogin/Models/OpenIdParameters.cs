using System.Linq;
using System.Net.Http;
using AngleSharp.Parser.Html;
using HedgehogSoft.BackpackLogin.Interfaces;

namespace HedgehogSoft.BackpackLogin.Models
{
    internal class OpenIdParameters:IOpenIdParameters
    {
        public string Action { get; set; }
        public string OpenIdMode { get; set; }
        public string OpenIdParams { get; set; }
        public string Nonce { get; set; }

        internal OpenIdParameters(HttpResponseMessage httpResponseMessage)
        {
            var responseBody = httpResponseMessage.Content.ReadAsStringAsync().Result;
            var document = new HtmlParser().Parse(responseBody);
            var allInputs = document.QuerySelectorAll("input");
            Action = allInputs.FirstOrDefault(e => e.GetAttribute("name") == "action")
                .Attributes.FirstOrDefault(e => e.Name == "value")
                .Value;
            OpenIdMode = allInputs.FirstOrDefault(e => e.GetAttribute("name") == "openid.mode")
                .Attributes.FirstOrDefault(e => e.Name == "value")
                .Value;
            OpenIdParams = allInputs.FirstOrDefault(e => e.GetAttribute("name") == "openidparams")
                .Attributes.FirstOrDefault(e => e.Name == "value")
                .Value;
            Nonce = allInputs.FirstOrDefault(e => e.GetAttribute("name") == "nonce")
                .Attributes.FirstOrDefault(e => e.Name == "value")
                .Value;
        }
    }
}
