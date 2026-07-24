using Tavstal.TLibrary.Models;
using Tavstal.TLibrary.Models.Database.Attributes;
using Tavstal.TZones.Models.Enums;
using UnityEngine;

namespace Tavstal.TZones.Models.Core
{
    /// <summary>
    /// Represents a positional node that defines the boundary of a zone.
    /// </summary>
    [SqlName("zones_nodes")]
    public class Node : SerializableVector3
    {
        /// <summary>
        /// The unique identifier of the node.
        /// </summary>
        [SqlMember(isPrimaryKey: true, isUnsigned: true, shouldAutoIncrement: true)]
        public ulong Id { get; set; }
        
        /// <summary>
        /// The identifier of the zone this node belongs to.
        /// </summary>
        [SqlMember(isUnsigned: true)]
        public ulong ZoneId { get; set; }
        
        /// <summary>
        /// The X coordinate of the node position.
        /// </summary>
        [SqlMember(columnType: "float")]
        public new float X { get; set; }
        
        /// <summary>
        /// The Y coordinate of the node position (height).
        /// </summary>
        [SqlMember(columnType: "float")]
        public new float Y { get; set; }
        
        /// <summary>
        /// The Z coordinate of the node position.
        /// </summary>
        [SqlMember(columnType: "float")]
        public new float Z { get; set; }
        
        /// <summary>
        /// The type of node (e.g. boundary point, upper limit, lower limit).
        /// </summary>
        [SqlMember]
        public ENodeType Type { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class with default values.
        /// </summary>
        public Node() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class with individual coordinates.
        /// </summary>
        /// <param name="zoneId">The identifier of the parent zone.</param>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        /// <param name="type">The node type.</param>
        public Node(ulong zoneId, float x, float y, float z, ENodeType type)
        {
            ZoneId = zoneId;
            X = x;
            Y = y;
            Z = z;
            Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class from a <see cref="Vector3"/>.
        /// </summary>
        /// <param name="zoneId">The identifier of the parent zone.</param>
        /// <param name="position">The position vector.</param>
        /// <param name="type">The node type.</param>
        public Node(ulong zoneId, Vector3 position, ENodeType type)
        {
            ZoneId = zoneId;
            X = position.x;
            Y = position.y;
            Z = position.z;
            Type = type;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class with an id and individual coordinates.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="zoneId">The identifier of the parent zone.</param>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        /// <param name="type">The node type.</param>
        public Node(ulong id, ulong zoneId, float x, float y, float z, ENodeType type)
        {
            Id = id;
            ZoneId = zoneId;
            X = x;
            Y = y;
            Z = z;
            Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class with an id and a <see cref="Vector3"/> position.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="zoneId">The identifier of the parent zone.</param>
        /// <param name="position">The position vector.</param>
        /// <param name="type">The node type.</param>
        public Node(ulong id, ulong zoneId, Vector3 position, ENodeType type)
        {
            Id = id;
            ZoneId = zoneId;
            X = position.x;
            Y = position.y;
            Z = position.z;
            Type = type;
        }
    }
}