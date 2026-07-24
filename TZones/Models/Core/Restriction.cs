using Tavstal.TLibrary.Models.Database.Attributes;
using Tavstal.TZones.Models.Enums;

namespace Tavstal.TZones.Models.Core
{
    /// <summary>
    /// Represents a restriction rule that blocks a specific action or item within a zone.
    /// </summary>
    [SqlName("zones_restrictions")]
    public class Restriction
    {
        /// <summary>
        /// The unique identifier of the restriction.
        /// </summary>
        [SqlMember(isPrimaryKey: true, isUnsigned: true, shouldAutoIncrement: true)]
        public ulong Id { get; set; }
        
        /// <summary>
        /// The identifier of the zone this restriction applies to.
        /// </summary>
        [SqlMember(isUnsigned: true)]
        public ulong ZoneId { get; set; }
        
        /// <summary>
        /// The Unturned asset id of the item or vehicle being restricted.
        /// </summary>
        [SqlMember(isUnsigned: true)]
        public ushort UnturnedId { get; set; }
        
        /// <summary>
        /// The type of restriction being applied.
        /// </summary>
        [SqlMember]
        public ERestrictionType Type { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Restriction"/> class with default values.
        /// </summary>
        public Restriction() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="Restriction"/> class with all values specified.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="zoneId">The identifier of the parent zone.</param>
        /// <param name="unturnedId">The Unturned asset id to restrict.</param>
        /// <param name="type">The type of restriction.</param>
        public Restriction(ulong id, ulong zoneId, ushort unturnedId, ERestrictionType type)
        {
            Id = id;
            ZoneId = zoneId;
            UnturnedId = unturnedId;
            Type = type;
        }
    }
}