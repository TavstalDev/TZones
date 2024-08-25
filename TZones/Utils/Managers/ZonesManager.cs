using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rocket.Unturned.Player;
using Tavstal.TZones.Models.Core;
using Tavstal.TZones.Models.Enums;
using UnityEngine;
using Tavstal.TZones.Components;
using SDG.Unturned;
using Block = Tavstal.TZones.Models.Core.Block;
using ENodeType = Tavstal.TZones.Models.Enums.ENodeType;
using Flag = Tavstal.TZones.Models.Core.Flag;
using Node = Tavstal.TZones.Models.Core.Node;

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
        public delegate void PlayerEnterZonedHandler(UnturnedPlayer player, Zone zone, Vector3 lastPosition, ref bool shouldAllow);
        public static event PlayerEnterZonedHandler OnPlayerEnterZone;
        internal static void FPlayerEnterZone(UnturnedPlayer player, Zone zone, Vector3 lastPosition,  ref bool shouldAllow)
        {
            OnPlayerEnterZone?.Invoke(player, zone, lastPosition, ref shouldAllow);
        }

        public delegate void PlayerLeaveZonedHandler(UnturnedPlayer player, Zone zone, Vector3 lastPosition, ref bool shouldAllow);
        public static event PlayerLeaveZonedHandler OnPlayerLeaveZone;
        internal static void FPlayerLeaveZone(UnturnedPlayer player, Zone zone, Vector3 lastPosition, ref bool shouldAllow)
        {
            OnPlayerLeaveZone?.Invoke(player, zone, lastPosition, ref shouldAllow);
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

        internal static async Task CheckDirtyAsync()
        {
            if (!_isDirty)
                return;

            _isDirty = false;
            await RefreshAllAsync();
        }

        private static async Task RefreshAllAsync() 
        {
            _flags = await TZones.DatabaseManager.GetFlagsAsync(string.Empty) ?? new List<Flag>();
            _zones = await TZones.DatabaseManager.GetZonesAsync(string.Empty) ?? new List<Zone>();
            _nodes = new Dictionary<ulong, List<Node>>();
            _zoneFlags = new Dictionary<ulong, List<ZoneFlag>>();
            _zoneEvents = new Dictionary<ulong, List<ZoneEvent>>();
            _zoneBlocks = new Dictionary<ulong, List<Block>>();
            foreach (Zone zone in _zones) {
                #region Nodes
                List<Node> nodes = (await TZones.DatabaseManager.GetNodesAsync($"ZoneId='{zone.Id}'")) ?? new List<Node>();
                _nodes.Add(zone.Id, nodes);
                #endregion

                #region Flags
                List<ZoneFlag> flags = (await TZones.DatabaseManager.GetZoneFlagsAsync($"ZoneId='{zone.Id}'")) ?? new List<ZoneFlag>();
                _zoneFlags.Add(zone.Id, flags);
                #endregion

                #region Events
                List<ZoneEvent> events = (await TZones.DatabaseManager.GetZoneEventsAsync($"ZoneId='{zone.Id}'")) ?? new List<ZoneEvent>();
                _zoneEvents.Add(zone.Id, events);
                #endregion

                #region Blocks
                List<Block> blocks = (await TZones.DatabaseManager.GetBlocksAsync($"ZoneId='{zone.Id}'")) ?? new List<Block>();
                _zoneBlocks.Add(zone.Id, blocks);
                #endregion
            }
        }
        
        internal static async Task RefreshZoneAsync(ulong zoneId) 
        {
            Zone zone = _zones.Find(x => x.Id == zoneId);
            if (zone != null) {
                _zones.Remove(zone);
                _nodes.Remove(zone.Id);
                _zoneFlags.Remove(zone.Id);
                _zoneEvents.Remove(zone.Id);
                _zoneBlocks.Remove(zone.Id);
            }
            zone = await TZones.DatabaseManager.FindZoneAsync($"Id='{zoneId}'");
            if (zone != null) {
                _zones.Add(zone);

                #region Nodes
                List<Node> nodes = (await TZones.DatabaseManager.GetNodesAsync($"ZoneId='{zone.Id}'")) ?? new List<Node>();
                _nodes.Add(zone.Id, nodes);
                #endregion

                #region Flags
                List<ZoneFlag> flags = (await TZones.DatabaseManager.GetZoneFlagsAsync($"ZoneId='{zone.Id}'")) ?? new List<ZoneFlag>();
                _zoneFlags.Add(zone.Id, flags);
                #endregion

                #region Events
                List<ZoneEvent> events = (await TZones.DatabaseManager.GetZoneEventsAsync($"ZoneId='{zone.Id}'")) ?? new List<ZoneEvent>();
                _zoneEvents.Add(zone.Id, events);
                #endregion

                #region Blocks
                List<Block> blocks = (await TZones.DatabaseManager.GetBlocksAsync($"ZoneId='{zone.Id}'")) ?? new List<Block>();
                _zoneBlocks.Add(zone.Id, blocks);
                #endregion
            }
        }
        
        public static async Task<bool> AddCustomFlagAsync(string name, string description, string register) 
        {
            Flag flag = Flags.Find(x => x.Name == name); 
            if (flag != null) 
                return false;
            
            await TZones.DatabaseManager.AddFlagAsync(name, description, register);
            SetDirty();
            return true;
        }

        public static async Task<int> RemoveCustomFlagAsync(string name)
        {
            Flag targetFlag = Flags.Find(x => x.Name == name);
            if (targetFlag == null)
                return 1;

            if (Constants.Flags.Defaults.Contains(targetFlag.Name))
                return 2;
            
            await TZones.DatabaseManager.RemoveFlagAsync(targetFlag.Id);
            SetDirty();
            return 0;
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

        public static Flag GetFlag(string flagName)
        {
            return _flags.Find(x => x.Name == flagName);
        }
        
        public static Zone GetZone(string name)
        {
            return Zones.Find(x => x.Name == name);
        }

        public static List<Node> GetNodes(string zoneName)
        {
            Zone zone = GetZone(zoneName);
            if (zone == null)
                return null;

            return _nodes[zone.Id];
        }

        public static List<ZoneFlag> GetZoneFlags(string zoneName)
        {
            Zone zone = GetZone(zoneName);
            if (zone == null)
                return null;

            return _zoneFlags[zone.Id];
        }

        public static List<Block> GetZoneBlocks(string zoneName)
        {
            Zone zone = GetZone(zoneName);
            if (zone == null)
                return null;

            return _zoneBlocks[zone.Id];
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

        #region  Unity Update
        internal static async Task UpdateUnityAsync()
        {
            await CheckDirtyAsync();
            
            // Note: 
            // Future performance issues might be solved with Parallel.ForEach instead of regular foreach
            // Only use it at heavy load, or else it won't make it faster
            UpdatePlayers();

            // Update Generators & Zombies
            if (Zones == null)
                return;
            
            Parallel.ForEach(Zones, zone => {
                UpdateGenerators(zone);
                UpdateZombies(zone);
            });
        }
        
        private static void UpdatePlayers() 
        {
            foreach (SteamPlayer steamPlayer in Provider.clients) 
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                ZonePlayerComponent comp = uPlayer.GetComponent<ZonePlayerComponent>();

                List<Zone> currentZones = GetZonesFromPosition(uPlayer.Position);
                bool updateLastPos = true;

                foreach (Zone zone in comp.Zones) 
                {
                    if (currentZones.All(x => x.Id != zone.Id))
                    {
                        bool shouldAllow = true;
                        OnPlayerLeaveZone?.Invoke(uPlayer, zone, comp.LastPosition, ref shouldAllow);
                        if (!shouldAllow)
                        {
                            currentZones.Add(zone);
                            updateLastPos = false;
                        }
                    }
                }

                foreach (Zone zone in currentZones.ToList()) {
                    if (comp.Zones.All(x => x.Id != zone.Id)) 
                    {
                        bool shouldAllow = true;
                        OnPlayerEnterZone?.Invoke(uPlayer, zone, comp.LastPosition, ref shouldAllow);
                        if (!shouldAllow)
                        {
                            currentZones.Remove(zone);
                            updateLastPos = false;
                        }
                    }
                }

                comp.Zones = currentZones;
                if (updateLastPos)
                    comp.LastPosition = uPlayer.Position;
            }
        }

        private static void UpdateZombies(Zone zone) 
        {
            
            if (zone.HasFlag(Constants.Flags.NoZombie) && ZombieManager.regions != null) 
            {
                foreach (ZombieRegion t in ZombieManager.regions.Where(t => t.zombies != null))
                {
                    foreach (var zombie in t.zombies.Where(z => z))
                    {
                        if (zombie.isDead) 
                            continue;
                        if (!zone.IsPointInZone(zombie.transform.position)) 
                            continue;
                        zombie.gear = 0;
                        zombie.isDead = true;
                        ZombieManager.sendZombieDead(zombie, Vector3.zero);
                    }
                }
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private static void UpdateGenerators(Zone zone)
        {
            if (!zone.HasFlag(Constants.Flags.InfiniteGenerator))
                return;
            InteractableGenerator[] generators = Object.FindObjectsOfType<InteractableGenerator>();
            foreach (var generator in generators)
            {
                if (zone.IsPointInZone(generator.transform.position) &&
                    generator.fuel < generator.capacity - 10)
                    BarricadeManager.sendFuel(generator.transform, generator.capacity);
            }
        }
        #endregion
    }
}