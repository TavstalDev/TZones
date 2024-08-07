using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using Tavstal.TLibrary.Compatibility;
using Tavstal.TLibrary.Compatibility.Interfaces;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers.Unturned;
using Tavstal.TZones.Models.Core;
using Tavstal.TZones.Utils.Managers;

namespace Tavstal.TZones.Commands
{
    public class CommandFlags: CommandBase
    {
        public override IPlugin Plugin => TZones.Instance; 
        public override AllowedCaller AllowedCaller => AllowedCaller.Both;
        public override string Name => "flags";
        public override string Help => "Manage flags.";
        public override string Syntax => "list | add | remove";
        public override List<string> Aliases => new List<string>();
        public override List<string> Permissions => new List<string> { "tzones.command.flags" };

        // 'help' subcommand is built-in, you don't need to add it
        public override List<SubCommand> SubCommands => new List<SubCommand>()
        {
            new SubCommand("add", "", "add [name] [description]", new List<string>(), new List<string>() { "tzones.command.flags.add" }, 
                async (IRocketPlayer caller, string[] args) =>
                {
                    if (args.Length != 2) 
                    {
                        TZones.Instance.SendCommandReply(caller, "command_flags_add_syntax");
                        return;
                    }

                    Flag flag = ZonesManager.Flags.Find(x => x.Name == args[0]); 
                    if (flag != null) 
                    {
                        TZones.Instance.SendCommandReply(caller, "command_flags_add_duplicate", args[0]);
                        return;
                    }

                    await TZones.DatabaseManager.AddFlagAsync(args[0], args[1], caller.DisplayName);
                    ZonesManager.SetDirty();
                    TZones.Instance.SendCommandReply(caller, "command_flags_add", args[0]);
                }),
            new SubCommand("list", "", "list <page>", new List<string>(), new List<string>() { "tzones.command.flags.list" }, 
                (IRocketPlayer caller, string[] args) =>
                {
                    List<Flag> flags = ZonesManager.Flags;

                    int page = 1;
                    int maxPage = 1 + flags.Count / 3;
                    try 
                    {
                        page = int.Parse(args[0]);
                        if (page < 1)
                           page = 1;
                    } catch {}

                    bool reachedEnd = false;
                    for (int i = 0; i < 3; i++) 
                    {
                        int index = i + 3 * (page - 1);
                        if (!flags.IsValidIndex(index)) 
                        {
                            reachedEnd = true;
                            break;
                        }

                        Flag flag = flags[index];

                        TZones.Instance.SendCommandReply(caller, "command_flags_list_element", flag.Name, flag.Description);
                    }

                    if (reachedEnd || maxPage <= page + 1)
                        TZones.Instance.SendCommandReply(caller, "command_flags_list_end");
                    else
                        TZones.Instance.SendCommandReply(caller, "command_flags_list_next", page + 1);
                }),
            new SubCommand("remove", "", "remove [name]", new List<string>(), new List<string>() { "tzones.command.flags.remove" }, 
                async (IRocketPlayer caller, string[] args) =>
                {
                    if (args.Length != 1) 
                    {
                        TZones.Instance.SendCommandReply(caller, "command_flags_remove_syntax");
                        return;
                    }

                    Flag targetFlag = ZonesManager.Flags.Find(x => x.Name == args[0]);
                    if (targetFlag == null)
                    {
                        TZones.Instance.SendCommandReply(caller, "error_flag_not_found", args[0]);
                        return;
                    }

                    await TZones.DatabaseManager.RemoveFlagAsync(targetFlag.Id);
                    ZonesManager.SetDirty();
                    TZones.Instance.SendCommandReply(caller, "command_flags_remove", targetFlag.Name);
                })
        };

        public override bool ExecutionRequested(IRocketPlayer caller, string[] args)
        {
            return true;
        }
    }
}