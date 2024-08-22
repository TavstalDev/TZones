using System;
using Tavstal.TLibrary.Models.Database.Attributes;
using Tavstal.TZones.Models.Enums;

namespace Tavstal.TZones.Models.Core
{
    [Serializable]
    public class ZoneEvent
    {
        [SqlMember(isUnsigned: true)]
        public ulong ZoneId { get; set; }
        [SqlMember]
        public EEventType Type { get; set; }
        [SqlMember(columnType: "varchar(255)")]
        public string Value { get; set; }

        public ZoneEvent() {}

        public ZoneEvent(ulong zoneId, EEventType type, string value)
        {
            ZoneId = zoneId;
            Type = type;
            Value = value;
        }
    }
}