using Tavstal.RocketFlow.Attributes;
using Tavstal.RocketFlow.Core;
using Tavstal.RocketFlow.Events.Level;
using Tavstal.TLibrary.Extensions;
using Tavstal.TZones.Utils.Managers;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace Tavstal.TZones.Handlers
{
    public class LevelEventListener : EventListener
    {
        [EventHandler]
        private void OnPostLevelLoaded(LevelPostLoadEvent e)
        {
            if (TZones.DatabaseManager.IsAuthenticationFailed)
            {
                TZones.Logger.Warning($"# Unloading {TZones.Instance.GetPluginName()} due to database authentication error.");
                TZones.Instance.UnloadPlugin();
                return;
            }

            ZoneManager.Cache.RefreshGeneratorCache();
            ZoneManager.Cache.MakeDirty();
        }
    }
}