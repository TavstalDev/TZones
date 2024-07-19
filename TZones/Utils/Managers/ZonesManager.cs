using System.Collections.Generic;
using System.Threading.Tasks;
using Rocket.Unturned.Player;
using Tavstal.TZones.Models.Core;

namespace Tavstal.TZones.Utils.Managers
{
    public static class ZonesManager
    {
        #region Fields
        // Because the unturned events use 'ref', and the database is async, cache must be used
        private static List<Flag> _flags { get; set; }
        public static List<Flag> Flags { get { return _flags; } }
        private static List<Zone> _zones { get; set; }
        public static List<Zone> Zones { get { return _zones; } }
        private static Dictionary<ulong, List<Node>> _nodes { get; set; }
        public static Dictionary<ulong, List<Node>> Nodes { get { return _nodes;}}
        private static Dictionary<ulong, List<ZoneFlag>> _zoneFlags { get; set; }
        public static Dictionary<ulong, List<ZoneFlag>> ZoneFlags { get { return _zoneFlags;}}
        private static Dictionary<ulong, List<ZoneEvent>> _zoneEvents { get; set; }
        public static Dictionary<ulong, List<ZoneEvent>> ZoneEvents { get { return _zoneEvents;}}
        private static Dictionary<ulong, List<Block>> _zoneBlocks { get; set; }
        public static Dictionary<ulong, List<Block>> ZoneBlocks { get { return _zoneBlocks;}}
        #endregion
        
        #region Events
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

        public delegate void ZoneCreatedHandler(Zone zone);
        public static event ZoneCreatedHandler OnZoneCreated;
        internal static void FZoneCreated(Zone zone)
        {
            OnZoneCreated?.Invoke(zone);
        }


        public delegate void ZoneUpdatedHandler(Zone zone);
        public static event ZoneUpdatedHandler OnZoneUpdated;
        internal static void FZoneUpdated(Zone zone)
        {
            OnZoneUpdated?.Invoke(zone);
        }

        public delegate void ZoneDeletedHandler(Zone zone);
        public static event ZoneDeletedHandler OnZoneDeleted;
        internal static void FZoneDeleted(Zone zone)
        {
            OnZoneDeleted?.Invoke(zone);
        }
        #endregion

        #region Methods
        public static async Task RefreshAllAsync() {
            _flags = await TZones.DatabaseManager.GetFlagsAsync(string.Empty);
            _zones = await TZones.DatabaseManager.GetZonesAsync(string.Empty);
            _nodes = new Dictionary<ulong, List<Node>>();
            _zoneFlags = new Dictionary<ulong, List<ZoneFlag>>();
            _zoneEvents = new Dictionary<ulong, List<ZoneEvent>>();
            _zoneBlocks = new Dictionary<ulong, List<Block>>();
            foreach (Zone zone in _zones) {
                #region Nodes
                List<Node> nodes = (await TZones.DatabaseManager.GetNodesAsync($"ZoneId=`{zone.Id}`")) ?? new List<Node>();
                _nodes.Add(zone.Id, nodes);
                #endregion

                #region Flags
                List<ZoneFlag> flags = (await TZones.DatabaseManager.GetZoneFlagsAsync($"ZoneId=`{zone.Id}`")) ?? new List<ZoneFlag>();
                _zoneFlags.Add(zone.Id, flags);
                #endregion

                #region Events
                List<ZoneEvent> events = (await TZones.DatabaseManager.GetZoneEventsAsync($"ZoneId=`{zone.Id}`")) ?? new List<ZoneEvent>();
                _zoneEvents.Add(zone.Id, events);
                #endregion

                #region Blocks
                List<Block> blocks = (await TZones.DatabaseManager.GetBlocksAsync($"ZoneId=`{zone.Id}`")) ?? new List<Block>();
                _zoneBlocks.Add(zone.Id, blocks);
                #endregion
            }
        }
        
        public static async Task RefreshZoneAsync(ulong zoneId) 
        {
            Zone zone = _zones.Find(x => x.Id == zoneId);
            if (zone != null) {
                _zones.Remove(zone);
                _nodes.Remove(zone.Id);
                _zoneFlags.Remove(zone.Id);
                _zoneEvents.Remove(zone.Id);
                _zoneBlocks.Remove(zone.Id);
            }
            zone = await TZones.DatabaseManager.FindZoneAsync($"Id=`{zoneId}`");
            if (zone != null) {
                _zones.Add(zone);

                #region Nodes
                List<Node> nodes = (await TZones.DatabaseManager.GetNodesAsync($"ZoneId=`{zone.Id}`")) ?? new List<Node>();
                _nodes.Add(zone.Id, nodes);
                #endregion

                #region Flags
                List<ZoneFlag> flags = (await TZones.DatabaseManager.GetZoneFlagsAsync($"ZoneId=`{zone.Id}`")) ?? new List<ZoneFlag>();
                _zoneFlags.Add(zone.Id, flags);
                #endregion

                #region Events
                List<ZoneEvent> events = (await TZones.DatabaseManager.GetZoneEventsAsync($"ZoneId=`{zone.Id}`")) ?? new List<ZoneEvent>();
                _zoneEvents.Add(zone.Id, events);
                #endregion

                #region Blocks
                List<Block> blocks = (await TZones.DatabaseManager.GetBlocksAsync($"ZoneId=`{zone.Id}`")) ?? new List<Block>();
                _zoneBlocks.Add(zone.Id, blocks);
                #endregion
            }
        }
        #endregion
    }
}