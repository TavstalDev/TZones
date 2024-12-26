using System.Collections.Generic;

namespace Tavstal.TZones.Utils.Constants
{
    /// <summary>
    /// A static class containing predefined zone flags and their default values.
    /// </summary>
    /// <remarks>
    /// This class holds a list of constants representing various flags that can be used within zones. These flags control actions such as damage prevention, item manipulation, and vehicle interactions.
    /// </remarks>
    public static class Flags
    {
        /// <summary>
        /// A list of default flags that are commonly used within zones.
        /// </summary>
        public static List<string> Defaults { get; } = new List<string>()
        {
            Damage, VehicleDamage, PlayerDamage, AnimalDamage, ZombieDamage, TireDamage,
            Lockpick, Barricades, Structures, BarricadeSalvage, StructureSalvage, ItemEquip, ItemUnequip,
            ItemDrop, Enter, Leave, Zombie, InfiniteGenerator, VehicleCarjack, VehicleSiphoning
        };

        /// <summary>
        /// A flag indicating that no damage is allowed.
        /// </summary>
        public const string Damage = "NoDamage";
        /// <summary>
        /// A flag indicating that no vehicle damage is allowed.
        /// </summary>
        public const string VehicleDamage = "NoVehicleDamage";
        /// <summary>
        /// A flag indicating that no player damage is allowed.
        /// </summary>
        public const string PlayerDamage = "NoPlayerDamage";
        /// <summary>
        /// A flag indicating that no animal damage is allowed.
        /// </summary>
        public const string AnimalDamage = "NoAnimalDamage";
        /// <summary>
        /// A flag indicating that no zombie damage is allowed.
        /// </summary>
        public const string ZombieDamage = "NoZombieDamage";
        /// <summary>
        /// A flag indicating that no tire damage is allowed.
        /// </summary>
        public const string TireDamage = "NoTireDamage";
        /// <summary>
        /// A flag indicating that no lockpicking is allowed.
        /// </summary>
        public const string Lockpick = "NoLockpick";
        /// <summary>
        /// A flag indicating that no barricades can be placed.
        /// </summary>
        public const string Barricades = "NoBarricades";
        /// <summary>
        /// A flag indicating that no structures can be placed.
        /// </summary>
        public const string Structures = "NoStructures";
        /// <summary>
        /// A flag indicating that no barricades can be salvaged.
        /// </summary>
        public const string BarricadeSalvage = "NoBarricadeSalvage";
        /// <summary>
        /// A flag indicating that no structures can be salvaged.
        /// </summary>
        public const string StructureSalvage = "NoStructureSalvage";
        /// <summary>
        /// A flag indicating that no items can be equipped.
        /// </summary>
        public const string ItemEquip = "NoItemEquip";
        /// <summary>
        /// A flag indicating that no items can be unequipped.
        /// </summary>
        public const string ItemUnequip = "NoItemUnequip";
        /// <summary>
        /// A flag indicating that no items can be dropped.
        /// </summary>
        public const string ItemDrop = "NoItemDrop";
        /// <summary>
        /// A flag indicating that no entry is allowed into the zone.
        /// </summary>
        public const string Enter = "NoEnter";
        /// <summary>
        /// A flag indicating that no exit is allowed from the zone.
        /// </summary>
        public const string Leave = "NoLeave";
        /// <summary>
        /// A flag indicating that zombies are not allowed within the zone.
        /// </summary>
        public const string Zombie = "NoZombie";
        /// <summary>
        /// A flag indicating that the generator in the zone is infinite.
        /// </summary>
        public const string InfiniteGenerator = "InfiniteGenerator";
        /// <summary>
        /// A flag indicating that vehicle carjacking is not allowed.
        /// </summary>
        public const string VehicleCarjack = "NoVehicleCarjack";
        /// <summary>
        /// A flag indicating that vehicle siphoning is not allowed.
        /// </summary>
        public const string VehicleSiphoning = "NoVehicleSiphoning";
    }
}