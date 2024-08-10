using SDG.Unturned;
using System.Collections.Generic;
using Tavstal.TZones.Utils.Handlers;
using Tavstal.TZones.Utils.Managers;
using Tavstal.TLibrary.Compatibility;
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
        public new static readonly TLogger Logger = new TLogger("TZones", false);
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
                this.UnloadPlugin();
                return;
            }

            Logger.LogLateInit();
        }


        public override Dictionary<string, string> DefaultLocalization =>
           new Dictionary<string, string>
           {
               { "prefix", "&d[TZones]" },
               { "error_player_not_found", "&cPlayer was not found." },
               { "error_flag_not_found", "&cThe &e{0} &cflag does not exist." },
               { "error_zone_not_found", "&cThe &e{0} &czone does not exist." },
               { "error_node_not_found", "&cThe &e{1} &cnode does not exist in the &e{0} &czone." },
               { "error_zoneflag_not_found", "&cThe &e{1} &cflag does not exist in the &e{0} &czone." },
               { "error_event_type_not_found", "&cThe &e{0} &czone event type does not exist." },
               { "error_zoneevent_not_found", "&cThe &e{1} &azone event does not exist in the &e{0} &ezone." },
               { "error_block_type_not_found", "&cThe &e{0} &cblock type does not exist." },
               { "error_zone_block_not_found", "&cThe zone block with &e{0} &ctype and &e{1} &cid does not exist." },
               { "command_flags_add_syntax", "&cWrong syntax! Usage: /flags add [name] [description]" },
               { "command_flags_add_duplicate", "&cThe '{0}' flag already exists." },
               { "command_flags_add", "&aYou have successfully added the &e{0} &aflag." },
               { "command_flags_list_element", "&a{0} - {1}"},
               { "command_flags_list_end", "&aYou have reached the end of the list."},
               { "command_flags_list_next", "&aUse &e/flags list {0} &ato view the next page."},
               { "command_flags_remove_syntax", "&cWrong syntax! Usage: /flags remove [name]"},
               { "command_flags_remove_default", "&cYou can not remove a default flag." },
               { "command_flags_remove", "&aYou have successfully removed the '{0}' flag."},
               { "command_zones_add_syntax", "&cWrong syntax! Usage: /zones add [zone | node | flag | event | block]" },
               { "", "" },
               { "", "" },
               { "", "" },
               { "", "" },
               { "", "" },
               { "", "" },
               { "", "" },
               { "", "" },
               { "", "" },
               { "", "" },
               { "", "" },
               { "", "" },
               { "", "" },
               { "", "" },
               { "", "" },
               { "", "" },
               { "", "" },
               { "", "" },
               { "", "" },
               { "", "" },
               { "command_zones_list_syntax", "&cWrong syntax! Usage: /zones list [[zone] <page> | [node | flag | event | block] [zoneName] <page>]" },
               { "command_zones_list_zone", "&3- &aId: {0}, Name: {1}, Description: {2}" },
               { "command_zones_list_node", "&3- &aId: {0}, Type: {1}, X: {2}, Y: {3}, Z: {4}" },
               { "command_zones_list_flag", "&3- &aName: {0}, Id: {1}" },
               { "command_zones_list_event", "&3- &aType: {0}, Value: {1}" },
               { "command_zones_list_block", "&3- &aId: {1}, Type: {0}" },
               { "command_zones_list_next", "&aUse &e/zones list {0} &ato view the next page." },
               { "command_zones_list_end", "&aYou have reached the end of the list." },
               { "command_zones_remove_syntax", "&cWrong syntax! Usage: /zones remove [zone | node | flag | event | block]" },
               { "command_zones_remove_zone_syntax", "&cWrong syntax! Usage: /zones remove zone [zoneName]" },
               { "command_zones_remove_node_syntax", "&cWrong syntax! Usage: /zones remove node [zoneName] [nodeId]" },
               { "command_zones_remove_flag_syntax", "&cWrong syntax! Usage: /zones remove flag [zoneName] [flagId]" },
               { "command_zones_remove_event_syntax", "&cWrong syntax! Usage: /zones remove event [zoneName] [eventType]" },
               { "command_zones_remove_block_syntax", "&cWrong syntax! Usage: /zones remove block [zoneName] [blockType] [id]" },
               { "command_zones_remove_zone", "&aYou have successfully removed the &e{0} &azone." },
               { "command_zones_remove_node", "&aYou have successfully removed the &e{0} &anode." },
               { "command_zones_remove_flag", "&aYou have successfully removed the &e{0} &aflag from the &e{0} &azone." },
               { "command_zones_remove_event", "&aYou have successfully removed the &e{0} &aevent." },
               { "command_zones_remove_block", "&aYou have successfully removed the block." },
               { "", "" },
               { "", "" },
               { "", "" },
               { "", "" },
               { "", "" },
               { "", "" },
               { "", "" },
           };

        #region Unity Update
        #pragma warning disable IDE0051
        private async void Update() {
            _frame++;
            if (_frame % 10 != 0) {
                return;
            }

            await ZonesManager.CheckDirtyAsync();
            
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
                    if (currentZones.All(x => x.Id != zone.Id)) {
                        ZonesManager.FPlayerLeaveZone(uPlayer, zone);
                    }
                }

                foreach (Zone zone in currentZones) {
                    if (comp.Zones.All(x => x.Id != zone.Id)) {
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
                    foreach (var zombie in t.zombies.Where(z => z))
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

        // ReSharper disable Unity.PerformanceAnalysis
        private void UpdateGenerators(Zone zone)
        {
            if (!zone.HasFlag(Flags.InfiniteGenerator))
                return;
            InteractableGenerator[] generators = FindObjectsOfType<InteractableGenerator>();
            foreach (var generator in generators)
            {
                if (zone.IsPointInZone(generator.transform.position) &&
                    generator.fuel < generator.capacity - 10)
                    BarricadeManager.sendFuel(generator.transform, generator.capacity);
            }
        }
        #pragma warning restore IDE0051
        #endregion
    }
}