using System;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using Tavstal.RocketFlow.Attributes;
using Tavstal.RocketFlow.Core;
using Tavstal.RocketFlow.Events.Vehicle;
using Tavstal.TLibrary.Extensions;
using Tavstal.TZones.Models.Enums;
using Tavstal.TZones.Utils.Constants;
using Tavstal.TZones.Utils.Managers;
using UnityEngine;
// ReSharper disable UnusedMember.Local

namespace Tavstal.TZones.Handlers
{
    /// <summary>
    /// Handles vehicle-related events such as enter, exit, damage, lockpick, carjack, and siphon, enforcing zone restrictions.
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class VehicleEventListener : EventListener
    {
        [EventHandler]
        private void OnEnterVehicleRequested(VehicleEnterEvent e)
        {
            if (!e.ShouldAllow)
                return;
            
            bool originalValue = e.ShouldAllow;
            try
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(e.Player);
                if (uPlayer == null)
                    return;

                if (ZoneManager.HasFlagOrBlocked(Flags.NoVehicleEnter,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoVehicleEnter,
                        uPlayer, e.Vehicle.asset.id, ERestrictionType.VEHICLE_ENTER))
                {
                    e.ShouldAllow = false;
                    return;
                }

                if (!ZoneManager.HasFlagOrBlocked(Flags.NoVehicleEnter,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoVehicleEnter,
                        e.Vehicle.transform.position, e.Vehicle.asset.id, ERestrictionType.VEHICLE_ENTER))
                    return;

                e.ShouldAllow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnEnterVehicleRequested)}.", ex);
                e.ShouldAllow = originalValue;
            }
        }
        
        [EventHandler]
        private void OnExitVehicleRequested(VehicleExitEvent e)
        {
            if (!e.ShouldAllow)
                return;
            
            bool originalValue = e.ShouldAllow;
            try
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(e.Player);
                if (uPlayer == null)
                    return;

                if (ZoneManager.HasFlagOrBlocked(Flags.NoVehicleExit,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoVehicleExit,
                        uPlayer, e.Vehicle.asset.id, ERestrictionType.VEHICLE_EXIT))
                {
                    e.ShouldAllow = false;
                    return;
                }

                if (!ZoneManager.HasFlagOrBlocked(Flags.NoVehicleExit,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoVehicleExit,
                        e.Vehicle.transform.position, e.Vehicle.asset.id, ERestrictionType.VEHICLE_EXIT))
                    return;

                e.ShouldAllow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnExitVehicleRequested)}.", ex);
                e.ShouldAllow = originalValue;
            }
        }
        
        [EventHandler]
        private void OnDamageVehicleRequested(VehicleDamageEvent e)
        {
            if (!e.ShouldAllow)
                return;
            
            bool originalValue = e.ShouldAllow;
            try
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(e.InstigatorSteamID);
                if (uPlayer == null)
                    return;
                
                if (ZoneManager.HasFlag(Flags.NoVehicleDamage,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoVehicleDamage,
                        uPlayer))
                {
                    e.ShouldAllow = false;
                    return;
                }

                if (!ZoneManager.HasFlag(Flags.NoVehicleDamage,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoVehicleDamage,
                        e.Vehicle.transform.position))
                    return;

                e.ShouldAllow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnDamageVehicleRequested)}.", ex);
                e.ShouldAllow = originalValue;
            }
        }
        
        [EventHandler]
        private void OnDamageTireRequested(VehicleTireDamageEvent e)
        {
            if (!e.ShouldAllow)
                return;
            
            bool originalValue = e.ShouldAllow;
            try
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(e.InstigatorSteamID);
                if (uPlayer == null)
                    return;

                if (ZoneManager.HasFlag(Flags.NoTireDamage,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoTireDamage,
                        uPlayer))
                {
                    e.ShouldAllow = false;
                    return;
                }

                if (!ZoneManager.HasFlag(Flags.NoTireDamage,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoTireDamage,
                        e.Vehicle.transform.position))
                    return;

                e.ShouldAllow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnDamageTireRequested)}.", ex);
                e.ShouldAllow = originalValue;
            }
        }
        
        [EventHandler]
        private void OnSiphonVehicleRequested(VehicleSiphonEvent e)
        {
            if (!e.ShouldAllow)
                return;
            
            bool originalValue = e.ShouldAllow;
            try
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(e.InstigatingPlayer);
                if (uPlayer == null)
                    return;

                if (ZoneManager.HasFlag(Flags.NoVehicleSiphoning,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoVehicleSiphoning,
                        uPlayer))
                {
                    e.ShouldAllow = false;
                    return;
                }

                if (!ZoneManager.HasFlag(Flags.NoVehicleSiphoning,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoVehicleSiphoning,
                        e.Vehicle.transform.position))
                    return;

                e.ShouldAllow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnSiphonVehicleRequested)}.", ex);
                e.ShouldAllow = originalValue;
            }
        }
        
        [EventHandler]
        private void OnVehicleLockpicked(VehicleLockpickEvent e)
        {
            if (!e.Allow)
                return;
            
            bool originalValue = e.Allow;
            try
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(e.InstigatingPlayer);
                if (uPlayer == null)
                    return;

                if (ZoneManager.HasFlag(Flags.NoLockpick,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoLockpick,
                        uPlayer))
                {
                    e.Allow = false;
                    return;
                }

                if (!ZoneManager.HasFlag(Flags.NoLockpick,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoLockpick,
                        e.Vehicle.transform.position))
                    return;

                e.Allow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnVehicleLockpicked)}.", ex);
                e.Allow = originalValue;
            }
        }
        
        [EventHandler]
        private void OnVehicleCarjacked(VehicleCarjackEvent e)
        {
            if (!e.Allow)
                return;
            
            bool originalValue = e.Allow;
            try
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(e.InstigatingPlayer);
                if (uPlayer == null)
                    return;

                if (ZoneManager.HasFlag(Flags.NoVehicleCarjack,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoVehicleCarjack,
                        uPlayer))
                {
                    e.Allow = false;
                    return;
                }

                if (!ZoneManager.HasFlag(Flags.NoVehicleCarjack,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoVehicleCarjack,
                        e.Vehicle.transform.position))
                    return;

                e.Allow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnVehicleCarjacked)}.", ex);
                e.Allow = originalValue;
            }
        }
    }
}