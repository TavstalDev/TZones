using SDG.Unturned;
using System.Collections.Generic;
using Tavstal.TZones.Utils.Handlers;
using Tavstal.TZones.Utils.Managers;
using Tavstal.TLibrary.Compatibility;
using Logger = Rocket.Core.Logging.Logger;
using Rocket.Unturned.Player;
using Tavstal.TZones.Components;
using Tavstal.TZones.Models.Core;
using System.Linq;
using Tavstal.TZones.Utils.Constants;
using UnityEngine;
using System.Threading.Tasks;

namespace Tavstal.TZones
{
    /// <summary>
    /// The main plugin class.
    /// </summary>
    public class TZones : PluginBase<TZonesConfig>
    {
        public new static TZones Instance { get; private set; }
        public new static TLogger Logger = new TLogger("TZones", false);
        public new static DatabaseManager DatabaseManager { get; private set; }
        /// <summary>
        /// Used to prevent error spamming that is related to database configuration.
        /// </summary>
        public static bool IsConnectionAuthFailed { get; set; }
        private static int _frame { get; set; }

        /// <summary>
        /// Fired when the plugin is loaded.
        /// </summary>
        public override void OnLoad()
        {
            Instance = this;
            _frame = 0;
            // Attach event, which will be fired when all plugins are loaded.
            Level.onPostLevelLoaded += Event_OnPluginsLoaded;
            // Attach player related events
            UnturnedEventHandler.AttachEvents();
            ZonesEventHandler.AttachEvents();

            Logger.Log("#########################################");
            Logger.Log("# Thanks for using my plugin");
            Logger.Log($"# Plugin Created By {VersionInfo.CompanyName}");
            Logger.Log("# Discord: @YourDiscordName");
            Logger.Log("# Website: https://your.website.example");
            Logger.Log("# Discord Guild: https://discord.gg/your_invite");
            // Please do not remove this region and its code, because the license require credits to the author.
            #region Credits to Tavstal
            Logger.Log("#########################################");
            Logger.Log($"# This plugin uses TLibrary.");
            Logger.Log($"# TLibrary Created By: Tavstal"); 
            Logger.Log($"# Github: https://github.com/TavstalDev/TLibrary/tree/master");
            #endregion
            Logger.Log("#########################################");
            Logger.Log($"# Build Version: {Version}");
            Logger.Log($"# Build Date: {BuildDate}");
            Logger.Log("#########################################");

            DatabaseManager = new DatabaseManager(this, Config);
            if (IsConnectionAuthFailed)
                return;

            Logger.Log($"# {Name} has been loaded.");
        }

        /// <summary>
        /// Fired when the plugin is unloaded.
        /// </summary>
        public override void OnUnLoad()
        {
            Level.onPostLevelLoaded -= Event_OnPluginsLoaded;
            UnturnedEventHandler.DetachEvents();
            ZonesEventHandler.DetachEvents();
            Logger.Log($"# {Name} has been successfully unloaded.");
        }

        private void Event_OnPluginsLoaded(int i)
        {
            if (IsConnectionAuthFailed)
            {
                Logger.LogWarning($"# Unloading {GetPluginName()} due to database authentication error.");
                this?.UnloadPlugin();
                return;
            }

            Logger.LogLateInit();
        }


        public override Dictionary<string, string> DefaultLocalization =>
           new Dictionary<string, string>
           {
               { "prefix", $"&e[{GetPluginName()}]" },
               { "error_player_not_found", "&cPlayer was not found." }
           };

#region Unity Update
#pragma warning disable IDE0051
        private void Update() {
            _frame++;
            if (_frame % 10 != 0) {
                return;
            }
            
            // Note: 
            // Future performance issues might be solved with Parallel.ForEach instead of regular foreach
            // Only use it at heavy load, or else it won't make it faster
            UpdatePlayers();

            // Update Generators & Zombies
            Parallel.ForEach(ZonesManager.Zones, zone => {
                UpdateGenerators(zone);
                UpdateZombies(zone);
            });
        }

        private void UpdatePlayers() 
        {
            foreach (SteamPlayer steamPlayer in Provider.clients) {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                ZonePlayerComponent comp = uPlayer.GetComponent<ZonePlayerComponent>();

                List<Zone> currentZones = ZonesManager.GetZonesFromPosition(uPlayer.Position);

                foreach (Zone zone in comp.Zones) {
                    if (!currentZones.Any(x => x.Id == zone.Id)) {
                        ZonesManager.FPlayerLeaveZone(uPlayer, zone);
                    }
                }

                foreach (Zone zone in currentZones) {
                    if (!comp.Zones.Any(x => x.Id == zone.Id)) {
                        ZonesManager.FPlayerEnterZone(uPlayer, zone);
                    }
                }

                comp.Zones = currentZones;

            }
        }

        private void UpdateZombies(Zone zone) 
        {
            if (zone.HasFlag(Flags.NoZombie) && ZombieManager.regions != null) 
            {
                foreach (ZombieRegion t in ZombieManager.regions.Where(t => t.zombies != null))
                {
                    foreach (var zombie in t.zombies.Where(z => z != null && z.transform?.position != null))
                    {
                        if (zombie.isDead) 
                            continue;
                        if (!zone.IsPointInZone(zombie.transform.position)) 
                            continue;
                        zombie.gear = 0;
                        zombie.isDead = true;
                        ZombieManager.sendZombieDead(zombie, Vector3.zero);
                    }
                }
            }
        }

        private void UpdateGenerators(Zone zone) {
            if (zone.HasFlag(Flags.InfiniteGenerator)) 
            {
                InteractableGenerator[] generators = FindObjectsOfType<InteractableGenerator>();
                foreach (var generator in generators)
                {
                    if (ZonesManager.IsPointInZone(zone, generator.transform.position) && generator.fuel < generator.capacity - 10)
                        BarricadeManager.sendFuel(generator.transform, generator.capacity);
                }
            }
        }
    #pragma warning restore IDE0051
    #endregion
    }
}