using Newtonsoft.Json;

namespace HedgehogSoft.BackpackLogin.Models
{
    internal class OffsetResponse
    {
        [JsonProperty(PropertyName = "response")]
        internal OffsetParameters OffsetParameters { get; set; }
    }
}
