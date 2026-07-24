using Tavstal.TLibrary.Models.Database.Attributes;

namespace Tavstal.TZones.Models.Core
{
    /// <summary>
    /// Represents a named flag that can be assigned to zones to control behavior.
    /// </summary>
    [SqlName("flags")]
    public class Flag
    {
        /// <summary>
        /// The unique identifier of the flag.
        /// </summary>
        [SqlMember(isPrimaryKey: true, isUnsigned: true, shouldAutoIncrement: true)]
        public ulong Id { get; set; }
        
        /// <summary>
        /// The unique name of the flag (e.g. "NoDamage").
        /// </summary>
        [SqlMember(columnType: "varchar(32)")]
        public string Name { get; set; }
        
        /// <summary>
        /// A human-readable description of what the flag does.
        /// </summary>
        [SqlMember(columnType: "varchar(128)", isNullable: true)]
        public string Description { get; set; }
        
        /// <summary>
        /// The identifier of the plugin or entity that registered this flag.
        /// </summary>
        [SqlMember(columnType: "varchar(32)")]
        public string FlagRegister { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Flag"/> class with default values.
        /// </summary>
        public Flag()
        {
            Name = string.Empty;
            Description = string.Empty;
            FlagRegister = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Flag"/> class with all values specified.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="name">The flag name.</param>
        /// <param name="description">The flag description.</param>
        /// <param name="flagRegister">The identifier of the registering entity.</param>
        public Flag(ulong id, string name, string description, string flagRegister)
        {
            Id = id;
            Name = name;
            Description = description;
            FlagRegister = flagRegister;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Flag"/> class without an id (for new entries).
        /// </summary>
        /// <param name="name">The flag name.</param>
        /// <param name="description">The flag description.</param>
        /// <param name="flagRegister">The identifier of the registering entity.</param>
        public Flag(string name, string description, string flagRegister)
        {
            Id = 0;
            Name = name;
            Description = description;
            FlagRegister = flagRegister;
        }
    }
}