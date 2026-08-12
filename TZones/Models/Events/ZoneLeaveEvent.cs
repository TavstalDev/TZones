using Rocket.Unturned.Player;
using Tavstal.RocketFlow.Core;
using Tavstal.TZones.Models.Core;
using UnityEngine;
// ReSharper disable UnusedMember.Global

namespace Tavstal.TZones.Models.Events
{
    public class ZoneLeaveEvent : Event, ICancellable
    {
        public UnturnedPlayer Player { get; }
        public Zone Zone { get; }
        public Vector3 LastPosition { get; }
        public bool ShouldAllow { get; set; }
        public bool IsCancelled { get; set; }

        public ZoneLeaveEvent(UnturnedPlayer player, Zone zone, Vector3 lastPosition, ref bool shouldAllow)
        {
            Player = player;
            Zone = zone;
            LastPosition = lastPosition;
            ShouldAllow = shouldAllow;
        }
    }
}