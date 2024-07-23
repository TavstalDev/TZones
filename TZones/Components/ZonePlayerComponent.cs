using System.Collections.Generic;
using Rocket.Unturned.Player;
using Tavstal.TZones.Models.Core;
using UnityEngine;

namespace Tavstal.TZones.Components
{
    public class ZonePlayerComponent : UnturnedPlayerComponent
    {
        public List<Zone> Zones { get; set; }
        public Vector3 LastPosition { get; set; }
    }
}