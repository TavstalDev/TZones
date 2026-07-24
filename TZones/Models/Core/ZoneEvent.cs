using Tavstal.TLibrary.Models.Database.Attributes;
using Tavstal.TZones.Models.Enums;

namespace Tavstal.TZones.Models.Core
{
    /// <summary>
    /// Represents an event triggered when a player enters or leaves a zone.
    /// </summary>
    [SqlName("zones_events")]
    public class ZoneEvent
    {
        /// <summary>
        /// The unique identifier of the event.
        /// </summary>
        [SqlMember(isPrimaryKey: true, isUnsigned: true, shouldAutoIncrement: true)]
        public ulong Id { get; set; }
        
        /// <summary>
        /// The identifier of the zone this event belongs to.
        /// </summary>
        [SqlMember(isUnsigned: true)]
        public ulong ZoneId { get; set; }
        
        /// <summary>
        /// The type of event to trigger.
        /// </summary>
        [SqlMember]
        public EEventType Type { get; set; }
        
        /// <summary>
        /// The value associated with the event (e.g. effect id, group name, or message text).
        /// </summary>
        [SqlMember(columnType: "varchar(255)")]
        public string Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneEvent"/> class with default values.
        /// </summary>
        public ZoneEvent()
        {
            Value = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneEvent"/> class with all values specified.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="zoneId">The identifier of the parent zone.</param>
        /// <param name="type">The event type.</param>
        /// <param name="value">The event value.</param>
        public ZoneEvent(ulong id, ulong zoneId, EEventType type, string value)
        {
            Id = id;
            ZoneId = zoneId;
            Type = type;
            Value = value;
        }
    }
}