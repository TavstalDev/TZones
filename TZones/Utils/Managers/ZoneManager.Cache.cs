using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SDG.Unturned;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Models.Database;
using Tavstal.TZones.Models.Core;
using Flag = Tavstal.TZones.Models.Core.Flag;
using Node = Tavstal.TZones.Models.Core.Node;
using Object = UnityEngine.Object;

// ReSharper disable UnusedMember.Global

namespace Tavstal.TZones.Utils.Managers
{
    /// <summary>
    /// Provides an in-memory cache of all zone data, with thread-safe access and dirty-state tracking.
    /// </summary>
    public class ZonesManager_Cache
    {
        private bool _isDirty;

        /// <summary>
        /// Gets whether the cache has unsaved changes that need to be refreshed.
        /// </summary>
        public bool IsDirty => _isDirty;
        
        private readonly object _flagLock = new object();
        // Because the unturned events use 'ref', and the database is async, cache must be used
        private readonly List<Flag> _flags = new List<Flag>();

        /// <summary>
        /// Gets a snapshot of all cached flags.
        /// </summary>
        public IReadOnlyList<Flag> Flags 
        {
            get 
            {
                lock (_flagLock) 
                {
                    return new List<Flag>(_flags); 
                }
            }
        }

        private readonly object _zonesLock = new object();
        private readonly List<Zone> _zones = new List<Zone>();

        /// <summary>
        /// Gets a snapshot of all cached zones.
        /// </summary>
        public IReadOnlyList<Zone> Zones
        {
            get
            {
                lock (_zonesLock)
                {
                    return new List<Zone>(_zones);
                }
            }
        }
        
        /// <summary>
        /// Gets the dictionary of cached nodes keyed by zone id.
        /// </summary>
        private readonly ConcurrentDictionary<ulong, List<Node>> _nodes = new ConcurrentDictionary<ulong, List<Node>>();
        public ConcurrentDictionary<ulong, List<Node>> Nodes => _nodes;
        
        /// <summary>
        /// Gets the dictionary of cached zone-flag associations keyed by zone id.
        /// </summary>
        private readonly ConcurrentDictionary<ulong, List<ZoneFlag>> _zoneFlags = new ConcurrentDictionary<ulong, List<ZoneFlag>>();
        public ConcurrentDictionary<ulong, List<ZoneFlag>> ZoneFlags => _zoneFlags;
        
        /// <summary>
        /// Gets the dictionary of cached zone events keyed by zone id.
        /// </summary>
        private readonly ConcurrentDictionary<ulong, List<ZoneEvent>> _zoneEvents = new ConcurrentDictionary<ulong, List<ZoneEvent>>();
        public ConcurrentDictionary<ulong, List<ZoneEvent>> ZoneEvents => _zoneEvents;
        
        /// <summary>
        /// Gets the dictionary of cached restrictions keyed by zone id.
        /// </summary>
        private readonly ConcurrentDictionary<ulong, List<Restriction>> _zoneBlocks = new ConcurrentDictionary<ulong, List<Restriction>>();
        public ConcurrentDictionary<ulong, List<Restriction>> ZoneBlocks => _zoneBlocks;
        
        private readonly object _generatorLock = new object();
        private readonly List<InteractableGenerator> _interactableGeneratorCache = new List<InteractableGenerator>();

        /// <summary>
        /// Gets a snapshot of all interactable generators in the level.
        /// </summary>
        public IReadOnlyList<InteractableGenerator> InteractableGeneratorCache
        {
            get
            {
                lock (_generatorLock)
                {
                    return new List<InteractableGenerator>(_interactableGeneratorCache);
                }
            }
        }
        
        /// <summary>
        /// Marks the cache as dirty, triggering a full refresh on the next update cycle.
        /// </summary>
        public void MakeDirty() 
        {
            _isDirty = true;
        }
        
        /// <summary>
        /// Refreshes the cached list of interactable generators from the level.
        /// </summary>
        public void RefreshGeneratorCache()
        {
            lock (_generatorLock)
            {
                _interactableGeneratorCache.Clear();
                _interactableGeneratorCache.AddRange(Object.FindObjectsOfType<InteractableGenerator>().ToList());
            }
        }

        public void AddGenerator(InteractableGenerator generator)
        {
            lock (_generatorLock)
            {
                _interactableGeneratorCache.Add(generator);
            }
        }

        public void RemoveGenerator(InteractableGenerator generator)
        {
            lock (_generatorLock)
            {
                _interactableGeneratorCache.Remove(generator);
            }
        }
        
        /// <summary>
        /// If the cache is dirty, clears the dirty flag and triggers a full refresh from the database.
        /// </summary>
        internal async Task CheckDirtyAsync()
        {
            if (!_isDirty)
                return;

            _isDirty = false;
            await RefreshAllAsync();
        }
        
        /// <summary>
        /// Reloads all zone data from the database into the cache.
        /// </summary>
        internal async Task RefreshAllAsync()
        {
            try
            {
                var newFlags = await TZones.DatabaseManager.Flags.GetAsync(1000, QueryParameter.not("Id", "0")) ??
                               new List<Flag>();
                lock (_flagLock)
                {
                    _flags.Clear();
                    _flags.AddRange(newFlags);
                }

                _nodes.Clear();
                _zoneFlags.Clear();
                _zoneEvents.Clear();
                _zoneBlocks.Clear();

                var newZones = await TZones.DatabaseManager.Zones.GetAsync(1000, QueryParameter.not("Id", "0")) ??
                               new List<Zone>();
                lock (_zonesLock)
                {
                    _zones.Clear();
                    _zones.AddRange(newZones);
                }
                
                foreach (Zone zone in newZones)
                {
                    List<Node> nodes =
                        await TZones.DatabaseManager.Nodes.GetAsync(
                            queryParameters: QueryParameter.eq("ZoneId", zone.Id)) ?? new List<Node>();
                    _nodes.TryAdd(zone.Id, nodes);

                    List<ZoneFlag> flags =
                        await TZones.DatabaseManager.ZoneFlags.GetAsync(
                            queryParameters: QueryParameter.eq("ZoneId", zone.Id)) ?? new List<ZoneFlag>();
                    _zoneFlags.TryAdd(zone.Id, flags);

                    List<ZoneEvent> events =
                        await TZones.DatabaseManager.ZoneEvents.GetAsync(
                            queryParameters: QueryParameter.eq("ZoneId", zone.Id)) ?? new List<ZoneEvent>();
                    _zoneEvents.TryAdd(zone.Id, events);

                    List<Restriction> blocks =
                        await TZones.DatabaseManager.Restrictions.GetAsync(
                            queryParameters: QueryParameter.eq("ZoneId", zone.Id)) ?? new List<Restriction>();
                    _zoneBlocks.TryAdd(zone.Id, blocks);
                }
            }
            catch (Exception ex)
            {
                TZones.Logger.Error("Failed to refresh the cache.", ex);
            }
        }
        
        /// <summary>
        /// Reloads a single zone and its associated data from the database.
        /// </summary>
        /// <param name="zoneId">The id of the zone to refresh.</param>
        internal async Task RefreshZoneAsync(ulong zoneId) 
        {
            Zone? zone = Zones.FirstOrDefault(x => x.Id == zoneId);
            if (zone != null) {
                lock (_zonesLock)
                {
                    _zones.Remove(zone);
                }
                _nodes.TryRemove(zone.Id, out _);
                _zoneFlags.TryRemove(zone.Id, out _);
                _zoneEvents.TryRemove(zone.Id, out _);
                _zoneBlocks.TryRemove(zone.Id, out _);
            }
            zone = await TZones.DatabaseManager.Zones.GetAsync(zoneId);
            if (zone == null)
                return;

            lock (_zonesLock)
            {
                _zones.Add(zone);
            }
            
            List<Node> nodes = await TZones.DatabaseManager.Nodes.GetAsync(queryParameters: QueryParameter.eq("ZoneId", zone.Id)) ?? new List<Node>();
            _nodes.TryAdd(zone.Id, nodes);

            List<ZoneFlag> flags = await TZones.DatabaseManager.ZoneFlags.GetAsync(queryParameters: QueryParameter.eq("ZoneId", zone.Id)) ?? new List<ZoneFlag>();
            _zoneFlags.TryAdd(zone.Id, flags);

            List<ZoneEvent> events = await TZones.DatabaseManager.ZoneEvents.GetAsync(queryParameters: QueryParameter.eq("ZoneId", zone.Id)) ?? new List<ZoneEvent>();
            _zoneEvents.TryAdd(zone.Id, events);

            List<Restriction> blocks = await TZones.DatabaseManager.Restrictions.GetAsync(queryParameters: QueryParameter.eq("ZoneId", zone.Id)) ?? new List<Restriction>();
            _zoneBlocks.TryAdd(zone.Id, blocks);
        }
    }
}