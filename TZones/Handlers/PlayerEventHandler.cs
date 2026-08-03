using System;
using System.Linq;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Extensions.Unturned;
using Tavstal.TZones.Components;
using Tavstal.TZones.Models.Core;
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
                var victimPLayer = UnturnedPlayer.FromPlayer(parameters.player);
                ZoneComponent victimComp = ComponentManager.Get(victimPLayer);
                if (victimComp.Zones.Any(x => ZoneManager.Queries.HasFlag(x, Flags.NoPlayerDamage)))
                {
                    shouldAllow = false;
                    return;
                }

                // TODO
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (parameters.killer != null)
                {
                    UnturnedPlayer killerPlayer = UnturnedPlayer.FromCSteamID(parameters.killer);
                    if (killerPlayer != null && killerPlayer.isOnline())
                    {
                        ZoneComponent targetComp = ComponentManager.Get(killerPlayer);
                        if (targetComp.Zones.Any(x => ZoneManager.Queries.HasFlag(x, Flags.NoPlayerDamage)))
                        {
                            shouldAllow = false;
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnPlayerDamageRequested)}.", ex);
                shouldAllow = originalValue;
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
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(equipment.player);
                ZoneComponent comp = ComponentManager.Get(player);

                // TODO
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
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(equipment.player);
                if (player == null)
                    return;
                
                ZoneComponent comp = ComponentManager.Get(player);
                foreach (var zone in comp.Zones)
                {
                    if (ZoneManager.Queries.HasFlag(zone, Flags.NoItemUnequip) ||
                        ZoneManager.Queries.IsBlocked(zone, equipment.asset.id, ERestrictionType.UNEQUIP))
                    {
                        shouldAllow = false;
                        break;
                    }
                }
                
                if (!shouldAllow)
                    return;
                
                Zone? globalZone = ZoneManager.Queries.GetZone("__global__");
                if (globalZone == null)
                    return;
                
                if (ZoneManager.Queries.HasFlag(globalZone, Flags.NoItemUnequip) ||
                    ZoneManager.Queries.IsBlocked(globalZone, equipment.asset.id, ERestrictionType.UNEQUIP))
                    shouldAllow = false;
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
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(inventory.player);
                if (player == null)
                    return;
                
                if (!ZoneManager.HasFlag(Flags.NoItemDrop, true, player))
                    return;
                
                shouldAllow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnDropItemRequested)}.", ex);
                shouldAllow = originalValue;
            }
        }
    }
}