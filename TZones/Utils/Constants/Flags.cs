using System.Collections.Generic;

namespace Tavstal.TZones.Utils.Constants
{
    /// <summary>
    /// A static class containing predefined zone flags and their default values.
    /// </summary>
    public static class Flags
    {
        /// <summary>
        /// A list of default flags that are commonly used within zones.
        /// </summary>
        public static List<string> Defaults { get; } = new List<string>()
        {
            NoDamage, NoVehicleDamage, AllowPlayerDamage, NoPlayerDamage, NoAnimalDamage, NoZombieDamage, NoTireDamage,
            NoLockpick, NoBarricades, NoStructures, NoBarricadeSalvage, NoStructureSalvage, NoItemEquip, NoItemUnequip,
            NoItemDrop, NoEnter, NoLeave, NoZombie, InfiniteGenerator, NoVehicleCarjack, NoVehicleSiphoning
        };

        /// <summary>
        /// A flag indicating that no damage is allowed.
        /// </summary>
        public const string NoDamage = "NoDamage";
        /// <summary>
        /// A flag indicating that no vehicle damage is allowed.
        /// </summary>
        public const string NoVehicleDamage = "NoVehicleDamage";
        /// <summary>
        /// A flag indicating that player damage is allowed.
        /// </summary>
        public const string AllowPlayerDamage = "AllowPlayerDamage";
        /// <summary>
        /// A flag indicating that no player damage is allowed.
        /// </summary>
        public const string NoPlayerDamage = "NoPlayerDamage";
        /// <summary>
        /// A flag indicating that no animal damage is allowed.
        /// </summary>
        public const string NoAnimalDamage = "NoAnimalDamage";
        /// <summary>
        /// A flag indicating that no zombie damage is allowed.
        /// </summary>
        public const string NoZombieDamage = "NoZombieDamage";
        /// <summary>
        /// A flag indicating that no tire damage is allowed.
        /// </summary>
        public const string NoTireDamage = "NoTireDamage";
        /// <summary>
        /// A flag indicating that no lockpicking is allowed.
        /// </summary>
        public const string NoLockpick = "NoLockpick";
        /// <summary>
        /// A flag indicating that no barricades can be placed.
        /// </summary>
        public const string NoBarricades = "NoBarricades";
        /// <summary>
        /// A flag indicating that no structures can be placed.
        /// </summary>
        public const string NoStructures = "NoStructures";
        /// <summary>
        /// A flag indicating that no barricades can be salvaged.
        /// </summary>
        public const string NoBarricadeSalvage = "NoBarricadeSalvage";
        /// <summary>
        /// A flag indicating that no structures can be salvaged.
        /// </summary>
        public const string NoStructureSalvage = "NoStructureSalvage";
        /// <summary>
        /// A flag indicating that no items can be equipped.
        /// </summary>
        public const string NoItemEquip = "NoItemEquip";
        /// <summary>
        /// A flag indicating that no items can be unequipped.
        /// </summary>
        public const string NoItemUnequip = "NoItemUnequip";
        /// <summary>
        /// A flag indicating that no items can be dropped.
        /// </summary>
        public const string NoItemDrop = "NoItemDrop";
        /// <summary>
        /// A flag indicating that no entry is allowed into the zone.
        /// </summary>
        public const string NoEnter = "NoEnter";
        /// <summary>
        /// A flag indicating that no exit is allowed from the zone.
        /// </summary>
        public const string NoLeave = "NoLeave";
        /// <summary>
        /// A flag indicating that zombies are not allowed within the zone.
        /// </summary>
        public const string NoZombie = "NoZombie";
        /// <summary>
        /// A flag indicating that the generator in the zone is infinite.
        /// </summary>
        public const string InfiniteGenerator = "InfiniteGenerator";
        /// <summary>
        /// A flag indicating that vehicle carjacking is not allowed.
        /// </summary>
        public const string NoVehicleCarjack = "NoVehicleCarjack";
        /// <summary>
        /// A flag indicating that vehicle siphoning is not allowed.
        /// </summary>
        public const string NoVehicleSiphoning = "NoVehicleSiphoning";
    }
}