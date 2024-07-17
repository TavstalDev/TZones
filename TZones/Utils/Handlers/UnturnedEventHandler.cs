using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
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
            
        }

        private static void OnEquipRequested(PlayerEquipment equipment, ItemJar jar, ItemAsset asset, ref bool shouldAllow)
        {
            
        }

        private static void OnDequipRequested(PlayerEquipment equipment, ref bool shouldAllow)
        {
            
        }

        private static void OnDropItemRequested(PlayerInventory inventory, Item item, ref bool shouldAllow)
        {
            
        }
        #endregion

        #region  Build
        #region Barricade
        private static void OnDeployBarricadeRequested(Barricade barricade, ItemBarricadeAsset asset, Transform hit, ref Vector3 point, ref float angle_x, ref float angle_y, ref float angle_z, ref ulong owner, ref ulong group, ref bool shouldAllow)
        {
            
        }

        private static void OnSalvageBarricadeRequested(BarricadeDrop barricade, SteamPlayer instigatorClient, ref bool shouldAllow)
        {
            
        }

        private static void OnDamageBarricadeRequested(CSteamID instigatorSteamID, Transform barricadeTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            
        }
        #endregion

        #region Structure
        private static void OnDeployStructureRequested(Structure structure, ItemStructureAsset asset, ref Vector3 point, ref float angle_x, ref float angle_y, ref float angle_z, ref ulong owner, ref ulong group, ref bool shouldAllow)
        {
            
        }

        private static void OnSalvageStructureRequested(StructureDrop structure, SteamPlayer instigatorClient, ref bool shouldAllow)
        {
            
        }

        private static void OnDamageStructureRequested(CSteamID instigatorSteamID, Transform structureTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            
        }
        #endregion
        #endregion

        #region Vehicle
        private static void OnVehicleEnterVehicleRequested(Player player, InteractableVehicle vehicle, ref bool shouldAllow)
        {
            
        }

        private static void OnVehicleExitVehicleRequested(Player player, InteractableVehicle vehicle, ref bool shouldAllow, ref Vector3 pendingLocation, ref float pendingYaw)
        {
            
        }

        private static void OnVehicleDamageVehicleRequested(CSteamID instigatorSteamID, InteractableVehicle vehicle, ref ushort pendingTotalDamage, ref bool canRepair, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            
        }

        private static void OnVehicleDamageTireRequested(CSteamID instigatorSteamID, InteractableVehicle vehicle, int tireIndex, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            
        }

        private static void OnSiphonVehicleRequested(InteractableVehicle vehicle, Player instigatingPlayer, ref bool shouldAllow, ref ushort desiredAmount)
        {
            
        }

        private static void OnVehicleLockpicked(InteractableVehicle vehicle, Player instigatingPlayer, ref bool allow)
        {
            
        }

        private static void OnVehicleCarjacked(InteractableVehicle vehicle, Player instigatingPlayer, ref bool allow, ref Vector3 force, ref Vector3 torque)
        {
            
        }
        #endregion

        #region PvE
        private static void OnDamageAnimalRequested(ref DamageAnimalParameters parameters, ref bool shouldAllow)
        {
            
        }

        private static void OnDamageZombieRequested(ref DamageZombieParameters parameters, ref bool shouldAllow)
        {
            
        }
        #endregion
    }
}
