﻿using Rocket.API;
using System.Collections.Generic;
using System.Reflection;
using Tavstal.TLibrary.Helpers.Unturned;

namespace Tavstal.TZones.Commands
{
    public class CommandVersion : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => ("v" + Assembly.GetExecutingAssembly().GetName().Name);
        public string Help => "Gets the version of the plugin";
        public string Syntax => "";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "example.command.version" };


        public void Execute(IRocketPlayer caller, string[] command)
        {
            // Please do not remove this region and its code, because the license require credits to the author.
            #region Credits to Tavstal
            TZones.Instance.SendPlainCommandReply(caller, "#########################################");
            TZones.Instance.SendPlainCommandReply(caller, $"# This plugin uses TLibrary.");
            TZones.Instance.SendPlainCommandReply(caller, $"# TLibrary Created By: Tavstal");
            TZones.Instance.SendPlainCommandReply(caller, $"# Github: https://github.com/TavstalDev/TLibrary/tree/master");
            #endregion
            TZones.Instance.SendPlainCommandReply(caller, "#########################################");
            TZones.Instance.SendPlainCommandReply(caller, string.Format("# Build Version: {0}", TZones.Version));
            TZones.Instance.SendPlainCommandReply(caller, string.Format("# Build Date: {0}", TZones.BuildDate));
            TZones.Instance.SendPlainCommandReply(caller, "#########################################");
        }
    }
}
