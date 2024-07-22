using System.Collections.Generic;
using Rocket.Unturned.Player;
using UnityEngine;

namespace Tavstal.TZones.Components
{
    public class ZonePlayerComponent : UnturnedPlayerComponent
    {
        public List<ulong> Zones { get; set; }
        public Vector3 LastPosition { get; set; }
    }
}