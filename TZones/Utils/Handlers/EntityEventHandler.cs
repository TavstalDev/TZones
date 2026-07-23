using System;
using SDG.Unturned;
using Tavstal.TLibrary.Extensions;
using Tavstal.TZones.Components;
using Tavstal.TZones.Models.Core;
using Tavstal.TZones.Utils.Constants;
using Tavstal.TZones.Utils.Managers;

namespace Tavstal.TZones.Utils.Handlers
{
    public static class EntityEventHandler
    {
        private static bool _isAttached;

        public static void AttachEvents()
        {
            if (_isAttached)
                return;
            
            DamageTool.damageAnimalRequested += OnDamageAnimalRequested;
            DamageTool.damageZombieRequested += OnDamageZombieRequested;

            _isAttached = true;
        }

        public static void DetachEvents()
        {
            if (!_isAttached)
                return;
            
            DamageTool.damageAnimalRequested -= OnDamageAnimalRequested;
            DamageTool.damageZombieRequested -= OnDamageZombieRequested;

            _isAttached = true;
        }
        
        private static void OnDamageAnimalRequested(ref DamageAnimalParameters parameters, ref bool shouldAllow)
        {
            try
            {
                if (parameters.instigator is Player player)
                {
                    ZonePlayerComponent comp = player.GetComponent<ZonePlayerComponent>();
                    foreach (var zone in comp.Zones)
                    {
                        if (ZoneManager.Queries.HasFlag(zone,Flags.AnimalDamage))
                        {
                            shouldAllow = false;
                            break;
                        }
                    }

                    var objectZones = ZoneManager.GetZonesFromPosition(parameters.animal.transform.position);
                    foreach (Zone zone in objectZones)
                    {
                        if (ZoneManager.Queries.HasFlag(zone, Flags.AnimalDamage))
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
                shouldAllow = true;
            }
        }
        
        private static void OnDamageZombieRequested(ref DamageZombieParameters parameters, ref bool shouldAllow)
        {
            try
            {
                if (parameters.instigator is Player player)
                {
                    ZonePlayerComponent comp = player.GetComponent<ZonePlayerComponent>();

                    foreach (var zone in comp.Zones)
                    {
                        if (ZoneManager.Queries.HasFlag(zone,Flags.ZombieDamage))
                        {
                            shouldAllow = false;
                            break;
                        }
                    }

                    var objectZones = ZoneManager.GetZonesFromPosition(parameters.zombie.transform.position);
                    foreach (Zone zone in objectZones)
                    {
                        if (ZoneManager.Queries.HasFlag(zone, Flags.ZombieDamage))
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
                shouldAllow = true;
            }
        }
    }
}