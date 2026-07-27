using System;
using System.Collections;
using SDG.Unturned;
using System.Collections.Generic;
using System.Text;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Models.Logging;
using Tavstal.TZones.Utils.Managers;
using Tavstal.TLibrary.Models.Plugin;
using Tavstal.TLibrary.Threading;
using Tavstal.TZones.Handlers;
using UnityEngine;

namespace Tavstal.TZones
{
    /// <summary>
    /// The main plugin class.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class TZones : PluginBase<ZonesConfig>
    {
        /// <summary>
        /// Singleton instance of the plugin.
        /// </summary>
        public static TZones Instance { get; private set; } = null!;

        /// <summary>
        /// Manages all database operations for zones, flags, nodes, and restrictions.
        /// </summary>
        public static DatabaseManager DatabaseManager { get; private set; } = null!;

        private static Coroutine? _updateRoutine;
        private static bool isInitialized;

        /// <summary>
        /// Called before the plugin is loaded. Prints the plugin banner to the console.
        /// </summary>
        public override void OnPreLoad()
        {
            Instance = this;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("────────────────────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine("████████╗███████╗ ██████╗ ███╗   ██╗███████╗███████╗");
            sb.AppendLine("╚══██╔══╝╚══███╔╝██╔═══██╗████╗  ██║██╔════╝██╔════╝");
            sb.AppendLine("   ██║     ███╔╝ ██║   ██║██╔██╗ ██║█████╗  ███████╗");
            sb.AppendLine("   ██║    ███╔╝  ██║   ██║██║╚██╗██║██╔══╝  ╚════██║");
            sb.AppendLine("   ██║   ███████╗╚██████╔╝██║ ╚████║███████╗███████║");
            sb.AppendLine("   ╚═╝   ╚══════╝ ╚═════╝ ╚═╝  ╚═══╝╚══════╝╚══════╝");
            sb.AppendLine();
            sb.AppendLine("[ About ]");
            sb.AppendLine(" ▸ Developer : Tavstal");
            sb.AppendLine(" ▸ Discord   : @Tavstal");
            sb.AppendLine(" ▸ Website   : https://redstoneplugins.com");
            sb.AppendLine(" ▸ GitHub    : https://github.com/TavstalDev");
            sb.AppendLine();
            sb.AppendLine("[ Build ]");
            sb.AppendLine($" ▸ Version   : {Version}");
            sb.AppendLine($" ▸ Build Date: {BuildDate} UTC");
            sb.AppendLine($" ▸ TLibrary  : {LibraryVersion}");
            sb.AppendLine();
            sb.AppendLine("[ Support ]");
            sb.AppendLine(" ▸ Report issues or request features:");
            sb.AppendLine(" ▸ https://github.com/TavstalDev/TZones/issues");
            sb.AppendLine();
            sb.AppendLine("────────────────────────────────────────────────────────");
            Logger.Log(ELogLevel.COMMAND, sb.ToString(), includePrefixes: false, color:  ConsoleColor.Cyan);
        }
        
        /// <summary>
        /// Fired when the plugin is loaded.
        /// </summary>
        public override void OnLoad()
        {
            Instance = this;

            // Attach event, which will be fired when all plugins are loaded.
            Level.onPostLevelLoaded += OnPluginsLoaded;
            // Attach player related events
            BarricadeEventHandler.AttachEvents();
            EntityEventHandler.AttachEvents();
            PlayerEventHandler.AttachEvents();
            StructureEventHandler.AttachEvents();
            VehicleEventHandler.AttachEvents();
            ZonesEventHandler.AttachEvents();

            DatabaseManager = new DatabaseManager(this, Config);
            if (DatabaseManager.IsAuthenticationFailed)
                return;

            isInitialized = true;
            _updateRoutine = StartCoroutine(UpdateRoutine());
            Logger.Info($"# {Name} has been loaded.");
        }

        /// <summary>
        /// Fired when the plugin is unloaded.
        /// </summary>
        public override void OnUnLoad()
        {
            Level.onPostLevelLoaded -= OnPluginsLoaded;
            BarricadeEventHandler.DetachEvents();
            EntityEventHandler.DetachEvents();
            PlayerEventHandler.DetachEvents();
            StructureEventHandler.DetachEvents();
            VehicleEventHandler.DetachEvents();
            ZonesEventHandler.DetachEvents();

            isInitialized = false;
            if (_updateRoutine != null)
            {
                StopCoroutine(_updateRoutine);
                _updateRoutine = null;
            }
            Logger.Info($"# {Name} has been successfully unloaded.");
        }

        /// <summary>
        /// Called after all plugins have loaded. Validates the database connection and refreshes the zone cache.
        /// </summary>
        /// <param name="i">The level index passed by the level loaded event.</param>
        private void OnPluginsLoaded(int i)
        {
            if (DatabaseManager.IsAuthenticationFailed)
            {
                Logger.Warning($"# Unloading {GetPluginName()} due to database authentication error.");
                this.UnloadPlugin();
                return;
            }

            ZoneManager.Cache.RefreshGeneratorCache();
            ZoneManager.Cache.MakeDirty();
        }

        /// <summary>
        /// Gets the localization key-value pairs for the plugin.
        /// </summary>
        public override Dictionary<string, string> LanguagePacks => new Dictionary<string, string>();

        /// <summary>
        /// Gets the default localization strings used when no language pack is loaded.
        /// </summary>
        public override Dictionary<string, string> DefaultLocalization => ZonesDefaultLocalizations.Values;
        
        /// <summary>
        /// Runs the periodic update loop that triggers zone updates on a background thread.
        /// </summary>
        private IEnumerator UpdateRoutine() {
            var waitTime = new WaitForSeconds(1f);

            while (true) {
                yield return waitTime;

                if (!isInitialized || DatabaseManager.IsAuthenticationFailed)
                    continue;

                if (ZoneManager.Updater.IsUpdating)
                    continue;

                ZoneManager.Updater.IsUpdating  = true;
                
                BackgroundThreadDispatcher.Run(async () => {
                    try {
                        await ZoneManager.Updater.UpdateAsync();
                    }
                    catch (Exception ex) {
                        Logger.Error("Unexpected error occured while running background update task.", ex);
                    }
                    finally
                    {
                        ZoneManager.Updater.IsUpdating = false;
                    }
                });
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}