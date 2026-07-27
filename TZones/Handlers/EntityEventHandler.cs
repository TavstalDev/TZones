using System;
using SDG.Unturned;
using Tavstal.TLibrary.Extensions;
using Tavstal.TZones.Components;
using Tavstal.TZones.Models.Core;
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
            bool originalValue = shouldAllow;
            try
            {
                if (parameters.instigator is Player player)
                {
                    ZonePlayerComponent comp = player.GetComponent<ZonePlayerComponent>();
                    foreach (var zone in comp.Zones)
                    {
                        if (ZoneManager.Queries.HasFlag(zone,Flags.NoAnimalDamage))
                        {
                            shouldAllow = false;
                            break;
                        }
                    }

                    var objectZones = ZoneManager.GetZonesFromPosition(parameters.animal.transform.position);
                    foreach (Zone zone in objectZones)
                    {
                        if (ZoneManager.Queries.HasFlag(zone, Flags.NoAnimalDamage))
                        {
                            shouldAllow = false;
                            break;
                        }
                    }
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
            bool originalValue = shouldAllow;
            try
            {
                if (parameters.instigator is Player player)
                {
                    ZonePlayerComponent comp = player.GetComponent<ZonePlayerComponent>();

                    foreach (var zone in comp.Zones)
                    {
                        if (ZoneManager.Queries.HasFlag(zone,Flags.NoZombieDamage))
                        {
                            shouldAllow = false;
                            break;
                        }
                    }

                    var objectZones = ZoneManager.GetZonesFromPosition(parameters.zombie.transform.position);
                    foreach (Zone zone in objectZones)
                    {
                        if (ZoneManager.Queries.HasFlag(zone, Flags.NoZombieDamage))
                        {
                            shouldAllow = false;
                            break;
                        }
                    }
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