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
    public static class UnturnedEventHandler
    {
        private static bool _isAttached;

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
        private static void OnPlayerConnected(UnturnedPlayer player)
        {
            player.Inventory.onDropItemRequested += OnDropItemRequested;
            player.Player.equipment.onEquipRequested += OnEquipRequested;
            player.Player.equipment.onDequipRequested += OnDequipRequested;
        }

        private static void OnPlayerDisconnected(UnturnedPlayer player)
        {
            player.Inventory.onDropItemRequested -= OnDropItemRequested;
            player.Player.equipment.onEquipRequested -= OnEquipRequested;
            player.Player.equipment.onDequipRequested -= OnDequipRequested;
        }

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
