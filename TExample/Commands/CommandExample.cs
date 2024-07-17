using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using Tavstal.TLibrary.Compatibility;
using Tavstal.TLibrary.Compatibility.Interfaces;
using Tavstal.TLibrary.Helpers.Unturned;

namespace Tavstal.TExample
{
    public class CommandExample : CommandBase
    {
        public override IPlugin Plugin => ExampleMain.Instance; 
        public override AllowedCaller AllowedCaller => AllowedCaller.Both;
        public override string Name => "example";
        public override string Help => "This is an example description what the command does.";
        public override string Syntax => "";
        public override List<string> Aliases => new List<string>() { "ex" };
        public override List<string> Permissions => new List<string> { "texample.command.example" };

        // 'help' subcommand is built-in, you don't need to add it
        public override List<SubCommand> SubCommands => new List<SubCommand>()
        {
            new SubCommand("hi", "Example subcommand for the command", "hi <player>", new List<string>() { "hello" }, new List<string>() { "texample.command.example.hi" }, 
                (IRocketPlayer caller, string[] args) =>
                {
                    if (args.Length == 1)
                    {
                        UnturnedPlayer player = UnturnedPlayer.FromName(args[0]);
                        if (player == null)
                        {
                            UChatHelper.SendCommandReply(Plugin, caller, "error_player_not_found");
                            return;
                        }

                        UChatHelper.SendPlainCommandReply(Plugin, player, "&aHello world!");
                         UChatHelper.SendCommandReply(Plugin, caller, "success_command_example_hi_sent");
                    }
                    else
                    {
                        UChatHelper.SendPlainCommandReply(Plugin, caller, "Hello world!");
                    }
                })
        };

        public override bool ExecutionRequested(IRocketPlayer caller, string[] args)
        {
            // Called if there was no subcommand to execute
            // Add stuff what the command does he
            // return true if you don't want to send the command usage to the caller, otherwise return false
            return true;
        }
    }
}
