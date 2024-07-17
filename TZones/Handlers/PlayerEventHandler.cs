using Rocket.Unturned;
using Rocket.Unturned.Player;
using Tavstal.TZones.Models;

namespace Tavstal.TZones.Handlers
{
    public static class PlayerEventHandler
    {
        private static bool _isAttached = false;

        public static void AttachEvents()
        {
            if (_isAttached)
                return;

            _isAttached = true;

            U.Events.OnPlayerConnected += OnPlayerConnected;
        }

        public static void DetachEvents()
        {
            if (!_isAttached)
                return;

            _isAttached = false;

            U.Events.OnPlayerConnected -= OnPlayerConnected;
        }

        private static async void OnPlayerConnected(UnturnedPlayer player)
        {
            PlayerData data = await TZones.DatabaseManager.FindPlayer(player.CSteamID.m_SteamID);
            if (data == null)
            {
                await TZones.DatabaseManager.AddPlayer(player.CSteamID.m_SteamID, player.SteamName, player.CharacterName);
            }
            else
            {
                if (data.LastCharacterName != player.CharacterName)
                    await TZones.DatabaseManager.UpdatePlayer(player.CSteamID.m_SteamID, player.CharacterName);
            }
        }
    }
}
