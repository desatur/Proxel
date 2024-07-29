using System;
using System.Collections.Generic;
using System.Text;
using Proxel.Protocol.Structs;

namespace Proxel.Protocol.Helpers
{
    public class StatusBuilder : IDisposable
    {
        public string VersionName { get; set; } = "Proxel";
        public int Protocol { get; set; } = 767;
        public int MaxPlayers { get; set; } = 1337;
        public int OnlinePlayers { get; set; } = 0;
        public string MOTD { get; set; } = "{\"text\": \"A Proxel Proxy\"}";
        public List<Player> Players { get; set; } = [];
        public string Favicon { get; private set; } = "data:image/png;base64,<data>";
        public bool EnforcesSecureChat { get; set; } = false;

        private string finalJson;

        public StatusBuilder()
        {

        }

        private void FormatJson()
        {
            var jsonParts = new List<string>();
            jsonParts.Add($"\"version\": {{ \"name\": \"{VersionName}\", \"protocol\": {Protocol} }}"); // Version
            jsonParts.Add($"\"players\": {{ \"max\": {MaxPlayers}, \"online\": {OnlinePlayers} }}"); // Players
            jsonParts.Add($"\"description\": {MOTD}"); // MOTD
            //jsonParts.Add($"\"favicon\": \"{Favicon}\"");
            //jsonParts.Add($"\"enforcesSecureChat\": {EnforcesSecureChat.ToString().ToLower()}");
            finalJson = "{" + string.Join(",", jsonParts) + "}";
        }

        public string GetFinalJson()
        {
            FormatJson();
            return finalJson;
        }

        public byte[] GetFinalJsonAsByteArray()
        {
            FormatJson();
            return Encoding.UTF8.GetBytes(finalJson);
        }

        public void Dispose()
        {
            VersionName = null;
            Protocol = 0;
            MaxPlayers = 0;
            OnlinePlayers = 0;
            MOTD = null;
            Players = null;
            Favicon = null;
            finalJson = null;
            GC.SuppressFinalize(this);
        }
    }
}
