using HedgehogSoft.BackpackLogin.Interfaces;

namespace HedgehogSoft.BackpackLogin.Models
{
    internal class OpenIdParameters:IOpenIdParameters
    {
        public string Action { get; set; }
        public string OpenIdMode { get; set; }
        public string OpenIdParams { get; set; }
        public string Nonce { get; set; }
    }
}
