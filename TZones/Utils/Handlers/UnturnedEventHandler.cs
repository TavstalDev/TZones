using System;
using System.Collections.Generic;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using Tavstal.TLibrary.Extensions.Unturned;
using Tavstal.TZones.Components;
using Tavstal.TZones.Models.Core;
using Tavstal.TZones.Models.Enums;
using Tavstal.TZones.Utils.Constants;
using Tavstal.TZones.Utils.Managers;
using UnityEngine;

namespace Tavstal.TZones.Utils.Handlers
{
    /// <summary>
    /// A static class responsible for handling various events in the Unturned game environment.
    /// </summary>
    /// <remarks>
    /// This class serves as a central point for managing event listeners, handling event subscriptions, and invoking event handlers for game-related actions.
    /// </remarks>
    public static class UnturnedEventHandler
    {
        private static bool _isAttached;

        /// <summary>
        /// Attaches event handlers to various events in the Unturned game environment.
        /// </summary>
        /// <remarks>
        /// This method subscribes event listeners to relevant game events, enabling the system to respond to various in-game actions and triggers.
        /// </remarks>
        public static void AttachEvents()
        {
            if (_isAttached)
                return;

            _isAttached = true;

        
            // Player
            U.Events.OnPlayerConnected += OnPlayerConnected;
            U.Events.OnPlayerDisconnected += OnPlayerDisconnected;
            DamageTool.damagePlayerRequested += OnPlayerDamageRequested;

            // Barricade
            BarricadeManager.onDamageBarricadeRequested += OnDamageBarricadeRequested;
            BarricadeManager.onDeployBarricadeRequested += OnDeployBarricadeRequested;
            BarricadeDrop.OnSalvageRequested_Global += OnSalvageBarricadeRequested;

            // Structure
            StructureManager.onDamageStructureRequested += OnDamageStructureRequested;
            StructureManager.onDeployStructureRequested += OnDeployStructureRequested;
            StructureDrop.OnSalvageRequested_Global += OnSalvageStructureRequested;

            // Vehicle
            VehicleManager.onDamageTireRequested += OnVehicleDamageTireRequested;
            VehicleManager.onDamageVehicleRequested += OnVehicleDamageVehicleRequested;
            VehicleManager.onEnterVehicleRequested += OnVehicleEnterVehicleRequested;
            VehicleManager.onExitVehicleRequested += OnVehicleExitVehicleRequested;
            VehicleManager.onVehicleCarjacked += OnVehicleCarjacked;
            VehicleManager.onVehicleLockpicked += OnVehicleLockpicked;
            VehicleManager.onSiphonVehicleRequested += OnSiphonVehicleRequested;

            // PvE
            DamageTool.damageAnimalRequested += OnDamageAnimalRequested;
            DamageTool.damageZombieRequested += OnDamageZombieRequested;
        }

        /// <summary>
        /// Detaches event handlers from various events in the Unturned game environment.
        /// </summary>
        /// <remarks>
        /// This method unsubscribes event listeners from game events, stopping the system from responding to those events.
        /// </remarks>
        public static void DetachEvents()
        {
            if (!_isAttached)
                return;

            _isAttached = false;

            // Player
            U.Events.OnPlayerConnected -= OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= OnPlayerDisconnected;
            DamageTool.damagePlayerRequested -= OnPlayerDamageRequested;

            // Barricade
            BarricadeManager.onDamageBarricadeRequested -= OnDamageBarricadeRequested;
            BarricadeManager.onDeployBarricadeRequested -= OnDeployBarricadeRequested;
            BarricadeDrop.OnSalvageRequested_Global -= OnSalvageBarricadeRequested;

            // Structure
            StructureManager.onDamageStructureRequested -= OnDamageStructureRequested;
            StructureManager.onDeployStructureRequested -= OnDeployStructureRequested;
            StructureDrop.OnSalvageRequested_Global -= OnSalvageStructureRequested;

            // Vehicle
            VehicleManager.onDamageTireRequested -= OnVehicleDamageTireRequested;
            VehicleManager.onDamageVehicleRequested -= OnVehicleDamageVehicleRequested;
            VehicleManager.onEnterVehicleRequested -= OnVehicleEnterVehicleRequested;
            VehicleManager.onExitVehicleRequested -= OnVehicleExitVehicleRequested;
            VehicleManager.onVehicleCarjacked -= OnVehicleCarjacked;
            VehicleManager.onVehicleLockpicked -= OnVehicleLockpicked;
            VehicleManager.onSiphonVehicleRequested -= OnSiphonVehicleRequested;

            // PvE
            DamageTool.damageAnimalRequested -= OnDamageAnimalRequested;
            DamageTool.damageZombieRequested -= OnDamageZombieRequested;

        }

        #region Player
        /// <summary>
        /// Handles the player connection event by attaching event listeners to the player's actions.
        /// </summary>
        /// <param name="player">The player who has connected.</param>
        /// <remarks>
        /// This method subscribes to events such as item dropping, equipping, and dequipping for the connected player.
        /// </remarks>
        private static void OnPlayerConnected(UnturnedPlayer player)
        {
            player.Inventory.onDropItemRequested += OnDropItemRequested;
            player.Player.equipment.onEquipRequested += OnEquipRequested;
            player.Player.equipment.onDequipRequested += OnDequipRequested;
        }

        /// <summary>
        /// Handles the player disconnection event by detaching event listeners from the player's actions.
        /// </summary>
        /// <param name="player">The player who has disconnected.</param>
        /// <remarks>
        /// This method unsubscribes from events such as item dropping, equipping, and dequipping for the disconnected player.
        /// </remarks>
        private static void OnPlayerDisconnected(UnturnedPlayer player)
        {
            player.Inventory.onDropItemRequested -= OnDropItemRequested;
            player.Player.equipment.onEquipRequested -= OnEquipRequested;
            player.Player.equipment.onDequipRequested -= OnDequipRequested;
        }

        /// <summary>
        /// Handles the event when a player is about to receive damage, allowing for modifications to the damage parameters.
        /// </summary>
        /// <param name="parameters">The parameters describing the damage being applied to the player.</param>
        /// <param name="shouldAllow">A flag indicating whether the damage should be allowed. Set to false to prevent damage.</param>
        /// <remarks>
        /// This method allows modification of the damage parameters before they are applied to the player. It can be used to adjust or block damage based on certain conditions.
        /// </remarks>
        private static void OnPlayerDamageRequested(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            try
            {
                ZonePlayerComponent comp = parameters.player.GetComponent<ZonePlayerComponent>();
                UnturnedPlayer targetPlayer = UnturnedPlayer.FromCSteamID(parameters.killer);
                if (targetPlayer != null && parameters.killer.isOnline())
                {
                    ZonePlayerComponent targetComp = targetPlayer.GetComponent<ZonePlayerComponent>();
                    foreach (Zone zone in targetComp.Zones)
                    {
                        if (zone.HasFlag(Flags.PlayerDamage))
                        {
                            shouldAllow = false;
                            break;
                        }
                    }
                }


                foreach (Zone zone in comp.Zones)
                {
                    if (zone.HasFlag(Flags.PlayerDamage))
                    {
                        shouldAllow = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                TZones.Logger.LogException("Error in OnPlayerDamageRequested:");
                TZones.Logger.LogError(ex);
                shouldAllow = true;
            }
        }

        /// <summary>
        /// Handles the event when a player attempts to equip an item, allowing for modifications to the equip request.
        /// </summary>
        /// <param name="equipment">The player's equipment system.</param>
        /// <param name="jar">The item jar containing the item to be equipped.</param>
        /// <param name="asset">The item asset representing the item being equipped.</param>
        /// <param name="shouldAllow">A flag indicating whether the equip action should be allowed. Set to false to prevent equipping the item.</param>
        /// <remarks>
        /// This method allows modification of the equipment process before the item is actually equipped. It can be used to block or adjust the item equip request based on certain conditions.
        /// </remarks>
        private static void OnEquipRequested(PlayerEquipment equipment, ItemJar jar, ItemAsset asset, ref bool shouldAllow)
        {
            ZonePlayerComponent comp = equipment.player.GetComponent<ZonePlayerComponent>();

            foreach (Zone zone in comp.Zones) 
            {
                if (zone.HasFlag(Flags.ItemEquip) ||zone.IsBlocked(asset.id, EBlockType.Equip)) {
                    shouldAllow = false;
                    break;
                }
            }
        }

        /// <summary>
        /// Handles the event when a player attempts to dequip an item, allowing for modifications to the dequip request.
        /// </summary>
        /// <param name="equipment">The player's equipment system.</param>
        /// <param name="shouldAllow">A flag indicating whether the dequip action should be allowed. Set to false to prevent dequipping the item.</param>
        /// <remarks>
        /// This method allows modification of the dequip process before the item is actually dequipped. It can be used to block or adjust the item dequip request based on certain conditions.
        /// </remarks>
        private static void OnDequipRequested(PlayerEquipment equipment, ref bool shouldAllow)
        {
            ZonePlayerComponent comp = equipment.player.GetComponent<ZonePlayerComponent>();

            foreach (Zone zone in comp.Zones) 
            {
                if (zone.HasFlag(Flags.ItemUnequip) || zone.IsBlocked(equipment.asset.id, EBlockType.Unequip)) {
                    shouldAllow = false;
                    break;
                }
            }
        }

        /// <summary>
        /// Handles the event when a player attempts to drop an item, allowing for modifications to the drop request.
        /// </summary>
        /// <param name="inventory">The player's inventory system.</param>
        /// <param name="item">The item that is being requested to drop.</param>
        /// <param name="shouldAllow">A flag indicating whether the drop action should be allowed. Set to false to prevent dropping the item.</param>
        /// <remarks>
        /// This method allows modification of the item drop process before the item is actually dropped. It can be used to block or adjust the item drop request based on certain conditions.
        /// </remarks>
        private static void OnDropItemRequested(PlayerInventory inventory, Item item, ref bool shouldAllow)
        {
            ZonePlayerComponent comp = inventory.player.GetComponent<ZonePlayerComponent>();

            foreach (Zone zone in comp.Zones) 
            {
                if (zone.HasFlag(Flags.ItemDrop)) 
                {
                    shouldAllow = false;
                    break;
                }
            }
        }
        #endregion

        #region  Build
        #region Barricade
        /// <summary>
        /// Handles the event when a player attempts to deploy a barricade, allowing for modifications to the deployment parameters.
        /// </summary>
        /// <param name="barricade">The barricade object being deployed.</param>
        /// <param name="asset">The asset representing the barricade item.</param>
        /// <param name="hit">The transform representing the location where the barricade is being deployed.</param>
        /// <param name="point">The position where the barricade is being deployed. Can be modified to change the placement location.</param>
        /// <param name="angleX">The X-axis rotation angle of the barricade. Can be modified to adjust the orientation.</param>
        /// <param name="angleY">The Y-axis rotation angle of the barricade. Can be modified to adjust the orientation.</param>
        /// <param name="angleZ">The Z-axis rotation angle of the barricade. Can be modified to adjust the orientation.</param>
        /// <param name="owner">The ID of the player who is deploying the barricade. Can be modified to change the owner.</param>
        /// <param name="group">The ID of the group associated with the barricade deployment. Can be modified to change the group.</param>
        /// <param name="shouldAllow">A flag indicating whether the barricade deployment should be allowed. Set to false to prevent deployment.</param>
        /// <remarks>
        /// This method allows modification of the barricade deployment parameters before the barricade is placed. It can be used to adjust the location, rotation, owner, group, or block the deployment entirely based on certain conditions.
        /// </remarks>
        private static void OnDeployBarricadeRequested(Barricade barricade, ItemBarricadeAsset asset, Transform hit, ref Vector3 point, ref float angleX, ref float angleY, ref float angleZ, ref ulong owner, ref ulong group, ref bool shouldAllow)
        {
            UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID((CSteamID)owner);
            if (uPlayer == null)
                return;

            ZonePlayerComponent comp = uPlayer.GetComponent<ZonePlayerComponent>();

            foreach (Zone zone in comp.Zones) 
            {
                if (zone.HasFlag(Flags.Barricades) || zone.IsBlocked(asset.id, EBlockType.Build)) 
                {
                    shouldAllow = false;
                    break;
                }
            }

            List<Zone> objectZones = ZonesManager.GetZonesFromPosition(point);
            foreach (Zone zone in objectZones) 
            {
                if (zone.HasFlag(Flags.Barricades) || zone.IsBlocked(asset.id, EBlockType.Build)) 
                {
                    shouldAllow = false;
                    break;
                }
            }
        }

        /// <summary>
        /// Handles the event when a player attempts to salvage a barricade, allowing for modifications to the salvage request.
        /// </summary>
        /// <param name="barricade">The barricade being salvaged.</param>
        /// <param name="instigatorClient">The player (client) who is requesting the salvage action.</param>
        /// <param name="shouldAllow">A flag indicating whether the salvage action should be allowed. Set to false to prevent salvaging the barricade.</param>
        /// <remarks>
        /// This method allows modification of the barricade salvage process before the action is completed. It can be used to block or adjust the salvage request based on specific conditions or requirements.
        /// </remarks>
        private static void OnSalvageBarricadeRequested(BarricadeDrop barricade, SteamPlayer instigatorClient, ref bool shouldAllow)
        {
            UnturnedPlayer uPlayer = UnturnedPlayer.FromSteamPlayer(instigatorClient);
            if (uPlayer == null)
                return;

            ZonePlayerComponent comp = uPlayer.GetComponent<ZonePlayerComponent>();

            foreach (Zone zone in comp.Zones) 
            {
                if (zone.HasFlag(Flags.BarricadeSalvage)) {
                    shouldAllow = false;
                    break;
                }
            }
            
            List<Zone> objectZones = ZonesManager.GetZonesFromPosition(barricade.interactable.transform.position);
            foreach (Zone zone in objectZones) 
            {
                if (zone.HasFlag(Flags.BarricadeSalvage)) 
                {
                    shouldAllow = false;
                    break;
                }
            }
        }

        /// <summary>
        /// Handles the event when damage is requested on a barricade, allowing for modifications to the damage parameters.
        /// </summary>
        /// <param name="instigatorSteamID">The Steam ID of the player or entity that is causing the damage.</param>
        /// <param name="barricadeTransform">The transform of the barricade being damaged.</param>
        /// <param name="pendingTotalDamage">The total amount of damage pending to be applied to the barricade. Can be modified to adjust the damage.</param>
        /// <param name="shouldAllow">A flag indicating whether the damage action should be allowed. Set to false to prevent the damage from being applied.</param>
        /// <param name="damageOrigin">The origin of the damage (e.g., environmental, player, etc.).</param>
        /// <remarks>
        /// This method allows modification of the damage applied to a barricade before it is actually processed. It can be used to adjust or block the damage based on certain conditions.
        /// </remarks>
        private static void OnDamageBarricadeRequested(CSteamID instigatorSteamID, Transform barricadeTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(instigatorSteamID);
            if (uPlayer == null)
                return;

            ZonePlayerComponent comp = uPlayer.GetComponent<ZonePlayerComponent>();

            foreach (Zone zone in comp.Zones) 
            {
                if (zone.HasFlag(Flags.Damage)) {
                    shouldAllow = false;
                    break;
                }
            }
            
            List<Zone> objectZones = ZonesManager.GetZonesFromPosition(barricadeTransform.position);
            foreach (Zone zone in objectZones) 
            {
                if (zone.HasFlag(Flags.Damage)) 
                {
                    shouldAllow = false;
                    break;
                }
            }
        }
        #endregion
        #region Structure
        /// <summary>
        /// Handles the event when a player attempts to deploy a structure, allowing for modifications to the deployment parameters.
        /// </summary>
        /// <param name="structure">The structure object being deployed.</param>
        /// <param name="asset">The asset representing the structure item.</param>
        /// <param name="point">The position where the structure is being deployed. Can be modified to change the placement location.</param>
        /// <param name="angleX">The X-axis rotation angle of the structure. Can be modified to adjust the orientation.</param>
        /// <param name="angleY">The Y-axis rotation angle of the structure. Can be modified to adjust the orientation.</param>
        /// <param name="angleZ">The Z-axis rotation angle of the structure. Can be modified to adjust the orientation.</param>
        /// <param name="owner">The ID of the player who is deploying the structure. Can be modified to change the owner.</param>
        /// <param name="group">The ID of the group associated with the structure deployment. Can be modified to change the group.</param>
        /// <param name="shouldAllow">A flag indicating whether the structure deployment should be allowed. Set to false to prevent deployment.</param>
        /// <remarks>
        /// This method allows modification of the structure deployment parameters before the structure is placed. It can be used to adjust the location, rotation, owner, group, or block the deployment entirely based on certain conditions.
        /// </remarks>
        private static void OnDeployStructureRequested(Structure structure, ItemStructureAsset asset, ref Vector3 point, ref float angleX, ref float angleY, ref float angleZ, ref ulong owner, ref ulong group, ref bool shouldAllow)
        {
            UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID((CSteamID)owner);
            if (uPlayer == null)
                return;

            ZonePlayerComponent comp = uPlayer.GetComponent<ZonePlayerComponent>();

            foreach (Zone zone in comp.Zones) 
            {
                if (zone.HasFlag(Flags.Structures) || zone.IsBlocked(asset.id, EBlockType.Build)) {
                    shouldAllow = false;
                    break;
                }
            }
            
            List<Zone> objectZones = ZonesManager.GetZonesFromPosition(point);
            foreach (Zone zone in objectZones) 
            {
                if (zone.HasFlag(Flags.Structures) || zone.IsBlocked(asset.id, EBlockType.Build)) 
                {
                    shouldAllow = false;
                    break;
                }
            }
        }

        /// <summary>
        /// Handles the event when a player attempts to salvage a structure, allowing for modifications to the salvage request.
        /// </summary>
        /// <param name="structure">The structure being salvaged.</param>
        /// <param name="instigatorClient">The player (client) who is requesting the salvage action.</param>
        /// <param name="shouldAllow">A flag indicating whether the salvage action should be allowed. Set to false to prevent salvaging the structure.</param>
        /// <remarks>
        /// This method allows modification of the structure salvage process before the action is completed. It can be used to block or adjust the salvage request based on specific conditions or requirements.
        /// </remarks>
        private static void OnSalvageStructureRequested(StructureDrop structure, SteamPlayer instigatorClient, ref bool shouldAllow)
        {
            UnturnedPlayer uPlayer = UnturnedPlayer.FromSteamPlayer(instigatorClient);
            if (uPlayer == null)
                return;

            ZonePlayerComponent comp = uPlayer.GetComponent<ZonePlayerComponent>();

            foreach (Zone zone in comp.Zones) {
                if (zone.HasFlag(Flags.StructureSalvage)) {
                    shouldAllow = false;
                    break;
                }
            }
            
            List<Zone> objectZones = ZonesManager.GetZonesFromPosition(structure.GetServersideData().point);
            foreach (Zone zone in objectZones) 
            {
                if (zone.HasFlag(Flags.StructureSalvage)) 
                {
                    shouldAllow = false;
                    break;
                }
            }
        }

        /// <summary>
        /// Handles the event when damage is requested on a structure, allowing for modifications to the damage parameters.
        /// </summary>
        /// <param name="instigatorSteamID">The Steam ID of the player or entity causing the damage.</param>
        /// <param name="structureTransform">The transform of the structure being damaged.</param>
        /// <param name="pendingTotalDamage">The total amount of damage pending to be applied to the structure. Can be modified to adjust the damage.</param>
        /// <param name="shouldAllow">A flag indicating whether the damage action should be allowed. Set to false to prevent the damage from being applied.</param>
        /// <param name="damageOrigin">The origin of the damage (e.g., environmental, player, etc.).</param>
        /// <remarks>
        /// This method allows modification of the damage applied to a structure before it is processed. It can be used to adjust or block the damage based on certain conditions.
        /// </remarks>
        private static void OnDamageStructureRequested(CSteamID instigatorSteamID, Transform structureTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(instigatorSteamID);
            if (uPlayer == null)
                return;

            ZonePlayerComponent comp = uPlayer.GetComponent<ZonePlayerComponent>();
            foreach (Zone zone in comp.Zones) 
            {
                if (zone.HasFlag(Flags.Damage)) {
                    shouldAllow = false;
                    break;
                }
            }
            
            List<Zone> objectZones = ZonesManager.GetZonesFromPosition(structureTransform.position);
            foreach (Zone zone in objectZones) 
            {
                if (zone.HasFlag(Flags.Damage)) 
                {
                    shouldAllow = false;
                    break;
                }
            }
        }
        #endregion
        #endregion

        #region Vehicle
        /// <summary>
        /// Handles the event when a player attempts to enter a vehicle, allowing for modifications to the entry request.
        /// </summary>
        /// <param name="player">The player attempting to enter the vehicle.</param>
        /// <param name="vehicle">The vehicle the player is attempting to enter.</param>
        /// <param name="shouldAllow">A flag indicating whether the vehicle entry should be allowed. Set to false to prevent entering the vehicle.</param>
        /// <remarks>
        /// This method allows modification of the vehicle entry process before the player is allowed to enter the vehicle. It can be used to block or adjust the entry request based on specific conditions.
        /// </remarks>
        private static void OnVehicleEnterVehicleRequested(Player player, InteractableVehicle vehicle, ref bool shouldAllow)
        {
            ZonePlayerComponent comp = player.GetComponent<ZonePlayerComponent>();

            foreach (Zone zone in comp.Zones) 
            {
                if (zone.IsBlocked(vehicle.id, EBlockType.VehicleEnter)) {
                    shouldAllow = false;
                    break;
                }
            }
        }

        /// <summary>
        /// Handles the event when a player attempts to exit a vehicle, allowing for modifications to the exit request.
        /// </summary>
        /// <param name="player">The player attempting to exit the vehicle.</param>
        /// <param name="vehicle">The vehicle the player is attempting to exit from.</param>
        /// <param name="shouldAllow">A flag indicating whether the vehicle exit should be allowed. Set to false to prevent exiting the vehicle.</param>
        /// <param name="pendingLocation">The location where the player is expected to be placed after exiting the vehicle. Can be modified to change the exit location.</param>
        /// <param name="pendingYaw">The yaw angle of the player after exiting the vehicle. Can be modified to adjust the player's orientation.</param>
        /// <remarks>
        /// This method allows modification of the vehicle exit process before the player actually exits the vehicle. It can be used to block or adjust the exit request, as well as modify the exit location and orientation.
        /// </remarks>
        private static void OnVehicleExitVehicleRequested(Player player, InteractableVehicle vehicle, ref bool shouldAllow, ref Vector3 pendingLocation, ref float pendingYaw)
        {
            ZonePlayerComponent comp = player.GetComponent<ZonePlayerComponent>();

            foreach (Zone zone in comp.Zones) {
                if (zone.IsBlocked(vehicle.id, EBlockType.VehicleLeave)) {
                    shouldAllow = false;
                    break;
                }
            }
        }

        /// <summary>
        /// Handles the event when damage is requested on a vehicle, allowing for modifications to the damage parameters.
        /// </summary>
        /// <param name="instigatorSteamID">The Steam ID of the player or entity causing the damage.</param>
        /// <param name="vehicle">The vehicle being damaged.</param>
        /// <param name="pendingTotalDamage">The total amount of damage pending to be applied to the vehicle. Can be modified to adjust the damage.</param>
        /// <param name="canRepair">A flag indicating whether the vehicle can be repaired after the damage. Can be modified to change the repair status.</param>
        /// <param name="shouldAllow">A flag indicating whether the damage action should be allowed. Set to false to prevent the damage from being applied.</param>
        /// <param name="damageOrigin">The origin of the damage (e.g., environmental, player, etc.).</param>
        /// <remarks>
        /// This method allows modification of the vehicle damage process before the damage is actually applied. It can be used to adjust the damage amount, block the damage entirely, or control whether the vehicle can be repaired after damage.
        /// </remarks>
        private static void OnVehicleDamageVehicleRequested(CSteamID instigatorSteamID, InteractableVehicle vehicle, ref ushort pendingTotalDamage, ref bool canRepair, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(instigatorSteamID);
            if (uPlayer == null)
                return;

            ZonePlayerComponent comp = uPlayer.GetComponent<ZonePlayerComponent>();

            foreach (Zone zone in comp.Zones) 
            {
                if (zone.HasFlag(Flags.VehicleDamage)) {
                    shouldAllow = false;
                    break;
                }
            }
            
            List<Zone> objectZones = ZonesManager.GetZonesFromPosition(vehicle.transform.position);
            foreach (Zone zone in objectZones) 
            {
                if (zone.HasFlag(Flags.VehicleDamage)) 
                {
                    shouldAllow = false;
                    break;
                }
            }
        }

        /// <summary>
        /// Handles the event when damage is requested on a specific tire of a vehicle, allowing for modifications to the tire damage parameters.
        /// </summary>
        /// <param name="instigatorSteamID">The Steam ID of the player or entity causing the tire damage.</param>
        /// <param name="vehicle">The vehicle with the damaged tire.</param>
        /// <param name="tireIndex">The index of the tire being damaged (e.g., front-left, rear-right, etc.).</param>
        /// <param name="shouldAllow">A flag indicating whether the tire damage action should be allowed. Set to false to prevent the tire from being damaged.</param>
        /// <param name="damageOrigin">The origin of the damage (e.g., environmental, player, etc.).</param>
        /// <remarks>
        /// This method allows modification of the tire damage process before the damage is applied. It can be used to block or adjust the damage to the tire based on certain conditions.
        /// </remarks>
        private static void OnVehicleDamageTireRequested(CSteamID instigatorSteamID, InteractableVehicle vehicle, int tireIndex, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(instigatorSteamID);
            if (uPlayer == null)
                return;

            ZonePlayerComponent comp = uPlayer.GetComponent<ZonePlayerComponent>();

            foreach (Zone zone in comp.Zones) {
                if (zone.HasFlag(Flags.TireDamage)) {
                    shouldAllow = false;
                    break;
                }
            }
            
            List<Zone> objectZones = ZonesManager.GetZonesFromPosition(vehicle.transform.position);
            foreach (Zone zone in objectZones) 
            {
                if (zone.HasFlag(Flags.TireDamage)) 
                {
                    shouldAllow = false;
                    break;
                }
            }
        }

        /// <summary>
        /// Handles the event when a player attempts to siphon fuel from a vehicle, allowing for modifications to the siphoning request.
        /// </summary>
        /// <param name="vehicle">The vehicle from which the player is attempting to siphon fuel.</param>
        /// <param name="instigatingPlayer">The player attempting to siphon fuel from the vehicle.</param>
        /// <param name="shouldAllow">A flag indicating whether the siphoning action should be allowed. Set to false to prevent siphoning.</param>
        /// <param name="desiredAmount">The amount of fuel the player wishes to siphon. Can be modified to adjust the siphoned amount.</param>
        /// <remarks>
        /// This method allows modification of the siphoning process before the fuel is actually siphoned. It can be used to block siphoning or adjust the desired siphoned amount based on specific conditions.
        /// </remarks>
        private static void OnSiphonVehicleRequested(InteractableVehicle vehicle, Player instigatingPlayer, ref bool shouldAllow, ref ushort desiredAmount)
        {
            ZonePlayerComponent comp = instigatingPlayer.GetComponent<ZonePlayerComponent>();

            foreach (Zone zone in comp.Zones) {
                if (zone.HasFlag(Flags.VehicleSiphoning)) {
                    shouldAllow = false;
                    break;
                }
            }
            
            List<Zone> objectZones = ZonesManager.GetZonesFromPosition(vehicle.transform.position);
            foreach (Zone zone in objectZones) 
            {
                if (zone.HasFlag(Flags.VehicleSiphoning)) 
                {
                    shouldAllow = false;
                    break;
                }
            }
        }

        /// <summary>
        /// Handles the event when a player attempts to lockpick a vehicle, allowing for modifications to the lockpicking request.
        /// </summary>
        /// <param name="vehicle">The vehicle that the player is attempting to lockpick.</param>
        /// <param name="instigatingPlayer">The player attempting to lockpick the vehicle.</param>
        /// <param name="shouldAllow">A flag indicating whether the lockpicking action should be allowed. Set to false to prevent lockpicking.</param>
        /// <remarks>
        /// This method allows modification of the lockpicking process before it is carried out. It can be used to block lockpicking or adjust the conditions under which it is allowed.
        /// </remarks>
        private static void OnVehicleLockpicked(InteractableVehicle vehicle, Player instigatingPlayer, ref bool shouldAllow)
        {
            ZonePlayerComponent comp = instigatingPlayer.GetComponent<ZonePlayerComponent>();

            foreach (Zone zone in comp.Zones) {
                if (zone.HasFlag(Flags.Lockpick)) {
                    shouldAllow = false;
                    break;
                }
            }
            
            List<Zone> objectZones = ZonesManager.GetZonesFromPosition(vehicle.transform.position);
            foreach (Zone zone in objectZones) 
            {
                if (zone.HasFlag(Flags.Lockpick)) 
                {
                    shouldAllow = false;
                    break;
                }
            }
        }

        /// <summary>
        /// Handles the event when a player attempts to carjack a vehicle, allowing for modifications to the carjacking request and physics parameters.
        /// </summary>
        /// <param name="vehicle">The vehicle that the player is attempting to carjack.</param>
        /// <param name="instigatingPlayer">The player attempting to carjack the vehicle.</param>
        /// <param name="shouldAllow">A flag indicating whether the carjacking action should be allowed. Set to false to prevent the carjacking.</param>
        /// <param name="force">The force to be applied to the vehicle during the carjack. Can be modified to adjust the force.</param>
        /// <param name="torque">The torque to be applied to the vehicle during the carjack. Can be modified to adjust the vehicle's rotation.</param>
        /// <remarks>
        /// This method allows modification of the carjacking process before it is carried out. It can be used to block carjacking or adjust the applied forces and torque based on specific conditions.
        /// </remarks>
        private static void OnVehicleCarjacked(InteractableVehicle vehicle, Player instigatingPlayer, ref bool shouldAllow, ref Vector3 force, ref Vector3 torque)
        {
            ZonePlayerComponent comp = instigatingPlayer.GetComponent<ZonePlayerComponent>();

            foreach (Zone zone in comp.Zones) {
                if (zone.HasFlag(Flags.VehicleCarjack)) {
                    shouldAllow = false;
                    break;
                }
            }
        }
        #endregion

        #region PvE
        /// <summary>
        /// Handles the event when damage is requested on an animal, allowing for modifications to the damage parameters.
        /// </summary>
        /// <param name="parameters">The parameters related to the damage being applied to the animal (e.g., damage amount, damage source, etc.).</param>
        /// <param name="shouldAllow">A flag indicating whether the damage action should be allowed. Set to false to prevent the damage from being applied.</param>
        /// <remarks>
        /// This method allows modification of the damage process before the damage is actually applied to the animal. It can be used to adjust the damage amount, block the damage, or modify the damage source based on specific conditions.
        /// </remarks>
        private static void OnDamageAnimalRequested(ref DamageAnimalParameters parameters, ref bool shouldAllow)
        {
            try
            {
                if (parameters.instigator is Player player)
                {
                    ZonePlayerComponent comp = player.GetComponent<ZonePlayerComponent>();
                    foreach (Zone zone in comp.Zones)
                    {
                        if (zone.HasFlag(Flags.AnimalDamage))
                        {
                            shouldAllow = false;
                            break;
                        }
                    }

                    List<Zone> objectZones = ZonesManager.GetZonesFromPosition(parameters.animal.transform.position);
                    foreach (Zone zone in objectZones)
                    {
                        if (zone.HasFlag(Flags.AnimalDamage))
                        {
                            shouldAllow = false;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TZones.Logger.LogException("Error in OnDamageAnimalRequested:");
                TZones.Logger.LogError(ex);
                shouldAllow = true;
            }
        }

        /// <summary>
        /// Handles the event when damage is requested on a zombie, allowing for modifications to the damage parameters.
        /// </summary>
        /// <param name="parameters">The parameters related to the damage being applied to the zombie (e.g., damage amount, damage source, etc.).</param>
        /// <param name="shouldAllow">A flag indicating whether the damage action should be allowed. Set to false to prevent the damage from being applied.</param>
        /// <remarks>
        /// This method allows modification of the zombie damage process before the damage is actually applied. It can be used to adjust the damage amount, block the damage, or modify the damage source based on specific conditions.
        /// </remarks>
        private static void OnDamageZombieRequested(ref DamageZombieParameters parameters, ref bool shouldAllow)
        {
            try
            {
                if (parameters.instigator is Player player)
                {
                    ZonePlayerComponent comp = player.GetComponent<ZonePlayerComponent>();

                    foreach (Zone zone in comp.Zones)
                    {
                        if (zone.HasFlag(Flags.ZombieDamage))
                        {
                            shouldAllow = false;
                            break;
                        }
                    }

                    List<Zone> objectZones = ZonesManager.GetZonesFromPosition(parameters.zombie.transform.position);
                    foreach (Zone zone in objectZones)
                    {
                        if (zone.HasFlag(Flags.ZombieDamage))
                        {
                            shouldAllow = false;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TZones.Logger.LogException("Error in OnDamageZombieRequested:");
                TZones.Logger.LogError(ex);
                shouldAllow = true;
            }
        }
        #endregion
    }
}
