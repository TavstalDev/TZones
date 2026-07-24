using System;
using Tavstal.TLibrary.Models.Database.Attributes;

namespace Tavstal.TZones.Models.Core
{
    /// <summary>
    /// Represents a zone defined by a set of nodes, with associated metadata.
    /// </summary>
    [SqlName("zones")]
    public class Zone
    {
        /// <summary>
        /// The unique identifier of the zone.
        /// </summary>
        [SqlMember(isPrimaryKey: true, isUnsigned: true, shouldAutoIncrement: true)]
        public ulong Id { get; set; }
        
        /// <summary>
        /// The display name of the zone.
        /// </summary>
        [SqlMember(columnType: "varchar(32)")]
        public string Name { get; set; }
        
        /// <summary>
        /// An optional description of the zone.
        /// </summary>
        [SqlMember(columnType: "varchar(128)", isNullable: true)]
        public string Description { get; set; }
        
        /// <summary>
        /// The steam id of the player who created the zone.
        /// </summary>
        [SqlMember(isUnsigned: true)]
        public ulong CreatorId { get; set; }
        
        /// <summary>
        /// The date and time the zone was created.
        /// </summary>
        [SqlMember]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Zone"/> class with default values.
        /// </summary>
        public Zone()
        {
            Name = string.Empty;
            Description = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Zone"/> class with all values specified.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="name">The zone name.</param>
        /// <param name="description">The zone description.</param>
        /// <param name="creatorId">The steam id of the creator.</param>
        /// <param name="creationDate">The creation date.</param>
        public Zone(ulong id, string name, string description, ulong creatorId, DateTime creationDate)
        {
            Id = id;
            Name = name;
            Description = description;
            CreatorId = creatorId;
            CreationDate = creationDate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Zone"/> class without an id (for new entries).
        /// </summary>
        /// <param name="name">The zone name.</param>
        /// <param name="description">The zone description.</param>
        /// <param name="creatorId">The steam id of the creator.</param>
        /// <param name="creationDate">The creation date.</param>
        public Zone(string name, string description, ulong creatorId, DateTime creationDate)
        {
            Id = 0;
            Name = name;
            Description = description;
            CreatorId = creatorId;
            CreationDate = creationDate;
        }
    }
}