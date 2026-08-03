using System;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using Tavstal.TLibrary.Extensions;
using Tavstal.TZones.Components;
using Tavstal.TZones.Models.Core;
using Tavstal.TZones.Models.Enums;
using Tavstal.TZones.Utils.Constants;
using Tavstal.TZones.Utils.Managers;
using UnityEngine;

namespace Tavstal.TZones.Handlers
{
    /// <summary>
    /// Handles structure-related events such as deploy, damage, and salvage, enforcing zone restrictions.
    /// </summary>
    public static class StructureEventHandler
    {
        private static bool _isAttached;

        /// <summary>
        /// Subscribes to all structure events if not already attached.
        /// </summary>
        public static void AttachEvents()
        {
            if (_isAttached)
                return;
            
            StructureManager.onDamageStructureRequested += OnDamageStructureRequested;
            StructureManager.onDeployStructureRequested += OnDeployStructureRequested;
            StructureDrop.OnSalvageRequested_Global += OnSalvageStructureRequested;

            _isAttached = true;
        }

        /// <summary>
        /// Unsubscribes from all structure events if currently attached.
        /// </summary>
        public static void DetachEvents()
        {
            if (!_isAttached)
                return;
            
            StructureManager.onDamageStructureRequested -= OnDamageStructureRequested;
            StructureManager.onDeployStructureRequested -= OnDeployStructureRequested;
            StructureDrop.OnSalvageRequested_Global -= OnSalvageStructureRequested;

            _isAttached = true;
        }
        
        /// <summary>
        /// Handles structure deploy requests, blocking placement when the zone forbids structures or the item is restricted.
        /// </summary>
        private static void OnDeployStructureRequested(Structure structure, ItemStructureAsset asset, ref Vector3 point, ref float angleX, ref float angleY, ref float angleZ, ref ulong owner, ref ulong group, ref bool shouldAllow)
        {
            bool originalValue = shouldAllow;
            try
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID((CSteamID)owner);
                if (uPlayer == null)
                    return;

                ZoneComponent comp = ComponentManager.Get(uPlayer);
                foreach (var zone in comp.Zones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.NoStructures) ||
                        ZoneManager.Queries.IsBlocked(zone, asset.id, ERestrictionType.BUILD))
                    {
                        shouldAllow = false;
                        break;
                    }
                }

                var objectZones = ZoneManager.GetZonesFromPosition(point);
                foreach (Zone zone in objectZones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.NoStructures) || 
                        ZoneManager.Queries.IsBlocked(zone, asset.id, ERestrictionType.BUILD))
                    {
                        shouldAllow = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnDeployStructureRequested)}.", ex);
                shouldAllow = originalValue;
            }
        }
        
        /// <summary>
        /// Handles structure salvage requests, blocking salvage when the zone has the NoStructureSalvage flag.
        /// </summary>
        private static void OnSalvageStructureRequested(StructureDrop structure, SteamPlayer instigatorClient, ref bool shouldAllow)
        {
            bool originalValue = shouldAllow;
            try
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromSteamPlayer(instigatorClient);
                if (uPlayer == null)
                    return;

                ZoneComponent comp = ComponentManager.Get(uPlayer);

                foreach (var zone in comp.Zones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.NoStructureSalvage))
                    {
                        shouldAllow = false;
                        break;
                    }
                }

                var objectZones = ZoneManager.GetZonesFromPosition(structure.GetServersideData().point);
                foreach (Zone zone in objectZones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.NoStructureSalvage))
                    {
                        shouldAllow = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnSalvageStructureRequested)}.", ex);
                shouldAllow = originalValue;
            }
        }
        
        /// <summary>
        /// Handles structure damage requests, blocking damage when the zone has the NoDamage flag.
        /// </summary>
        private static void OnDamageStructureRequested(CSteamID instigatorSteamID, Transform structureTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            bool originalValue = shouldAllow;
            try
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(instigatorSteamID);
                if (uPlayer == null)
                    return;

                ZoneComponent comp = ComponentManager.Get(uPlayer);
                foreach (var zone in comp.Zones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.NoDamage))
                    {
                        shouldAllow = false;
                        break;
                    }
                }

                var objectZones = ZoneManager.GetZonesFromPosition(structureTransform.position);
                foreach (Zone zone in objectZones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.NoDamage))
                    {
                        shouldAllow = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnDamageStructureRequested)}.", ex);
                shouldAllow = originalValue;
            }
        }
    }
}