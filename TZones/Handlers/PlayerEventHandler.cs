using System;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.TLibrary.Extensions;
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
                var victimPlayer = UnturnedPlayer.FromPlayer(parameters.player);
                UnturnedPlayer? killerPlayer = UnturnedPlayer.FromCSteamID(parameters.killer);
                var players = killerPlayer == null
                    ? new[] { victimPlayer }
                    : new[] { victimPlayer, killerPlayer };

                if (ZoneManager.HasFlag(Flags.AllowPlayerDamage, TZones.Instance.Config.GlobalZoneFlagChecks.AllowPlayerDamage, players))
                    return;

                if (ZoneManager.HasFlag(Flags.NoPlayerDamage,  TZones.Instance.Config.GlobalZoneFlagChecks.NoPlayerDamage, players))
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
            try
            {
                if (Provider.isPvP || isAllowed)
                    return;

                // Because the server is in PvE mode isAllowed should be rechecked
                if (!Provider.modeConfigData.Gameplay.Friendly_Fire && instigator.quests.isMemberOfSameGroupAs(victim))
                    return;

                if (!instigator.movement.canAddSimulationResultsToUpdates)
                    return;

                UnturnedPlayer instigatorPlayer = UnturnedPlayer.FromPlayer(instigator);
                UnturnedPlayer victimPlayer = UnturnedPlayer.FromPlayer(victim);
                isAllowed = ZoneManager.HasFlag(Flags.AllowPlayerDamage,
                    TZones.Instance.Config.GlobalZoneFlagChecks.AllowPlayerDamage, instigatorPlayer, victimPlayer);
            }
            finally
            {
                // ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
                string instigatorName = instigator?.channel?.owner?.playerID?.characterName ?? "unknown";
                string victimName =  victim?.channel?.owner?.playerID?.characterName ?? "unknown";
                // ReSharper restore ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
                TZones.Logger.Debug($"OnPlayerAllowedToDamagePlayer: isAllowed={isAllowed}, instigator={instigatorName}, victim={victimName}");
            }
        }
        
        /// <summary>
        /// Handles equip requests, blocking equipping when the zone forbids item equip or the item is restricted.
        /// </summary>
        private static void OnEquipRequested(PlayerEquipment equipment, ItemJar jar, ItemAsset asset, ref bool shouldAllow)
        {
            if (!shouldAllow)
                return;
            
            bool originalValue = shouldAllow;
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(equipment.player);
                if (player == null)
                    return;
                
                if (!ZoneManager.HasFlagOrBlocked(Flags.NoItemEquip, TZones.Instance.Config.GlobalZoneFlagChecks.NoItemEquip, player, asset.id, ERestrictionType.EQUIP))
                    return;
                
                shouldAllow = false;
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
            if (!shouldAllow)
                return;
            
            bool originalValue = shouldAllow;
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(equipment.player);
                if (player == null)
                    return;
                
                if (!ZoneManager.HasFlagOrBlocked(Flags.NoItemUnequip, TZones.Instance.Config.GlobalZoneFlagChecks.NoItemUnequip, player, equipment.asset.id, ERestrictionType.UNEQUIP))
                   return;
                
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
            if (!shouldAllow)
                return;
            
            bool originalValue = shouldAllow;
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(inventory.player);
                if (player == null)
                    return;
                
                if (!ZoneManager.HasFlag(Flags.NoItemDrop, TZones.Instance.Config.GlobalZoneFlagChecks.NoItemDrop, player))
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