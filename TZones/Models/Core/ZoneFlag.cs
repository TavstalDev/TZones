using Tavstal.TLibrary.Models.Database.Attributes;

namespace Tavstal.TZones.Models.Core
{
    [SqlName("zones_flags")]
    public class ZoneFlag
    {
        [SqlMember(isPrimaryKey: true, isUnsigned: true, shouldAutoIncrement: true)]
        public ulong Id { get; set; }
        
        [SqlMember(isUnsigned: true)]
        public ulong ZoneId { get; set; }
        
        [SqlMember(isUnsigned: true)]
        public ulong FlagId { get; set; }   

        public ZoneFlag() {}

        public ZoneFlag(ulong id, ulong zoneId, ulong flagId)
        {
            Id = id;
            ZoneId = zoneId;
            FlagId = flagId;
        }
    }
}