using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using Tavstal.TLibrary.Compatibility;
using Tavstal.TLibrary.Compatibility.Interfaces;
using Tavstal.TLibrary.Helpers.Unturned;

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