using System;
using System.Linq;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Extensions.Unturned;
using Tavstal.TZones.Components;
using Tavstal.TZones.Models.Enums;
using Tavstal.TZones.Utils.Constants;
using Tavstal.TZones.Utils.Managers;

namespace Tavstal.TZones.Handlers
{
    /// <summary>
    /// Handles player-related events such as damage, equip, dequip, and item drop, enforcing zone restrictions.
    /// </summary>
    public static class PlayerEventHandler
    {
        private static bool _isAttached;

        /// <summary>
        /// Subscribes to all player events if not already attached.
        /// </summary>
        public static void AttachEvents()
        {
            if (_isAttached)
                return;

            U.Events.OnPlayerConnected += OnPlayerConnected;
            U.Events.OnPlayerDisconnected += OnPlayerDisconnected;
            DamageTool.damagePlayerRequested += OnPlayerDamageRequested;
            DamageTool.onPlayerAllowedToDamagePlayer += OnPlayerAllowedToDamagePlayer;
            
            _isAttached = true;
        }

        /// <summary>
        /// Unsubscribes from all player events if currently attached.
        /// </summary>
        public static void DetachEvents()
        {
            if (!_isAttached)
                return;
            
            U.Events.OnPlayerConnected -= OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= OnPlayerDisconnected;
            DamageTool.damagePlayerRequested -= OnPlayerDamageRequested;
            DamageTool.onPlayerAllowedToDamagePlayer -= OnPlayerAllowedToDamagePlayer;

            _isAttached = true;
        }
        
        /// <summary>
        /// Registers per-player event handlers when a player connects.
        /// </summary>
        private static void OnPlayerConnected(UnturnedPlayer player)
        {
            player.Inventory.onDropItemRequested += OnDropItemRequested;
            player.Player.equipment.onEquipRequested += OnEquipRequested;
            player.Player.equipment.onDequipRequested += OnDequipRequested;
        }


        /// <summary>
        /// Unregisters per-player event handlers when a player disconnects.
        /// </summary>
        private static void OnPlayerDisconnected(UnturnedPlayer player)
        {
            player.Inventory.onDropItemRequested -= OnDropItemRequested;
            player.Player.equipment.onEquipRequested -= OnEquipRequested;
            player.Player.equipment.onDequipRequested -= OnDequipRequested;
        }
        
        /// <summary>
        /// Handles player damage requests, blocking damage when the zone has the NoPlayerDamage flag.
        /// </summary>
        private static void OnPlayerDamageRequested(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            if (Provider.isPvP && !shouldAllow)
                return;
            
            bool originalValue = shouldAllow;
            try
            {
                ZonePlayerComponent victimComp = parameters.player.GetComponent<ZonePlayerComponent>();
                if (victimComp.Zones.Any(x => ZoneManager.Queries.HasFlag(x, Flags.NoPlayerDamage)))
                {
                    shouldAllow = false;
                    return;
                }
                
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (parameters.killer == null)
                    return;

                UnturnedPlayer targetPlayer = UnturnedPlayer.FromCSteamID(parameters.killer);
                if (targetPlayer == null || !targetPlayer.isOnline())
                    return;

                ZonePlayerComponent targetComp = targetPlayer.GetComponent<ZonePlayerComponent>();
                if (targetComp.Zones.Any(x => ZoneManager.Queries.HasFlag(x, Flags.NoPlayerDamage)))
                    shouldAllow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnPlayerDamageRequested)}.", ex);
                shouldAllow = originalValue;
            }
        }
        
        private static void OnPlayerAllowedToDamagePlayer(Player instigator, Player victim, ref bool isAllowed)
        {
            // Ignore if the server is pvp or the damage is already allowed
            if (Provider.isPvP || isAllowed)
                return;
            
            // Ignore if instigator is in vanish mode
            if (!instigator.movement.canAddSimulationResultsToUpdates)
                return;
            
            // Ignore if they are in the same group
            if (instigator.quests.isMemberOfSameGroupAs(victim))
                return;
            
            bool originalValue = isAllowed;
            try
            {
                ZonePlayerComponent victimComp = victim.GetComponent<ZonePlayerComponent>();
                bool isVictimInPvPZone =
                    victimComp.Zones.Any(x => ZoneManager.Queries.HasFlag(x, Flags.AllowPlayerDamage));

                ZonePlayerComponent killerComp = instigator.GetComponent<ZonePlayerComponent>();
                bool isKillerInPvPZone = 
                    killerComp.Zones.Any(x => ZoneManager.Queries.HasFlag(x, Flags.AllowPlayerDamage));
                if (!isVictimInPvPZone || !isKillerInPvPZone)
                    return;
                
                isAllowed = true;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnPlayerAllowedToDamagePlayer)}.", ex);
                isAllowed = originalValue;
            }
        }
        
        /// <summary>
        /// Handles equip requests, blocking equipping when the zone forbids item equip or the item is restricted.
        /// </summary>
        private static void OnEquipRequested(PlayerEquipment equipment, ItemJar jar, ItemAsset asset, ref bool shouldAllow)
        {
            bool originalValue = shouldAllow;
            try
            {
                ZonePlayerComponent comp = equipment.player.GetComponent<ZonePlayerComponent>();

                foreach (var zone in comp.Zones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.NoItemEquip) ||
                        ZoneManager.Queries.IsBlocked(zone, asset.id, ERestrictionType.EQUP))
                    {
                        shouldAllow = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnEquipRequested)}.", ex);
                shouldAllow = originalValue;
            }
        }
        
        /// <summary>
        /// Handles dequip requests, blocking unequipping when the zone forbids item deequip or the item is restricted.
        /// </summary>
        private static void OnDequipRequested(PlayerEquipment equipment, ref bool shouldAllow)
        {
            bool originalValue = shouldAllow;
            try
            {
                ZonePlayerComponent comp = equipment.player.GetComponent<ZonePlayerComponent>();

                foreach (var zone in comp.Zones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.NoItemUnequip) ||
                        ZoneManager.Queries.IsBlocked(zone, equipment.asset.id, ERestrictionType.UNEQUIP))
                    {
                        shouldAllow = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnDequipRequested)}.", ex);
                shouldAllow = originalValue;
            }
        }
        
        /// <summary>
        /// Handles item drop requests, blocking drops when the zone has the NoItemDrop flag.
        /// </summary>
        private static void OnDropItemRequested(PlayerInventory inventory, Item item, ref bool shouldAllow)
        {
            bool originalValue = shouldAllow;
            try
            {
                ZonePlayerComponent comp = inventory.player.GetComponent<ZonePlayerComponent>();

                foreach (var zone in comp.Zones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.NoItemDrop))
                    {
                        shouldAllow = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnDropItemRequested)}.", ex);
                shouldAllow = originalValue;
            }
        }
    }
}