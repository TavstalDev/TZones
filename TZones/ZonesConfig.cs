using Tavstal.TLibrary.Models.Config;
using Tavstal.TZones.Models.Database;
using YamlDotNet.Serialization;
// ReSharper disable ClassNeverInstantiated.Global

namespace Tavstal.TZones
{
    public class ZonesConfig : YamlConfiguration
    {
        [YamlMember(Order = 3)]
        public DatabaseData Database { get; set; } = new  DatabaseData();

        public override void LoadDefaults()
        {
            General = new GeneralConfig
            {
                MessageIcon = "https://raw.githubusercontent.com/TavstalDev/TZones/refs/heads/master/assets/icon.png"
            };
            Database = new DatabaseData();
        }

        // Required because of the library
        public ZonesConfig() {}

        public ZonesConfig(string fileName, string path) : base(fileName, path) {}
    }
}
