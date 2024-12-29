using System;
using System.Collections.Generic;
using Rocket.Unturned.Player;
using Tavstal.TZones.Models.Core;
using UnityEngine;

namespace Tavstal.TZones.Components
{
    public class ZonePlayerComponent : UnturnedPlayerComponent
    {
        public HashSet<ulong> Zones { get; set; }
        public Vector3 LastPosition { get; set; }
        public DateTime SpamPreventEnd { get; set; }

        protected override void Load()
        {
            base.Load();
            Zones = new HashSet<ulong>();
            LastPosition = Player.Position;
            SpamPreventEnd = DateTime.Now;
        }
    }
}