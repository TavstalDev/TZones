using System;
using Rocket.Unturned.Player;
using Tavstal.TZones.Models.Core;
using Tavstal.TZones.Utils.Managers;

namespace Tavstal.TZones.Utils.Handlers
{
    public static class ZonesEventHandler
    {
        private static bool _isAttached = false;


        public static void AttachEvents()
        {
            if (_isAttached)
                return;

            _isAttached = true;

            ZonesManager.OnPlayerEnterZone += OnPlayerEnterZone;
            ZonesManager.OnPlayerLeaveZone += OnPlayerLeaveZone;
            ZonesManager.OnZoneUpdated += OnZoneUpdated;
        }

        public static void DetachEvents()
        {
            if (!_isAttached)
                return;

            _isAttached = false;

            ZonesManager.OnPlayerEnterZone -= OnPlayerEnterZone;
            ZonesManager.OnPlayerLeaveZone -= OnPlayerLeaveZone;
            ZonesManager.OnZoneUpdated -= OnZoneUpdated;
        }

        private static void OnPlayerEnterZone(UnturnedPlayer player, Zone zone)
        {
            
        }

        private static void OnPlayerLeaveZone(UnturnedPlayer player, Zone zone)
        {
            
        }

        private static void OnZoneUpdated(Zone zone)
        {
            
        }
    }
}