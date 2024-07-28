using System.Linq;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
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
        private static bool _isAttached = false;

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
            ZonePlayerComponent comp = parameters.player.GetComponent<ZonePlayerComponent>();

            foreach (Zone zone in comp.Zones) {
                if (zone.HasFlag(Flags.NoPlayerDamage)) {
                    shouldAllow = false;
                    break;
                }
            }
        }

        private static void OnEquipRequested(PlayerEquipment equipment, ItemJar jar, ItemAsset asset, ref bool shouldAllow)
        {
            ZonePlayerComponent comp = equipment.player.GetComponent<ZonePlayerComponent>();

            foreach (Zone zone in comp.Zones) {
                if (zone.HasFlag(Flags.NoItemEquip) ||zone.IsBlocked(asset.id, EBlockType.Equip)) {
                    shouldAllow = false;
                    break;
                }
            }
        }

        private static void OnDequipRequested(PlayerEquipment equipment, ref bool shouldAllow)
        {
            ZonePlayerComponent comp = equipment.player.GetComponent<ZonePlayerComponent>();

            foreach (Zone zone in comp.Zones) {
                if (zone.HasFlag(Flags.NoItemUnequip) || zone.IsBlocked(equipment.asset.id, EBlockType.Unequip)) {
                    shouldAllow = false;
                    break;
                }
            }
        }

        private static void OnDropItemRequested(PlayerInventory inventory, Item item, ref bool shouldAllow)
        {
            ZonePlayerComponent comp = inventory.player.GetComponent<ZonePlayerComponent>();

            foreach (Zone zone in comp.Zones) {
                if (zone.HasFlag(Flags.NoItemDrop)) {
                    shouldAllow = false;
                    break;
                }
            }
        }
        #endregion

        #region  Build
        #region Barricade
        private static void OnDeployBarricadeRequested(Barricade barricade, ItemBarricadeAsset asset, Transform hit, ref Vector3 point, ref float angle_x, ref float angle_y, ref float angle_z, ref ulong owner, ref ulong group, ref bool shouldAllow)
        {
            UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID((CSteamID)owner);
            if (uPlayer == null)
                return;

            ZonePlayerComponent comp = uPlayer.GetComponent<ZonePlayerComponent>();

            foreach (Zone zone in comp.Zones) {
                if (zone.HasFlag(Flags.NoBarricades) || zone.IsBlocked(asset.id, EBlockType.Build)) {
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

            foreach (Zone zone in comp.Zones) {
                if (zone.HasFlag(Flags.NoBarricadeSalvage)) {
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

            foreach (Zone zone in comp.Zones) {
                if (zone.HasFlag(Flags.NoDamage)) {
                    shouldAllow = false;
                    break;
                }
            }
        }
        #endregion

        #region Structure
        private static void OnDeployStructureRequested(Structure structure, ItemStructureAsset asset, ref Vector3 point, ref float angle_x, ref float angle_y, ref float angle_z, ref ulong owner, ref ulong group, ref bool shouldAllow)
        {
            UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID((CSteamID)owner);
            if (uPlayer == null)
                return;

            ZonePlayerComponent comp = uPlayer.GetComponent<ZonePlayerComponent>();

            foreach (Zone zone in comp.Zones) {
                if (zone.HasFlag(Flags.NoStructures) || zone.IsBlocked(asset.id, EBlockType.Build)) {
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
                if (zone.HasFlag(Flags.NoStructureSalvage)) {
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

            foreach (Zone zone in comp.Zones) {
                if (zone.HasFlag(Flags.NoDamage)) {
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

            foreach (Zone zone in comp.Zones) {
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

            foreach (Zone zone in comp.Zones) {
                if (zone.HasFlag(Flags.NoVehicleDamage)) {
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
                if (zone.HasFlag(Flags.NoTireDamage)) {
                    shouldAllow = false;
                    break;
                }
            }
        }

        private static void OnSiphonVehicleRequested(InteractableVehicle vehicle, Player instigatingPlayer, ref bool shouldAllow, ref ushort desiredAmount)
        {
            ZonePlayerComponent comp = instigatingPlayer.GetComponent<ZonePlayerComponent>();

            foreach (Zone zone in comp.Zones) {
                if (zone.HasFlag(Flags.NoVehicleSiphoning)) {
                    shouldAllow = false;
                    break;
                }
            }
        }

        private static void OnVehicleLockpicked(InteractableVehicle vehicle, Player instigatingPlayer, ref bool shouldAllow)
        {
            ZonePlayerComponent comp = instigatingPlayer.GetComponent<ZonePlayerComponent>();

            foreach (Zone zone in comp.Zones) {
                if (zone.HasFlag(Flags.NoLockpick)) {
                    shouldAllow = false;
                    break;
                }
            }
        }

        private static void OnVehicleCarjacked(InteractableVehicle vehicle, Player instigatingPlayer, ref bool shouldAllow, ref Vector3 force, ref Vector3 torque)
        {
            ZonePlayerComponent comp = instigatingPlayer.GetComponent<ZonePlayerComponent>();

            foreach (Zone zone in comp.Zones) {
                if (zone.HasFlag(Flags.NoVehicleCarjack)) {
                    shouldAllow = false;
                    break;
                }
            }
        }
        #endregion

        #region PvE
        private static void OnDamageAnimalRequested(ref DamageAnimalParameters parameters, ref bool shouldAllow)
        {
            if (parameters.instigator is Player player)
            {
                ZonePlayerComponent comp = player.GetComponent<ZonePlayerComponent>();

                foreach (Zone zone in comp.Zones) {
                    if (zone.HasFlag(Flags.NoAnimalDamage)) {
                        shouldAllow = false;
                        break;
                    }
                }
            }
        }

        private static void OnDamageZombieRequested(ref DamageZombieParameters parameters, ref bool shouldAllow)
        {
            if (parameters.instigator is Player player)
            {
                ZonePlayerComponent comp = player.GetComponent<ZonePlayerComponent>();

                foreach (Zone zone in comp.Zones) {
                    if (zone.HasFlag(Flags.NoZombieDamage)) {
                        shouldAllow = false;
                        break;
                    }
                }
            }
        }
        #endregion
    }
}
