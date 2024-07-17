using System;
using Tavstal.TLibrary.Compatibility;
using Tavstal.TLibrary.Compatibility.Database;
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
        public bool IsUpper { get; set; }

        public Node() {}

        public Node(ulong zoneId, float x, float y, float z, bool isUpper)
        {
            ZoneId = zoneId;
            X = x;
            Y = y;
            Z = z;
            IsUpper = isUpper;
        }

        public Node(ulong zoneId, Vector3 position, bool isUpper)
        {
            ZoneId = zoneId;
            X = position.x;
            Y = position.y;
            Z = position.z;
            IsUpper = isUpper;
        }
    }
}