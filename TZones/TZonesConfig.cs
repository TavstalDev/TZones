using Newtonsoft.Json;
using Tavstal.TZones.Models;
using Tavstal.TLibrary.Compatibility;

namespace Tavstal.TZones
{
    public class TZonesConfig : ConfigurationBase
    {
        [JsonProperty(Order = 3)]
        public DatabaseData Database { get; set; }

        public override void LoadDefaults()
        {
            Database = new DatabaseData("example_players");
        }

        // Required because of the library
        public TZonesConfig() { }
        public TZonesConfig(string fileName, string path) : base(fileName, path) { }
    }
}
