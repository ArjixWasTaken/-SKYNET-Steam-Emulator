﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using SKYNET.Callback;
using SKYNET.Helper;
using SKYNET.Managers;
using SKYNET.Properties;
using SKYNET.Overlay;
using SKYNET.Network.Packets;
using SKYNET.Types;

using SteamAPICall_t = System.UInt64;
using FriendsGroupID_t = System.UInt16;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamFriends : ISteamInterface
    {
        public static SteamFriends Instance;

        public  List<SteamPlayer> Users;
        public  List<ulong> QueryingAvatar;

        private Dictionary<string, string> RichPresence;
        private ConcurrentDictionary<ulong, ImageAvatar> Avatars;
        private int ImageIndex;
        private ImageAvatar DefaultAvatar;

        public SteamFriends()
        {
            Instance = this;
            InterfaceName = "SteamFriends";
            InterfaceVersion = "SteamFriends017";
            Users   = new List<SteamPlayer>();
            QueryingAvatar = new List<SteamAPICall_t>();
            RichPresence = new Dictionary<string, string>();
            Avatars = new ConcurrentDictionary<ulong, ImageAvatar>();
            ImageIndex = 10;

            #region Default Avatar

            DefaultAvatar = new ImageAvatar(Resources.Image, ref ImageIndex);
            
            #endregion

            #region Own Avatar

            try
            {
                string fileName = Path.Combine(modCommon.GetPath(), "SKYNET", "Avatar.jpg");
                if (!File.Exists(fileName))
                    fileName = Path.Combine(modCommon.GetPath(), "SKYNET", "Avatar.png");

                Bitmap Avatar = default;
                if (File.Exists(fileName))
                {
                    Avatar = (Bitmap)Bitmap.FromFile(fileName);
                }
                else
                {
                    Avatar = ImageHelper.GetDesktopWallpaper(true);
                }
                if (Avatar != null)
                {
                    ImageAvatar avatar = new ImageAvatar(Avatar, ref ImageIndex); 
                    Avatars.TryAdd((ulong)SteamEmulator.SteamID, avatar);
                }
            }
            catch (Exception ex)
            {
                Write($"Error loading default avatar {ex}");
            }

            #endregion

            #region Own User

            Users.Add(new SteamPlayer()
            {
                AccountID = SteamEmulator.SteamID.AccountId,
                GameID = SteamEmulator.AppID,
                HasFriend = false,
                PersonaName = SteamEmulator.PersonaName,
                SteamID = (ulong)SteamEmulator.SteamID,
                IPAddress = NetworkManager.GetIPAddress().ToString()
            });

            #endregion

            //return;
            Users.Add(new SteamPlayer()
            {
                AccountID = 1001,
                GameID = SteamEmulator.GameID,
                HasFriend = true,
                PersonaName = "Yohel.com",
                SteamID = (ulong)new CSteamID(1001),
                IPAddress = "10.31.0.1"
            });
            Users.Add(new SteamPlayer()
            {
                AccountID = 1002,
                GameID = SteamEmulator.GameID,
                HasFriend = true,
                PersonaName = "Elier",
                SteamID = (ulong)new CSteamID(1002),
                IPAddress = "10.31.0.2"
            });
            Users.Add(new SteamPlayer()
            {
                AccountID = 1003,
                GameID = SteamEmulator.GameID,
                HasFriend = true,
                PersonaName = "BrolySSL",
                SteamID = (ulong)new CSteamID(1003),
                IPAddress = "10.31.0.3"
            });
        }

        public void ReportUserChanged(ulong SteamID, EPersonaChange changeFlags)
        {
            PersonaStateChange_t data = new PersonaStateChange_t();
            data.m_ulSteamID = SteamID;
            data.m_nChangeFlags = (int)changeFlags;
            CallbackManager.AddCallbackResult(data);
        }

        public string GetPersonaName()
        {
            string PersonaName = SteamEmulator.PersonaName;
            Write($"GetPersonaName {PersonaName}");
            return PersonaName;
        }

        public short GetGroupIdByIndex(int index)
        {
            Write($"GetGroupIdByIndex {index}");
            return 0;
        }

        public void ActivateGameOverlay(string friendsGroupID)
        {
            Write($"ActivateGameOverlay {friendsGroupID}");
        }

        public void ActivateGameOverlayInviteDialog(ulong steamIDLobby)
        {
            try
            {
                Write($"ActivateGameOverlayInviteDialog (Lobby SteamID = {steamIDLobby})");
                OverlayType type = OverlayType.LobbyInvite;
                var users = new List<SteamPlayer>();
                users.AddRange(Users);
                if (modCommon.Overlay == null)
                {
                    modCommon.Overlay = new frmOverlay();
                    modCommon.Overlay.ProcessOverlay(users, type, steamIDLobby);
                    modCommon.Overlay.Show();
                    return;
                }
                modCommon.Overlay.ProcessOverlay(users, type, steamIDLobby);
            }
            catch (Exception ex)
            {
                Write(ex);
            }
        }

        public void ActivateGameOverlayInviteDialogConnectString(string pchConnectString)
        {
            Write($"ActivateGameOverlayInviteDialogConnectString (URI = {pchConnectString})");
        }

        public void ActivateGameOverlayRemotePlayTogetherInviteDialog(ulong steamIDLobby)
        {
            Write($"ActivateGameOverlayRemotePlayTogetherInviteDialog (Lobby SteamID = {steamIDLobby})");
        }

        public void ActivateGameOverlayToStore(uint nAppID, int eFlag)
        {
            Write($"ActivateGameOverlayToStore (AppID = {nAppID}, Flag = {eFlag})");
        }

        public void ActivateGameOverlayToUser(string friendsGroupID, ulong steamID)
        {
            Write($"ActivateGameOverlayToUser (GroupID = {friendsGroupID}, SteamID = {(CSteamID)steamID})");
            OverlayType type = default;
            switch (friendsGroupID)
            {
                case "steamid":
                    type = OverlayType.SteamProfile;
                    break;
                case "chat":
                    type = OverlayType.Chat;
                    break;
                case "jointrade":
                    type = OverlayType.JoinTrade;
                    break;
                case "stats":
                    type = OverlayType.Stats;
                    break;
                case "achievements":
                    type = OverlayType.Achievements;
                    break;
                case "friendadd":
                    type = OverlayType.FriendAdd;
                    break;
                case "friendremove":
                    type = OverlayType.FriendRemove;
                    break;
                case "friendrequestaccept":
                    type = OverlayType.FriendRequestAccept;
                    break;
                case "friendrequestignore":
                    type = OverlayType.FriendRequestIgnore;
                    break;
                default:
                    break;
            }
            var users = new List<SteamPlayer>();
            if (modCommon.Overlay == null)
            {
                modCommon.Overlay = new frmOverlay();
                modCommon.Overlay.ProcessOverlay(users, type, steamID);
                modCommon.Overlay.Show();
            }
            modCommon.Overlay.ProcessOverlay(users, type, steamID);
        }

        public void ActivateGameOverlayToWebPage(string pchURL, int eMode)
        {
            Write($"ActivateGameOverlayToWebPage {pchURL}");
        }

        public void ClearRichPresence()
        {
            Write($"ClearRichPresence");
        }

        public bool CloseClanChatWindowInSteam(ulong steamIDClanChat)
        {
            Write($"CloseClanChatWindowInSteam {steamIDClanChat}");
            return true;
        }

        public SteamAPICall_t DownloadClanActivityCounts(IntPtr clans, int cClansToRequest)
        {
            Write($"DownloadClanActivityCounts {cClansToRequest}");
            return k_uAPICallInvalid;
        }

        public SteamAPICall_t EnumerateFollowingList(uint unStartIndex)
        {
            Write($"EnumerateFollowingList {unStartIndex}");
            // FriendsEnumerateFollowingList_t
            return k_uAPICallInvalid;
        }

        public CSteamID GetChatMemberByIndex(ulong steamIDClan, int iUser)
        {
            Write($"GetChatMemberByIndex {steamIDClan}");
            return CSteamID.Invalid;
        }

        public bool GetClanActivityCounts(ulong steamIDClan, ref int online, ref int in_game, ref int chatting)
        {
            Write($"ActivateGameOverlay {steamIDClan}");
            online = 0;
            in_game = 0;
            chatting = 0;
            return true;
        }

        public CSteamID GetClanByIndex(int iClan)
        {
            Write($"GetClanByIndex {iClan}");
            return CSteamID.Invalid;
        }

        public int GetClanChatMemberCount(ulong steamIDClan)
        {
            Write($"GetClanChatMemberCount {steamIDClan}");
            return 0;
        }

        public int GetClanChatMessage(ulong steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, int peChatEntryType, ref ulong[] psteamidChatter)
        {
            //psteamidChatter = 0;
            Write($"GetClanChatMessage {steamIDClanChat}");
            return 0;
        }

        public int GetClanCount()
        {
            Write($"GetClanCount");
            return 0;
        }

        public string GetClanName(ulong steamIDClan)
        {
            Write($"GetClanName {steamIDClan}");
            return "";
        }

        public CSteamID GetClanOfficerByIndex(ulong steamIDClan, int iOfficer)
        {
            Write($"GetClanOfficerByIndex {steamIDClan}");
            return CSteamID.Invalid;
        }

        public int GetClanOfficerCount(ulong steamIDClan)
        {
            Write($"GetClanOfficerCount {steamIDClan}");
            return 0;
        }

        public CSteamID GetClanOwner(ulong steamIDClan)
        {
            Write($"GetClanOwner {steamIDClan}");
            return CSteamID.Invalid;
        }

        public string GetClanTag(ulong steamIDClan)
        {
            Write($"GetClanTag {steamIDClan}");
            return "";
        }

        public CSteamID GetCoplayFriend(int iCoplayFriend)
        {
            Write($"GetCoplayFriend {iCoplayFriend}");
            return CSteamID.Invalid;
        }

        public int GetCoplayFriendCount()
        {
            Write($"GetCoplayFriendCount");
            return 0;
        }

        public SteamAPICall_t GetFollowerCount(ulong steamID)
        {
            Write($"GetFollowerCount {steamID}");
            // FriendsGetFollowerCount_t
            return k_uAPICallInvalid;
        }

        [return: MarshalAs(UnmanagedType.Struct)]
        public CSteamID GetFriendByIndex([MarshalAs(UnmanagedType.I4)] int iFriend, int iFriendFlags)
        {
            var Friends = Users.FindAll(f => f.HasFriend);

            if (iFriend < 0 | iFriend > Friends.Count)
            {
                iFriend = iFriendFlags;
            }

            CSteamID Result = CSteamID.Invalid;
            MutexHelper.Wait("GetFriendByIndex", delegate
            {
                if (Friends.Count > iFriend)
                {
                    var friend = Friends[iFriend];
                    if (friend != null)
                    {
                        Result = new CSteamID(friend.SteamID);
                    }
                }
            });
            Write($"GetFriendByIndex (Index = {iFriend}, FriendFlags = {iFriendFlags}) = {Result.ToString()}");
            return Result;
        }

        public uint GetFriendCoplayGame(ulong steamIDFriend)
        {
            Write($"GetFriendCoplayGame {steamIDFriend}");
            return (uint)0;
        }

        public int GetFriendCoplayTime(ulong steamIDFriend)
        {
            Write($"GetFriendCoplayTime {steamIDFriend}");
            return 0;
        }

        public int GetFriendCount(int iFriendFlags)
        {
            int Result = 0;
            if ((iFriendFlags & (int)EFriendFlags.k_EFriendFlagImmediate) == (int)EFriendFlags.k_EFriendFlagImmediate)
            {
                MutexHelper.Wait("Users", delegate
                {
                    var Friends = Users.FindAll(f => f.HasFriend);
                    Result = Friends.Count;
                });
            }
            Write($"GetFriendCount {Result}");
            return Result;
        }

        public int GetFriendCountFromSource(ulong steamIDSource)
        {
            Write($"GetFriendCountFromSource {steamIDSource}");
            return 0;
        }

        public CSteamID GetFriendFromSourceByIndex(ulong steamIDSource, int iFriend)
        {
            Write($"GetFriendFromSourceByIndex {steamIDSource} {iFriend}");
            return CSteamID.Invalid;
        }

        public bool GetFriendGamePlayed(ulong steamIDFriend, IntPtr ptrFriendGameInfo)
        {
            Write($"GetFriendGamePlayed {steamIDFriend}");

            bool Result = false;
            FriendGameInfo_t pFriendGameInfo = Marshal.PtrToStructure<FriendGameInfo_t>(ptrFriendGameInfo);
            MutexHelper.Wait("Users", delegate
            {

            });
            if (steamIDFriend == SteamEmulator.SteamID)
            {
                pFriendGameInfo.GameID = (uint)SteamEmulator.GameID;
                pFriendGameInfo.GameIP = 0;
                pFriendGameInfo.GamePort = 0;
            }
            else
            {
                var friend = GetUser(steamIDFriend);
                if (friend == null)
                {
                    pFriendGameInfo.GameID = 0;
                    pFriendGameInfo.GameIP = 0;
                    pFriendGameInfo.GamePort = 0;
                    Result = true;
                }
                else
                {
                    pFriendGameInfo.GameID = friend.GameID;
                    pFriendGameInfo.GameIP = 0;
                    pFriendGameInfo.GamePort = 0;
                }
            }

            Marshal.StructureToPtr(pFriendGameInfo, ptrFriendGameInfo, false);
            return Result;
        }

        public int GetFriendMessage(ulong steamIDFriend, int iMessageID, IntPtr pvData, int cubData, int peChatEntryType)
        {
            Write($"GetFriendMessage {steamIDFriend} {(EChatEntryType)peChatEntryType}");
            peChatEntryType = (int)EChatEntryType.ChatMsg;
            return 0;
        }

        public string GetFriendPersonaName(ulong steamIDFriend)
        {
            string Result = "Unknown";
            MutexHelper.Wait("Users", delegate
            {
                if ((ulong)steamIDFriend == SteamEmulator.SteamID)
                {
                    Result = SteamEmulator.PersonaName;
                }
                else
                {
                    var friend = GetUser(steamIDFriend);
                    if (friend != null) Result = friend.PersonaName;
                }

                Write($"GetFriendPersonaName (SteamID = {new CSteamID(steamIDFriend)}) = {Result}");
            });
            return Result;
        }

        public string GetFriendPersonaNameHistory(ulong steamIDFriend, int iPersonaName)
        {
            Write($"GetFriendPersonaNameHistory {steamIDFriend}");
            return "SKYNET";
        }

        public int GetFriendPersonaState(ulong steamIDFriend)
        {
            Write($"GetFriendPersonaState {steamIDFriend}");
            EPersonaState Result = EPersonaState.k_EPersonaStateOnline;
            MutexHelper.Wait("Users", delegate
            {
                if (steamIDFriend == SteamEmulator.SteamID)
                {
                    Result = EPersonaState.k_EPersonaStateOnline;
                }
                else if (Users.Find(f => f.SteamID == steamIDFriend) != null)
                {
                    Result = EPersonaState.k_EPersonaStateOnline;
                }
            });

            return (int)Result;
        }

        public int GetFriendRelationship(ulong steamIDFriend)
        {
            Write($"GetFriendRelationship {steamIDFriend}");
            EFriendRelationship Result = EFriendRelationship.k_EFriendRelationshipNone;

            MutexHelper.Wait("Users", delegate
            {
                var friend = GetUser(steamIDFriend);
                if (friend != null && friend.HasFriend)
                    Result = EFriendRelationship.k_EFriendRelationshipFriend;
            });

            return (int)Result;
        }

        public string GetFriendRichPresence(ulong steamIDFriend, string pchKey)
        {
            Write($"GetFriendRichPresence [{steamIDFriend}]: {pchKey}");
            if (RichPresence.ContainsKey(pchKey))
            {
                return RichPresence[pchKey];
            }
            return "";
        }

        public string GetFriendRichPresenceKeyByIndex(ulong steamIDFriend, int iKey)
        {
            Write($"GetFriendRichPresenceKeyByIndex {steamIDFriend} {iKey}");
            return "";
        }

        public int GetFriendRichPresenceKeyCount(ulong steamIDFriend)
        {
            Write($"GetFriendRichPresenceKeyCount {steamIDFriend}");
            return 0;
        }

        public int GetFriendsGroupCount()
        {
            Write($"GetFriendsGroupCount");
            return 0;
        }

        public FriendsGroupID_t GetFriendsGroupIDByIndex(int iFG)
        {
            Write($"GetFriendsGroupIDByIndex {iFG}");
            return (int)0;
        }

        public int GetFriendsGroupMembersCount(FriendsGroupID_t friendsGroupID)
        {
            Write($"GetFriendsGroupMembersCount {friendsGroupID}");
            return 0;
        }

        public void GetFriendsGroupMembersList(FriendsGroupID_t friendsGroupID, IntPtr pOutSteamIDMembers, int nMembersCount)
        {
            Write($"GetFriendsGroupMembersList {friendsGroupID}");
            Marshal.StructureToPtr(SteamEmulator.SteamID, pOutSteamIDMembers, false);
        }

        public string GetFriendsGroupName(FriendsGroupID_t friendsGroupID)
        {
            Write($"GetFriendsGroupName {friendsGroupID}");
            return "";
        }

        public int GetFriendSteamLevel(ulong steamIDFriend)
        {
            Write($"GetFriendSteamLevel {steamIDFriend}");
            return 100;
        }

        public int GetSmallFriendAvatar(ulong steamIDFriend)
        {
            Write($"GetSmallFriendAvatar {(CSteamID)steamIDFriend}");
            if (Avatars.TryGetValue(steamIDFriend, out ImageAvatar avatar))
            {
                return avatar.Small;
            }
            else
            {
                avatar = LoadFromCache(steamIDFriend);
                if (avatar != null)
                {
                    return avatar.Small;
                }
                ThreadPool.QueueUserWorkItem(RequestAvatar, steamIDFriend);
            }
            return DefaultAvatar.Small;
        }

        public int GetMediumFriendAvatar(ulong steamIDFriend)
        {
            Write($"GetMediumFriendAvatar {(CSteamID)steamIDFriend}");
            if (Avatars.TryGetValue(steamIDFriend, out ImageAvatar avatar))
            {
                return avatar.Medium;
            }
            else
            {
                avatar = LoadFromCache(steamIDFriend);
                if (avatar != null)
                {
                    return avatar.Medium;
                }
                ThreadPool.QueueUserWorkItem(RequestAvatar, steamIDFriend);
            }
            return DefaultAvatar.Medium;
        }

        public int GetLargeFriendAvatar(ulong steamIDFriend)
        {
            Write($"GetLargeFriendAvatar {(CSteamID)steamIDFriend}");
            if (Avatars.TryGetValue(steamIDFriend, out ImageAvatar avatar))
            {
                return avatar.Large;
            }
            else
            {
                avatar = LoadFromCache(steamIDFriend);
                if (avatar != null)
                {
                    return avatar.Large;
                }
                ThreadPool.QueueUserWorkItem(RequestAvatar, steamIDFriend);
            }
            return DefaultAvatar.Large;
        }

        public int GetNumChatsWithUnreadPriorityMessages()
        {
            Write($"GetNumChatsWithUnreadPriorityMessages");
            return 0;
        }

        public int GetPersonaState()
        {
            Write($"GetPersonaState");
            return (int)EPersonaState.k_EPersonaStateOnline;
        }

        public string GetPlayerNickname(ulong steamIDPlayer)
        {
            Write($"GetPlayerNickname {steamIDPlayer}");
            var Result = "";
            MutexHelper.Wait("Users", delegate
            {
                if (steamIDPlayer == SteamEmulator.SteamID)
                {
                    Result = SteamEmulator.PersonaName;
                }
                var friend = GetUser(steamIDPlayer);
                if (friend == null)
                {
                    Result = "";
                }

            });
            return Result;
        }

        public uint GetUserRestrictions()
        {
            Write($"GetUserRestrictions");
            return 0;
        }

        public bool HasFriend(ulong steamIDFriend, int iFriendFlags)
        {
            Write($"HasFriend {steamIDFriend}");
            var friend = GetUser(steamIDFriend);
            return friend != null && friend.HasFriend;
        }

        public bool InviteUserToGame(ulong steamIDFriend, string pchConnectString)
        {
            Write($"InviteUserToGame {steamIDFriend} {pchConnectString}");
            return false;
        }

        public bool IsClanChatAdmin(ulong steamIDClanChat, ulong steamIDUser)
        {
            Write($"IsClanChatAdmin {steamIDClanChat}");
            return false;
        }

        public bool IsClanChatWindowOpenInSteam(ulong steamIDClanChat)
        {
            Write($"IsClanChatWindowOpenInSteam {steamIDClanChat}");
            return false;
        }

        public bool IsClanOfficialGameGroup(ulong steamIDClan)
        {
            Write($"IsClanOfficialGameGroup {steamIDClan}");
            return false;
        }

        public bool IsClanPublic(ulong steamIDClan)
        {
            Write($"IsClanpublic {steamIDClan}");
            return false;
        }

        public SteamAPICall_t IsFollowing(ulong steamID)
        {
            Write($"IsFollowing {steamID}");
            // FriendsIsFollowing_t
            return k_uAPICallInvalid;
        }

        public bool IsUserInSource(ulong steamIDUser, ulong steamIDSource)
        {
            Write($"IsUserInSource {steamIDUser}");
            return false;
        }

        public SteamAPICall_t JoinClanChatRoom(ulong steamIDClan)
        {
            Write($"JoinClanChatRoom {steamIDClan}");
            // JoinClanChatRoomCompletionResult_t
            return k_uAPICallInvalid;
        }

        public bool LeaveClanChatRoom(ulong steamIDClan)
        {
            Write($"LeaveClanChatRoom {steamIDClan}");
            return true;
        }

        public bool OpenClanChatWindowInSteam(ulong steamIDClanChat)
        {
            Write($"OpenClanChatWindowInSteam {steamIDClanChat}");
            return false;
        }

        public bool RegisterProtocolInOverlayBrowser(string pchProtocol)
        {
            Write($"RegisterProtocolInOverlayBrowser {pchProtocol}");
            return false;
        }

        public bool ReplyToFriendMessage(ulong steamIDFriend, string pchMsgToSend)
        {
            Write($"ReplyToFriendMessage {steamIDFriend} {pchMsgToSend}");
            return false;
        }

        public SteamAPICall_t RequestClanOfficerList(ulong steamIDClan)
        {
            Write($"RequestClanOfficerList {steamIDClan}");
            // ClanOfficerListResponse_t
            return k_uAPICallInvalid;
        }

        public void RequestFriendRichPresence(ulong steamIDFriend)
        {
            Write($"RequestFriendRichPresence {steamIDFriend}");
        }

        public bool RequestUserInformation(ulong steamIDUser, bool bRequireNameOnly)
        {
            Write($"RequestUserInformation {(CSteamID)steamIDUser}");
            return false;
        }

        public bool SendClanChatMessage(ulong steamIDClanChat, string pchText)
        {
            Write($"SendClanChatMessage {steamIDClanChat} {pchText}");
            return false;
        }

        public void SetInGameVoiceSpeaking(ulong steamIDUser, bool bSpeaking)
        {
            Write($"SetInGameVoiceSpeaking {steamIDUser}");
        }

        public bool SetListenForFriendsMessages(bool bInterceptEnabled)
        {
            Write($"SetListenForFriendsMessages {bInterceptEnabled}");
            return true;
        }

        public SteamAPICall_t SetPersonaName(string pchPersonaName)
        {
            Write($"SetPersonaName {pchPersonaName}");
            SteamAPICall_t APICall = k_uAPICallInvalid;

            SetPersonaNameResponse_t data = new SetPersonaNameResponse_t();
            data.m_bSuccess = true;
            data.m_bLocalSuccess = true;
            data.m_result = EResult.k_EResultOK;

            APICall = CallbackManager.AddCallbackResult(data);
            ReportUserChanged((ulong)SteamEmulator.SteamID, EPersonaChange.k_EPersonaChangeName);

            SteamEmulator.PersonaName = pchPersonaName;
            MutexHelper.Wait("Users", delegate
            {

            });
            var user = GetUser((ulong)SteamEmulator.SteamID);
            if (user != null)
            {
                user.PersonaName = pchPersonaName;
                NetworkManager.BroadcastStatusUpdated(user);
            }

            return APICall;
        }

        public void SetPlayedWith(ulong steamIDUserPlayedWith)
        {
            Write($"SetPlayedWith {steamIDUserPlayedWith}");
        }

        public bool SetRichPresence(string pchKey, string pchValue)
        {
            Write($"SetRichPresence (Key = {pchKey}, Value = {pchValue})");

            if (!string.IsNullOrEmpty(pchValue))
            {
                if (RichPresence.ContainsKey(pchKey))
                {
                    RichPresence[pchKey] = pchValue;
                }
                else
                {
                    RichPresence.Add(pchKey, pchValue);
                }
            }
            else
            {
                if (RichPresence.ContainsKey(pchKey))
                {
                    RichPresence.Remove(pchKey);
                }
            }

            return true;
        }

        public void UpdateUserLobby(ulong userSteamId, ulong lobbySteamId)
        {
            var user = GetUser(userSteamId);
            if (user != null)
            {
                user.LobbyID = lobbySteamId;
                if (userSteamId == SteamEmulator.SteamID)
                {
                    NetworkManager.BroadcastStatusUpdated(user);
                }
            }
        }

        public void UpdateUserStatus(NET_UserDataUpdated statusChanged, string IPAddress)
        {
            var user = GetUser(new CSteamID(statusChanged.AccountID));
            if (user != null)
            {
                user.LobbyID = statusChanged.LobbyID;
                if (user.PersonaName != statusChanged.PersonaName)
                {
                    user.PersonaName = statusChanged.PersonaName;
                    ReportUserChanged(user.SteamID, EPersonaChange.k_EPersonaChangeName);
                }
                if (user.LobbyID != statusChanged.LobbyID)
                {
                    user.LobbyID = statusChanged.LobbyID;
                    // TODO: Update in SteamMatchmaking
                }
            }
            else
            {
                user = new SteamPlayer()
                {
                    AccountID = statusChanged.AccountID,
                    SteamID = (ulong)new CSteamID(statusChanged.AccountID),
                    HasFriend = true,
                    PersonaName = statusChanged.PersonaName,
                    IPAddress = IPAddress
                };
                Users.Add(user);
            }
        }

        public SteamPlayer GetUser(ulong steamID)
        {
            SteamPlayer user = default;
            MutexHelper.Wait("Users", delegate
            {
                user = Users.Find(u => u.SteamID == steamID);
            });
            return user;
        }
        public SteamPlayer GetUser(CSteamID steamID)
        {
            return GetUser((ulong)steamID);
        }

        public SteamPlayer GetUserByAddress(string iPAddress)
        {
            SteamPlayer user = default;
            MutexHelper.Wait("Users", delegate
            {
                user = Users.Find(u => u.IPAddress.Contains(iPAddress));
            });
            return user;
        }

        public byte[] GetAvatar(ulong steamID)
        {
            if (Avatars.TryGetValue(steamID, out var avatar))
            {
                return avatar.GetImage();
            }
            return new byte[0];
        }

        public (int, int) GetImageSize(int index)
        {
            if (DefaultAvatar.Small == index) return (32, 32);
            if (DefaultAvatar.Medium == index) return (64, 64);
            if (DefaultAvatar.Large == index) return (184, 184);

            foreach (var KV in SteamFriends.Instance.Avatars)
            {
                var avatar = KV.Value;
                if (avatar.Small == index)  return (32, 32);
                if (avatar.Medium == index) return (64, 64);
                if (avatar.Large == index)  return (184, 184);
            }

            return (0, 0);
        }

        public void AddOrUpdateUser(uint accountID, string personaName, uint appID, string IPAddress)
        {
            MutexHelper.Wait("Users", delegate
            {
                var user = GetUser((ulong)new CSteamID(accountID));
                if (user == null)
                {
                    CSteamID steamID = new CSteamID(accountID);
                    user = new SteamPlayer()
                    {
                        PersonaName = personaName,
                        AccountID = accountID,
                        SteamID = (ulong)steamID,
                        GameID = appID,
                        IPAddress = IPAddress,
                        HasFriend = true
                    };
                    Users.Add(user);
                    Write($"Added user {personaName} {steamID}, from {user.IPAddress}");
                }
                else
                {
                    user.PersonaName = personaName;
                }
            });
        }
        public ImageAvatar GetImageAvatar(int index)
        {
            if (DefaultAvatar.Small == index) return DefaultAvatar;
            if (DefaultAvatar.Medium == index) return DefaultAvatar;
            if (DefaultAvatar.Large == index) return DefaultAvatar;

            foreach (var KV in Avatars)
            {
                var avatar = KV.Value;
                if (avatar.Small == index || avatar.Medium == index || avatar.Large == index)
                {
                    return avatar;
                }
            }
            return null;
        }

        private void RequestAvatar(object threadObj)
        {
            ulong steamIDFriend = (ulong)threadObj;

            try
            {

                if (QueryingAvatar.Contains(steamIDFriend)) return;

                var User = GetUser(steamIDFriend);
                if (User != null)
                {
                    QueryingAvatar.Add(steamIDFriend);
                    NetworkManager.RequestAvatar(User.IPAddress);
                }
            }
            catch
            {
                if (QueryingAvatar.Contains(steamIDFriend))
                    QueryingAvatar.Remove(steamIDFriend);
            }
        }

        public void AddOrUpdateAvatar(Bitmap image, ulong steamID)
        {
            if (Avatars.TryGetValue(steamID, out ImageAvatar avatar))
            {
                avatar.UpdateImage(image);
            }
            else
            {
                avatar = new ImageAvatar(image, ref ImageIndex);
                Avatars.TryAdd(steamID, avatar);
                ReportUserChanged(steamID, EPersonaChange.k_EPersonaChangeAvatar);
            }
            if (QueryingAvatar.Contains(steamID))
                QueryingAvatar.Remove(steamID);
        }

        private ImageAvatar LoadFromCache(ulong steamIDFriend)
        {
            string fullPath = Path.Combine(SteamEmulator.SteamRemoteStorage.AvatarCachePath, steamIDFriend.GetAccountID() + ".jpg");
            if (File.Exists(fullPath))
            {
                try
                {
                    var image = (Bitmap)Bitmap.FromFile(fullPath);
                    if (image != null)
                    {
                        ImageAvatar avatar = new ImageAvatar(image, ref ImageIndex);
                        Avatars.TryAdd(steamIDFriend, avatar);
                        return avatar;
                    }
                }
                catch (Exception)
                {
                }
            }
            return null;
        }

        public class ImageAvatar
        {
            public int Small;
            public int Medium;
            public int Large;

            public byte[] SmallBytes;
            public byte[] MediumBytes;
            public byte[] LargeBytes;

            public uint Width;
            public uint Height;
            public byte[] Image;

            public ImageAvatar(Bitmap image, ref int imageIndex)
            {
                try
                {
                    imageIndex++;
                    Small = imageIndex;

                    imageIndex++;
                    Medium = imageIndex;

                    imageIndex++;
                    Large = imageIndex;

                    var resized32 = ImageHelper.Resize(image, 32, 32); 
                    SmallBytes = ImageHelper.ConvertToRGBA(resized32); 

                    var resized64 = ImageHelper.Resize(image, 64, 64); 
                    MediumBytes = ImageHelper.ConvertToRGBA(resized64); 

                    var resized184 = ImageHelper.Resize(image, 184, 184); 
                    LargeBytes = ImageHelper.ConvertToRGBA(resized184); 

                    var resized = ImageHelper.Resize(image, 200, 200); 
                    Image = ImageHelper.ImageToBytes(resized);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            public byte[] GetImage()
            {
                return Image;
            }


            public byte[] GetImage(int iImage)
            {
                Bitmap image = (Bitmap)ImageHelper.ImageFromBytes(Image);
                if (iImage == Small)
                {
                    if (SmallBytes.Length == 0)
                    {
                        var resized = ImageHelper.Resize(image, 32, 32);
                        SmallBytes = ImageHelper.ConvertToRGBA(resized);
                    }
                    return SmallBytes;
                } 
                if (iImage == Medium)
                {
                    if (MediumBytes.Length == 0)
                    {
                        var resized = ImageHelper.Resize(image, 64, 64);
                        MediumBytes = ImageHelper.ConvertToRGBA(resized);
                    }
                    return MediumBytes;
                }
                if (iImage == Large)
                {
                    if (LargeBytes.Length == 0)
                    {
                        var resized = ImageHelper.Resize(image, 184, 184);
                        LargeBytes = ImageHelper.ConvertToRGBA(resized);
                    }
                    return LargeBytes;
                }

                var resizedIMG = ImageHelper.Resize(image, 32, 32);
                var Bytes = ImageHelper.ConvertToRGBA(resizedIMG);
                return Bytes;
            }

            public void UpdateImage(Bitmap image)
            {
                var resized32 = ImageHelper.Resize(image, 32, 32);
                SmallBytes = ImageHelper.ConvertToRGBA(resized32);

                var resized64 = ImageHelper.Resize(image, 64, 64);
                MediumBytes = ImageHelper.ConvertToRGBA(resized64);

                var resized184 = ImageHelper.Resize(image, 184, 184);
                LargeBytes = ImageHelper.ConvertToRGBA(resized184);

                var resized = ImageHelper.Resize(image, 200, 200);
                Image = ImageHelper.ImageToBytes(resized);
            }
        }
    }
}

