using System;
using Tavstal.TLibrary.Compatibility;
using Tavstal.TLibrary.Compatibility.Database;
using Tavstal.TZones.Models.Enums;
using UnityEngine;

namespace Tavstal.TZones.Models.Core
{
    [Serializable]
    public class Node : SerializableVector3
    {
        [SqlMember(isUnsigned: true)]
        public ulong ZoneId { get; set; }
        [SqlMember]
        public new float X { get; set; }
        [SqlMember]
        public new float Y { get; set; }
        [SqlMember]
        public new float Z { get; set; }
        [SqlMember]
        public ENodeType Type { get; set; }

        public Node() {}

        public Node(ulong zoneId, float x, float y, float z, ENodeType type)
        {
            ZoneId = zoneId;
            X = x;
            Y = y;
            Z = z;
            Type = type;
        }

        public Node(ulong zoneId, Vector3 position, ENodeType type)
        {
            ZoneId = zoneId;
            X = position.x;
            Y = position.y;
            Z = position.z;
            Type = type;
        }
    }
}