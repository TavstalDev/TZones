using Tavstal.RocketFlow.Core;
using Tavstal.TZones.Models.Core;
// ReSharper disable UnusedMember.Global

namespace Tavstal.TZones.Models.Events
{
    public class ZoneCreatedEvent : Event
    {
        public Zone Zone { get; }

        public ZoneCreatedEvent(Zone zone)
        {
            Zone = zone;
        }
    }
}