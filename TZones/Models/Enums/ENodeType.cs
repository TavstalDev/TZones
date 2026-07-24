namespace Tavstal.TZones.Models.Enums
{
    /// <summary>
    /// Defines the types of nodes used to construct zone boundaries.
    /// </summary>
    public enum ENodeType
    {
        /// <summary>A standard boundary node used for 2D polygon containment checks.</summary>
        NONE = 0,
        /// <summary>Defines the upper height limit of the zone.</summary>
        UPPER = 1,
        /// <summary>Defines the lower height limit of the zone.</summary>
        LOWER = 2,
    }
}