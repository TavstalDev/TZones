using Tavstal.TZones.Models.Enums;
using YamlDotNet.Serialization;

namespace Tavstal.TZones.Models.Config
{
    public class GlobalZoneFlagChecks
    {
        [YamlMember(Order = 0)]
        public EGlobalCheckMode NoDamage { get; set; } = EGlobalCheckMode.NOT_IN_ZONE;
 
        [YamlMember(Order = 1)]
        public EGlobalCheckMode NoVehicleDamage { get; set; } = EGlobalCheckMode.NOT_IN_ZONE;

        [YamlMember(Order = 2)]
        public EGlobalCheckMode AllowPlayerDamage { get; set; } = EGlobalCheckMode.NEVER;

        [YamlMember(Order = 3)]
        public EGlobalCheckMode NoPlayerDamage { get; set; } = EGlobalCheckMode.NOT_IN_ZONE;

        [YamlMember(Order = 4)]
        public EGlobalCheckMode NoAnimalDamage { get; set; } = EGlobalCheckMode.NOT_IN_ZONE;

        [YamlMember(Order = 5)]
        public EGlobalCheckMode NoZombieDamage { get; set; } = EGlobalCheckMode.NOT_IN_ZONE;

        [YamlMember(Order = 6)]
        public EGlobalCheckMode NoTireDamage { get; set; } = EGlobalCheckMode.NOT_IN_ZONE;
        
        [YamlMember(Order = 7)]
        public EGlobalCheckMode NoLockpick { get; set; } = EGlobalCheckMode.NOT_IN_ZONE;
        
        [YamlMember(Order = 8)]
        public EGlobalCheckMode NoBarricades { get; set; } = EGlobalCheckMode.NOT_IN_ZONE;

        [YamlMember(Order = 9)]
        public EGlobalCheckMode NoStructures { get; set; } = EGlobalCheckMode.NOT_IN_ZONE;

        [YamlMember(Order = 10)]
        public EGlobalCheckMode NoBarricadeSalvage { get; set; } = EGlobalCheckMode.NOT_IN_ZONE;
        
        [YamlMember(Order = 11)]
        public EGlobalCheckMode NoStructureSalvage { get; set; } = EGlobalCheckMode.NOT_IN_ZONE;

        [YamlMember(Order = 12)]
        public EGlobalCheckMode NoItemEquip { get; set; } = EGlobalCheckMode.NOT_IN_ZONE;

        [YamlMember(Order = 13)]
        public EGlobalCheckMode NoItemUnequip { get; set; } = EGlobalCheckMode.NOT_IN_ZONE;

        [YamlMember(Order = 14)]
        public EGlobalCheckMode NoItemDrop { get; set; } = EGlobalCheckMode.NOT_IN_ZONE;

        [YamlMember(Order = 15)]
        public EGlobalCheckMode NoEnter { get; set; } = EGlobalCheckMode.NOT_IN_ZONE;
        
        [YamlMember(Order = 16)]
        public EGlobalCheckMode NoLeave { get; set; } = EGlobalCheckMode.NOT_IN_ZONE;

        [YamlMember(Order = 17)]
        public EGlobalCheckMode NoZombie { get; set; } = EGlobalCheckMode.NOT_IN_ZONE;

        [YamlMember(Order = 18)]
        public EGlobalCheckMode InfiniteGenerator { get; set; } = EGlobalCheckMode.NOT_IN_ZONE;

        [YamlMember(Order = 19)]
        public EGlobalCheckMode NoVehicleCarjack { get; set; } = EGlobalCheckMode.NOT_IN_ZONE;

        [YamlMember(Order = 20)]
        public EGlobalCheckMode NoVehicleSiphoning { get; set; } = EGlobalCheckMode.NOT_IN_ZONE;
        
        [YamlMember(Order = 21)]
        public EGlobalCheckMode NoVehicleEnter { get; set; } = EGlobalCheckMode.NOT_IN_ZONE;
        
        [YamlMember(Order = 22)]
        public EGlobalCheckMode NoVehicleExit { get; set; } = EGlobalCheckMode.NOT_IN_ZONE;
    }
}