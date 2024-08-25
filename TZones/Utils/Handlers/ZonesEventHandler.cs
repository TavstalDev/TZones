using System;
using Rocket.Core;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.TLibrary.Helpers.Unturned;
using Tavstal.TZones.Components;
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

        private static void OnPlayerEnterZone(UnturnedPlayer player, Zone zone, Vector3 lastPosition, ref bool shouldAllow)
        {
            if (!ZonesManager.ZoneEvents.TryGetValue(zone.Id, out var events))
                return;

            if (zone.HasFlag(Flags.NoEnter))
            {
                shouldAllow = false;
                ZonePlayerComponent comp = player.GetComponent<ZonePlayerComponent>();
                
                if (player.IsInVehicle)
                    player.CurrentVehicle.forceRemovePlayer(out _, player.CSteamID, out _, out _);

                player.Teleport(new Vector3(lastPosition.x, lastPosition.y, lastPosition.z), player.Rotation);

                if (comp.SpamPreventEnd < DateTime.Now)
                {
                    TZones.Instance.SendCommandReply(player, "warn_zone_noenter", zone.Name);
                    comp.SpamPreventEnd = DateTime.Now.AddSeconds(5);
                }
                return;
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
                        TZones.Instance.SendPlainCommandReply(player, zEvent.Value);
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

        private static void OnPlayerLeaveZone(UnturnedPlayer player, Zone zone, Vector3 lastPosition, ref bool shouldAllow)
        {
            if (!ZonesManager.ZoneEvents.TryGetValue(zone.Id, out var events))
                return;

            if (zone.HasFlag(Flags.NoLeave))
            {
                shouldAllow = false;
                ZonePlayerComponent comp = player.GetComponent<ZonePlayerComponent>();

                if (player.IsInVehicle)
                    player.CurrentVehicle.forceRemovePlayer(out _, player.CSteamID, out _, out _);

                player.Teleport(new Vector3(lastPosition.x, lastPosition.y, lastPosition.z), player.Rotation);
                
                if (comp.SpamPreventEnd < DateTime.Now)
                {
                    TZones.Instance.SendCommandReply(player, "warn_zone_noleave", zone.Name);
                    comp.SpamPreventEnd = DateTime.Now.AddSeconds(5);
                }
                return;
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
                        TZones.Instance.SendPlainCommandReply(player, zEvent.Value);
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