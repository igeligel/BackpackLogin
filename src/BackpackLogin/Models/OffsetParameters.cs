using Newtonsoft.Json;

namespace HedgehogSoft.BackpackLogin.Models
{
    internal class OffsetParameters
    {
        [JsonProperty(PropertyName = "server_time")]
        internal string ServerTime { get; set; }

        [JsonProperty(PropertyName = "skew_tolerance_seconds")]
        internal string SkewToleranceSeconds { get; set; }

        [JsonProperty(PropertyName = "large_time_jink")]
        internal string LargeTimeJink { get; set; }

        [JsonProperty(PropertyName = "probe_frequency_seconds")]
        internal int ProbeFrequencySeconds { get; set; }

        [JsonProperty(PropertyName = "adjusted_time_probe_frequency_seconds")]
        internal int AdjustedTimeProbeFrequencySeconds { get; set; }

        [JsonProperty(PropertyName = "hint_probe_frequency_seconds")]
        internal int HintProbeFrequencySeconds { get; set; }

        [JsonProperty(PropertyName = "sync_timeout")]
        internal int SyncTimeout { get; set; }

        [JsonProperty(PropertyName = "try_again_seconds")]
        internal int TryAgainSeconds { get; set; }

        [JsonProperty(PropertyName = "max_attempts")]
        internal int MaxAttempts { get; set; }
    }
}
