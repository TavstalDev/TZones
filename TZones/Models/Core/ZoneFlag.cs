using Tavstal.TLibrary.Models.Database.Attributes;

namespace Tavstal.TZones.Models.Core
{
    /// <summary>
    /// Represents an association between a zone and a flag.
    /// </summary>
    [SqlName("zones_flags")]
    public class ZoneFlag
    {
        /// <summary>
        /// The unique identifier of this association.
        /// </summary>
        [SqlMember(isPrimaryKey: true, isUnsigned: true, shouldAutoIncrement: true)]
        public ulong Id { get; set; }
        
        /// <summary>
        /// The identifier of the zone.
        /// </summary>
        [SqlMember(isUnsigned: true)]
        public ulong ZoneId { get; set; }
        
        /// <summary>
        /// The identifier of the flag assigned to the zone.
        /// </summary>
        [SqlMember(isUnsigned: true)]
        public ulong FlagId { get; set; }   

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneFlag"/> class with default values.
        /// </summary>
        public ZoneFlag() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneFlag"/> class with all values specified.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="zoneId">The identifier of the zone.</param>
        /// <param name="flagId">The identifier of the flag.</param>
        public ZoneFlag(ulong id, ulong zoneId, ulong flagId)
        {
            Id = id;
            ZoneId = zoneId;
            FlagId = flagId;
        }
    }
}