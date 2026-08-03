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

namespace Tavstal.TZones.Handlers
{
    /// <summary>
    /// A static class responsible for handling events related to zones, such as player interactions, zone creation, updates, and deletions.
    /// </summary>
    public static class ZonesEventHandler
    {
        private static bool _isAttached;

        /// <summary>
        /// Attaches event handlers for zone-related events, enabling the handling of player interactions and other zone actions.
        /// </summary>
        public static void AttachEvents()
        {
            if (_isAttached)
                return;

            _isAttached = true;

            ZoneManager.OnPlayerEnterZone += OnPlayerEnterZone;
            ZoneManager.OnPlayerLeaveZone += OnPlayerLeaveZone;
            ZoneManager.OnZoneCreated += OnZoneCreated;
            ZoneManager.OnZoneUpdated += OnZoneUpdated;
            ZoneManager.OnZoneDeleted += OnZoneDeleted;
        }

        /// <summary>
        /// Detaches event handlers for zone-related events, disabling the handling of player interactions and other zone actions.
        /// </summary>
        public static void DetachEvents()
        {
            if (!_isAttached)
                return;

            _isAttached = false;

            ZoneManager.OnPlayerEnterZone -= OnPlayerEnterZone;
            ZoneManager.OnPlayerLeaveZone -= OnPlayerLeaveZone;
            ZoneManager.OnZoneCreated -= OnZoneCreated;
            ZoneManager.OnZoneUpdated -= OnZoneUpdated;
            ZoneManager.OnZoneDeleted -= OnZoneDeleted;
        }

        /// <summary>
        /// Handles the event when a player enters a zone, allowing for modifications to the entry conditions.
        /// </summary>
        /// <param name="player">The player who is entering the zone.</param>
        /// <param name="zone">The zone that the player is entering.</param>
        /// <param name="lastPosition">The player's last known position before entering the zone.</param>
        /// <param name="shouldAllow">A flag indicating whether the player should be allowed to enter the zone. Set to 'false' to prevent entry.</param>
        private static void OnPlayerEnterZone(UnturnedPlayer player, Zone zone, Vector3 lastPosition, ref bool shouldAllow)
        {
            var events = ZoneManager.Queries.GetZoneEvents(zone.Id);
            if (events == null)
                return;

            if (ZoneManager.Queries.HasFlag(zone, Flags.NoEnter))
            {
                shouldAllow = false;
                ZoneComponent comp = ComponentManager.Get(player);
                
                if (player.IsInVehicle)
                    player.CurrentVehicle.forceRemovePlayer(out _, player.CSteamID, out _, out _);

                player.Teleport(new Vector3(lastPosition.x, lastPosition.y, lastPosition.z), player.Rotation);

                if (comp.SpamPreventEnd < DateTime.Now)
                {
                    TZones.Instance.SendCommandReply(player, "warn_zone_noenter", TZones.Instance.Config.General.MessageIcon, zone.Name);
                    comp.SpamPreventEnd = DateTime.Now.AddSeconds(5);
                }
                return;
            }

            foreach (ZoneEvent zEvent in events)
            {
                switch (zEvent.Type)
                {
                    case EEventType.ADD_EFFECT_ENTER:
                    {
                        if (ushort.TryParse(zEvent.Value, out ushort effect))
                        {
                            player.TriggerEffect(effect);
                        }

                        break;
                    }
                    case EEventType.REMOVE_EFFECT_ENTER:
                    {
                        if (ushort.TryParse(zEvent.Value, out ushort effect))
                        {
                            EffectManager.askEffectClearByID(effect, player.SteamPlayer().transportConnection);
                        }

                        break;
                    }
                    case EEventType.ADD_GROUP_ENTER:
                    {
                        R.Permissions.AddPlayerToGroup(zEvent.Value, player);
                        break;
                    }
                    case EEventType.REMOVE_GROUP_ENTER:
                    {
                        R.Permissions.RemovePlayerFromGroup(zEvent.Value, player);
                        break;
                    }
                    case EEventType.MESSAGE_ENTER:
                    {
                        TZones.Instance.SendPlainCommandReply(player, zEvent.Value);
                        break;
                    }
                    case EEventType.MESSAGE_LEAVE:
                    case EEventType.ADD_GROUP_LEAVE:
                    case EEventType.REMOVE_GROUP_LEAVE:
                    case EEventType.ADD_EFFECT_LEAVE:
                    case EEventType.REMOVE_EFFECT_LEAVE:
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Handles the event when a player leaves a zone, allowing for modifications to the exit conditions.
        /// </summary>
        /// <param name="player">The player who is leaving the zone.</param>
        /// <param name="zone">The zone the player is leaving.</param>
        /// <param name="lastPosition">The player's last known position before leaving the zone.</param>
        /// <param name="shouldAllow">A flag indicating whether the player should be allowed to leave the zone. Set to false to prevent exit.</param>
        private static void OnPlayerLeaveZone(UnturnedPlayer player, Zone zone, Vector3 lastPosition, ref bool shouldAllow)
        {
            var events = ZoneManager.Queries.GetZoneEvents(zone.Id);
            if (events == null)
                return;

            if (ZoneManager.Queries.HasFlag(zone, Flags.NoLeave))
            {
                shouldAllow = false;
                ZoneComponent comp = ComponentManager.Get(player);

                if (player.IsInVehicle)
                    player.CurrentVehicle.forceRemovePlayer(out _, player.CSteamID, out _, out _);

                player.Teleport(new Vector3(lastPosition.x, lastPosition.y, lastPosition.z), player.Rotation);
                
                if (comp.SpamPreventEnd < DateTime.Now)
                {
                    TZones.Instance.SendCommandReply(player, "warn_zone_noleave", TZones.Instance.Config.General.MessageIcon, zone.Name);
                    comp.SpamPreventEnd = DateTime.Now.AddSeconds(5);
                }
                return;
            }

            foreach (ZoneEvent zEvent in events)
            {
                switch (zEvent.Type)
                {
                    case EEventType.ADD_EFFECT_LEAVE:
                    {
                        if (ushort.TryParse(zEvent.Value, out ushort effect))
                        {
                            player.TriggerEffect(effect);
                        }

                        break;
                    }
                    case EEventType.ADD_GROUP_LEAVE:
                    {
                        R.Permissions.AddPlayerToGroup(zEvent.Value, player);
                        break;
                    }
                    case EEventType.MESSAGE_LEAVE:
                    {
                        TZones.Instance.SendPlainCommandReply(player, zEvent.Value);
                        break;
                    }
                    case EEventType.REMOVE_EFFECT_LEAVE:
                    {
                        if (ushort.TryParse(zEvent.Value, out ushort effect))
                        {
                            EffectManager.askEffectClearByID(effect, player.SteamPlayer().transportConnection);
                        }

                        break;
                    }
                    case EEventType.REMOVE_GROUP_LEAVE:
                    {
                        R.Permissions.RemovePlayerFromGroup(zEvent.Value, player);
                        break;
                    }
                    case EEventType.MESSAGE_ENTER:
                    case EEventType.ADD_GROUP_ENTER:
                    case EEventType.REMOVE_GROUP_ENTER:
                    case EEventType.ADD_EFFECT_ENTER:
                    case EEventType.REMOVE_EFFECT_ENTER:
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Handles the event when a zone is created.
        /// </summary>
        /// <param name="zone">The zone that has been created.</param>
        private static void OnZoneCreated(Zone zone)
        {
            
        }

        /// <summary>
        /// Handles the event when a zone is updated.
        /// </summary>
        /// <param name="zone">The zone that has been updated.</param>
        private static void OnZoneUpdated(Zone zone)
        {
            
        }

        /// <summary>
        /// Handles the event when a zone is deleted.
        /// </summary>
        /// <param name="zone">The zone that has been deleted.</param>
        private static void OnZoneDeleted(Zone zone)
        {
            
        }
    }
}