using System;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using Tavstal.RocketFlow.Attributes;
using Tavstal.RocketFlow.Core;
using Tavstal.RocketFlow.Events.Damage;
using Tavstal.RocketFlow.Events.Player;
using Tavstal.RocketFlow.Events.Player.Inventory;
using Tavstal.TLibrary.Extensions;
using Tavstal.TZones.Models.Enums;
using Tavstal.TZones.Utils.Constants;
using Tavstal.TZones.Utils.Managers;
// ReSharper disable UnusedMember.Local

namespace Tavstal.TZones.Handlers
{
    /// <summary>
    /// Handles player-related events such as damage, equip, dequip, and item drop, enforcing zone restrictions.
    /// </summary>
    public class PlayerEventListener : EventListener
    {
        [EventHandler]
        private void OnPlayerDisconnected(PlayerDisconnectEvent e)
        {
            ComponentManager.Invalidate(e.Player.Id);
        }
        
        [EventHandler(EEventPriority.HIGH)]
        private void OnPlayerDamageRequested(PlayerDamageEvent e)
        {
            if (Provider.isPvP && !e.ShouldAllow)
                return;
            
            bool originalValue = e.ShouldAllow;
            try
            {
                UnturnedPlayer victimPlayer = UnturnedPlayer.FromPlayer(e.Parameters.player);
                UnturnedPlayer killerPlayer = UnturnedPlayer.FromCSteamID(e.Parameters.killer);
                if (victimPlayer == null && killerPlayer == null)
                    return;
                
                UnturnedPlayer[] players = killerPlayer == null || killerPlayer.Player == null || killerPlayer.CSteamID == CSteamID.Nil
                    ? new[] { victimPlayer! }
                    : new[] { victimPlayer!, killerPlayer };

                if (ZoneManager.HasFlag(Flags.AllowPlayerDamage, TZones.Instance.Config.GlobalZoneFlagChecks.AllowPlayerDamage, players))
                    return;

                if (ZoneManager.HasFlag(Flags.NoPlayerDamage,  TZones.Instance.Config.GlobalZoneFlagChecks.NoPlayerDamage, players))
                    e.ShouldAllow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnPlayerDamageRequested)}.", ex);
                e.ShouldAllow = originalValue;
            }
        }
        
        [EventHandler(EEventPriority.HIGH)]
        private void OnPlayerAllowedToDamagePlayer(PlayerAllowedToDamagePlayerEvent e)
        {
            try
            {
                if (Provider.isPvP || e.IsAllowed)
                    return;

                // Because the server is in PvE mode isAllowed should be rechecked
                if (!Provider.modeConfigData.Gameplay.Friendly_Fire && e.Instigator.quests.isMemberOfSameGroupAs(e.Victim))
                    return;

                if (!e.Instigator.movement.canAddSimulationResultsToUpdates)
                    return;

                UnturnedPlayer instigatorPlayer = UnturnedPlayer.FromPlayer(e.Instigator);
                UnturnedPlayer victimPlayer = UnturnedPlayer.FromPlayer(e.Victim);
                e.IsAllowed = ZoneManager.HasFlag(Flags.AllowPlayerDamage,
                    TZones.Instance.Config.GlobalZoneFlagChecks.AllowPlayerDamage, instigatorPlayer, victimPlayer);
            }
            finally
            {
                // ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
                string instigatorName = e.Instigator?.channel?.owner?.playerID?.characterName ?? "unknown";
                string victimName =  e.Victim?.channel?.owner?.playerID?.characterName ?? "unknown";
                // ReSharper restore ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
                TZones.Logger.Debug($"OnPlayerAllowedToDamagePlayer: isAllowed={e.IsAllowed}, instigator={instigatorName}, victim={victimName}");
            }
        }
        
        [EventHandler]
        private void OnEquipRequested(PlayerEquipEvent e)
        {
            if (!e.ShouldAllow)
                return;
            
            bool originalValue = e.ShouldAllow;
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(e.Equipment.player);
                if (player == null)
                    return;
                
                if (!ZoneManager.HasFlagOrBlocked(Flags.NoItemEquip, TZones.Instance.Config.GlobalZoneFlagChecks.NoItemEquip, player, e.Asset.id, ERestrictionType.EQUIP))
                    return;
                
                e.ShouldAllow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnEquipRequested)}.", ex);
                e.ShouldAllow = originalValue;
            }
        }
        
        [EventHandler]
        private void OnDequipRequested(PlayerDequipEvent e)
        {
            if (!e.ShouldAllow)
                return;
            
            bool originalValue = e.ShouldAllow;
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(e.Equipment.player);
                if (player == null)
                    return;
                
                if (e.Equipment == null || e.Equipment.asset == null)
                    return;
                
                if (!ZoneManager.HasFlagOrBlocked(Flags.NoItemUnequip, TZones.Instance.Config.GlobalZoneFlagChecks.NoItemUnequip, player, e.Equipment.asset.id, ERestrictionType.UNEQUIP))
                   return;
                
                e.ShouldAllow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnDequipRequested)}.", ex);
                e.ShouldAllow = originalValue;
            }
        }
        
        [EventHandler]
        private void OnDropItemRequested(PlayerInventoryDropEvent e)
        {
            if (!e.ShouldAllow)
                return;
            
            bool originalValue = e.ShouldAllow;
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(e.Inventory.player);
                if (player == null)
                    return;
                
                if (!ZoneManager.HasFlag(Flags.NoItemDrop, TZones.Instance.Config.GlobalZoneFlagChecks.NoItemDrop, player))
                    return;
                
                e.ShouldAllow = false;
            }
            catch (Exception ex)
            {
                TZones.Logger.Error($"Unexpected error occured in {nameof(OnDropItemRequested)}.", ex);
                e.ShouldAllow = originalValue;
            }
        }
    }
}