using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.TLibrary.Threading;
using Tavstal.TZones.Components;
using Tavstal.TZones.Models.Core;
using UnityEngine;

namespace Tavstal.TZones.Utils.Managers
{
    /// <summary>
    /// Handles periodic update logic for player zone tracking, generator refueling, and zombie removal.
    /// </summary>
    public class ZonesManager_Update
    {
        private ZonesManager_Cache _cache { get; }
        private ZonesManager_Queries _queries { get; }

        /// <summary>
        /// Gets or sets whether an update cycle is currently in progress.
        /// </summary>
        public bool IsUpdating {  get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZonesManager_Update"/> class.
        /// </summary>
        /// <param name="cache">The zone data cache.</param>
        /// <param name="queries">The zone query helper.</param>
        public ZonesManager_Update(ZonesManager_Cache cache, ZonesManager_Queries queries)
        {
            _cache = cache;
            _queries = queries;
        }
        
        /// <summary>
        /// Runs a full update cycle: refreshes dirty cache, updates player zone tracking, generators, and zombies.
        /// </summary>
        internal async Task UpdateAsync()
        {
            await _cache.CheckDirtyAsync();
            
            var zones = _cache.Zones;
            if (zones.Count == 0)
                return;
            
            await MainThreadDispatcher.RunAsync(() =>
            {
                UpdatePlayers();
                
                foreach (Zone zone in zones)
                {
                    UpdateGenerators(zone);
                    UpdateZombies(zone);
                }
            });
        }
        
        /// <summary>
        /// Iterates all connected players and fires enter/leave zone events based on their current position.
        /// </summary>
        private void UpdatePlayers()
        {
            var existingZones = _cache.Zones;
            if (Provider.clients.Count == 0)
                return;
            
            foreach (SteamPlayer steamPlayer in Provider.clients) 
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                ZonePlayerComponent comp = uPlayer.GetComponent<ZonePlayerComponent>();

                var currentZones = new HashSet<ulong>(ZoneManager.GetZoneIdsFromPosition(uPlayer.Position));
                bool updateLastPos = true;

                foreach (var zoneId in comp.Zones)
                {
                    bool shouldAllow = true;
                    // Check for zones that the player has left
                    if (currentZones.Contains(zoneId))
                        continue;

                    var zone = existingZones.FirstOrDefault(x => x.Id == zoneId);
                    if (zone == null)
                        continue;

                    ZoneManager.FPlayerLeaveZone(uPlayer, zone, comp.LastPosition, ref shouldAllow);
                    if (!shouldAllow)
                    {
                        currentZones.Add(zoneId);
                        updateLastPos = false;
                    }
                }

                foreach (var zoneId in currentZones.ToList()) // .ToList prevents list edit errors
                {
                    bool shouldAllow = true;
                    // Check for zones that the player has left
                    if (comp.Zones.Contains(zoneId))
                        continue;

                    var zone = existingZones.FirstOrDefault(x => x.Id == zoneId);
                    if (zone == null)
                    {
                        currentZones.Remove(zoneId);
                        continue;
                    }

                    ZoneManager.FPlayerEnterZone(uPlayer, zone, comp.LastPosition, ref shouldAllow);
                    if (!shouldAllow)
                    {
                        currentZones.Remove(zoneId);
                        updateLastPos = false;
                    }
                }

                comp.Zones = currentZones;
                if (updateLastPos)
                    comp.LastPosition = uPlayer.Position;
            }
        }
        
        /// <summary>
        /// Removes zombies inside zones that have the NoZombie flag.
        /// </summary>
        private void UpdateZombies(Zone zone)
        {
            if (ZombieManager.regions == null || !_queries.HasFlag(zone, Constants.Flags.NoZombie))
                return;
            
            foreach (var zombie in ZombieManager.regions
                         .Where(t => t.zombies != null) // Filter regions that have zombies
                         .SelectMany(t => t.zombies) // Flatten the list of zombies from each region
                         .Where(z => z && !z.isDead && ZoneManager.IsPointInZone(zone, z.transform.position))) // Filter out dead zombies and those outside the zone
            {
                // The zombie is alive and within the zone
                zombie.gear = 0;
                zombie.isDead = true;
                ZombieManager.sendZombieDead(zombie, Vector3.zero);
            }
        }
        
        /// <summary>
        /// Refills generators inside zones that have the InfiniteGenerator flag.
        /// </summary>
        private void UpdateGenerators(Zone zone)
        {
            if (!_queries.HasFlag(zone, Constants.Flags.InfiniteGenerator))
                return;
            
            foreach (var generator in _cache.InteractableGeneratorCache)
            {
                if (!ZoneManager.IsPointInZone(zone, generator.transform.position))
                    continue;
                
                BarricadeManager.sendFuel(generator.transform, generator.capacity);
            }
        }
    }
}