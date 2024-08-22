using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rocket.Unturned.Player;
using Tavstal.TZones.Models.Core;
using Tavstal.TZones.Models.Enums;
using UnityEngine;

namespace Tavstal.TZones.Utils.Managers
{
    public static class ZonesManager
    {
        #region Fields
        private static bool _isDirty { get; set; }
        // Because the unturned events use 'ref', and the database is async, cache must be used
        private static List<Flag> _flags { get; set; }
        public static List<Flag> Flags => _flags;
        private static List<Zone> _zones { get; set; }
        public static List<Zone> Zones => _zones;
        private static Dictionary<ulong, List<Node>> _nodes { get; set; }
        public static Dictionary<ulong, List<Node>> Nodes => _nodes;
        private static Dictionary<ulong, List<ZoneFlag>> _zoneFlags { get; set; }
        public static Dictionary<ulong, List<ZoneFlag>> ZoneFlags => _zoneFlags;
        private static Dictionary<ulong, List<ZoneEvent>> _zoneEvents { get; set; }
        public static Dictionary<ulong, List<ZoneEvent>> ZoneEvents => _zoneEvents;
        private static Dictionary<ulong, List<Block>> _zoneBlocks { get; set; }
        public static Dictionary<ulong, List<Block>> ZoneBlocks => _zoneBlocks;

        #endregion
        
        #region Events
        public delegate void PlayerEnterZonedHandler(UnturnedPlayer player, Zone zone, Vector3 lastPosition);
        public static event PlayerEnterZonedHandler OnPlayerEnterZone;
        internal static void FPlayerEnterZone(UnturnedPlayer player, Zone zone, Vector3 lastPosition)
        {
            OnPlayerEnterZone?.Invoke(player, zone, lastPosition);
        }

        public delegate void PlayerLeaveZonedHandler(UnturnedPlayer player, Zone zone, Vector3 lastPosition);
        public static event PlayerLeaveZonedHandler OnPlayerLeaveZone;
        internal static void FPlayerLeaveZone(UnturnedPlayer player, Zone zone, Vector3 lastPosition)
        {
            OnPlayerLeaveZone?.Invoke(player, zone, lastPosition);
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
        public static void SetDirty() 
        {
            _isDirty = true;
        }

        public static async Task CheckDirtyAsync()
        {
            if (!_isDirty)
                return;

            _isDirty = false;
            await RefreshAllAsync();
        }

        private static async Task RefreshAllAsync() 
        {
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
        
        public static List<Zone> GetZonesFromPosition(Vector3 position) 
        {
            List<Zone> zones = new List<Zone>();

            foreach (var data in _nodes) {
                if (IsPointInNodes(data.Value, position)) {
                    Zone zone = _zones.FirstOrDefault(x => x.Id == data.Key);
                    if (zone != null)
                        zones.Add(zone);
                }
            }

            return zones;
        }

        public static bool IsPointInZone(this Zone zone, Vector3 position) 
        {
            if (_nodes.TryGetValue(zone.Id, out List<Node> nodes)) {
                return IsPointInNodes(nodes, position);
            }
            return false;
        }

        private static bool IsPointInNodes(List<Node> nodes, Vector3 point)
        {
            bool isInside = false;
            int j = nodes.Count - 1;

            for (int i = 0; i < nodes.Count; j = i++)
            {
                if (nodes[i].Type != ENodeType.None)
                    continue;

                if (((nodes[i].Z > point.z) != (nodes[j].Z > point.z)) &&
                    (point.x < (nodes[j].X - nodes[i].X) * (point.z - nodes[i].Z) / (nodes[j].Z - nodes[i].Z) + nodes[i].X))
                {
                    isInside = !isInside;
                }
            }

            if (isInside)
            {
                Node upperNode = nodes.FirstOrDefault(x => x.Type == ENodeType.Upper);
                if (upperNode != null) {
                    if (point.y > upperNode.Y) {
                        return false;
                    }
                }
                Node lowerNode = nodes.FirstOrDefault(x => x.Type == ENodeType.Lower);
                if (lowerNode != null) {
                    if (point.y < lowerNode.Y)
                    {
                        return false;
                    }
                }
            }

            return isInside;
        }
        
        public static bool HasFlag(this Zone zone, string flagName) {
            Flag flag = _flags.Find(x => x.Name == flagName);
            if (flag == null) 
                return false;

            if (_zoneFlags.TryGetValue(zone.Id, out List<ZoneFlag> flags)) {
                return flags.Any(x => x.FlagId == flag.Id);
            }
            return false;
        }

        public static ZoneEvent GetEvent(this Zone zone, EEventType eventType) {
            if (_zoneEvents.TryGetValue(zone.Id, out List<ZoneEvent> events)) {
                return events.Find(x => x.Type == eventType);
            }
            return null;
        }

        public static bool IsBlocked(this Zone zone, ushort unturnedId, EBlockType blockType) 
        {
            if (_zoneBlocks.TryGetValue(zone.Id, out List<Block> blocks)) {
                return blocks.Any(x => x.UnturnedId == unturnedId && x.Type == blockType);
            }
            return false;
        }
        #endregion
    }
}