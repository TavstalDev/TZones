using System;
using System.Collections.Concurrent;
using Rocket.Unturned.Player;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Models.Logging;
using Tavstal.TZones.Components;

namespace Tavstal.TZones.Utils.Managers
{
    public static class ComponentManager
    {
        private static readonly ConcurrentDictionary<string, ZoneComponent> _components = new ConcurrentDictionary<string, ZoneComponent>();
        private static TLogger Logger => TZones.Logger;

        public static ZoneComponent Get(UnturnedPlayer player) => _components.GetOrAdd(player.Id, player.GetComponent<ZoneComponent>());

        public static void Invalidate(string id)
        {
            try
            {
                _components.TryRemove(id, out _);
            }
            catch (Exception ex)
            {
                Logger.Error($"Unexpected occured while invalidating {id}'s component.", ex);
            }
        }
    }
}