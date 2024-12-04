using Newtonsoft.Json;
using Tavstal.TZones.Models.Database;
using Tavstal.TLibrary.Models.Plugin;

namespace Tavstal.TZones
{
    public class ZonesConfig : ConfigurationBase
    {
        /*[JsonProperty(Order = 3)]
        public string StorageType { get; set; }*/
        [JsonProperty(Order = 3)]
        public DatabaseData Database { get; set; }

        public override void LoadDefaults()
        {
            DebugMode = false;
            Locale = "en";
            DownloadLocalePacks = true;
            Database = new DatabaseData("tzones_zones", "tzones_nodes", "tzones_flags", "tzones_zoneflags", "tzones_events", "tzones_blocklist");
        }

        // Required because of the library
        public ZonesConfig() { }
        public ZonesConfig(string fileName, string path) : base(fileName, path) { }
    }
}
