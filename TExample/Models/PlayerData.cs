using System;
using Tavstal.TLibrary.Compatibility.Database;

namespace Tavstal.TExample.Models
{
    [Serializable]
    public class PlayerData
    {
        [SqlMember("SteamId", "varchar(17)", false, isPrimaryKey: true, isUnsigned: true)]
        public ulong SteamId { get; set; }
        [SqlMember("SteamName", "varchar(32)", false)]
        public string SteamName { get; set; }
        [SqlMember("LastCharacterName", "varchar(50)", false)]
        public string LastCharacterName { get; set; }
        [SqlMember("LastLogin")]
        public DateTime LastLogin { get; set; }

        public PlayerData() { }

        public PlayerData(ulong steamId, string steamName, string characterName, DateTime lastLogin)
        {
            SteamId = steamId;
            SteamName = steamName;
            LastCharacterName = characterName;
            LastLogin = lastLogin;
        }
    }
}
