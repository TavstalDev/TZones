using System;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.TLibrary.Extensions;
using Tavstal.TZones.Utils.Constants;
using Tavstal.TZones.Utils.Managers;

namespace Tavstal.TZones.Handlers
{
    /// <summary>
    /// Handles entity-related events such as animal and zombie damage, enforcing zone restrictions.
    /// </summary>
    public static class EntityEventHandler
    {
        private static bool _isAttached;

        /// <summary>
        /// Subscribes to all entity events if not already attached.
        /// </summary>
        public static void AttachEvents()
        {
            if (_isAttached)
                return;
            
            DamageTool.damageAnimalRequested += OnDamageAnimalRequested;
            DamageTool.damageZombieRequested += OnDamageZombieRequested;

            _isAttached = true;
        }

        /// <summary>
        /// Unsubscribes from all entity events if currently attached.
        /// </summary>
        public static void DetachEvents()
        {
            if (!_isAttached)
                return;
            
            DamageTool.damageAnimalRequested -= OnDamageAnimalRequested;
            DamageTool.damageZombieRequested -= OnDamageZombieRequested;

            _isAttached = true;
        }
        
        /// <summary>
        /// Handles animal damage requests, blocking damage when the zone has the NoAnimalDamage flag.
        /// </summary>
        private static void OnDamageAnimalRequested(ref DamageAnimalParameters parameters, ref bool shouldAllow)
        {
            if (!shouldAllow)
                return;
            
            bool originalValue = shouldAllow;
            try
            {
                if (parameters.instigator is Player player)
                {
                    UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(player);
                    if (ZoneManager.HasFlag(Flags.NoAnimalDamage, TZones.Instance.Config.GlobalZoneFlagChecks.NoAnimalDamage,
                            uPlayer))
                    {
                        shouldAllow = false;
                        return;
                    }

                    if (!ZoneManager.HasFlag(Flags.NoAnimalDamage,
                            TZones.Instance.Config.GlobalZoneFlagChecks.NoAnimalDamage,
                            parameters.animal.transform.position))
                        return;

                    shouldAllow = false;
                }
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnDamageAnimalRequested)}.", ex);
                shouldAllow = originalValue;
            }
        }
        
        /// <summary>
        /// Handles zombie damage requests, blocking damage when the zone has the NoZombieDamage flag.
        /// </summary>
        private static void OnDamageZombieRequested(ref DamageZombieParameters parameters, ref bool shouldAllow)
        {
            if (!shouldAllow)
                return;
            
            bool originalValue = shouldAllow;
            try
            {
                if (parameters.instigator is Player player)
                {
                    UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(player);
                    if (ZoneManager.HasFlag(Flags.NoZombieDamage, TZones.Instance.Config.GlobalZoneFlagChecks.NoZombieDamage,
                            uPlayer))
                    {
                        shouldAllow = false;
                        return;
                    }

                    if (!ZoneManager.HasFlag(Flags.NoZombieDamage,
                            TZones.Instance.Config.GlobalZoneFlagChecks.NoZombieDamage,
                            parameters.zombie.transform.position))
                        return;

                    shouldAllow = false;
                }
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnDamageZombieRequested)}.", ex);
                shouldAllow = originalValue;
            }
        }
    }
}