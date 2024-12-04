using System.Collections.Generic;

namespace Tavstal.TZones.Utils.Constants
{
    public static class Flags
    {
        public static List<string> Defaults { get; } = new List<string>()
        {
            Damage, VehicleDamage, PlayerDamage, AnimalDamage, ZombieDamage, TireDamage,
            Lockpick, Barricades, Structures, BarricadeSalvage, StructureSalvage, ItemEquip, ItemUnequip,
            ItemDrop, Enter, Leave, Zombie, InfiniteGenerator, VehicleCarjack, VehicleSiphoning
        };

        public const string Damage = "NoDamage";
        public const string VehicleDamage = "NoVehicleDamage";
        public const string PlayerDamage = "NoPlayerDamage";
        public const string AnimalDamage = "NoAnimalDamage";
        public const string ZombieDamage = "NoZombieDamage";
        public const string TireDamage = "NoTireDamage";
        public const string Lockpick = "NoLockpick";
        public const string Barricades = "NoBarricades";
        public const string Structures = "NoStructures";
        public const string BarricadeSalvage = "NoBarricadeSalvage";
        public const string StructureSalvage = "NoStructureSalvage";
        public const string ItemEquip = "NoItemEquip";
        public const string ItemUnequip = "NoItemUnequip";
        public const string ItemDrop = "NoItemDrop";
        public const string Enter = "NoEnter";
        public const string Leave = "NoLeave";
        public const string Zombie = "NoZombie";
        public const string InfiniteGenerator = "InfiniteGenerator";
        public const string VehicleCarjack = "NoVehicleCarjack";
        public const string VehicleSiphoning = "NoVehicleSiphoning";
        
    }
}