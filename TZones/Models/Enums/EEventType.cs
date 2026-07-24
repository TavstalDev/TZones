namespace Tavstal.TZones.Models.Enums
{
    /// <summary>
    /// Defines the types of events that can be triggered on zone enter or leave.
    /// </summary>
    public enum EEventType
    {
        /// <summary>Displays a message to the player on zone enter.</summary>
        MESSAGE_ENTER = 0,
        /// <summary>Displays a message to the player on zone leave.</summary>
        MESSAGE_LEAVE = 1,
        /// <summary>Adds the player to a permission group on zone enter.</summary>
        ADD_GROUP_ENTER = 2,
        /// <summary>Adds the player to a permission group on zone leave.</summary>
        ADD_GROUP_LEAVE = 3,
        /// <summary>Removes the player from a permission group on zone enter.</summary>
        REMOVE_GROUP_ENTER = 4,
        /// <summary>Removes the player from a permission group on zone leave.</summary>
        REMOVE_GROUP_LEAVE = 5,
        /// <summary>Triggers a visual effect on zone enter.</summary>
        ADD_EFFECT_ENTER = 6,
        /// <summary>Triggers a visual effect on zone leave.</summary>
        ADD_EFFECT_LEAVE = 7,
        /// <summary>Removes a visual effect on zone enter.</summary>
        REMOVE_EFFECT_ENTER = 8,
        /// <summary>Removes a visual effect on zone leave.</summary>
        REMOVE_EFFECT_LEAVE = 9,
    }
}