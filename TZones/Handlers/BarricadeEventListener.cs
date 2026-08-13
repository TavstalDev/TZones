using System;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using Tavstal.RocketFlow.Attributes;
using Tavstal.RocketFlow.Core;
using Tavstal.RocketFlow.Events.Barricade;
using Tavstal.TLibrary.Extensions;
using Tavstal.TZones.Models.Enums;
using Tavstal.TZones.Utils.Constants;
using Tavstal.TZones.Utils.Managers;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace Tavstal.TZones.Handlers
{
    /// <summary>
    /// Handles barricade-related events such as deploy, damage, and salvage, enforcing zone restrictions.
    /// </summary>
    public class BarricadeEventListener : EventListener
    {
        [EventHandler]   
        private static void OnBarricadeSpawned(BarricadeSpawnedEvent e)
        {
            if (e.Drop.interactable is InteractableGenerator generator)
                ZoneManager.Cache.AddGenerator(generator);
        }
        
        [EventHandler]
        private static void OnDeployBarricadeRequested(BarricadeDeployEvent e)
        {
            if (!e.ShouldAllow)
                return;
            
            bool originalValue = e.ShouldAllow;
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID((CSteamID)e.Owner);
                if (player == null)
                    return;

                if (ZoneManager.HasFlagOrBlocked(Flags.NoBarricades,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoBarricades,
                        player, e.Asset.id, ERestrictionType.BUILD))
                {
                    e.ShouldAllow = false;
                    return;
                }

                if (!ZoneManager.HasFlagOrBlocked(Flags.NoBarricades,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoBarricades,
                        e.Point,  e.Asset.id, ERestrictionType.BUILD))
                    return;

                e.ShouldAllow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnDeployBarricadeRequested)}.", ex);
                e.ShouldAllow = originalValue;
            }
        }
        
        [EventHandler]
        private void OnSalvageBarricadeRequested(BarricadeSalvageEvent e)
        {
            if (!e.ShouldAllow)
                return;
            
            var barricade = e.Barricade;
            bool originalValue = e.ShouldAllow;
            try
            {
                if (barricade.interactable?.transform.position == null)
                    return;
                
                var point = barricade.interactable.transform.position;
                UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(e.InstigatorClient);
                if (player == null)
                    return;

                if (ZoneManager.HasFlag(Flags.NoBarricadeSalvage, TZones.Instance.Config.GlobalZoneFlagChecks.NoBarricadeSalvage,
                        player))
                {
                    e.ShouldAllow = false;
                    return;
                }

                if (!ZoneManager.HasFlag(Flags.NoBarricadeSalvage,
                        TZones.Instance.Config.GlobalZoneFlagChecks.NoBarricadeSalvage,
                        point))
                    return;

                e.ShouldAllow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnSalvageBarricadeRequested)}.", ex);
                e.ShouldAllow = originalValue;
            }
            finally
            {
                if (e.ShouldAllow && barricade.asset.build == EBuild.GENERATOR)
                    if (barricade.interactable != null && barricade.interactable is InteractableGenerator generator)
                        ZoneManager.Cache.RemoveGenerator(generator);
            }
        }
        
        [EventHandler]
        private void OnDamageBarricadeRequested(BarricadeDamageEvent e)
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
                        e.BarricadeTransform.position))
                    return;

                e.ShouldAllow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnDamageBarricadeRequested)}.", ex);
                e.ShouldAllow = originalValue;
            }
            finally
            {
                if (e.BarricadeTransform != null)
                {
                    var barricade = BarricadeManager.FindBarricadeByRootTransform(e.BarricadeTransform);
                    if (barricade != null)
                    {
                        if (e.ShouldAllow &&
                            barricade.GetServersideData().barricade.health - e.PendingTotalDamage <= 0 &&
                            barricade.asset.build == EBuild.GENERATOR)
                            if (barricade.interactable is InteractableGenerator generator)
                                ZoneManager.Cache.RemoveGenerator(generator);
                    }
                }
            }
        }
    }
}