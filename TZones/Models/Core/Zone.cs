using System;
using Tavstal.TLibrary.Compatibility.Database;

namespace Tavstal.TZones.Models.Core
{
    [Serializable]
    public class Zone
    {
        [SqlMember(isPrimaryKey: true, isUnsigned: true, shouldAutoIncrement: true)]
        public ulong Id { get; set; }
        [SqlMember(columnType: "varchar(32)")]
        public string Name { get; set; }
        [SqlMember(columnType: "varchar(128)", isNullable: true)]
        public string Description { get; set; }
        [SqlMember(isUnsigned: true)]
        public ulong CreatorId { get; set; }
        [SqlMember]
        public DateTime CreationDate { get; set; }

        public Zone() { }

        public Zone(ulong id, string name, string description, ulong creatorId, DateTime creationDate)
        {
            Id = id;
            Name = name;
            Description = description;
            CreatorId = creatorId;
            CreationDate = creationDate;
        }

        public Zone(string name, string description, ulong creatorId, DateTime creationDate)
        {
            Id = 0;
            Name = name;
            Description = description;
            CreatorId = creatorId;
            CreationDate = creationDate;
        }
    }
}