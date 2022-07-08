﻿
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Types
{
    public class SteamPlayer
    {
        public uint AccountID { get; set; }
        public ulong SteamID { get; set; }
        public string PersonaName { get; set; }
        public uint GameID { get; set; }
        public ulong LobbyID { get; set; }
        public bool HasFriend { get; set; }
        public string IPAddress { get; set; }
        public Image Avatar { get; set; }
    }
}
