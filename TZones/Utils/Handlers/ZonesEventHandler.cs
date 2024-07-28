using System;
using System.Collections.Generic;
using Rocket.Unturned.Player;
using Tavstal.TZones.Models.Core;
using Tavstal.TZones.Models.Enums;
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
            ZonesManager.OnZoneCreated += OnZoneCreated;
            ZonesManager.OnZoneUpdated += OnZoneUpdated;
            ZonesManager.OnZoneDeleted += OnZoneDeleted;
        }

        public static void DetachEvents()
        {
            if (!_isAttached)
                return;

            _isAttached = false;

            ZonesManager.OnPlayerEnterZone -= OnPlayerEnterZone;
            ZonesManager.OnPlayerLeaveZone -= OnPlayerLeaveZone;
            ZonesManager.OnZoneCreated -= OnZoneCreated;
            ZonesManager.OnZoneUpdated -= OnZoneUpdated;
            ZonesManager.OnZoneDeleted -= OnZoneDeleted;
        }

        private static void OnPlayerEnterZone(UnturnedPlayer player, Zone zone)
        {
            if (ZonesManager.ZoneEvents.TryGetValue(zone.Id, out List<ZoneEvent> events)) {
                foreach (ZoneEvent zEvent in events) {

                    switch (zEvent.Type) {
                        case EEventType.EnterAddEffect: 
                        {

                            break;
                        }
                        case EEventType.EnterRemoveEffect: 
                        {

                            break;
                        }
                        case EEventType.EnterAddGroup: 
                        {

                            break;
                        }
                        case EEventType.EnterRemoveGroup: 
                        {

                            break;
                        }
                        case EEventType.EnterMessage: 
                        {

                            break;
                        }
                    }
                }
            }
        }

        private static void OnPlayerLeaveZone(UnturnedPlayer player, Zone zone)
        {
            if (ZonesManager.ZoneEvents.TryGetValue(zone.Id, out List<ZoneEvent> events)) {
                foreach (ZoneEvent zEvent in events) {

                    switch (zEvent.Type) {
                        case EEventType.LeaveAddEffect: 
                        {

                            break;
                        }
                        case EEventType.LeaveAddGroup: 
                        {

                            break;
                        }
                        case EEventType.LeaveMessage: 
                        {

                            break;
                        }
                        case EEventType.LeaveRemoveEffect: 
                        {

                            break;
                        }
                        case EEventType.LeaveRemoveGroup: 
                        {

                            break;
                        }
                    }
                }
            }
        }

        private static void OnZoneCreated(Zone zone)
        {
            
        }

        private static void OnZoneUpdated(Zone zone)
        {
            
        }

        private static void OnZoneDeleted(Zone zone)
        {
            
        }

    }
}