using System;
using Tavstal.TLibrary.Models.Database.Attributes;

namespace Tavstal.TZones.Models.Core
{
    [Serializable]
    public class ZoneFlag
    {
        [SqlMember(isUnsigned: true)]
        public ulong ZoneId { get; set; }
        [SqlMember(isUnsigned: true)]
        public ulong FlagId { get; set; }   

        public ZoneFlag() {}

        public ZoneFlag(ulong zoneId, ulong flagId)
        {
            ZoneId = zoneId;
            FlagId = flagId;
        }
    }
}