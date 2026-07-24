using Rocket.API;
using System.Collections.Generic;
using System.Reflection;
using Tavstal.TLibrary.Helpers.Unturned;
// ReSharper disable UnusedType.Global

namespace Tavstal.TZones.Commands
{
    /// <summary>
    /// Command that displays the plugin version and build information.
    /// </summary>
    public class CommandVersion : IRocketCommand
    {
        /// <inheritdoc/>
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        /// <inheritdoc/>
        public string Name => ("v" + Assembly.GetExecutingAssembly().GetName().Name);
        /// <inheritdoc/>
        public string Help => "Gets the version of the plugin";
        /// <inheritdoc/>
        public string Syntax => "";
        /// <inheritdoc/>
        public List<string> Aliases => new List<string>();
        /// <inheritdoc/>
        public List<string> Permissions => new List<string> { "example.command.version" };


        /// <inheritdoc/>
        public void Execute(IRocketPlayer caller, string[] command)
        {
            var instance = TZones.Instance;
            var config = instance.Config.General;
            var icon = config.MessageIcon;
            string message = string.Join(System.Environment.NewLine, 
                $"&b&l[{instance.GetPluginName()}]&r System Info:",
                $"&b • Version: &r{TZones.Version}",
                $"&b • Build Date: &r{TZones.BuildDate}",
                "&b • Developer: &rTavstal");
            
            instance.SendPlainCommandReply(caller, message, icon);
        }
    }
}
