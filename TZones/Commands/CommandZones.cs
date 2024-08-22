using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Unturned.Player;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers.Unturned;
using Tavstal.TLibrary.Models.Commands;
using Tavstal.TLibrary.Models.Plugin;
using Tavstal.TZones.Models.Core;
using Tavstal.TZones.Models.Enums;
using Tavstal.TZones.Utils.Managers;

namespace Tavstal.TZones.Commands
{
    public class CommandZones: CommandBase
    {
        protected override IPlugin Plugin => TZones.Instance; 
        public override AllowedCaller AllowedCaller => AllowedCaller.Both;
        public override string Name => "zones";
        public override string Help => "Manage zones.";
        public override string Syntax => "add | list | remove";
        public override List<string> Aliases => new List<string> { "regions" };
        public override List<string> Permissions => new List<string> { "tzones.command.zones" };

        // 'help' subcommand is built-in, you don't need to add it
        protected override List<SubCommand> SubCommands => new List<SubCommand>
        {
            new SubCommand("add", "", "add [zone | node | flag | event | block]", new List<string>(), new List<string> { "tzones.command.zones.add" }, 
                async (caller, args) =>
                {
                    if (args.Length < 1)
                    {
                        TZones.Instance.SendCommandReply(caller, "command_zones_add_syntax");
                        return;
                    }

                    if (!(caller is UnturnedPlayer player))
                    {
                        TZones.Instance.SendCommandReply(caller, "error_not_player");
                        return;
                    }

                    switch (args[0].ToLower())
                    {
                        case "zone":
                        {
                            if (args.Length != 3)
                            {
                                TZones.Instance.SendCommandReply(caller, "command_zones_add_zone_syntax");
                                return;
                            }

                            Zone zone = ZonesManager.Zones.Find(x => x.Name == args[1]);
                            if (zone != null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_already_exist", zone.Name);
                                return;
                            }
                            
                            await TZones.DatabaseManager.AddZoneAsync(args[1], args[2], ulong.Parse(caller.Id));
                            ZonesManager.SetDirty();
                            
                            TZones.Instance.SendCommandReply(caller, "command_zones_add_zone", args[1]);
                            break;
                        }
                        case "node":
                        {
                            if (args.Length < 2 || args.Length > 3)
                            {
                                TZones.Instance.SendCommandReply(caller, "command_zones_add_node_syntax");
                                return;
                            }

                            if (args.Length == 2)
                                args[2] = "none";
                            
                            Zone zone = ZonesManager.Zones.Find(x => x.Name == args[1]);
                            if (zone == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_not_found", args[1]);
                                return;
                            }
                            
                            ENodeType nodeType;
                            switch (args[2].ToLower())
                            {
                                case "none":
                                {
                                    nodeType = ENodeType.None;
                                    break;
                                }
                                case "lower":
                                case "low":
                                {
                                    nodeType = ENodeType.Lower;
                                    break;
                                }
                                case "upper":
                                case "up":
                                {
                                    nodeType = ENodeType.Upper;
                                    break;
                                }
                                default:
                                {
                                    TZones.Instance.SendCommandReply(caller, "error_node_type_not_found", args[2]);
                                    return;
                                }
                            }
                            
                            await TZones.DatabaseManager.AddNodeAsync(zone.Id, player.Position.x, player.Position.y, player.Position.z, nodeType);
                            ZonesManager.SetDirty();
                            
                            TZones.Instance.SendCommandReply(caller, "command_zones_add_node");
                            break;
                        }
                        case "flag":
                        {
                            if (args.Length != 3)
                            {
                                TZones.Instance.SendCommandReply(caller, "command_zones_add_flag_syntax");
                                return;
                            }
                            
                            Zone zone = ZonesManager.Zones.Find(x => x.Name == args[1]);
                            if (zone == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_not_found", args[1]);
                                return;
                            }
                            
                            Flag flag = ZonesManager.Flags.Find(x => x.Name == args[2]);
                            if (flag == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_flag_not_found", args[2]);
                                return;
                            }

                            ZoneFlag zoneFlag = ZonesManager.ZoneFlags[zone.Id]?.Find(x => x.FlagId == flag.Id);
                            if (zoneFlag != null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zoneflag_already_exist");
                                return;
                            }
                            
                            await TZones.DatabaseManager.AddZoneFlagAsync(zone.Id, flag.Id);
                            ZonesManager.SetDirty();
                            
                            TZones.Instance.SendCommandReply(caller, "command_zones_add_flag");
                            break;
                        }
                        case "event":
                        {
                            if (args.Length != 4)
                            {
                                TZones.Instance.SendCommandReply(caller, "command_zones_add_event_syntax");
                                return;
                            }
                            
                            Zone zone = ZonesManager.Zones.Find(x => x.Name == args[1]);
                            if (zone == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_not_found", args[1]);
                                return;
                            }

                            EEventType eventType;
                            try
                            {
                                eventType = (EEventType)Enum.Parse(typeof(EEventType), args[2], true);
                            }
                            catch
                            {
                                TZones.Instance.SendCommandReply(caller, "error_event_type_not_found", args[2]);
                                return;
                            }
                            
                            await TZones.DatabaseManager.AddZoneEventAsync(zone.Id, eventType, args[3]);
                            ZonesManager.SetDirty();
                            
                            TZones.Instance.SendCommandReply(caller, "command_zones_add_event");
                            break;
                        }
                        case "block":
                        {
                            if (args.Length != 4)
                            {
                                TZones.Instance.SendCommandReply(caller, "command_zones_add_block_syntax");
                                return;
                            }
                            
                            Zone zone = ZonesManager.Zones.Find(x => x.Name == args[1]);
                            if (zone == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_not_found", args[1]);
                                return;
                            }
                            
                            EBlockType blockType;
                            try
                            {
                                blockType = (EBlockType)Enum.Parse(typeof(EBlockType), args[2], true);
                            }
                            catch
                            {
                                TZones.Instance.SendCommandReply(caller, "error_block_type_not_found", args[2]);
                                return;
                            }
                    
                            ushort unturnedId = 0;
                            try
                            {
                                unturnedId = ushort.Parse(args[3]);
                            }
                            catch { /* ignored */}
                            
                            
                            await TZones.DatabaseManager.AddBlockAsync(zone.Id, unturnedId, blockType);
                            ZonesManager.SetDirty();
                            
                            TZones.Instance.SendCommandReply(caller, "command_zones_add_block");
                            break;
                        }
                        default:
                        {
                            TZones.Instance.SendCommandReply(caller, "command_zones_add_syntax");
                            break;
                        }
                    }
                }),
            new SubCommand("list", "", "list [[zone] <page> | [node | flag | event | block] [zoneName] <page>]", new List<string>(), new List<string> { "tzones.command.zones.list" }, 
                (caller, args) =>
                {
                    if (args.Length < 1)
                    {
                        TZones.Instance.SendCommandReply(caller, "command_zones_list_syntax");
                        return Task.CompletedTask;
                    }
                    
                    if (args.Length < 2 && !args[0].ToLower().Equals("zone"))
                    {
                        TZones.Instance.SendCommandReply(caller, "command_zones_list_syntax");
                        return Task.CompletedTask;
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
                                return Task.CompletedTask;
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
                                return Task.CompletedTask;
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
                                return Task.CompletedTask;
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
                                return Task.CompletedTask;
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
                            return Task.CompletedTask;
                        }
                    }
                    
                    if (reachedEnd || maxPage <= page + 1)
                        TZones.Instance.SendCommandReply(caller, "command_zones_list_end");
                    else
                        TZones.Instance.SendCommandReply(caller, "command_zones_list_next", nextPage);
                    
                    return Task.CompletedTask;
                }),
            new SubCommand("remove", "", "remove [zone | node | flag | event | block]", new List<string>(), new List<string> { "tzones.command.zones.remove" }, 
                async (caller, args) =>
                {
                    if (args.Length < 1)
                    {
                        TZones.Instance.SendCommandReply(caller, "command_zones_remove_syntax");
                        return;
                    }

                    switch (args[0].ToLower())
                    {
                        case "zone":
                        {
                            if (args.Length != 2)
                            {
                                TZones.Instance.SendCommandReply(caller, "command_zones_remove_zone_syntax");
                                return;
                            }

                            Zone zone = ZonesManager.Zones.Find(x => x.Name == args[1]);
                            if (zone == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_not_found", args[1]);
                                return;
                            }

                            await TZones.DatabaseManager.RemoveZoneAsync(zone.Id);
                            ZonesManager.SetDirty();
                            
                            TZones.Instance.SendCommandReply(caller, "command_zones_remove_zone", args[1]);
                            break;
                        }
                        case "node":
                        {
                            if (args.Length != 3)
                            {
                                TZones.Instance.SendCommandReply(caller, "command_zones_remove_node_syntax");
                                return;
                            }

                            Zone zone = ZonesManager.Zones.Find(x => x.Name == args[1]);
                            if (zone == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_not_found", args[1]);
                                return;
                            }

                            ulong nodeId = 0;
                            try
                            {
                                nodeId = ulong.Parse(args[2]);
                            }
                            catch { /* ignore */}
                            
                            Node node = ZonesManager.Nodes[zone.Id]?.Find(x => x.Id == nodeId);
                            if (node == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_node_not_found", zone.Name, args[2]);
                                return;
                            }

                            await TZones.DatabaseManager.RemoveNodeAsync(node);
                            ZonesManager.SetDirty();
                            
                            TZones.Instance.SendCommandReply(caller, "command_zones_remove_node", node.Id);
                            break;
                        }
                        case "flag":
                        {
                            if (args.Length != 3)
                            {
                                TZones.Instance.SendCommandReply(caller, "command_zones_remove_flag_syntax");
                                return;
                            }
                            
                            Zone zone = ZonesManager.Zones.Find(x => x.Name == args[1]);
                            if (zone == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_not_found", args[1]);
                                return;
                            }
                            
                            ulong flagId = 0;
                            try
                            {
                                flagId = ulong.Parse(args[2]);
                            }
                            catch { /* ignore */}
                    
                            ZoneFlag flag = ZonesManager.ZoneFlags[zone.Id]?.Find(x => x.FlagId == flagId);
                            if (flag == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zoneflag_not_found", zone.Name, args[2]);
                                return;
                            }

                            await TZones.DatabaseManager.RemoveZoneFlagAsync(zone.Id, flag.FlagId);
                            ZonesManager.SetDirty();
                            
                            TZones.Instance.SendCommandReply(caller, "command_zones_remove_flag", flag.FlagId, zone.Name);
                            break;
                        }
                        case "event":
                        {
                            if (args.Length != 3)
                            {
                                TZones.Instance.SendCommandReply(caller, "command_zones_remove_event_syntax");
                                return;
                            }
                            
                            Zone zone = ZonesManager.Zones.Find(x => x.Name == args[1]);
                            if (zone == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_not_found", args[1]);
                                return;
                            }
                            
                            EEventType eventType;
                            try
                            {
                                eventType = (EEventType)Enum.Parse(typeof(EEventType), args[2], true);
                            }
                            catch
                            {
                                TZones.Instance.SendCommandReply(caller, "error_event_type_not_found", args[2]);
                                return;
                            }
                            
                            ZoneEvent zoneEvent = ZonesManager.ZoneEvents[zone.Id]?.Find(x => x.Type == eventType);
                            if (zoneEvent == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zoneevent_not_found", zone.Name, args[2]);
                                return;
                            }

                            await TZones.DatabaseManager.RemoveZoneEventAsync(zone.Id, zoneEvent.Type);
                            ZonesManager.SetDirty();
                            
                            TZones.Instance.SendCommandReply(caller, "command_zones_remove_event", zoneEvent.Type.ToString(), zone.Name);
                            break;
                        }
                        case "block":
                        {
                            if (args.Length != 4)
                            {
                                TZones.Instance.SendCommandReply(caller, "command_zones_remove_block_syntax");
                                return;
                            }
                            
                            Zone zone = ZonesManager.Zones.Find(x => x.Name == args[1]);
                            if (zone == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_not_found", args[1]);
                                return;
                            }

                            EBlockType blockType;
                            try
                            {
                                blockType = (EBlockType)Enum.Parse(typeof(EBlockType), args[2], true);
                            }
                            catch
                            {
                                TZones.Instance.SendCommandReply(caller, "error_block_type_not_found", args[2]);
                                return;
                            }

                            ushort id = 0;
                            try
                            {
                                id = ushort.Parse(args[3]);
                            }
                            catch { /* ignored */}

                            Block block = ZonesManager.ZoneBlocks[zone.Id]?.Find(x => x.Type == blockType && x.UnturnedId == id);
                            if (block == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_block_not_found", blockType.ToString(), id);
                                return;
                            }

                            await TZones.DatabaseManager.RemoveBlockAsync(block);
                            ZonesManager.SetDirty();
                            
                            TZones.Instance.SendCommandReply(caller, "command_zones_remove_block");
                            break; 
                        }
                        default:
                        {
                            TZones.Instance.SendCommandReply(caller, "command_zones_remove_syntax");
                            break;
                        }
                    }
                })
        };

        protected override Task<bool> ExecutionRequested(IRocketPlayer caller, string[] args)
        {
            return Task.FromResult(true);
        }
    }
}