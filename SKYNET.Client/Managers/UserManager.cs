﻿using SKYNET.Common;
using SKYNET.Helper;
using SKYNET.Steamworks;
using SKYNET.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Managers
{
    public class UserManager
    {
        public static event EventHandler<SteamPlayer> OnUserAdded;
        public static event EventHandler<SteamPlayer> OnUserUpdated;
        public static event EventHandler<SteamPlayer> OnUserRemoved;
        public static List<SteamPlayer> Users;

        static UserManager()
        {
            Users = new List<SteamPlayer>();
        }

        public static void Initialize()
        {
            
        }

        public static SteamPlayer GetUser(ulong steamID)
        {
            SteamPlayer user = default;
            MutexHelper.Wait("Users", delegate
            {
                user = Users.Find(u => u.SteamID == steamID);
            });
            return user;
        }
        public static SteamPlayer GetUser(CSteamID steamID)
        {
            return GetUser((ulong)steamID);
        }

        public static void AddOrUpdateUser(uint accountID, string personaName, uint appID, string IPAddress)
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
                    OnUserAdded?.Invoke(null, user);
                    Write($"Added user {personaName} {steamID}, from {user.IPAddress}");
                }
                else
                {
                    user.PersonaName = personaName;
                    OnUserUpdated?.Invoke(null, user);
                }
            });
        }

        public static void RemoveUser(uint accountID)
        {
            MutexHelper.Wait("Users", delegate
            {
                var user = GetUser((ulong)new CSteamID(accountID));
                if (user != null)
                {
                    OnUserRemoved?.Invoke(null, user);
                    Write($"Removed user {user.PersonaName} {user.SteamID}");
                    Users.Remove(user);
                    
                }
            });
        }

        private static void Write(object msg)
        {
            Log.Write("UserManager", msg);
        }
    }
}