using System;
using Rocket.Unturned.Player;
using Steamworks;
using Tavstal.RocketFlow.Attributes;
using Tavstal.RocketFlow.Core;
using Tavstal.RocketFlow.Events.Structure;
using Tavstal.TLibrary.Extensions;
using Tavstal.TZones.Models.Enums;
using Tavstal.TZones.Utils.Constants;
using Tavstal.TZones.Utils.Managers;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace Tavstal.TZones.Handlers
{
    /// <summary>
    /// Handles structure-related events such as deploy, damage, and salvage, enforcing zone restrictions.
    /// </summary>
    public class StructureEventListener : EventListener
    {
        [EventHandler]
        private void OnDeployStructureRequested(StructureDeployEvent e)
        {
            if (!e.ShouldAllow)
                return;
            
            bool originalValue = e.ShouldAllow;
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID((CSteamID)e.Owner);
                if (player == null)
                    return;

                if (ZoneManager.HasFlagOrBlocked(Flags.NoStructures,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoStructures,
                        player, e.Asset.id, ERestrictionType.BUILD))
                {
                    e.ShouldAllow = false;
                    return;
                }

                if (!ZoneManager.HasFlagOrBlocked(Flags.NoStructures,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoStructures,
                        e.Point, e.Asset.id, ERestrictionType.BUILD))
                    return;

                e.ShouldAllow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnDeployStructureRequested)}.", ex);
                e.ShouldAllow = originalValue;
            }
        }
        
        
        [EventHandler]
        private void OnSalvageStructureRequested(StructureSalvageEvent e)
        {
            if (!e.ShouldAllow)
                return;
            
            bool originalValue = e.ShouldAllow;
            try
            {
                var point = e.Structure.GetServersideData().point;
                UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(e.InstigatorClient);
                if (player == null)
                    return;

                if (ZoneManager.HasFlag(Flags.NoStructureSalvage, TZones.Instance.Config.GlobalZoneFlagChecks.NoStructureSalvage,
                        player))
                {
                    e.ShouldAllow = false;
                    return;
                }

                if (!ZoneManager.HasFlag(Flags.NoStructureSalvage,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoStructureSalvage,
                        point))
                    return;

                e.ShouldAllow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnSalvageStructureRequested)}.", ex);
                e.ShouldAllow = originalValue;
            }
        }
        
        [EventHandler]
        private void OnDamageStructureRequested(StructureDamageEvent e)
        {
            if (!e.ShouldAllow)
                return;
            
            bool originalValue = e.ShouldAllow;
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(e.Instigator);
                if (player == null)
                    return;

                if (ZoneManager.HasFlag(Flags.NoDamage, TZones.Instance.Config.GlobalZoneFlagChecks.NoDamage,
                        player))
                {
                    e.ShouldAllow = false;
                    return;
                }

                if (!ZoneManager.HasFlag(Flags.NoDamage,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoDamage,
                        e.Transform.position))
                    return;

                e.ShouldAllow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnDamageStructureRequested)}.", ex);
                e.ShouldAllow = originalValue;
            }
        }
    }
}