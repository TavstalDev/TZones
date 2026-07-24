namespace Tavstal.TZones.Models.Enums
{
    /// <summary>
    /// Defines the types of restrictions that can be applied to items or vehicles within a zone.
    /// </summary>
    public enum ERestrictionType
    {
        /// <summary>Blocks building/placing the item.</summary>
        BUILD = 0,
        /// <summary>Blocks equipping the item.</summary>
        EQUP = 1,
        /// <summary>Blocks unequipping the item.</summary>
        UNEQUIP = 2,
        /// <summary>Blocks entering the vehicle.</summary>
        VEHICLE_ENTER = 3,
        /// <summary>Blocks exiting the vehicle.</summary>
        VEHICLE_LEAVE = 4,
    }
}