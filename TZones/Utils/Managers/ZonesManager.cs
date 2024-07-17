using System.Collections.Generic;
using Tavstal.TZones.Models.Core;

namespace Tavstal.TZones.Utils.Managers
{
    public class ZonesManager
    {
        // Because the unturned events use 'ref', and the database is async, cache must be used
        private static List<Flag> _flags { get; set; }
        private static List<Zone> _zones { get; set; }
        private static Dictionary<ulong, List<Node>> _nodes { get; set; }
        private static Dictionary<ulong, List<ZoneFlag>> _zoneFlags { get; set; }
        private static Dictionary<ulong, List<ZoneEvent>> _zoneEvents { get; set; }
        private static Dictionary<ulong, List<Block>> _zoneBlocks { get; set; }

        
    }
}