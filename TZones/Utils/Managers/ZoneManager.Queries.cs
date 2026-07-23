using System.Collections.Generic;
using System.Linq;
using Tavstal.TZones.Models.Core;
using Tavstal.TZones.Models.Enums;

namespace Tavstal.TZones.Utils.Managers
{
    public class ZonesManager_Queries
    {
        private readonly ZonesManager_Cache _cache;

        public ZonesManager_Queries(ZonesManager_Cache cache)
        {
            _cache = cache;
        }
        
        public bool HasFlag(Zone zone, string flagName) => HasFlag(zone.Id, flagName);
        
        public bool HasFlag(ulong zoneId, string flagName) 
        {
            Flag? flag = GetFlag(flagName);
            if (flag == null) 
                return false;
            var zoneflags = GetZoneFlags(zoneId);
            return zoneflags?.Any(x => x.FlagId == flag.Id) ?? false;
        }
        
        public Flag? GetFlag(string flagName) => _cache.Flags.FirstOrDefault(x => x.Name == flagName);
        
        public Flag? GetFlag(ulong flagId) => _cache.Flags.FirstOrDefault(x => x.Id  == flagId);
        
        public Zone? GetZone(string name) => _cache.Zones.FirstOrDefault(x => x.Name == name);
        
        public Zone? GetZone(ulong id) => _cache.Zones.FirstOrDefault(x => x.Id == id);
        
        public IReadOnlyList<Node>? GetNodes(string zoneName)
        {
            Zone? zone = GetZone(zoneName);
            return zone == null ? null : GetNodes(zone.Id);
        }
        
        public IReadOnlyList<Node>? GetNodes(ulong zoneId) =>
            _cache.Nodes.TryGetValue(zoneId, out List<Node> nodes) ? new List<Node>(nodes) : null;

        public Node? GetNode(ulong zoneId, ulong nodeId)
        {
            var nodes = GetNodes(zoneId);
            return nodes?.FirstOrDefault(x => x.Id == nodeId);
        }
        
        public IReadOnlyList<ZoneFlag>? GetZoneFlags(string zoneName)
        {
            Zone? zone = GetZone(zoneName);
            return zone == null ? null : GetZoneFlags(zone.Id);
        }
        
        public IReadOnlyList<ZoneFlag>? GetZoneFlags(ulong zoneId) =>
            _cache.ZoneFlags.TryGetValue(zoneId, out List<ZoneFlag> flags) ? new List<ZoneFlag>(flags) : null;

        public ZoneFlag? GetZoneFlag(ulong zoneId, ulong flagId)
        {
            var zoneFlags = GetZoneFlags(zoneId);
            return zoneFlags?.FirstOrDefault(x => x.FlagId == flagId);
        }
        
        public IReadOnlyList<Restriction>? GetZoneBlocks(string zoneName)
        {
            Zone? zone = GetZone(zoneName);
            return zone == null ? null : GetZoneBlocks(zone.Id);
        }
        
        public IReadOnlyList<Restriction>? GetZoneBlocks(ulong zoneId) =>
            _cache.ZoneBlocks.TryGetValue(zoneId,  out List<Restriction> blocks) ? new List<Restriction>(blocks) : null;

        public IReadOnlyList<ZoneEvent>? GetZoneEvents(string zoneName)
        {
            Zone? zone = GetZone(zoneName);
            return zone == null ? null : GetZoneEvents(zone.Id);
        }
        
        public IReadOnlyList<ZoneEvent>? GetZoneEvents(ulong zoneId) =>
            _cache.ZoneEvents.TryGetValue(zoneId, out  List<ZoneEvent> events) ? new List<ZoneEvent>(events) : null;
        
        public ZoneEvent? GetZoneEvent(Zone zone, EEventType eventType) => GetZoneEvent(zone.Id, eventType);

        public ZoneEvent? GetZoneEvent(ulong zoneId, EEventType eventType)
        {
            var events = GetZoneEvents(zoneId);
            return events?.FirstOrDefault(x => x.Type == eventType);
        }

        public Restriction? GetZoneBlock(ulong zoneId, ERestrictionType type, ushort unturnedId)
        {
            var blocks =  GetZoneBlocks(zoneId);
            return blocks?.FirstOrDefault(x => x.Type == type && x.UnturnedId == unturnedId) ?? null;
        }
        
        public bool IsBlocked(Zone zone, ushort unturnedId, ERestrictionType restrictionType) => 
            IsBlocked(zone.Id, unturnedId, restrictionType);
        
        public bool IsBlocked(ulong zoneId, ushort unturnedId, ERestrictionType restrictionType) 
        {
            var blocks = GetZoneBlocks(zoneId);
            return blocks != null && blocks.Any(x => x.UnturnedId == unturnedId && x.Type == restrictionType);
        }
    }
}