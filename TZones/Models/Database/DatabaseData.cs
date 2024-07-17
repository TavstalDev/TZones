using Newtonsoft.Json;
using Tavstal.TLibrary.Compatibility;

namespace Tavstal.TZones.Models.Database
{
    public class DatabaseData : DatabaseSettingsBase
    {
        // Note: It starts from 7 because there are 6 defined property in the base class
        [JsonProperty(Order = 7)]
        public string TableZones { get; set; }
        [JsonProperty(Order = 8)]
        public string TableZoneNodes { get; set; }
        [JsonProperty(Order = 9)]
        public string TableFlags { get; set; }
        [JsonProperty(Order = 10)]
        public string TableZoneFlags { get; set; }
        [JsonProperty(Order = 11)]
        public string TableZoneEvents { get; set; }
        [JsonProperty(Order = 12)]
        public string TableZoneBlocklist { get; set; }

        public DatabaseData(string tableZones, string tableZoneNodes, string tableFlags, string tableZoneFlags, string tableZoneEvents, string tableZoneBlocklist)
        {
            TableZones = tableZones;
            TableZoneNodes = tableZoneNodes;
            TableFlags = tableFlags;
            TableZoneFlags = tableZoneFlags;
            TableZoneEvents = tableZoneEvents;
            TableZoneBlocklist = tableZoneBlocklist;
        }
    }
}
