using Rocket.API;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tavstal.TLibrary.Models.Commands;
using Tavstal.TLibrary.Models.Plugin;
using Tavstal.TLibrary.Helpers.Unturned;
using Tavstal.TZones.Models.Core;
using Tavstal.TZones.Utils.Managers;

namespace Tavstal.TZones.Commands
{
    public class CommandFlags: CustomCommandBase
    {
        public override IPlugin Plugin => TZones.Instance;
        public override bool UseBackgroundThread => false;
        public override AllowedCaller AllowedCaller => AllowedCaller.Both;
        public override string Name => "flags";
        public override string Help => "Manage flags.";
        public override string Syntax => "list | add | remove";
        public override List<string> Aliases => new List<string>();
        public override List<string> Permissions => new List<string> { "tzones.command.flags" };

        // 'help' subcommand is built-in, you don't need to add it
        public override List<ISubcommand> SubCommands => new List<ISubcommand>
        {
            new SubCommand("add", "", "add [name] [description]", new List<string>(), new List<string> { "tzones.command.flags.add" }, 
                Plugin, AllowedCaller,
                async (caller, args) =>
                {
                    if (args.Length != 2) 
                    {
                        TZones.Instance.SendCommandReply(caller, "command_flags_add_syntax", TZones.Instance.Config.General.MessageIcon);
                        return;
                    }
                    
                    if (await ZoneManager.AddCustomFlagAsync(args[0], args[1], caller.DisplayName))
                        TZones.Instance.SendCommandReply(caller, "command_flags_add", TZones.Instance.Config.General.MessageIcon, args[0]);
                    else
                        TZones.Instance.SendCommandReply(caller, "command_flags_add_duplicate", TZones.Instance.Config.General.MessageIcon, args[0]);
                }),
            new SubCommand("list", "", "list <page>", new List<string>(), new List<string> { "tzones.command.flags.list" }, 
                Plugin, AllowedCaller,
                (caller, args) =>
                {
                    var flags = ZoneManager.Cache.Flags;

                    int page = 1;
                    int maxPage = 1 + flags.Count / 3;
                    try 
                    {
                        page = int.Parse(args[0]);
                        if (page < 1)
                           page = 1;
                    } catch { /*ignore*/ }

                    bool reachedEnd = false;
                    for (int i = 0; i < 3; i++) 
                    {
                        int index = i + 3 * (page - 1);
                        if (flags.Count - 1 < index) 
                        {
                            reachedEnd = true;
                            break;
                        }

                        Flag flag = flags[index];

                        TZones.Instance.SendCommandReply(caller, "command_flags_list_element", TZones.Instance.Config.General.MessageIcon, flag.Name, flag.Description);
                    }

                    if (reachedEnd || maxPage <= page + 1)
                        TZones.Instance.SendCommandReply(caller, "command_flags_list_end", TZones.Instance.Config.General.MessageIcon);
                    else
                        TZones.Instance.SendCommandReply(caller, "command_flags_list_next", TZones.Instance.Config.General.MessageIcon, page + 1);
                    
                    return  Task.CompletedTask;
                }),
            new SubCommand("remove", "", "remove [name]", new List<string>(), new List<string> { "tzones.command.flags.remove" }, 
                Plugin, AllowedCaller,
                async (caller, args) =>
                {
                    if (args.Length != 1) 
                    {
                        TZones.Instance.SendCommandReply(caller, "command_flags_remove_syntax", TZones.Instance.Config.General.MessageIcon);
                        return;
                    }
                    
                    int result = await ZoneManager.RemoveCustomFlagAsync(args[0]);
                    switch (result)
                    {
                        case 0:
                            TZones.Instance.SendCommandReply(caller, "command_flags_remove", TZones.Instance.Config.General.MessageIcon, args[0]);
                            break;
                        case 1:
                            TZones.Instance.SendCommandReply(caller, "error_flag_not_found", TZones.Instance.Config.General.MessageIcon, args[0]);
                            break;
                        default:
                            TZones.Instance.SendCommandReply(caller, "command_flags_remove_default", TZones.Instance.Config.General.MessageIcon, args[0]);
                            break;
                    }
                })
        };

        protected override bool HandleExecute(IRocketPlayer caller, string[] args) => false;
    }
}