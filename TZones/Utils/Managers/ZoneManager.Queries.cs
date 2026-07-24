using System.Collections.Generic;
using System.Linq;
using Tavstal.TZones.Models.Core;
using Tavstal.TZones.Models.Enums;

namespace Tavstal.TZones.Utils.Managers
{
    /// <summary>
    /// Provides read-only query methods for looking up zone data from the cache.
    /// </summary>
    public class ZonesManager_Queries
    {
        private readonly ZonesManager_Cache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZonesManager_Queries"/> class.
        /// </summary>
        /// <param name="cache">The zone data cache to query against.</param>
        public ZonesManager_Queries(ZonesManager_Cache cache)
        {
            _cache = cache;
        }
        
        /// <summary>
        /// Checks whether a zone has a specific flag assigned.
        /// </summary>
        /// <param name="zone">The zone to check.</param>
        /// <param name="flagName">The flag name to look for.</param>
        /// <returns>True if the zone has the specified flag.</returns>
        public bool HasFlag(Zone zone, string flagName) => HasFlag(zone.Id, flagName);
        
        /// <summary>
        /// Checks whether a zone has a specific flag assigned.
        /// </summary>
        /// <param name="zoneId">The zone id to check.</param>
        /// <param name="flagName">The flag name to look for.</param>
        /// <returns>True if the zone has the specified flag.</returns>
        public bool HasFlag(ulong zoneId, string flagName) 
        {
            Flag? flag = GetFlag(flagName);
            if (flag == null) 
                return false;
            var zoneflags = GetZoneFlags(zoneId);
            return zoneflags?.Any(x => x.FlagId == flag.Id) ?? false;
        }
        
        /// <summary>
        /// Gets a flag by its name.
        /// </summary>
        /// <param name="flagName">The flag name.</param>
        /// <returns>The matching flag, or null if not found.</returns>
        public Flag? GetFlag(string flagName) => _cache.Flags.FirstOrDefault(x => x.Name == flagName);
        
        /// <summary>
        /// Gets a flag by its id.
        /// </summary>
        /// <param name="flagId">The flag id.</param>
        /// <returns>The matching flag, or null if not found.</returns>
        public Flag? GetFlag(ulong flagId) => _cache.Flags.FirstOrDefault(x => x.Id  == flagId);
        
        /// <summary>
        /// Gets a zone by its name.
        /// </summary>
        /// <param name="name">The zone name.</param>
        /// <returns>The matching zone, or null if not found.</returns>
        public Zone? GetZone(string name) => _cache.Zones.FirstOrDefault(x => x.Name == name);
        
        /// <summary>
        /// Gets a zone by its id.
        /// </summary>
        /// <param name="id">The zone id.</param>
        /// <returns>The matching zone, or null if not found.</returns>
        public Zone? GetZone(ulong id) => _cache.Zones.FirstOrDefault(x => x.Id == id);
        
        /// <summary>
        /// Gets all nodes for a zone by its name.
        /// </summary>
        /// <param name="zoneName">The zone name.</param>
        /// <returns>A read-only list of nodes, or null if the zone was not found.</returns>
        public IReadOnlyList<Node>? GetNodes(string zoneName)
        {
            Zone? zone = GetZone(zoneName);
            return zone == null ? null : GetNodes(zone.Id);
        }

        /// <summary>
        /// Gets all nodes for a zone by its id.
        /// </summary>
        /// <param name="zoneId">The zone id.</param>
        /// <returns>A read-only list of nodes, or null if no nodes exist.</returns>
        public IReadOnlyList<Node>? GetNodes(ulong zoneId) =>
            _cache.Nodes.TryGetValue(zoneId, out List<Node> nodes) ? new List<Node>(nodes) : null;

        /// <summary>
        /// Gets a specific node within a zone.
        /// </summary>
        /// <param name="zoneId">The zone id.</param>
        /// <param name="nodeId">The node id.</param>
        /// <returns>The matching node, or null if not found.</returns>
        public Node? GetNode(ulong zoneId, ulong nodeId)
        {
            var nodes = GetNodes(zoneId);
            return nodes?.FirstOrDefault(x => x.Id == nodeId);
        }
        
        /// <summary>
        /// Gets all zone-flag associations for a zone by its name.
        /// </summary>
        /// <param name="zoneName">The zone name.</param>
        /// <returns>A read-only list of zone flags, or null if the zone was not found.</returns>
        public IReadOnlyList<ZoneFlag>? GetZoneFlags(string zoneName)
        {
            Zone? zone = GetZone(zoneName);
            return zone == null ? null : GetZoneFlags(zone.Id);
        }

        /// <summary>
        /// Gets all zone-flag associations for a zone by its id.
        /// </summary>
        /// <param name="zoneId">The zone id.</param>
        /// <returns>A read-only list of zone flags, or null if none exist.</returns>
        public IReadOnlyList<ZoneFlag>? GetZoneFlags(ulong zoneId) =>
            _cache.ZoneFlags.TryGetValue(zoneId, out List<ZoneFlag> flags) ? new List<ZoneFlag>(flags) : null;

        /// <summary>
        /// Gets a specific zone-flag association.
        /// </summary>
        /// <param name="zoneId">The zone id.</param>
        /// <param name="flagId">The flag id.</param>
        /// <returns>The matching zone flag, or null if not found.</returns>
        public ZoneFlag? GetZoneFlag(ulong zoneId, ulong flagId)
        {
            var zoneFlags = GetZoneFlags(zoneId);
            return zoneFlags?.FirstOrDefault(x => x.FlagId == flagId);
        }
        
        /// <summary>
        /// Gets all restrictions for a zone by its name.
        /// </summary>
        /// <param name="zoneName">The zone name.</param>
        /// <returns>A read-only list of restrictions, or null if the zone was not found.</returns>
        public IReadOnlyList<Restriction>? GetZoneBlocks(string zoneName)
        {
            Zone? zone = GetZone(zoneName);
            return zone == null ? null : GetZoneBlocks(zone.Id);
        }

        /// <summary>
        /// Gets all restrictions for a zone by its id.
        /// </summary>
        /// <param name="zoneId">The zone id.</param>
        /// <returns>A read-only list of restrictions, or null if none exist.</returns>
        public IReadOnlyList<Restriction>? GetZoneBlocks(ulong zoneId) =>
            _cache.ZoneBlocks.TryGetValue(zoneId,  out List<Restriction> blocks) ? new List<Restriction>(blocks) : null;

        /// <summary>
        /// Gets all events for a zone by its name.
        /// </summary>
        /// <param name="zoneName">The zone name.</param>
        /// <returns>A read-only list of zone events, or null if the zone was not found.</returns>
        public IReadOnlyList<ZoneEvent>? GetZoneEvents(string zoneName)
        {
            Zone? zone = GetZone(zoneName);
            return zone == null ? null : GetZoneEvents(zone.Id);
        }

        /// <summary>
        /// Gets all events for a zone by its id.
        /// </summary>
        /// <param name="zoneId">The zone id.</param>
        /// <returns>A read-only list of zone events, or null if none exist.</returns>
        public IReadOnlyList<ZoneEvent>? GetZoneEvents(ulong zoneId) =>
            _cache.ZoneEvents.TryGetValue(zoneId, out  List<ZoneEvent> events) ? new List<ZoneEvent>(events) : null;

        /// <summary>
        /// Gets a specific event from a zone by its type.
        /// </summary>
        /// <param name="zone">The zone to search.</param>
        /// <param name="eventType">The event type to find.</param>
        /// <returns>The matching zone event, or null if not found.</returns>
        public ZoneEvent? GetZoneEvent(Zone zone, EEventType eventType) => GetZoneEvent(zone.Id, eventType);

        /// <summary>
        /// Gets a specific event from a zone by its id and type.
        /// </summary>
        /// <param name="zoneId">The zone id.</param>
        /// <param name="eventType">The event type to find.</param>
        /// <returns>The matching zone event, or null if not found.</returns>
        public ZoneEvent? GetZoneEvent(ulong zoneId, EEventType eventType)
        {
            var events = GetZoneEvents(zoneId);
            return events?.FirstOrDefault(x => x.Type == eventType);
        }

        /// <summary>
        /// Gets a specific restriction from a zone by type and asset id.
        /// </summary>
        /// <param name="zoneId">The zone id.</param>
        /// <param name="type">The restriction type.</param>
        /// <param name="unturnedId">The Unturned asset id.</param>
        /// <returns>The matching restriction, or null if not found.</returns>
        public Restriction? GetZoneBlock(ulong zoneId, ERestrictionType type, ushort unturnedId)
        {
            var blocks =  GetZoneBlocks(zoneId);
            return blocks?.FirstOrDefault(x => x.Type == type && x.UnturnedId == unturnedId) ?? null;
        }

        /// <summary>
        /// Checks whether a specific item or vehicle is restricted in a zone.
        /// </summary>
        /// <param name="zone">The zone to check.</param>
        /// <param name="unturnedId">The Unturned asset id.</param>
        /// <param name="restrictionType">The restriction type.</param>
        /// <returns>True if the asset is restricted in the zone.</returns>
        public bool IsBlocked(Zone zone, ushort unturnedId, ERestrictionType restrictionType) => 
            IsBlocked(zone.Id, unturnedId, restrictionType);

        /// <summary>
        /// Checks whether a specific item or vehicle is restricted in a zone.
        /// </summary>
        /// <param name="zoneId">The zone id.</param>
        /// <param name="unturnedId">The Unturned asset id.</param>
        /// <param name="restrictionType">The restriction type.</param>
        /// <returns>True if the asset is restricted in the zone.</returns>
        public bool IsBlocked(ulong zoneId, ushort unturnedId, ERestrictionType restrictionType) 
        {
            var blocks = GetZoneBlocks(zoneId);
            return blocks != null && blocks.Any(x => x.UnturnedId == unturnedId && x.Type == restrictionType);
        }
    }
}