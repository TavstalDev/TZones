using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.Unturned.Player;
using Tavstal.TZones.Utils.Managers;
using UnityEngine;

namespace Tavstal.TZones.Components
{
    /// <summary>
    /// MonoBehaviour component attached to each player, tracking their current zone membership and position state.
    /// </summary>
    public class ZoneComponent : UnturnedPlayerComponent
    {
        /// <summary>
        /// The set of zone ids the player is currently inside.
        /// </summary>
        public HashSet<ulong> Zones { get; set; } = new  HashSet<ulong>();

        /// <summary>
        /// The player's last recorded position, used for enter/leave transition checks.
        /// </summary>
        public Vector3 LastPosition { get; set; }

        /// <summary>
        /// The timestamp after which spam prevention expires for zone entry/exit messages.
        /// </summary>
        public DateTime SpamPreventEnd { get; set; }

        /// <summary>
        /// Initializes the component state when the player loads.
        /// </summary>
        protected override void Load()
        {
            base.Load();
            Zones = new HashSet<ulong>();
            LastPosition = Player.Position;
            SpamPreventEnd = DateTime.Now;
        }

        public bool HasFlag(string flagName, bool checkGlobal = true)
        {
            if (Zones.Any(x => ZoneManager.Queries.HasFlag(x, flagName)))
                return true;
            
            if (!checkGlobal)
                return false;

            var globalZone = ZoneManager.Queries.GetZone("__global__");
        }
    }
}