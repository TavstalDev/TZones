using Tavstal.TLibrary.Models.Config;
using YamlDotNet.Serialization;

namespace Tavstal.TZones.Models.Database
{
    public class DatabaseData : DatabaseConfigBase
    {
        // Note: It starts from 7 because there are 6 defined property in the base class
        [YamlMember(Order = 7)] 
        public string TablePrefix { get; set; } = "tzones_";
    }
}
