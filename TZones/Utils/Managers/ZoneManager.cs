using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rocket.Unturned.Player;
using Tavstal.TZones.Models.Core;
using UnityEngine;
using Tavstal.TLibrary.Extensions;
using ENodeType = Tavstal.TZones.Models.Enums.ENodeType;
using Flag = Tavstal.TZones.Models.Core.Flag;
using Node = Tavstal.TZones.Models.Core.Node;

namespace Tavstal.TZones.Utils.Managers
{
    /// <summary>
    /// Provides functionality to manage and handle zones within the application.
    /// </summary>
    public static class ZoneManager
    {
        /// <summary>
        /// Provides access to the zone data cache.
        /// </summary>
        public static ZonesManager_Cache Cache { get; } = new ZonesManager_Cache();

        /// <summary>
        /// Provides read-only query methods for zone data.
        /// </summary>
        public static ZonesManager_Queries Queries { get; } = new ZonesManager_Queries(Cache);

        /// <summary>
        /// Provides periodic update logic for players, generators, and zombies.
        /// </summary>
        public static ZonesManager_Update Updater { get; } = new ZonesManager_Update(Cache, Queries);
        
        #region Events
        #region PlayerEnterZone
        /// <summary>
        /// Delegate for the player-enter-zone event.
        /// </summary>
        /// <param name="player">The player entering the zone.</param>
        /// <param name="zone">The zone being entered.</param>
        /// <param name="lastPosition">The player's last known position.</param>
        /// <param name="shouldAllow">Set to false to prevent the player from entering.</param>
        public delegate void PlayerEnterZonedHandler(UnturnedPlayer player, Zone zone, Vector3 lastPosition, ref bool shouldAllow);

        /// <summary>
        /// Occurs when a player enters a zone.
        /// </summary>
        public static event PlayerEnterZonedHandler? OnPlayerEnterZone;

        internal static void FPlayerEnterZone(UnturnedPlayer player, Zone zone, Vector3 lastPosition,  ref bool shouldAllow) =>
            OnPlayerEnterZone?.Invoke(player, zone, lastPosition, ref shouldAllow);
        #endregion

        #region PlayerLeaveZone
        /// <summary>
        /// Delegate for the player-leave-zone event.
        /// </summary>
        /// <param name="player">The player leaving the zone.</param>
        /// <param name="zone">The zone being left.</param>
        /// <param name="lastPosition">The player's last known position.</param>
        /// <param name="shouldAllow">Set to false to prevent the player from leaving.</param>
        public delegate void PlayerLeaveZonedHandler(UnturnedPlayer player, Zone zone, Vector3 lastPosition, ref bool shouldAllow);

        /// <summary>
        /// Occurs when a player leaves a zone.
        /// </summary>
        public static event PlayerLeaveZonedHandler? OnPlayerLeaveZone;

        internal static void FPlayerLeaveZone(UnturnedPlayer player, Zone zone, Vector3 lastPosition, ref bool shouldAllow) =>
            OnPlayerLeaveZone?.Invoke(player, zone, lastPosition, ref shouldAllow);
        #endregion

        #region ZoneCreated
        /// <summary>
        /// Delegate for the zone-created event.
        /// </summary>
        /// <param name="zone">The zone that was created.</param>
        public delegate void ZoneCreatedHandler(Zone zone);

        /// <summary>
        /// Occurs when a new zone is created.
        /// </summary>
        public static event ZoneCreatedHandler? OnZoneCreated;

        internal static void FZoneCreated(Zone zone) =>
            OnZoneCreated?.Invoke(zone);
        #endregion

        #region ZoneUpdated
        /// <summary>
        /// Delegate for the zone-updated event.
        /// </summary>
        /// <param name="zone">The zone that was updated.</param>
        public delegate void ZoneUpdatedHandler(Zone zone);

        /// <summary>
        /// Occurs when a zone is updated.
        /// </summary>
        public static event ZoneUpdatedHandler? OnZoneUpdated;

        internal static void FZoneUpdated(Zone zone) =>
            OnZoneUpdated?.Invoke(zone);
        #endregion

        #region ZoneDeleted
        /// <summary>
        /// Delegate for the zone-deleted event.
        /// </summary>
        /// <param name="zone">The zone that was deleted.</param>
        public delegate void ZoneDeletedHandler(Zone zone);

        /// <summary>
        /// Occurs when a zone is deleted.
        /// </summary>
        public static event ZoneDeletedHandler? OnZoneDeleted;

        internal static void FZoneDeleted(Zone zone) =>
            OnZoneDeleted?.Invoke(zone);
        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// Adds a new custom flag to the database if one with the same name does not already exist.
        /// </summary>
        /// <param name="name">The flag name.</param>
        /// <param name="description">The flag description.</param>
        /// <param name="register">The identifier of the registering entity.</param>
        /// <returns>True if the flag was added; false if a flag with the same name already exists.</returns>
        public static async Task<bool> AddCustomFlagAsync(string name, string description, string register) 
        {
            Flag? flag = Cache.Flags.FirstOrDefault(x => x.Name == name); 
            if (flag != null) 
                return false;
            
            await TZones.DatabaseManager.Flags.AddAsync(new Flag
            {
                Name = name,
                Description = description,
                FlagRegister = register
            });
            Cache.MakeDirty();
            return true;
        }
        
        /// <summary>
        /// Removes a custom flag and its zone associations from the database.
        /// </summary>
        /// <param name="name">The flag name to remove.</param>
        /// <returns>0 on success, 1 if the flag was not found, 2 if the flag is a built-in default.</returns>
        public static async Task<int> RemoveCustomFlagAsync(string name)
        {
            Flag? targetFlag = Cache.Flags.FirstOrDefault(x => x.Name == name);
            if (targetFlag == null)
                return 1;

            if (Constants.Flags.Defaults.Contains(targetFlag.Name))
                return 2;

            await using var connection = TZones.DatabaseManager.CreateConnection();
            if (connection == null)
            {
                TZones.Logger.Error("Failed to remove custom flag due to database connection error.");
                return 1;
            }
            await using var transaction = connection.BeginTransaction();
            try
            {
                await TZones.DatabaseManager.ZoneFlags.DeleteRangeAsync("FlagId", new List<object> { targetFlag.Id }, connection, transaction);
                await TZones.DatabaseManager.Flags.DeleteAsync(targetFlag.Id, connection, transaction);
                await transaction.CommitAsync();
                Cache.MakeDirty();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TZones.Logger.Error("Failed to remove custom flag due to database transaction error.", ex);
                return 1;
            }
            return 0;
        }
        
        /// <summary>
        /// Checks whether a custom flag with the given name exists in the cache.
        /// </summary>
        /// <param name="name">The flag name to look up.</param>
        /// <returns>True if a flag with the name exists.</returns>
        public static bool CustomFlagExistAsync(string name) =>
            Cache.Flags.FirstOrDefault(x => x.Name == name) != null;
        
        /// <summary>
        /// Gets all zones that contain the given world position.
        /// </summary>
        /// <param name="position">The world position to check.</param>
        /// <returns>A read-only list of zones containing the position.</returns>
        public static IReadOnlyList<Zone> GetZonesFromPosition(Vector3 position) 
        {
            List<Zone> zones = new List<Zone>();
            var zonesToRead = Cache.Zones;
            foreach (var data in Cache.Nodes)
            {
                if (!IsPointInNodes(data.Value, position))
                    continue;
                Zone? zone = zonesToRead.FirstOrDefault(x => x.Id == data.Key);
                if (zone == null)
                    continue;
                zones.Add(zone);
            }
            return zones;
        }
        
        /// <summary>
        /// Gets the ids of all zones that contain the given world position.
        /// </summary>
        /// <param name="position">The world position to check.</param>
        /// <returns>A read-only list of zone ids containing the position.</returns>
        public static IReadOnlyList<ulong> GetZoneIdsFromPosition(Vector3 position) 
        {
            List<ulong> zones = new List<ulong>();
            var zonesToRead = Cache.Zones;
            foreach (var data in Cache.Nodes)
            {
                if (!IsPointInNodes(data.Value, position))
                    continue;
                Zone? zone = zonesToRead.FirstOrDefault(x => x.Id == data.Key);
                if (zone == null)
                    continue;
                zones.Add(zone.Id);
            }
            return zones;
        }

        /// <summary>
        /// Checks whether a world position falls within a specific zone's boundary.
        /// </summary>
        /// <param name="zone">The zone to test against.</param>
        /// <param name="position">The world position to check.</param>
        /// <returns>True if the position is inside the zone.</returns>
        public static bool IsPointInZone(Zone zone, Vector3 position)
        {
            var nodes = Queries.GetNodes(zone.Id);
            return nodes != null && IsPointInNodes(nodes, position);
        }
        
        /// <summary>
        /// Checks whether a point falls inside a polygon defined by boundary nodes, with optional height limits.
        /// Uses the ray-casting algorithm for 2D containment and upper/lower node height constraints.
        /// </summary>
        /// <param name="nodes">The list of nodes defining the zone boundary.</param>
        /// <param name="point">The world position to test.</param>
        /// <returns>True if the point is inside the node boundary.</returns>
        public static bool IsPointInNodes(IReadOnlyList<Node> nodes, Vector3 point)
        {
            bool isInside = false;
            int j = nodes.Count - 1;

            for (int i = 0; i < nodes.Count; j = i++)
            {
                if (nodes[i].Type != ENodeType.NONE)
                    continue;

                if (((nodes[i].Z > point.z) != (nodes[j].Z > point.z)) &&
                    (point.x < (nodes[j].X - nodes[i].X) * (point.z - nodes[i].Z) / (nodes[j].Z - nodes[i].Z) + nodes[i].X))
                    isInside = !isInside;
            }

            if (isInside)
            {
                Node? upperNode = nodes.FirstOrDefault(x => x.Type == ENodeType.UPPER);
                if (upperNode != null) {
                    if (point.y > upperNode.Y)
                        return false;
                }
                Node? lowerNode = nodes.FirstOrDefault(x => x.Type == ENodeType.LOWER);
                if (lowerNode != null) {
                    if (point.y < lowerNode.Y)
                        return false;
                }
            }

            return isInside;
        }
        #endregion
    }
}