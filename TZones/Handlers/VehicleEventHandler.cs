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
    /// Handles vehicle-related events such as enter, exit, damage, lockpick, carjack, and siphon, enforcing zone restrictions.
    /// </summary>
    public static class VehicleEventHandler
    {
        private static bool _isAttached;

        /// <summary>
        /// Subscribes to all vehicle events if not already attached.
        /// </summary>
        public static void AttachEvents()
        {
            if (_isAttached)
                return;
            
            VehicleManager.onDamageTireRequested += OnDamageTireRequested;
            VehicleManager.onDamageVehicleRequested += OnDamageVehicleRequested;
            VehicleManager.onEnterVehicleRequested += OnEnterVehicleRequested;
            VehicleManager.onExitVehicleRequested += OnExitVehicleRequested;
            VehicleManager.onVehicleCarjacked += OnVehicleCarjacked;
            VehicleManager.onVehicleLockpicked += OnVehicleLockpicked;
            VehicleManager.onSiphonVehicleRequested += OnSiphonVehicleRequested;

            _isAttached = true;
        }

        /// <summary>
        /// Unsubscribes from all vehicle events if currently attached.
        /// </summary>
        public static void DetachEvents()
        {
            if (!_isAttached)
                return;
            
            VehicleManager.onDamageTireRequested -= OnDamageTireRequested;
            VehicleManager.onDamageVehicleRequested -= OnDamageVehicleRequested;
            VehicleManager.onEnterVehicleRequested -= OnEnterVehicleRequested;
            VehicleManager.onExitVehicleRequested -= OnExitVehicleRequested;
            VehicleManager.onVehicleCarjacked -= OnVehicleCarjacked;
            VehicleManager.onVehicleLockpicked -= OnVehicleLockpicked;
            VehicleManager.onSiphonVehicleRequested -= OnSiphonVehicleRequested;

            _isAttached = true;
        }
        
        /// <summary>
        /// Handles vehicle enter requests, blocking entry when the vehicle is restricted in the zone.
        /// </summary>
        private static void OnEnterVehicleRequested(Player player, InteractableVehicle vehicle, ref bool shouldAllow)
        {
            if (!shouldAllow)
                return;
            
            bool originalValue = shouldAllow;
            try
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(player);
                if (uPlayer == null)
                    return;

                if (ZoneManager.HasFlagOrBlocked(Flags.NoVehicleEnter,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoVehicleEnter,
                        uPlayer, vehicle.asset.id, ERestrictionType.VEHICLE_ENTER))
                {
                    shouldAllow = false;
                    return;
                }

                if (!ZoneManager.HasFlagOrBlocked(Flags.NoVehicleEnter,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoVehicleEnter,
                        vehicle.transform.position, vehicle.asset.id, ERestrictionType.VEHICLE_ENTER))
                    return;

                shouldAllow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnEnterVehicleRequested)}.", ex);
                shouldAllow = originalValue;
            }
        }
        
        /// <summary>
        /// Handles vehicle exit requests, blocking exit when the vehicle is restricted in the zone.
        /// </summary>
        private static void OnExitVehicleRequested(Player player, InteractableVehicle vehicle, ref bool shouldAllow, ref Vector3 pendingLocation, ref float pendingYaw)
        {
            if (!shouldAllow)
                return;
            
            bool originalValue = shouldAllow;
            try
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(player);
                if (uPlayer == null)
                    return;

                if (ZoneManager.HasFlagOrBlocked(Flags.NoVehicleExit,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoVehicleExit,
                        uPlayer, vehicle.asset.id, ERestrictionType.VEHICLE_EXIT))
                {
                    shouldAllow = false;
                    return;
                }

                if (!ZoneManager.HasFlagOrBlocked(Flags.NoVehicleExit,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoVehicleExit,
                        vehicle.transform.position, vehicle.asset.id, ERestrictionType.VEHICLE_EXIT))
                    return;

                shouldAllow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnExitVehicleRequested)}.", ex);
                shouldAllow = originalValue;
            }
        }
        
        /// <summary>
        /// Handles vehicle damage requests, blocking damage when the zone has the NoVehicleDamage flag.
        /// </summary>
        private static void OnDamageVehicleRequested(CSteamID instigatorSteamID, InteractableVehicle vehicle, ref ushort pendingTotalDamage, ref bool canRepair, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            if (!shouldAllow)
                return;
            
            bool originalValue = shouldAllow;
            try
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(instigatorSteamID);
                if (uPlayer == null)
                    return;
                
                if (ZoneManager.HasFlag(Flags.NoVehicleDamage,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoVehicleDamage,
                        uPlayer))
                {
                    shouldAllow = false;
                    return;
                }

                if (!ZoneManager.HasFlag(Flags.NoVehicleDamage,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoVehicleDamage,
                        vehicle.transform.position))
                    return;

                shouldAllow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnDamageVehicleRequested)}.", ex);
                shouldAllow = originalValue;
            }
        }
        
        /// <summary>
        /// Handles tire damage requests, blocking damage when the zone has the NoTireDamage flag.
        /// </summary>
        private static void OnDamageTireRequested(CSteamID instigatorSteamID, InteractableVehicle vehicle, int tireIndex, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            if (!shouldAllow)
                return;
            
            bool originalValue = shouldAllow;
            try
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(instigatorSteamID);
                if (uPlayer == null)
                    return;

                if (ZoneManager.HasFlag(Flags.NoTireDamage,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoTireDamage,
                        uPlayer))
                {
                    shouldAllow = false;
                    return;
                }

                if (!ZoneManager.HasFlag(Flags.NoTireDamage,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoTireDamage,
                        vehicle.transform.position))
                    return;

                shouldAllow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnDamageTireRequested)}.", ex);
                shouldAllow = originalValue;
            }
        }
        
        /// <summary>
        /// Handles fuel siphon requests, blocking siphoning when the zone has the NoVehicleSiphoning flag.
        /// </summary>
        private static void OnSiphonVehicleRequested(InteractableVehicle vehicle, Player instigatingPlayer, ref bool shouldAllow, ref ushort desiredAmount)
        {
            if (!shouldAllow)
                return;
            
            bool originalValue = shouldAllow;
            try
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(instigatingPlayer);
                if (uPlayer == null)
                    return;

                if (ZoneManager.HasFlag(Flags.NoVehicleSiphoning,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoVehicleSiphoning,
                        uPlayer))
                {
                    shouldAllow = false;
                    return;
                }

                if (!ZoneManager.HasFlag(Flags.NoVehicleSiphoning,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoVehicleSiphoning,
                        vehicle.transform.position))
                    return;

                shouldAllow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnSiphonVehicleRequested)}.", ex);
                shouldAllow = originalValue;
            }
        }
        
        /// <summary>
        /// Handles lockpick requests, blocking lockpicking when the zone has the NoLockpick flag.
        /// </summary>
        private static void OnVehicleLockpicked(InteractableVehicle vehicle, Player instigatingPlayer, ref bool shouldAllow)
        {
            if (!shouldAllow)
                return;
            
            bool originalValue = shouldAllow;
            try
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(instigatingPlayer);
                if (uPlayer == null)
                    return;

                if (ZoneManager.HasFlag(Flags.NoLockpick,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoLockpick,
                        uPlayer))
                {
                    shouldAllow = false;
                    return;
                }

                if (!ZoneManager.HasFlag(Flags.NoLockpick,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoLockpick,
                        vehicle.transform.position))
                    return;

                shouldAllow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnVehicleLockpicked)}.", ex);
                shouldAllow = originalValue;
            }
        }
        
        /// <summary>
        /// Handles carjack requests, blocking carjacking when the zone has the NoVehicleCarjack flag.
        /// </summary>
        private static void OnVehicleCarjacked(InteractableVehicle vehicle, Player instigatingPlayer, ref bool shouldAllow, ref Vector3 force, ref Vector3 torque)
        {
            if (!shouldAllow)
                return;
            
            bool originalValue = shouldAllow;
            try
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(instigatingPlayer);
                if (uPlayer == null)
                    return;

                if (ZoneManager.HasFlag(Flags.NoVehicleCarjack,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoVehicleCarjack,
                        uPlayer))
                {
                    shouldAllow = false;
                    return;
                }

                if (!ZoneManager.HasFlag(Flags.NoVehicleCarjack,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoVehicleCarjack,
                        vehicle.transform.position))
                    return;

                shouldAllow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnVehicleCarjacked)}.", ex);
                shouldAllow = originalValue;
            }
        }
    }
}