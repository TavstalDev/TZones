using Tavstal.TLibrary.Models.Database.Attributes;
using Tavstal.TZones.Models.Enums;

namespace Tavstal.TZones.Models.Core
{
    [SqlName("zones_events")]
    public class ZoneEvent
    {
        [SqlMember(isPrimaryKey: true, isUnsigned: true, shouldAutoIncrement: true)]
        public ulong Id { get; set; }
        
        [SqlMember(isUnsigned: true)]
        public ulong ZoneId { get; set; }
        
        [SqlMember]
        public EEventType Type { get; set; }
        
        [SqlMember(columnType: "varchar(255)")]
        public string Value { get; set; }

        public ZoneEvent()
        {
            Value = string.Empty;
        }

        public ZoneEvent(ulong id, ulong zoneId, EEventType type, string value)
        {
            Id = id;
            ZoneId = zoneId;
            Type = type;
            Value = value;
        }
    }
}