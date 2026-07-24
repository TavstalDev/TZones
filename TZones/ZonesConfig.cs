using Tavstal.TLibrary.Models.Config;
using Tavstal.TZones.Models.Database;
using YamlDotNet.Serialization;
// ReSharper disable ClassNeverInstantiated.Global

namespace Tavstal.TZones
{
    /// <summary>
    /// YAML configuration model for the TZones plugin.
    /// </summary>
    public class ZonesConfig : YamlConfiguration
    {
        /// <summary>
        /// The database connection and table configuration.
        /// </summary>
        [YamlMember(Order = 3)]
        public DatabaseData Database { get; set; } = new  DatabaseData();

        /// <summary>
        /// Loads the default configuration values.
        /// </summary>
        public override void LoadDefaults()
        {
            General = new GeneralConfig
            {
                MessageIcon = "https://raw.githubusercontent.com/TavstalDev/TZones/refs/heads/master/assets/icon.png"
            };
            Database = new DatabaseData();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZonesConfig"/> class. Required by the library.
        /// </summary>
        public ZonesConfig() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ZonesConfig"/> class with a file name and path.
        /// </summary>
        /// <param name="fileName">The configuration file name.</param>
        /// <param name="path">The directory path of the configuration file.</param>
        public ZonesConfig(string fileName, string path) : base(fileName, path) {}
    }
}
