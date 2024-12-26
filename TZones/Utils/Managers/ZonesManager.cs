using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rocket.Unturned.Player;
using Tavstal.TZones.Models.Core;
using Tavstal.TZones.Models.Enums;
using UnityEngine;
using Tavstal.TZones.Components;
using SDG.Unturned;
using Tavstal.TLibrary;
using Block = Tavstal.TZones.Models.Core.Block;
using ENodeType = Tavstal.TZones.Models.Enums.ENodeType;
using Flag = Tavstal.TZones.Models.Core.Flag;
using Node = Tavstal.TZones.Models.Core.Node;

namespace Tavstal.TZones.Utils.Managers
{
    /// <summary>
    /// Provides functionality to manage and handle zones within the application.
    /// </summary>
    public static class ZonesManager
    {
        #region Fields
        private static bool _isDirty;
        // Because the unturned events use 'ref', and the database is async, cache must be used
        private static List<Flag> _flags;
        public static List<Flag> Flags => _flags;
        private static List<Zone> _zones;
        public static List<Zone> Zones => _zones;
        private static Dictionary<ulong, List<Node>> _nodes;
        public static Dictionary<ulong, List<Node>> Nodes => _nodes;
        private static Dictionary<ulong, List<ZoneFlag>> _zoneFlags;
        public static Dictionary<ulong, List<ZoneFlag>> ZoneFlags => _zoneFlags;
        private static Dictionary<ulong, List<ZoneEvent>> _zoneEvents;
        public static Dictionary<ulong, List<ZoneEvent>> ZoneEvents => _zoneEvents;
        private static Dictionary<ulong, List<Block>> _zoneBlocks;
        public static Dictionary<ulong, List<Block>> ZoneBlocks => _zoneBlocks;
        #endregion
        
        #region Events
        #region PlayerEnterZone
        /// <summary>
        /// Delegate for handling the event when a player enters a zone.
        /// </summary>
        /// <param name="player">The player entering the zone.</param>
        /// <param name="zone">The zone being entered.</param>
        /// <param name="lastPosition">The last position of the player before entering the zone.</param>
        /// <param name="shouldAllow">A reference boolean indicating whether the entry should be allowed.</param>
        public delegate void PlayerEnterZonedHandler(UnturnedPlayer player, Zone zone, Vector3 lastPosition, ref bool shouldAllow);
        /// <summary>
        /// Event triggered when a player enters a zone.
        /// </summary>
        public static event PlayerEnterZonedHandler OnPlayerEnterZone;
        /// <summary>
        /// Invokes the <see cref="OnPlayerEnterZone"/> event with the provided parameters.
        /// </summary>
        /// <param name="player">The player entering the zone.</param>
        /// <param name="zone">The zone being entered.</param>
        /// <param name="lastPosition">The last position of the player before entering the zone.</param>
        /// <param name="shouldAllow">A reference boolean indicating whether the entry should be allowed.</param>
        internal static void FPlayerEnterZone(UnturnedPlayer player, Zone zone, Vector3 lastPosition,  ref bool shouldAllow)
        {
            OnPlayerEnterZone?.Invoke(player, zone, lastPosition, ref shouldAllow);
        }
        #endregion

        #region PlayerLeaveZone
        /// <summary>
        /// Delegate for handling the event when a player leaves a zone.
        /// </summary>
        /// <param name="player">The player leaving the zone.</param>
        /// <param name="zone">The zone being left.</param>
        /// <param name="lastPosition">The last position of the player before leaving the zone.</param>
        /// <param name="shouldAllow">A reference boolean indicating whether the exit should be allowed.</param>
        public delegate void PlayerLeaveZonedHandler(UnturnedPlayer player, Zone zone, Vector3 lastPosition, ref bool shouldAllow);
        /// <summary>
        /// Event triggered when a player leaves a zone.
        /// </summary>
        public static event PlayerLeaveZonedHandler OnPlayerLeaveZone;
        /// <summary>
        /// Invokes the <see cref="OnPlayerLeaveZone"/> event with the provided parameters.
        /// </summary>
        /// <param name="player">The player leaving the zone.</param>
        /// <param name="zone">The zone being left.</param>
        /// <param name="lastPosition">The last position of the player before leaving the zone.</param>
        /// <param name="shouldAllow">A reference boolean indicating whether the exit should be allowed.</param>
        internal static void FPlayerLeaveZone(UnturnedPlayer player, Zone zone, Vector3 lastPosition, ref bool shouldAllow)
        {
            OnPlayerLeaveZone?.Invoke(player, zone, lastPosition, ref shouldAllow);
        }
        #endregion

        #region ZoneCreated
        /// <summary>
        /// Delegate for handling the event when a zone is created.
        /// </summary>
        /// <param name="zone">The zone that was created.</param>
        public delegate void ZoneCreatedHandler(Zone zone);
        /// <summary>
        /// Event triggered when a zone is created.
        /// </summary>
        public static event ZoneCreatedHandler OnZoneCreated;
        /// <summary>
        /// Invokes the <see cref="OnZoneCreated"/> event with the provided parameters.
        /// </summary>
        /// <param name="zone">The zone that was created.</param>
        internal static void FZoneCreated(Zone zone)
        {
            OnZoneCreated?.Invoke(zone);
        }
        #endregion

        #region ZoneUpdated
        /// <summary>
        /// Delegate for handling the event when a zone is updated.
        /// </summary>
        /// <param name="zone">The zone that was updated.</param>
        public delegate void ZoneUpdatedHandler(Zone zone);
        /// <summary>
        /// Event triggered when a zone is updated.
        /// </summary>
        public static event ZoneUpdatedHandler OnZoneUpdated;
        /// <summary>
        /// Invokes the <see cref="OnZoneUpdated"/> event with the provided parameters.
        /// </summary>
        /// <param name="zone">The zone that was updated.</param>
        internal static void FZoneUpdated(Zone zone)
        {
            OnZoneUpdated?.Invoke(zone);
        }
        #endregion

        #region ZoneDeleted
        /// <summary>
        /// Delegate for handling the event when a zone is deleted.
        /// </summary>
        /// <param name="zone">The zone that was deleted.</param>
        public delegate void ZoneDeletedHandler(Zone zone);
        /// <summary>
        /// Event triggered when a zone is deleted.
        /// </summary>
        public static event ZoneDeletedHandler OnZoneDeleted;
        /// <summary>
        /// Invokes the <see cref="OnZoneDeleted"/> event with the provided parameters.
        /// </summary>
        /// <param name="zone">The zone that was deleted.</param>
        internal static void FZoneDeleted(Zone zone)
        {
            OnZoneDeleted?.Invoke(zone);
        }
        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// Marks the current state as dirty, indicating that changes have been made and need to be processed.
        /// </summary>
        public static void SetDirty() 
        {
            _isDirty = true;
        }

        /// <summary>
        /// Checks if the state is marked as dirty and, if so, refreshes all related data asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static async Task CheckDirtyAsync()
        {
            if (!_isDirty)
                return;

            _isDirty = false;
            await RefreshAllAsync();
        }

        /// <summary>
        /// Asynchronously refreshes all related data or states.
        /// </summary>
        /// <returns>A task representing the asynchronous refresh operation.</returns>
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
        
        /// <summary>
        /// Asynchronously refreshes the data or state for a specific zone identified by its ID.
        /// </summary>
        /// <param name="zoneId">The unique identifier of the zone to be refreshed.</param>
        /// <returns>A task representing the asynchronous refresh operation.</returns>
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
        
        /// <summary>
        /// Asynchronously adds a custom flag with the specified name, description, and registration details.
        /// </summary>
        /// <param name="name">The name of the custom flag.</param>
        /// <param name="description">A description of the custom flag.</param>
        /// <param name="register">The registration details for the custom flag.</param>
        /// <returns>A task representing the asynchronous operation, with a boolean result indicating success or failure.</returns>
        public static async Task<bool> AddCustomFlagAsync(string name, string description, string register) 
        {
            Flag flag = Flags.Find(x => x.Name == name); 
            if (flag != null) 
                return false;
            
            await TZones.DatabaseManager.AddFlagAsync(name, description, register);
            SetDirty();
            return true;
        }

        /// <summary>
        /// Asynchronously removes a custom flag identified by its name.
        /// </summary>
        /// <param name="name">The name of the custom flag to be removed.</param>
        /// <returns>A task representing the asynchronous operation, with an integer result indicating the outcome (e.g., number of flags removed or error code).</returns>
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

        /// <summary>
        /// Retrieves a list of zones based on the given position.
        /// </summary>
        /// <param name="position">The position to check for zones.</param>
        /// <returns>A list of <see cref="Zone"/> objects that are located at or near the specified position.</returns>
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

        /// <summary>
        /// Determines whether a given position is inside the specified zone.
        /// </summary>
        /// <param name="zone">The zone to check.</param>
        /// <param name="position">The position to test for inclusion within the zone.</param>
        /// <returns>True if the position is inside the zone, otherwise false.</returns>
        public static bool IsPointInZone(this Zone zone, Vector3 position) 
        {
            if (_nodes.TryGetValue(zone.Id, out List<Node> nodes)) {
                return IsPointInNodes(nodes, position);
            }
            return false;
        }

        /// <summary>
        /// Checks if a given point is inside any of the nodes in the list.
        /// </summary>
        /// <param name="nodes">The list of nodes to check against.</param>
        /// <param name="point">The point to test for inclusion within the nodes.</param>
        /// <returns>True if the point is inside one of the nodes, otherwise false.</returns>
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
        
        /// <summary>
        /// Checks if the specified zone has a particular flag.
        /// </summary>
        /// <param name="zone">The zone to check.</param>
        /// <param name="flagName">The name of the flag to check for.</param>
        /// <returns>True if the zone has the specified flag, otherwise false.</returns>
        public static bool HasFlag(this Zone zone, string flagName) 
        {
            Flag flag = _flags.Find(x => x.Name == flagName);
            if (flag == null) 
                return false;

            if (_zoneFlags.TryGetValue(zone.Id, out List<ZoneFlag> flags)) {
                return flags.Any(x => x.FlagId == flag.Id);
            }
            return false;
        }

        /// <summary>
        /// Retrieves a flag based on the specified flag name.
        /// </summary>
        /// <param name="flagName">The name of the flag to retrieve.</param>
        /// <returns>The <see cref="Flag"/> object associated with the given flag name, or null if not found.</returns>
        public static Flag GetFlag(string flagName)
        {
            return _flags.Find(x => x.Name == flagName);
        }
        
        /// <summary>
        /// Retrieves a zone based on the specified name.
        /// </summary>
        /// <param name="name">The name of the zone to retrieve.</param>
        /// <returns>The <see cref="Zone"/> object associated with the given name, or null if not found.</returns>
        public static Zone GetZone(string name)
        {
            return Zones.Find(x => x.Name == name);
        }

        /// <summary>
        /// Retrieves a list of nodes associated with the specified zone name.
        /// </summary>
        /// <param name="zoneName">The name of the zone to retrieve nodes for.</param>
        /// <returns>A list of <see cref="Node"/> objects associated with the specified zone name, or an empty list if no nodes are found.</returns>
        public static List<Node> GetNodes(string zoneName)
        {
            Zone zone = GetZone(zoneName);
            if (zone == null)
                return null;

            return _nodes[zone.Id];
        }

        /// <summary>
        /// Retrieves a list of flags associated with the specified zone name.
        /// </summary>
        /// <param name="zoneName">The name of the zone to retrieve flags for.</param>
        /// <returns>A list of <see cref="ZoneFlag"/> objects associated with the specified zone name, or an empty list if no flags are found.</returns>
        public static List<ZoneFlag> GetZoneFlags(string zoneName)
        {
            Zone zone = GetZone(zoneName);
            if (zone == null)
                return null;

            return _zoneFlags[zone.Id];
        }

        /// <summary>
        /// Retrieves a list of blocks associated with the specified zone name.
        /// </summary>
        /// <param name="zoneName">The name of the zone to retrieve blocks for.</param>
        /// <returns>A list of <see cref="Block"/> objects associated with the specified zone name, or an empty list if no blocks are found.</returns>
        public static List<Block> GetZoneBlocks(string zoneName)
        {
            Zone zone = GetZone(zoneName);
            if (zone == null)
                return null;

            return _zoneBlocks[zone.Id];
        }
        
        /// <summary>
        /// Retrieves the event of the specified type associated with the given zone.
        /// </summary>
        /// <param name="zone">The zone to check for the event.</param>
        /// <param name="eventType">The type of event to retrieve.</param>
        /// <returns>The <see cref="ZoneEvent"/> associated with the specified event type, or null if not found.</returns>
        public static ZoneEvent GetEvent(this Zone zone, EEventType eventType) 
        {
            if (_zoneEvents.TryGetValue(zone.Id, out List<ZoneEvent> events))
                return events.Find(x => x.Type == eventType);
            return null;
        }

        /// <summary>
        /// Checks if the specified entity is blocked in the given zone based on its ID and block type.
        /// </summary>
        /// <param name="zone">The zone to check.</param>
        /// <param name="unturnedId">The ID of the entity to check for blocking.</param>
        /// <param name="blockType">The type of block to check, which can be one of the following:
        /// <see cref="EBlockType.Build"/> - Block for building,
        /// <see cref="EBlockType.Equip"/> - Block for equipping,
        /// <see cref="EBlockType.Unequip"/> - Block for unequipping,
        /// <see cref="EBlockType.VehicleEnter"/> - Block for entering a vehicle,
        /// <see cref="EBlockType.VehicleLeave"/> - Block for leaving a vehicle.</param>
        /// <returns>True if the entity is blocked in the zone for the specified block type, otherwise false.</returns>
        public static bool IsBlocked(this Zone zone, ushort unturnedId, EBlockType blockType) 
        {
            if (_zoneBlocks.TryGetValue(zone.Id, out List<Block> blocks)) {
                return blocks.Any(x => x.UnturnedId == unturnedId && x.Type == blockType);
            }
            return false;
        }
        #endregion

        #region  Unity Update
        /// <summary>
        /// Asynchronous version of Unity's Update()
        /// </summary>
        /// <returns>A task representing the asynchronous update operation.</returns>
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
        
        /// <summary>
        /// Updates the player data or state in the system.
        /// </summary>
        /// <remarks>
        /// This method is typically used for operations related to player status, actions, or other ongoing updates.
        /// </remarks>
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

        /// <summary>
        /// Updates the zombie data or state within a specified zone.
        /// </summary>
        /// <param name="zone">The zone in which to update the zombies.</param>
        /// <remarks>
        /// This method is typically used to manage zombie behavior, actions, or other ongoing updates within a particular zone.
        /// </remarks>
        private static void UpdateZombies(Zone zone)
        {
            if (!zone.HasFlag(Constants.Flags.Zombie) || ZombieManager.regions == null)
                return;
            
            MainThreadDispatcher.RunOnMainThread(() =>
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
            });

        }

        /// <summary>
        /// Updates the generator data or state within a specified zone.
        /// </summary>
        /// <param name="zone">The zone in which to update the generators.</param>
        /// <remarks>
        /// This method is typically used to manage generator behavior, actions, or other ongoing updates within a particular zone.
        /// </remarks>
        private static void UpdateGenerators(Zone zone)
        {
            if (!zone.HasFlag(Constants.Flags.InfiniteGenerator))
                return;

            MainThreadDispatcher.RunOnMainThread(() =>
            {
                // TODO: Find a better way to get all InteractableGenerators
                InteractableGenerator[] generators = Object.FindObjectsOfType<InteractableGenerator>();
                foreach (var generator in generators)
                {
                    if (zone.IsPointInZone(generator.transform.position) &&
                        generator.fuel < generator.capacity - 10)
                        BarricadeManager.sendFuel(generator.transform, generator.capacity);
                }
            });
        }
        #endregion
    }
}