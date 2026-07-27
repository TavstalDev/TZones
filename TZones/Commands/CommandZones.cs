using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;
using Rocket.API;
using Rocket.Unturned.Player;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers.Unturned;
using Tavstal.TLibrary.Models.Commands;
using Tavstal.TLibrary.Models.Database;
using Tavstal.TLibrary.Models.Plugin;
using Tavstal.TZones.Models.Core;
using Tavstal.TZones.Models.Enums;
using Tavstal.TZones.Utils.Managers;
// ReSharper disable UnusedType.Global

namespace Tavstal.TZones.Commands
{
    /// <summary>
    /// Command for managing zones. Supports adding, listing, and removing zones, nodes, flags, events, and restrictions.
    /// </summary>
    public class CommandZones: CustomCommandBase
    {
        /// <inheritdoc/>
        public override IPlugin Plugin => TZones.Instance; 
        /// <inheritdoc/>
        public override bool UseBackgroundThread => false;
        /// <inheritdoc/>
        public override AllowedCaller AllowedCaller => AllowedCaller.Both;
        /// <inheritdoc/>
        public override string Name => "zones";
        /// <inheritdoc/>
        public override string Help => "Manage zones.";
        /// <inheritdoc/>
        public override string Syntax => "add | list | remove";
        /// <inheritdoc/>
        public override List<string> Aliases => new List<string> { "regions" };
        /// <inheritdoc/>
        public override List<string> Permissions => new List<string> { "tzones.command.zones" };

        // 'help' subcommand is built-in, you don't need to add it
        public override List<ISubcommand> SubCommands => new List<ISubcommand>
        {
            new SubCommand("add", "", "add [zone | node | flag | event | block]", new List<string>(), new List<string> { "tzones.command.zones.add" }, 
                Plugin, AllowedCaller,
                async (caller, args) =>
                {
                    if (args.Length < 1)
                    {
                        TZones.Instance.SendCommandReply(caller, "command_zones_add_syntax", TZones.Instance.Config.General.MessageIcon);
                        return;
                    }

                    if (!(caller is UnturnedPlayer player))
                    {
                        TZones.Instance.SendCommandReply(caller, "error_not_player", TZones.Instance.Config.General.MessageIcon);
                        return;
                    }

                    switch (args[0].ToLower())
                    {
                        case "zone":
                        {
                            if (args.Length != 3)
                            {
                                TZones.Instance.SendCommandReply(caller, "command_zones_add_zone_syntax", TZones.Instance.Config.General.MessageIcon);
                                return;
                            }

                            Zone? zone = ZoneManager.Queries.GetZone(args[1]);
                            if (zone != null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_already_exist", TZones.Instance.Config.General.MessageIcon, zone.Name);
                                return;
                            }

                            await TZones.DatabaseManager.Zones.AddAsync(new Zone
                            {
                                Name = args[1],
                                Description = args[2],
                                CreatorId = ulong.Parse(caller.Id),
                                CreationDate = DateTime.Now
                            });
                            ZoneManager.Cache.MakeDirty();
                            
                            TZones.Instance.SendCommandReply(caller, "command_zones_add_zone", TZones.Instance.Config.General.MessageIcon, args[1]);
                            break;
                        }
                        case "node":
                        {
                            if (args.Length < 2 || args.Length > 3)
                            {
                                TZones.Instance.SendCommandReply(caller, "command_zones_add_node_syntax", TZones.Instance.Config.General.MessageIcon);
                                return;
                            }

                            string type = "none";
                            if (args.Length > 2)
                                type = args[2];
                            
                            Zone? zone = ZoneManager.Queries.GetZone(args[1]);
                            if (zone == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_not_found", TZones.Instance.Config.General.MessageIcon, args[1]);
                                return;
                            }
                            
                            ENodeType nodeType;
                            switch (type.ToLower())
                            {
                                case "none":
                                {
                                    nodeType = ENodeType.NONE;
                                    break;
                                }
                                case "lower":
                                case "low":
                                {
                                    nodeType = ENodeType.LOWER;
                                    break;
                                }
                                case "upper":
                                case "up":
                                {
                                    nodeType = ENodeType.UPPER;
                                    break;
                                }
                                default:
                                {
                                    TZones.Instance.SendCommandReply(caller, "error_node_type_not_found", TZones.Instance.Config.General.MessageIcon, type);
                                    return;
                                }
                            }

                            await TZones.DatabaseManager.Nodes.AddAsync(new Node
                            {
                                ZoneId = zone.Id,
                                X = player.Position.x,
                                Y = player.Position.y,
                                Z = player.Position.z,
                                Type = nodeType
                            });
                            ZoneManager.Cache.MakeDirty();
                            
                            TZones.Instance.SendCommandReply(caller, "command_zones_add_node", TZones.Instance.Config.General.MessageIcon);
                            break;
                        }
                        case "flag":
                        {
                            if (args.Length != 3)
                            {
                                TZones.Instance.SendCommandReply(caller, "command_zones_add_flag_syntax", TZones.Instance.Config.General.MessageIcon);
                                return;
                            }
                            
                            Zone? zone = ZoneManager.Queries.GetZone(args[1]);
                            if (zone == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_not_found", TZones.Instance.Config.General.MessageIcon, args[1]);
                                return;
                            }
                            
                            Flag? flag = ZoneManager.Queries.GetFlag(args[2]);
                            if (flag == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_flag_not_found", TZones.Instance.Config.General.MessageIcon, args[2]);
                                return;
                            }

                            ZoneFlag? zoneFlag = ZoneManager.Queries.GetZoneFlag(zone.Id, flag.Id);
                            if (zoneFlag != null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zoneflag_already_exist", TZones.Instance.Config.General.MessageIcon);
                                return;
                            }

                            await TZones.DatabaseManager.ZoneFlags.AddAsync(new ZoneFlag
                            {
                                ZoneId = zone.Id,
                                FlagId = flag.Id
                            });
                            ZoneManager.Cache.MakeDirty();
                            
                            TZones.Instance.SendCommandReply(caller, "command_zones_add_flag", TZones.Instance.Config.General.MessageIcon);
                            break;
                        }
                        case "event":
                        {
                            if (args.Length != 4)
                            {
                                TZones.Instance.SendCommandReply(caller, "command_zones_add_event_syntax", TZones.Instance.Config.General.MessageIcon);
                                return;
                            }
                            
                            Zone? zone = ZoneManager.Queries.GetZone(args[1]);
                            if (zone == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_not_found", TZones.Instance.Config.General.MessageIcon, args[1]);
                                return;
                            }

                            EEventType eventType;
                            try
                            {
                                eventType = (EEventType)Enum.Parse(typeof(EEventType), args[2], true);
                            }
                            catch
                            {
                                TZones.Instance.SendCommandReply(caller, "error_event_type_not_found", TZones.Instance.Config.General.MessageIcon, args[2]);
                                return;
                            }

                            await TZones.DatabaseManager.ZoneEvents.AddAsync(new ZoneEvent
                            {
                                ZoneId = zone.Id,
                                Type = eventType,
                                Value = args[3]
                            });
                            ZoneManager.Cache.MakeDirty();
                            
                            TZones.Instance.SendCommandReply(caller, "command_zones_add_event", TZones.Instance.Config.General.MessageIcon);
                            break;
                        }
                        case "block":
                        {
                            if (args.Length != 4)
                            {
                                TZones.Instance.SendCommandReply(caller, "command_zones_add_block_syntax", TZones.Instance.Config.General.MessageIcon);
                                return;
                            }
                            
                            Zone? zone = ZoneManager.Queries.GetZone(args[1]);
                            if (zone == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_not_found", TZones.Instance.Config.General.MessageIcon, args[1]);
                                return;
                            }
                            
                            ERestrictionType restrictionType;
                            try
                            {
                                restrictionType = (ERestrictionType)Enum.Parse(typeof(ERestrictionType), args[2], true);
                            }
                            catch
                            {
                                TZones.Instance.SendCommandReply(caller, "error_block_type_not_found", TZones.Instance.Config.General.MessageIcon, args[2]);
                                return;
                            }
                    
                            ushort unturnedId = 0;
                            try
                            {
                                unturnedId = ushort.Parse(args[3]);
                            }
                            catch { /* ignored */}

                            await TZones.DatabaseManager.Restrictions.AddAsync(new Restriction
                            {
                                ZoneId = zone.Id,
                                Type = restrictionType,
                                UnturnedId = unturnedId
                            });
                            ZoneManager.Cache.MakeDirty();
                            
                            TZones.Instance.SendCommandReply(caller, "command_zones_add_block", TZones.Instance.Config.General.MessageIcon);
                            break;
                        }
                        default:
                        {
                            TZones.Instance.SendCommandReply(caller, "command_zones_add_syntax", TZones.Instance.Config.General.MessageIcon);
                            break;
                        }
                    }
                }),
            new SubCommand("list", "", "list [[zone] <page> | [node | flag | event | block] [zoneName] <page>]", new List<string>(), new List<string> { "tzones.command.zones.list" }, 
                Plugin, AllowedCaller,
                (caller, args) =>
                {
                    if (args.Length < 1)
                    {
                        TZones.Instance.SendCommandReply(caller, "command_zones_list_syntax", TZones.Instance.Config.General.MessageIcon);
                        return Task.CompletedTask;
                    }
                    
                    if (args.Length < 2 && !args[0].ToLower().Equals("zone"))
                    {
                        TZones.Instance.SendCommandReply(caller, "command_zones_list_syntax", TZones.Instance.Config.General.MessageIcon);
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
                            var list = ZoneManager.Cache.Zones;
                            maxPage += list.Count / 3;
                            
                            for (int i = 0; i < 3; i++) 
                            {
                                int index = i + 3 * (page - 1);
                                if (list.Count - 1 < index) 
                                {
                                    reachedEnd = true;
                                    break;
                                }

                                var value = list[index];
                                TZones.Instance.SendCommandReply(caller, "command_zones_list_zone", TZones.Instance.Config.General.MessageIcon, value.Id, value.Name, value.Description);
                            }
                            break;
                        }
                        case "node":
                        {
                            nextPage = $"node {args[1]} {page + 1}";
                            Zone? zone = ZoneManager.Queries.GetZone(args[1]);
                            if (zone == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_not_found", TZones.Instance.Config.General.MessageIcon, args[1]);
                                return Task.CompletedTask;
                            }
                            
                            var list = ZoneManager.Queries.GetNodes(zone.Id) ?? new List<Node>();
                            maxPage += list.Count / 3;
                            
                            for (int i = 0; i < 3; i++) 
                            {
                                int index = i + 3 * (page - 1);
                                if (list.Count - 1 < index) 
                                {
                                    reachedEnd = true;
                                    break;
                                }

                                var value = list[index];
                                TZones.Instance.SendCommandReply(caller, "command_zones_list_node", TZones.Instance.Config.General.MessageIcon, value.Id, value.Type.ToString(), value.X, value.Y, value.Z);
                            }
                            break;
                        }
                        case "flag":
                        {
                            nextPage = $"flag {args[1]} {page + 1}";
                            Zone? zone = ZoneManager.Queries.GetZone(args[1]);
                            if (zone == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_not_found", TZones.Instance.Config.General.MessageIcon, args[1]);
                                return Task.CompletedTask;
                            }
                            
                            var list = ZoneManager.Queries.GetZoneFlags(zone.Id) ?? new  List<ZoneFlag>();
                            maxPage += list.Count / 3;
                            
                            for (int i = 0; i < 3; i++) 
                            {
                                int index = i + 3 * (page - 1);
                                if (list.Count - 1 < index) 
                                {
                                    reachedEnd = true;
                                    break;
                                }

                                var value = list[index];
                                Flag? flag = ZoneManager.Queries.GetFlag(value.FlagId);
                                TZones.Instance.SendCommandReply(caller, "command_zones_list_flag", TZones.Instance.Config.General.MessageIcon, flag?.Name ?? "null", value.FlagId);
                            }
                            break;
                        }
                        case "event":
                        {
                            nextPage = $"event {args[1]} {page + 1}";
                            Zone? zone = ZoneManager.Queries.GetZone(args[1]);
                            if (zone == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_not_found", TZones.Instance.Config.General.MessageIcon, args[1]);
                                return Task.CompletedTask;
                            }
                            
                            var list = ZoneManager.Queries.GetZoneEvents(zone.Id) ?? new  List<ZoneEvent>();
                            maxPage += list.Count / 3;
                            
                            for (int i = 0; i < 3; i++) 
                            {
                                int index = i + 3 * (page - 1);
                                if (list.Count - 1 < index) 
                                {
                                    reachedEnd = true;
                                    break;
                                }

                                var value = list[index];
                                TZones.Instance.SendCommandReply(caller, "command_zones_list_event", TZones.Instance.Config.General.MessageIcon, value.Type.ToString(), value.Value);
                            }
                            break;
                        }
                        case "block":
                        {
                            nextPage = $"block {args[1]} {page + 1}";
                            Zone? zone = ZoneManager.Queries.GetZone(args[1]);
                            if (zone == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_not_found", TZones.Instance.Config.General.MessageIcon, args[1]);
                                return Task.CompletedTask;
                            }

                            var list = ZoneManager.Queries.GetZoneBlocks(zone.Id) ?? new List<Restriction>();
                            maxPage += list.Count / 3;
                            
                            for (int i = 0; i < 3; i++) 
                            {
                                int index = i + 3 * (page - 1);
                                if (list.Count - 1 < index) 
                                {
                                    reachedEnd = true;
                                    break;
                                }

                                var value = list[index];
                                TZones.Instance.SendCommandReply(caller, "command_zones_list_block", TZones.Instance.Config.General.MessageIcon, value.Type.ToString(), value.UnturnedId);
                            }
                            break;
                        }
                        default:
                        {
                            TZones.Instance.SendCommandReply(caller, "command_zones_list_syntax", TZones.Instance.Config.General.MessageIcon);
                            return Task.CompletedTask;
                        }
                    }
                    
                    if (reachedEnd || maxPage <= page + 1)
                        TZones.Instance.SendCommandReply(caller, "command_zones_list_end", TZones.Instance.Config.General.MessageIcon);
                    else
                        TZones.Instance.SendCommandReply(caller, "command_zones_list_next", TZones.Instance.Config.General.MessageIcon, nextPage);
                    
                    return Task.CompletedTask;
                }),
            new SubCommand("remove", "", "remove [zone | node | flag | event | block]", new List<string>(), new List<string> { "tzones.command.zones.remove" }, 
                Plugin, AllowedCaller,
                async (caller, args) =>
                {
                    if (args.Length < 1)
                    {
                        TZones.Instance.SendCommandReply(caller, "command_zones_remove_syntax", TZones.Instance.Config.General.MessageIcon);
                        return;
                    }

                    switch (args[0].ToLower())
                    {
                        case "zone":
                        {
                            if (args.Length != 2)
                            {
                                TZones.Instance.SendCommandReply(caller, "command_zones_remove_zone_syntax", TZones.Instance.Config.General.MessageIcon);
                                return;
                            }

                            Zone? zone = ZoneManager.Queries.GetZone(args[1]);
                            if (zone == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_not_found", TZones.Instance.Config.General.MessageIcon, args[1]);
                                return;
                            }

                            MySqlConnection? connection = TZones.DatabaseManager.CreateConnection();
                            if (connection == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_database_connection", TZones.Instance.Config.General.MessageIcon);
                                return;
                            }

                            await using var transaction = connection.BeginTransaction();
                            
                            try
                            {
                                var rangeValues = new List<object> { zone.Id };
                                await TZones.DatabaseManager.ZoneFlags.DeleteRangeAsync("ZoneId", rangeValues, connection, transaction);
                                await TZones.DatabaseManager.ZoneEvents.DeleteRangeAsync("ZoneId", rangeValues, connection, transaction);
                                await TZones.DatabaseManager.Restrictions.DeleteRangeAsync("ZoneId", rangeValues, connection, transaction);
                                await TZones.DatabaseManager.Nodes.DeleteRangeAsync("ZoneId", rangeValues, connection, transaction);
                                await TZones.DatabaseManager.Zones.DeleteAsync(zone.Id, connection, transaction);

                                await transaction.CommitAsync();
                                ZoneManager.Cache.MakeDirty();
                            }
                            catch (Exception ex)
                            {
                                await transaction.RollbackAsync();
                                TZones.Instance.SendCommandReply(caller, "error_exception", TZones.Instance.Config.General.MessageIcon);
                                TZones.Logger.Error("Unexpected error occured while removing zones.", ex);
                                return;
                            }
                            
                            TZones.Instance.SendCommandReply(caller, "command_zones_remove_zone", TZones.Instance.Config.General.MessageIcon, args[1]);
                            break;
                        }
                        case "node":
                        {
                            if (args.Length != 3)
                            {
                                TZones.Instance.SendCommandReply(caller, "command_zones_remove_node_syntax", TZones.Instance.Config.General.MessageIcon);
                                return;
                            }

                            Zone? zone = ZoneManager.Queries.GetZone(args[1]);
                            if (zone == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_not_found", TZones.Instance.Config.General.MessageIcon, args[1]);
                                return;
                            }

                            ulong nodeId = 0;
                            try
                            {
                                nodeId = ulong.Parse(args[2]);
                            }
                            catch { /* ignore */}
                            
                            Node? node = ZoneManager.Queries.GetNode(zone.Id, nodeId);
                            if (node == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_node_not_found", TZones.Instance.Config.General.MessageIcon, zone.Name, args[2]);
                                return;
                            }

                            await TZones.DatabaseManager.Nodes.DeleteAsync(node.Id);
                            ZoneManager.Cache.MakeDirty();
                            
                            TZones.Instance.SendCommandReply(caller, "command_zones_remove_node", TZones.Instance.Config.General.MessageIcon, node.Id);
                            break;
                        }
                        case "flag":
                        {
                            if (args.Length != 3)
                            {
                                TZones.Instance.SendCommandReply(caller, "command_zones_remove_flag_syntax", TZones.Instance.Config.General.MessageIcon);
                                return;
                            }
                            
                            Zone? zone = ZoneManager.Queries.GetZone(args[1]);
                            if (zone == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_not_found", TZones.Instance.Config.General.MessageIcon, args[1]);
                                return;
                            }
                            
                            Flag? flag = ZoneManager.Queries.GetFlag(args[2]);
                            if (flag == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_flag_not_found", TZones.Instance.Config.General.MessageIcon, args[2]);
                                return;
                            }
                            
                            ZoneFlag? zoneFlag = ZoneManager.Queries.GetZoneFlag(zone.Id, flag.Id);
                            if (zoneFlag == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zoneflag_not_found", TZones.Instance.Config.General.MessageIcon, zone.Name, args[2]);
                                return;
                            }

                            await TZones.DatabaseManager.ZoneFlags.DeleteAsync(zoneFlag.Id);
                            ZoneManager.Cache.MakeDirty();
                            
                            TZones.Instance.SendCommandReply(caller, "command_zones_remove_flag", TZones.Instance.Config.General.MessageIcon, flag.Name, zone.Name);
                            break;
                        }
                        case "event":
                        {
                            if (args.Length != 3)
                            {
                                TZones.Instance.SendCommandReply(caller, "command_zones_remove_event_syntax", TZones.Instance.Config.General.MessageIcon);
                                return;
                            }
                            
                            Zone? zone = ZoneManager.Queries.GetZone(args[1]);
                            if (zone == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_not_found", TZones.Instance.Config.General.MessageIcon, args[1]);
                                return;
                            }
                            
                            EEventType eventType;
                            try
                            {
                                eventType = (EEventType)Enum.Parse(typeof(EEventType), args[2], true);
                            }
                            catch
                            {
                                TZones.Instance.SendCommandReply(caller, "error_event_type_not_found", TZones.Instance.Config.General.MessageIcon, args[2]);
                                return;
                            }
                            
                            ZoneEvent? zoneEvent = ZoneManager.Queries.GetZoneEvent(zone.Id, eventType);
                            if (zoneEvent == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zoneevent_not_found", TZones.Instance.Config.General.MessageIcon, zone.Name, args[2]);
                                return;
                            }

                            await TZones.DatabaseManager.ZoneEvents.DeleteAsync(zoneEvent.Id);
                            ZoneManager.Cache.MakeDirty();
                            
                            TZones.Instance.SendCommandReply(caller, "command_zones_remove_event", TZones.Instance.Config.General.MessageIcon, zoneEvent.Type.ToString(), zone.Name);
                            break;
                        }
                        case "block":
                        {
                            if (args.Length != 4)
                            {
                                TZones.Instance.SendCommandReply(caller, "command_zones_remove_block_syntax", TZones.Instance.Config.General.MessageIcon);
                                return;
                            }
                            
                            Zone? zone = ZoneManager.Queries.GetZone(args[1]);
                            if (zone == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_not_found", TZones.Instance.Config.General.MessageIcon, args[1]);
                                return;
                            }

                            ERestrictionType restrictionType;
                            try
                            {
                                restrictionType = (ERestrictionType)Enum.Parse(typeof(ERestrictionType), args[2], true);
                            }
                            catch
                            {
                                TZones.Instance.SendCommandReply(caller, "error_block_type_not_found", TZones.Instance.Config.General.MessageIcon, args[2]);
                                return;
                            }

                            ushort id = 0;
                            try
                            {
                                id = ushort.Parse(args[3]);
                            }
                            catch { /* ignored */}

                            Restriction? restriction = ZoneManager.Queries.GetZoneBlock(zone.Id, restrictionType, id);
                            if (restriction == null)
                            {
                                TZones.Instance.SendCommandReply(caller, "error_zone_block_not_found", TZones.Instance.Config.General.MessageIcon, restrictionType.ToString(), id);
                                return;
                            }

                            await TZones.DatabaseManager.Restrictions.DeleteAsync(restriction.Id);
                            ZoneManager.Cache.MakeDirty();
                            
                            TZones.Instance.SendCommandReply(caller, "command_zones_remove_block", TZones.Instance.Config.General.MessageIcon);
                            break; 
                        }
                        default:
                        {
                            TZones.Instance.SendCommandReply(caller, "command_zones_remove_syntax", TZones.Instance.Config.General.MessageIcon);
                            break;
                        }
                    }
                })
        };

        /// <inheritdoc/>
        protected override bool HandleExecute(IRocketPlayer caller, string[] args) => false;
    }
}