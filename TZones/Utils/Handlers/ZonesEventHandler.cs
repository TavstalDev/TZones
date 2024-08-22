using Rocket.Core;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.TLibrary.Helpers.Unturned;
using Tavstal.TZones.Models.Core;
using Tavstal.TZones.Models.Enums;
using Tavstal.TZones.Utils.Constants;
using Tavstal.TZones.Utils.Managers;
using UnityEngine;

namespace Tavstal.TZones.Utils.Handlers
{
    public static class ZonesEventHandler
    {
        private static bool _isAttached;

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

        private static void OnPlayerEnterZone(UnturnedPlayer player, Zone zone, Vector3 lastPosition)
        {
            if (!ZonesManager.ZoneEvents.TryGetValue(zone.Id, out var events))
                return;

            if (zone.HasFlag(Flags.NoEnter))
            {
                player.Teleport(lastPosition, player.Rotation);
                TZones.Instance.SendChatMessage("warn_zone_noenter", player.SteamPlayer());
            }

            foreach (ZoneEvent zEvent in events)
            {
                switch (zEvent.Type)
                {
                    case EEventType.EnterAddEffect:
                    {
                        if (ushort.TryParse(zEvent.Value, out ushort effect))
                        {
                            player.TriggerEffect(effect);
                        }

                        break;
                    }
                    case EEventType.EnterRemoveEffect:
                    {
                        if (ushort.TryParse(zEvent.Value, out ushort effect))
                        {
                            EffectManager.askEffectClearByID(effect, player.SteamPlayer().transportConnection);
                        }

                        break;
                    }
                    case EEventType.EnterAddGroup:
                    {
                        R.Permissions.AddPlayerToGroup(zEvent.Value, player);
                        break;
                    }
                    case EEventType.EnterRemoveGroup:
                    {
                        R.Permissions.RemovePlayerFromGroup(zEvent.Value, player);
                        break;
                    }
                    case EEventType.EnterMessage:
                    {
                        UChatHelper.SendPlainChatMessage(player.SteamPlayer(), zEvent.Value);
                        break;
                    }
                    case EEventType.LeaveMessage:
                    case EEventType.LeaveAddGroup:
                    case EEventType.LeaveRemoveGroup:
                    case EEventType.LeaveAddEffect:
                    case EEventType.LeaveRemoveEffect:
                    default:
                        break;
                }
            }
        }

        private static void OnPlayerLeaveZone(UnturnedPlayer player, Zone zone, Vector3 lastPosition)
        {
            if (!ZonesManager.ZoneEvents.TryGetValue(zone.Id, out var events))
                return;

            if (zone.HasFlag(Flags.NoLeave))
            {
                player.Teleport(lastPosition, player.Rotation);
                TZones.Instance.SendChatMessage("warn_zone_noleave", player.SteamPlayer());
            }

            foreach (ZoneEvent zEvent in events)
            {
                switch (zEvent.Type)
                {
                    case EEventType.LeaveAddEffect:
                    {
                        if (ushort.TryParse(zEvent.Value, out ushort effect))
                        {
                            player.TriggerEffect(effect);
                        }

                        break;
                    }
                    case EEventType.LeaveAddGroup:
                    {
                        R.Permissions.AddPlayerToGroup(zEvent.Value, player);
                        break;
                    }
                    case EEventType.LeaveMessage:
                    {
                        UChatHelper.SendPlainChatMessage(player.SteamPlayer(), zEvent.Value);
                        break;
                    }
                    case EEventType.LeaveRemoveEffect:
                    {
                        if (ushort.TryParse(zEvent.Value, out ushort effect))
                        {
                            EffectManager.askEffectClearByID(effect, player.SteamPlayer().transportConnection);
                        }

                        break;
                    }
                    case EEventType.LeaveRemoveGroup:
                    {
                        R.Permissions.RemovePlayerFromGroup(zEvent.Value, player);
                        break;
                    }
                    case EEventType.EnterMessage:
                    case EEventType.EnterAddGroup:
                    case EEventType.EnterRemoveGroup:
                    case EEventType.EnterAddEffect:
                    case EEventType.EnterRemoveEffect:
                    default:
                        break;
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