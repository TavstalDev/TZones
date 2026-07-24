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

namespace Tavstal.TZones.Utils.Handlers
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
            try
            {
                ZonePlayerComponent comp = player.GetComponent<ZonePlayerComponent>();

                foreach (var zone in comp.Zones)
                {
                    if (ZoneManager.Queries.IsBlocked(zone, vehicle.id, ERestrictionType.VEHICLE_ENTER))
                    {
                        shouldAllow = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnEnterVehicleRequested)}.", ex);
                shouldAllow = true;
            }
        }
        
        /// <summary>
        /// Handles vehicle exit requests, blocking exit when the vehicle is restricted in the zone.
        /// </summary>
        private static void OnExitVehicleRequested(Player player, InteractableVehicle vehicle, ref bool shouldAllow, ref Vector3 pendingLocation, ref float pendingYaw)
        {
            try
            {
                ZonePlayerComponent comp = player.GetComponent<ZonePlayerComponent>();

                foreach (var zone in comp.Zones)
                {
                    if (ZoneManager.Queries.IsBlocked(zone, vehicle.id, ERestrictionType.VEHICLE_LEAVE))
                    {
                        shouldAllow = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnExitVehicleRequested)}.", ex);
                shouldAllow = true;
            }
        }
        
        /// <summary>
        /// Handles vehicle damage requests, blocking damage when the zone has the NoVehicleDamage flag.
        /// </summary>
        private static void OnDamageVehicleRequested(CSteamID instigatorSteamID, InteractableVehicle vehicle, ref ushort pendingTotalDamage, ref bool canRepair, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            try
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(instigatorSteamID);
                if (uPlayer == null)
                    return;

                ZonePlayerComponent comp = uPlayer.GetComponent<ZonePlayerComponent>();

                foreach (var zone in comp.Zones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.VehicleDamage))
                    {
                        shouldAllow = false;
                        break;
                    }
                }

                var objectZones = ZoneManager.GetZonesFromPosition(vehicle.transform.position);
                foreach (Zone zone in objectZones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.VehicleDamage))
                    {
                        shouldAllow = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnDamageVehicleRequested)}.", ex);
                shouldAllow = true;
            }
        }
        
        /// <summary>
        /// Handles tire damage requests, blocking damage when the zone has the NoTireDamage flag.
        /// </summary>
        private static void OnDamageTireRequested(CSteamID instigatorSteamID, InteractableVehicle vehicle, int tireIndex, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            try
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(instigatorSteamID);
                if (uPlayer == null)
                    return;

                ZonePlayerComponent comp = uPlayer.GetComponent<ZonePlayerComponent>();

                foreach (var zone in comp.Zones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.TireDamage))
                    {
                        shouldAllow = false;
                        break;
                    }
                }

                var objectZones = ZoneManager.GetZonesFromPosition(vehicle.transform.position);
                foreach (Zone zone in objectZones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.TireDamage))
                    {
                        shouldAllow = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnDamageTireRequested)}.", ex);
                shouldAllow = true;
            }
        }
        
        /// <summary>
        /// Handles fuel siphon requests, blocking siphoning when the zone has the NoVehicleSiphoning flag.
        /// </summary>
        private static void OnSiphonVehicleRequested(InteractableVehicle vehicle, Player instigatingPlayer, ref bool shouldAllow, ref ushort desiredAmount)
        {
            try
            {
                ZonePlayerComponent comp = instigatingPlayer.GetComponent<ZonePlayerComponent>();

                foreach (var zone in comp.Zones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.VehicleSiphoning))
                    {
                        shouldAllow = false;
                        break;
                    }
                }

                var objectZones = ZoneManager.GetZonesFromPosition(vehicle.transform.position);
                foreach (Zone zone in objectZones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.VehicleSiphoning))
                    {
                        shouldAllow = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnSiphonVehicleRequested)}.", ex);
                shouldAllow = true;
            }
        }
        
        /// <summary>
        /// Handles lockpick requests, blocking lockpicking when the zone has the NoLockpick flag.
        /// </summary>
        private static void OnVehicleLockpicked(InteractableVehicle vehicle, Player instigatingPlayer, ref bool shouldAllow)
        {
            try
            {
                ZonePlayerComponent comp = instigatingPlayer.GetComponent<ZonePlayerComponent>();

                foreach (var zone in comp.Zones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.Lockpick))
                    {
                        shouldAllow = false;
                        break;
                    }
                }

                var objectZones = ZoneManager.GetZonesFromPosition(vehicle.transform.position);
                foreach (Zone zone in objectZones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.Lockpick))
                    {
                        shouldAllow = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnVehicleLockpicked)}.", ex);
                shouldAllow = true;
            }
        }
        
        /// <summary>
        /// Handles carjack requests, blocking carjacking when the zone has the NoVehicleCarjack flag.
        /// </summary>
        private static void OnVehicleCarjacked(InteractableVehicle vehicle, Player instigatingPlayer, ref bool shouldAllow, ref Vector3 force, ref Vector3 torque)
        {
            try
            {
                ZonePlayerComponent comp = instigatingPlayer.GetComponent<ZonePlayerComponent>();

                foreach (var zone in comp.Zones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.VehicleCarjack))
                    {
                        shouldAllow = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnVehicleCarjacked)}.", ex);
                shouldAllow = true;
            }
        }
    }
}