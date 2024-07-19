using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using Tavstal.TLibrary.Compatibility;
using Tavstal.TLibrary.Compatibility.Interfaces;
using Tavstal.TLibrary.Helpers.Unturned;

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
                (IRocketPlayer caller, string[] args) =>
                {
                    
                })
        };

        public override bool ExecutionRequested(IRocketPlayer caller, string[] args)
        {
            return true;
        }
    }
}