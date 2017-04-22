using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BackpackLoginExample.Settings
{
    internal class Loader
    {
        internal static void LoadSettings()
        {
            var fileContent = LoadSettingsFile();
            var settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileContent);
            foreach (var entry in settings)
            {
                ConsoleSettings.Instance.Add(entry.Key, entry.Value);
            }
        }

        private static string LoadSettingsFile()
        {
            string result;
            using (var streamReader = File.OpenText("./settings.json"))
            {
                result = streamReader.ReadToEnd();
            }
            return result;
        }
    }
}
