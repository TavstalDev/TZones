using Rocket.API;
using System.Collections.Generic;
using System.Linq;
using MySqlX.XDevAPI.Common;
using Tavstal.TLibrary.Compatibility;
using Tavstal.TLibrary.Compatibility.Interfaces;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers.Unturned;
using Tavstal.TZones.Models.Core;
using Tavstal.TZones.Utils.Managers;

namespace Tavstal.TZones.Commands
{
    public class CommandZones: CommandBase
    {
        public override IPlugin Plugin => TZones.Instance; 
        public override AllowedCaller AllowedCaller => AllowedCaller.Both;
        public override string Name => "zones";
        public override string Help => "Manage zones.";
        public override string Syntax => "list | add | edit | remove";
        public override List<string> Aliases => new List<string>() { "regions" };
        public override List<string> Permissions => new List<string> { "tzones.command.zones" };

        // 'help' subcommand is built-in, you don't need to add it
        public override List<SubCommand> SubCommands => new List<SubCommand>()
        {
            new SubCommand("add", "", "add [zone | node | flag | event | block]", new List<string>(), new List<string>() { "tzones.command.zones.add" }, 
                async (IRocketPlayer caller, string[] args) =>
                {
                    
                }),
            new SubCommand("list", "", "list [[zone] <page> | [node | flag | event | block] [zoneName] <page>]", new List<string>(), new List<string>() { "tzones.command.zones.list" }, 
                (IRocketPlayer caller, string[] args) =>
                {
                    if (args.Length < 1)
                    {
                        TZones.Instance.SendCommandReply(caller, "command_zones_list_syntax");
                        return;
                    }
                    
                    if (args.Length < 2 && !args[0].ToLower().Equals("zone"))
                    {
                        TZones.Instance.SendCommandReply(caller, "command_zones_list_syntax");
                        return;
                    }

                    bool reachedEnd = false;
                    int page = 1;
                    int maxPage = 1;
                    try
                    {
                        switch (args.Length)
                        {
                            case 2:
                            {
                                page = int.Parse(args[1]);
                                if (page < 1)
                                    page = 1;
                                break;
                            }
                            case 3:
                            {
                                page = int.Parse(args[2]);
                                if (page < 1)
                                    page = 1;
                                break;
                            }
                            default:
                                break;
                        }
                    } catch { /*ignore*/ }

                    string nextPage;
                    switch (args[0].ToLower())
                    {
                        case "zone":
                        {
                            nextPage = $"zone {page + 1}";
                            var list = ZonesManager.Zones;
                            maxPage += list.Count / 3;
                            
                            for (int i = 0; i < 3; i++) 
                            {
                                int index = i + 3 * (page - 1);
                                if (!list.IsValidIndex(index)) 
                                {
                                    reachedEnd = true;
                                    break;
                                }

                                var value = list[index];
                                TZones.Instance.SendCommandReply(caller, "command_zones_list_zone", value.Id, value.Name, value.Description);
                            }
                            break;
                        }
                        case "node":
                        {
                            nextPage = $"node {args[1]} {page + 1}";
                            Zone zone = ZonesManager.Zones.FirstOrDefault(x => x.Name.EqualsIgnoreCase(args[1]));
                            if (zone == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_not_found", args[1]);
                                return;
                            }
                            
                            var list = ZonesManager.Nodes[zone.Id];
                            maxPage += list.Count / 3;
                            
                            for (int i = 0; i < 3; i++) 
                            {
                                int index = i + 3 * (page - 1);
                                if (!list.IsValidIndex(index)) 
                                {
                                    reachedEnd = true;
                                    break;
                                }

                                var value = list[index];
                                TZones.Instance.SendCommandReply(caller, "command_zones_list_node", value.Id, value.Type.ToString(), value.X, value.Y, value.Z);
                            }
                            break;
                        }
                        case "flag":
                        {
                            nextPage = $"flag {args[1]} {page + 1}";
                            Zone zone = ZonesManager.Zones.FirstOrDefault(x => x.Name.EqualsIgnoreCase(args[1]));
                            if (zone == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_not_found", args[1]);
                                return;
                            }
                            
                            var list = ZonesManager.ZoneFlags[zone.Id];
                            maxPage += list.Count / 3;
                            
                            for (int i = 0; i < 3; i++) 
                            {
                                int index = i + 3 * (page - 1);
                                if (!list.IsValidIndex(index)) 
                                {
                                    reachedEnd = true;
                                    break;
                                }

                                var value = list[index];
                                Flag flag = ZonesManager.Flags.Find(x => x.Id == value.FlagId);
                                
                                TZones.Instance.SendCommandReply(caller, "command_zones_list_flag", flag?.Name ?? "null", value.FlagId);
                            }
                            break;
                        }
                        case "event":
                        {
                            nextPage = $"event {args[1]} {page + 1}";
                            Zone zone = ZonesManager.Zones.FirstOrDefault(x => x.Name.EqualsIgnoreCase(args[1]));
                            if (zone == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_not_found", args[1]);
                                return;
                            }
                            
                            var list = ZonesManager.ZoneEvents[zone.Id];
                            maxPage += list.Count / 3;
                            
                            for (int i = 0; i < 3; i++) 
                            {
                                int index = i + 3 * (page - 1);
                                if (!list.IsValidIndex(index)) 
                                {
                                    reachedEnd = true;
                                    break;
                                }

                                var value = list[index];
                                TZones.Instance.SendCommandReply(caller, "command_zones_list_event", value.Type.ToString(), value.Value);
                            }
                            break;
                        }
                        case "block":
                        {
                            nextPage = $"block {args[1]} {page + 1}";
                            Zone zone = ZonesManager.Zones.FirstOrDefault(x => x.Name.EqualsIgnoreCase(args[1]));
                            if (zone == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_not_found", args[1]);
                                return;
                            }
                            
                            var list = ZonesManager.ZoneBlocks[zone.Id];
                            maxPage += list.Count / 3;
                            
                            for (int i = 0; i < 3; i++) 
                            {
                                int index = i + 3 * (page - 1);
                                if (!list.IsValidIndex(index)) 
                                {
                                    reachedEnd = true;
                                    break;
                                }

                                var value = list[index];
                                TZones.Instance.SendCommandReply(caller, "command_zones_list_block", value.Type.ToString(), value.UnturnedId);
                            }
                            break;
                        }
                        default:
                        {
                            TZones.Instance.SendCommandReply(caller, "command_zones_list_syntax");
                            return;
                        }
                    }
                    
                    if (reachedEnd || maxPage <= page + 1)
                        TZones.Instance.SendCommandReply(caller, "command_zones_list_end");
                    else
                        TZones.Instance.SendCommandReply(caller, "command_zones_list_next", nextPage);
                }),
            new SubCommand("edit", "", "edit [zone | event]", new List<string>(), new List<string>() { "tzones.command.zones.edit" }, 
                async (IRocketPlayer caller, string[] args) =>
                {
                    
                }),
            new SubCommand("remove", "", "remove [zone | node | flag | event | block]", new List<string>(), new List<string>() { "tzones.command.zones.remove" }, 
                async (IRocketPlayer caller, string[] args) =>
                {
                    
                })
        };

        public override bool ExecutionRequested(IRocketPlayer caller, string[] args)
        {
            return true;
        }
    }
}