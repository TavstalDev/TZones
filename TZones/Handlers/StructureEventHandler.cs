using System;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using Tavstal.TLibrary.Extensions;
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
            if (!shouldAllow)
                return;
            
            bool originalValue = shouldAllow;
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID((CSteamID)owner);
                if (player == null)
                    return;

                if (ZoneManager.HasFlagOrBlocked(Flags.NoStructures,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoStructures,
                        player, asset.id, ERestrictionType.BUILD))
                {
                    shouldAllow = false;
                    return;
                }

                if (!ZoneManager.HasFlagOrBlocked(Flags.NoStructures,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoStructures,
                        point, asset.id, ERestrictionType.BUILD))
                    return;

                shouldAllow = false;
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
            if (!shouldAllow)
                return;
            
            bool originalValue = shouldAllow;
            try
            {
                var point = structure.GetServersideData().point;
                UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(instigatorClient);
                if (player == null)
                    return;

                if (ZoneManager.HasFlag(Flags.NoStructureSalvage, TZones.Instance.Config.GlobalZoneFlagChecks.NoStructureSalvage,
                        player))
                {
                    shouldAllow = false;
                    return;
                }

                if (!ZoneManager.HasFlag(Flags.NoStructureSalvage,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoStructureSalvage,
                        point))
                    return;

                shouldAllow = false;
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
            if (!shouldAllow)
                return;
            
            bool originalValue = shouldAllow;
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(instigatorSteamID);
                if (player == null)
                    return;

                if (ZoneManager.HasFlag(Flags.NoDamage, TZones.Instance.Config.GlobalZoneFlagChecks.NoDamage,
                        player))
                {
                    shouldAllow = false;
                    return;
                }

                if (!ZoneManager.HasFlag(Flags.NoDamage,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoDamage,
                        structureTransform.position))
                    return;

                shouldAllow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnDamageStructureRequested)}.", ex);
                shouldAllow = originalValue;
            }
        }
    }
}