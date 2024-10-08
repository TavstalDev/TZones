using System.Collections.Generic;

namespace Tavstal.TZones.Utils.Constants
{
    public static class Flags
    {
        private static readonly List<string> _defaults = new List<string>()
        {
            NoDamage, NoVehicleDamage, NoPlayerDamage, NoAnimalDamage, NoZombieDamage, NoTireDamage,
            NoLockpick, NoBarricades, NoStructures, NoBarricadeSalvage, NoStructureSalvage, NoItemEquip, NoItemUnequip,
            NoItemDrop, NoEnter, NoLeave, NoZombie, InfiniteGenerator, NoVehicleCarjack, NoVehicleSiphoning
        };
        public static List<string> Defaults => _defaults;
        
        public const string NoDamage = "NoDamage";
        public const string NoVehicleDamage = "NoVehicleDamage";
        public const string NoPlayerDamage = "NoPlayerDamage";
        public const string NoAnimalDamage = "NoAnimalDamage";
        public const string NoZombieDamage = "NoZombieDamage";
        public const string NoTireDamage = "NoTireDamage";
        public const string NoLockpick = "NoLockpick";
        public const string NoBarricades = "NoBarricades";
        public const string NoStructures = "NoStructures";
        public const string NoBarricadeSalvage = "NoBarricadeSalvage";
        public const string NoStructureSalvage = "NoStructureSalvage";
        public const string NoItemEquip = "NoItemEquip";
        public const string NoItemUnequip = "NoItemUnequip";
        public const string NoItemDrop = "NoItemDrop";
        public const string NoEnter = "NoEnter";
        public const string NoLeave = "NoLeave";
        public const string NoZombie = "NoZombie";
        public const string InfiniteGenerator = "InfiniteGenerator";
        public const string NoVehicleCarjack = "NoVehicleCarjack";
        public const string NoVehicleSiphoning = "NoVehicleSiphoning";
        
    }
}