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
    /// Handles barricade-related events such as deploy, damage, and salvage, enforcing zone restrictions.
    /// </summary>
    public static class BarricadeEventHandler
    {
        private static bool _isAttached;

        /// <summary>
        /// Subscribes to all barricade events if not already attached.
        /// </summary>
        public static void AttachEvents()
        {
            if (_isAttached)
                return;

            BarricadeManager.onBarricadeSpawned += OnBarricadeSpawned;
            BarricadeManager.onDamageBarricadeRequested += OnDamageBarricadeRequested;
            BarricadeManager.onDeployBarricadeRequested += OnDeployBarricadeRequested;
            BarricadeDrop.OnSalvageRequested_Global += OnSalvageBarricadeRequested;
            
            _isAttached = true;
        }

        /// <summary>
        /// Unsubscribes from all barricade events if currently attached.
        /// </summary>
        public static void DetachEvents()
        {
            if (!_isAttached)
                return;
            
            BarricadeManager.onBarricadeSpawned -= OnBarricadeSpawned;
            BarricadeManager.onDamageBarricadeRequested -= OnDamageBarricadeRequested;
            BarricadeManager.onDeployBarricadeRequested -= OnDeployBarricadeRequested;
            BarricadeDrop.OnSalvageRequested_Global -= OnSalvageBarricadeRequested;

            _isAttached = true;
        }
        
        private static void OnBarricadeSpawned(BarricadeRegion region, BarricadeDrop drop)
        {
            if (drop.interactable is InteractableGenerator generator)
                ZoneManager.Cache.AddGenerator(generator);
        }
        
        /// <summary>
        /// Handles barricade deploy requests, blocking placement when the zone forbids barricades or the item is restricted.
        /// </summary>
        private static void OnDeployBarricadeRequested(Barricade barricade, ItemBarricadeAsset asset, Transform hit, ref Vector3 point, ref float angleX, ref float angleY, ref float angleZ, ref ulong owner, ref ulong group, ref bool shouldAllow)
        {
            if (!shouldAllow)
                return;
            
            bool originalValue = shouldAllow;
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID((CSteamID)owner);
                if (player == null)
                    return;

                if (ZoneManager.HasFlagOrBlocked(Flags.NoBarricades,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoBarricades,
                        player, asset.id, ERestrictionType.BUILD))
                {
                    shouldAllow = false;
                    return;
                }

                if (!ZoneManager.HasFlagOrBlocked(Flags.NoBarricades,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoBarricades,
                        point, asset.id, ERestrictionType.BUILD))
                    return;

                shouldAllow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnDeployBarricadeRequested)}.", ex);
                shouldAllow = originalValue;
            }
        }
        
        /// <summary>
        /// Handles barricade salvage requests, blocking salvage when the zone has the NoBarricadeSalvage flag.
        /// </summary>
        private static void OnSalvageBarricadeRequested(BarricadeDrop barricade, SteamPlayer instigatorClient, ref bool shouldAllow)
        {
            if (!shouldAllow)
                return;
            
            bool originalValue = shouldAllow;
            try
            {
                var point = barricade.interactable.transform.position;
                UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(instigatorClient);
                if (player == null)
                    return;

                if (ZoneManager.HasFlag(Flags.NoBarricadeSalvage, TZones.Instance.Config.GlobalZoneFlagChecks.NoBarricadeSalvage,
                        player))
                {
                    shouldAllow = false;
                    return;
                }

                if (!ZoneManager.HasFlag(Flags.NoBarricadeSalvage,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoBarricadeSalvage,
                        point))
                    return;

                shouldAllow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnSalvageBarricadeRequested)}.", ex);
                shouldAllow = originalValue;
            }
            finally
            {
                if (shouldAllow && barricade.asset.build == EBuild.GENERATOR)
                    if (barricade.interactable is InteractableGenerator generator)
                        ZoneManager.Cache.RemoveGenerator(generator);
            }
        }
        
        /// <summary>
        /// Handles barricade damage requests, blocking damage when the zone has the NoDamage flag.
        /// </summary>
        private static void OnDamageBarricadeRequested(CSteamID instigatorSteamID, Transform barricadeTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
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
                        barricadeTransform.position))
                    return;

                shouldAllow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnDamageBarricadeRequested)}.", ex);
                shouldAllow = originalValue;
            }
            finally
            {
                var barricade = BarricadeManager.FindBarricadeByRootTransform(barricadeTransform);
                if (barricade != null)
                {
                    if (shouldAllow && barricade.GetServersideData().barricade.health - pendingTotalDamage <= 0 &&
                        barricade.asset.build == EBuild.GENERATOR)
                        if (barricade.interactable is InteractableGenerator generator)
                            ZoneManager.Cache.RemoveGenerator(generator);
                }
            }
        }
    }
}