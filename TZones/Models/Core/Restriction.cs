using Tavstal.TLibrary.Models.Database.Attributes;
using Tavstal.TZones.Models.Enums;

namespace Tavstal.TZones.Models.Core
{
    [SqlName("zones_restrictions")]
    public class Restriction
    {
        [SqlMember(isPrimaryKey: true, isUnsigned: true, shouldAutoIncrement: true)]
        public ulong Id { get; set; }
        
        [SqlMember(isUnsigned: true)]
        public ulong ZoneId { get; set; }
        
        [SqlMember(isUnsigned: true)]
        public ushort UnturnedId { get; set; }
        
        [SqlMember]
        public ERestrictionType Type { get; set; }

        public Restriction() {}

        public Restriction(ulong id, ulong zoneId, ushort unturnedId, ERestrictionType type)
        {
            Id = id;
            ZoneId = zoneId;
            UnturnedId = unturnedId;
            Type = type;
        }
    }
}