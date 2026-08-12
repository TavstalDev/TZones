using Tavstal.RocketFlow.Core;
using Tavstal.TZones.Models.Core;
// ReSharper disable UnusedMember.Global

namespace Tavstal.TZones.Models.Events
{
    public class ZoneUpdatedEvent : Event
    {
        public Zone Zone { get; }

        public ZoneUpdatedEvent(Zone zone)
        {
            Zone = zone;
        }
    }
}