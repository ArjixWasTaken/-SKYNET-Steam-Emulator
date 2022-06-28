﻿using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace SKYNET.WEB.Types
{
    public class WebMessage
    {
        public WEB_MessageType MessageType { get; set; }
        public string Body { get; set; }
        public T Deserialize<T>()
        {
            try
            {
                return new JavaScriptSerializer().Deserialize<T>(Body);
            }
            catch 
            {
                return default;
            }
        }
    }

    public class WEB_Base
    {
    }

    public enum WEB_MessageType : int
    {
        WEB_CreateAccountRequest,
        WEB_CreateAccountResponse,
        WEB_AuthRequest,
        WEB_AuthResponse, 
        WEB_GameListRequest, 
        WEB_GameListResponse, 
        WEB_GameAdded, 
        WEB_GameUpdated, 
        WEB_GameRemoved,
        WEB_GameLaunch,
        WEB_GameStoped,
        WEB_GameInfoRequest,
        WEB_GameInfoResponse,
        WEB_GameInfoMinimalResponse,
        WEB_UserOnline,
        WEB_UserOffline,
        WEB_UserInfoRequest,
        WEB_UserInfoResponse,
        WEB_ChatMessage,
        WEB_PrivateChatMessage,
    }

    public class WEB_ChatMessage : WEB_Base
    {
        public uint SenderAccountID { get; set; }
        public uint Message { get; set; }
    }

    public class WEB_PrivateChatMessage : WEB_Base
    {
        public uint SenderAccountID { get; set; }
        public uint TargetAccountID { get; set; }
        public uint Message { get; set; }
    }

    public class WEB_UserOnline : WEB_Base
    {
        public UserInfo User { get; set; }
    }

    public class WEB_UserOffline : WEB_Base
    {
        public uint AccountID { get; set; }
    }

    public class WEB_GameInfoRequest : WEB_Base
    {
        public string Guid { get; set; }
        public bool Minimal { get; set; }
    }

    public class WEB_GameInfoResponse : WEB_Base
    {
        public bool Playing { get; set; }
        public uint LastPlayed { get; set; }
        public uint TimePlayed { get; set; }
        public uint UsersPlaying { get; set; }
        public uint Description { get; set; }
        public bool FreeToPlay { get; set; }
        public string ImageHex { get; set; }
    }

    public class WEB_GameInfoMinimalResponse : WEB_Base
    {
        public bool Playing { get; set; }
        public uint LastPlayed { get; set; }
        public uint TimePlayed { get; set; }
        public uint UsersPlaying { get; set; }
    }

    public class WEB_CreateAccountRequest : WEB_Base
    {
        public string AccountName { get; set; }
        public string Password { get; set; }
    }

    public class WEB_CreateAccountResponse : WEB_Base
    {
        public Result CreateAccountResult { get; set; }

        public string AccountName { get; set; }
        public uint AccountID { get; set; }
        public ulong SteamID { get; set; }
        public enum Result
        {
            ERROR,
            SUCCESS,
            ACCOUNTEXISTS
        }
    }

    public class WEB_GameAdded : WEB_Base
    {
        public Game Game { get; set; }
    }

    public class WEB_GameUpdated : WEB_Base
    {
        public Game Game { get; set; }
    }

    public class WEB_GameRemoved : WEB_Base
    {
        public string Guid { get; set; }
    }

    public class WEB_GameLaunch : WEB_Base
    {
        public string Guid { get; set; }
    }

    public class WEB_GameStoped : WEB_Base
    {
        public string Guid { get; set; }
    }

    public class WEB_AuthRequest : WEB_Base
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class WEB_AuthResponse : WEB_Base
    {
        public WEB_AuthResponseType Response { get; set; }
        public UserInfo UserInfo { get; set; }
    }

    public class WEB_GameListRequest : WEB_Base
    {
    }

    public class WEB_GameListResponse : WEB_Base
    {
        public List<Game> GameList { get; set; }
    }

    public enum WEB_AuthResponseType : int
    {
        UnknownError    = 0,
        Success         = 1,
        AccountNotFound = 2,
        PasswordWrong   = 3,
    }

    public class UserInfo
    {
        public uint AccountID { get; set; }
        public ulong SteamID { get; set; }
        public string PersonaName { get; set; }
        public string Language { get; set; }
        public string AvatarHex { get; set; }
    }
}