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
        public static ZonesManager_Cache Cache { get; } = new ZonesManager_Cache();
        public static ZonesManager_Queries Queries { get; } = new ZonesManager_Queries(Cache);
        public static ZonesManager_Update Updater { get; } = new ZonesManager_Update(Cache, Queries);
        
        #region Events
        #region PlayerEnterZone
        public delegate void PlayerEnterZonedHandler(UnturnedPlayer player, Zone zone, Vector3 lastPosition, ref bool shouldAllow);
        public static event PlayerEnterZonedHandler? OnPlayerEnterZone;
        internal static void FPlayerEnterZone(UnturnedPlayer player, Zone zone, Vector3 lastPosition,  ref bool shouldAllow) =>
            OnPlayerEnterZone?.Invoke(player, zone, lastPosition, ref shouldAllow);
        #endregion

        #region PlayerLeaveZone
        public delegate void PlayerLeaveZonedHandler(UnturnedPlayer player, Zone zone, Vector3 lastPosition, ref bool shouldAllow);
        public static event PlayerLeaveZonedHandler? OnPlayerLeaveZone;
        internal static void FPlayerLeaveZone(UnturnedPlayer player, Zone zone, Vector3 lastPosition, ref bool shouldAllow) =>
            OnPlayerLeaveZone?.Invoke(player, zone, lastPosition, ref shouldAllow);
        #endregion

        #region ZoneCreated
        public delegate void ZoneCreatedHandler(Zone zone);
        public static event ZoneCreatedHandler? OnZoneCreated;
        internal static void FZoneCreated(Zone zone) =>
            OnZoneCreated?.Invoke(zone);
        #endregion

        #region ZoneUpdated
        public delegate void ZoneUpdatedHandler(Zone zone);
        public static event ZoneUpdatedHandler? OnZoneUpdated;
        internal static void FZoneUpdated(Zone zone) =>
            OnZoneUpdated?.Invoke(zone);
        #endregion

        #region ZoneDeleted
        public delegate void ZoneDeletedHandler(Zone zone);
        public static event ZoneDeletedHandler? OnZoneDeleted;
        internal static void FZoneDeleted(Zone zone) =>
            OnZoneDeleted?.Invoke(zone);
        #endregion
        #endregion

        #region Methods
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
        
        public static bool CustomFlagExistAsync(string name) =>
            Cache.Flags.FirstOrDefault(x => x.Name == name) != null;
        
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

        public static bool IsPointInZone(Zone zone, Vector3 position)
        {
            var nodes = Queries.GetNodes(zone.Id);
            return nodes != null && IsPointInNodes(nodes, position);
        }
        
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