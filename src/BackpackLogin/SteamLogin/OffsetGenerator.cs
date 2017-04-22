using HedgehogSoft.BackpackLogin.Models;
using HedgehogSoft.BackpackLogin.Rest;
using Newtonsoft.Json;

namespace HedgehogSoft.BackpackLogin.SteamLogin
{
    internal class OffsetGenerator
    {
        internal static OffsetResponse GetOffset()
        {
            var response = new RestClient().GetSteamOffset();
            return JsonConvert.DeserializeObject<OffsetResponse>(response.Content.ReadAsStringAsync().Result);
        }
    }
}
