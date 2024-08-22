using System;
using Tavstal.TLibrary.Models.Database.Attributes;
using Tavstal.TZones.Models.Enums;

namespace Tavstal.TZones.Models.Core
{
    [Serializable]
    public class Block
    {
        [SqlMember(isUnsigned: true)]
        public ulong ZoneId { get; set; }
        [SqlMember(isUnsigned: true)]
        public ushort UnturnedId { get; set; }
        [SqlMember]
        public EBlockType Type { get; set; }

        public Block() {}

        public Block(ulong zoneId, ushort unturnedId, EBlockType type)
        {
            ZoneId = zoneId;
            UnturnedId = unturnedId;
            Type = type;
        }
    }
}