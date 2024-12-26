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
    /// <summary>
    /// A static class responsible for handling events related to zones, such as player interactions, zone creation, updates, and deletions.
    /// </summary>
    /// <remarks>
    /// This class contains methods for attaching and detaching events, as well as handling specific actions within zones. It manages interactions like entering or leaving zones, zone updates, and the creation or deletion of zones.
    /// </remarks>
    public static class ZonesEventHandler
    {
        private static bool _isAttached;

        /// <summary>
        /// Attaches event handlers for zone-related events, enabling the handling of player interactions and other zone actions.
        /// </summary>
        /// <remarks>
        /// This method subscribes to events such as players entering or leaving zones, zone creation, update, and deletion, allowing the system to respond to changes and actions within zones.
        /// </remarks>
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

        /// <summary>
        /// Detaches event handlers for zone-related events, disabling the handling of player interactions and other zone actions.
        /// </summary>
        /// <remarks>
        /// This method unsubscribes from events such as players entering or leaving zones, zone creation, update, and deletion, effectively stopping the system from responding to changes and actions within zones.
        /// </remarks>
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

        /// <summary>
        /// Handles the event when a player enters a zone, allowing for modifications to the entry conditions.
        /// </summary>
        /// <param name="player">The player who is entering the zone.</param>
        /// <param name="zone">The zone that the player is entering.</param>
        /// <param name="lastPosition">The player's last known position before entering the zone.</param>
        /// <param name="shouldAllow">A flag indicating whether the player should be allowed to enter the zone. Set to false to prevent entry.</param>
        /// <remarks>
        /// This method allows modification of the player zone entry process. It can be used to block a player from entering the zone based on specific conditions, such as permissions or other criteria.
        /// </remarks>
        private static void OnPlayerEnterZone(UnturnedPlayer player, Zone zone, Vector3 lastPosition, ref bool shouldAllow)
        {
            if (!ZonesManager.ZoneEvents.TryGetValue(zone.Id, out var events))
                return;

            if (zone.HasFlag(Flags.Enter))
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

        /// <summary>
        /// Handles the event when a player leaves a zone, allowing for modifications to the exit conditions.
        /// </summary>
        /// <param name="player">The player who is leaving the zone.</param>
        /// <param name="zone">The zone the player is leaving.</param>
        /// <param name="lastPosition">The player's last known position before leaving the zone.</param>
        /// <param name="shouldAllow">A flag indicating whether the player should be allowed to leave the zone. Set to false to prevent exit.</param>
        /// <remarks>
        /// This method allows modification of the player zone exit process. It can be used to block or modify the conditions under which a player leaves the zone.
        /// </remarks>
        private static void OnPlayerLeaveZone(UnturnedPlayer player, Zone zone, Vector3 lastPosition, ref bool shouldAllow)
        {
            if (!ZonesManager.ZoneEvents.TryGetValue(zone.Id, out var events))
                return;

            if (zone.HasFlag(Flags.Leave))
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

        /// <summary>
        /// Handles the event when a zone is created.
        /// </summary>
        /// <param name="zone">The zone that has been created.</param>
        /// <remarks>
        /// This method is triggered when a new zone is created in the system. It can be used to perform any necessary actions or initialize properties when a zone is created.
        /// </remarks>
        private static void OnZoneCreated(Zone zone)
        {
            
        }

        /// <summary>
        /// Handles the event when a zone is updated.
        /// </summary>
        /// <param name="zone">The zone that has been updated.</param>
        /// <remarks>
        /// This method is triggered when an existing zone is updated. It can be used to perform any necessary actions or adjustments when a zone's properties are changed.
        /// </remarks>
        private static void OnZoneUpdated(Zone zone)
        {
            
        }

        /// <summary>
        /// Handles the event when a zone is deleted.
        /// </summary>
        /// <param name="zone">The zone that has been deleted.</param>
        /// <remarks>
        /// This method is triggered when a zone is deleted from the system. It can be used to perform cleanup or other necessary actions when a zone is removed.
        /// </remarks>
        private static void OnZoneDeleted(Zone zone)
        {
            
        }
    }
}