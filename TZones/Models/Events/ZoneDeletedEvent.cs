using Tavstal.RocketFlow.Core;
using Tavstal.TZones.Models.Core;

namespace Tavstal.TZones.Models.Events
{
    public class ZoneDeletedEvent : Event
    {
        public Zone Zone { get; }

        public ZoneDeletedEvent(Zone zone)
        {
            Zone = zone;
        }
    }
}