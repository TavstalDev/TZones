using System;
using Tavstal.TLibrary.Models.Database.Attributes;

namespace Tavstal.TZones.Models.Core
{
    [Serializable]
    public class Flag
    {
        [SqlMember(isPrimaryKey: true, isUnsigned: true, shouldAutoIncrement: true)]
        public ulong Id { get; set; }
        [SqlMember(columnType: "varchar(32)")]
        public string Name { get; set; }
        [SqlMember(columnType: "varchar(128)", isNullable: true)]
        public string Description { get; set; }
        [SqlMember(columnType: "varchar(32)")]
        public string FlagRegister { get; set; }

        public Flag() {}

        public Flag(ulong id, string name, string description, string flagRegister)
        {
            Id = id;
            Name = name;
            Description = description;
            FlagRegister = flagRegister;
        }

        public Flag(string name, string description, string flagRegister)
        {
            Id = 0;
            Name = name;
            Description = description;
            FlagRegister = flagRegister;
        }
    }
}