using System;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.RocketFlow.Attributes;
using Tavstal.RocketFlow.Core;
using Tavstal.RocketFlow.Events.Damage;
using Tavstal.TLibrary.Extensions;
using Tavstal.TZones.Utils.Constants;
using Tavstal.TZones.Utils.Managers;
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace Tavstal.TZones.Handlers
{
    /// <summary>
    /// Handles entity-related events such as animal and zombie damage, enforcing zone restrictions.
    /// </summary>
    public class EntityEventListener : EventListener
    {
        [EventHandler]
        private static void OnDamageAnimalRequested(AnimalDamageEvent e)
        {
            if (!e.ShouldAllow)
                return;
            
            bool originalValue = e.ShouldAllow;
            try
            {
                if (e.Parameters.instigator is Player player)
                {
                    UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(player);
                    if (ZoneManager.HasFlag(Flags.NoAnimalDamage, TZones.Instance.Config.GlobalZoneFlagChecks.NoAnimalDamage,
                            uPlayer))
                    {
                        e.ShouldAllow = false;
                        return;
                    }

                    if (!ZoneManager.HasFlag(Flags.NoAnimalDamage,
                            TZones.Instance.Config.GlobalZoneFlagChecks.NoAnimalDamage,
                            e.Parameters.animal.transform.position))
                        return;

                    e.ShouldAllow = false;
                }
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnDamageAnimalRequested)}.", ex);
                e.ShouldAllow = originalValue;
            }
        }
        
        [EventHandler]
        private void OnDamageZombieRequested(ZombieDamageEvent e)
        {
            if (!e.ShouldAllow)
                return;
            
            bool originalValue = e.ShouldAllow;
            try
            {
                if (e.Parameters.instigator is Player player)
                {
                    UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(player);
                    if (ZoneManager.HasFlag(Flags.NoZombieDamage, TZones.Instance.Config.GlobalZoneFlagChecks.NoZombieDamage,
                            uPlayer))
                    {
                        e.ShouldAllow = false;
                        return;
                    }

                    if (!ZoneManager.HasFlag(Flags.NoZombieDamage,
                            TZones.Instance.Config.GlobalZoneFlagChecks.NoZombieDamage,
                            e.Parameters.zombie.transform.position))
                        return;

                    e.ShouldAllow = false;
                }
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnDamageZombieRequested)}.", ex);
                e.ShouldAllow = originalValue;
            }
        }
    }
}