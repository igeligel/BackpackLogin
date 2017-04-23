using Newtonsoft.Json;

namespace HedgehogSoft.BackpackLogin.Models
{
    internal class RsaResponse
    {
        [JsonProperty(PropertyName = "success")]
        internal bool Success { get; set; }

        [JsonProperty(PropertyName = "publickey_mod")]
        internal string PublickeyMod { get; set; }

        [JsonProperty(PropertyName = "publickey_exp")]
        internal string PublickeyExp { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        internal string Timestamp { get; set; }

        [JsonProperty(PropertyName = "token_gid")]
        internal string TokenGid { get; set; }
    }
}
