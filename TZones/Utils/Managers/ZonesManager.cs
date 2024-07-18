using System.Collections.Generic;
using Rocket.Unturned.Player;
using Tavstal.TZones.Models.Core;

namespace Tavstal.TZones.Utils.Managers
{
    public static class ZonesManager
    {
        // Because the unturned events use 'ref', and the database is async, cache must be used
        private static List<Flag> _flags { get; set; }
        private static List<Zone> _zones { get; set; }
        private static Dictionary<ulong, List<Node>> _nodes { get; set; }
        private static Dictionary<ulong, List<ZoneFlag>> _zoneFlags { get; set; }
        private static Dictionary<ulong, List<ZoneEvent>> _zoneEvents { get; set; }
        private static Dictionary<ulong, List<Block>> _zoneBlocks { get; set; }

        public delegate void PlayerEnterZonedHandler(UnturnedPlayer player, Zone zone);
        public static event PlayerEnterZonedHandler OnPlayerEnterZone;
        internal static void FPlayerEnterZone(UnturnedPlayer player, Zone zone)
        {
            OnPlayerEnterZone?.Invoke(player, zone);
        }

        public delegate void PlayerLeaveZonedHandler(UnturnedPlayer player, Zone zone);
        public static event PlayerLeaveZonedHandler OnPlayerLeaveZone;
        internal static void FPlayerLeaveZone(UnturnedPlayer player, Zone zone)
        {
            OnPlayerLeaveZone?.Invoke(player, zone);
        }

        public delegate void ZoneUpdatedHandler(Zone zone);
        public static event ZoneUpdatedHandler OnZoneUpdated;
        internal static void FZoneUpdated(Zone zone)
        {
            OnZoneUpdated?.Invoke(zone);
        }

        public static void Refresh() {

        }
        
    }
}