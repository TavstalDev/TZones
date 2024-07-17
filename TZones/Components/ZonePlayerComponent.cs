using System.Collections.Generic;
using Rocket.Unturned.Player;

namespace Tavstal.TZones.Components
{
    public class ZonePlayerComponent : UnturnedPlayerComponent
    {
        public List<ulong> Zones { get; set; }
    }
}