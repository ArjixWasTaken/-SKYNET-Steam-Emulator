﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Steamworks;

namespace SKYNET.Managers
{
    public unsafe class CallbackManager
    {
        public const byte k_ECallbackFlagsRegistered = 1;
        public const byte k_ECallbackFlagsGameServer = 2;
        public static List<SteamAPICall_t> SteamAPICallsCompleted;

        private static Dictionary<int, CCallbackBase> Client_Callbacks;
        private static Dictionary<int, CCallbackBase> Server_Callbacks;
        private static List<SteamAPICall_t> SteamAPICalls;

        static CallbackManager()
        {
            if (Client_Callbacks == null)
            {
                Client_Callbacks = new Dictionary<int, CCallbackBase>();
            }
            if (Server_Callbacks == null)
            {
                Server_Callbacks = new Dictionary<int, CCallbackBase>();
            }
            if (SteamAPICalls == null)
            {
                SteamAPICalls = new List<SteamAPICall_t>();
            }
            if (SteamAPICallsCompleted == null)
            {
                SteamAPICallsCompleted = new List<SteamAPICall_t>();
            }
        }

        public unsafe static void RegisterCallback(int iCallback, CCallbackBase pCallback, bool Server = false)
        {
            pCallback.m_nCallbackFlags |= k_ECallbackFlagsRegistered;
            pCallback.m_iCallback = iCallback;

            if (Server)
            {
                Server_Callbacks.Add(iCallback, pCallback);
            }
            else
            {
                Server_Callbacks.Add(iCallback, pCallback);
            }
        }

        public void UnregisterCallResult(IntPtr pCallback, SteamAPICall_t hAPICall)
        {

        }

        public void UnregisterCallback(IntPtr pCallback)
        {

        }

        public static void RegisterCallResult(CCallbackBase pCallback, SteamAPICall_t hAPICall)
        {
            SteamAPICalls.Add(hAPICall);
        }

        public void RunCallbacks()
        {
            //for (auto & c : callbacks)
            //{
            //    c.second.results.clear();
            //}
        }

        public void FreeCallback(int pipe_id)
        {
            //bool found = Callbacks.TryGetValue(pipe_id, out CCallbackBase value);

            //if (found)
            //{
            //    IntPtr ptr = IntPtr.Zero;
            //    Marshal.StructureToPtr<CCallbackBase>(value, ptr, true);
            //    Marshal.FreeHGlobal(ptr);
            //    Callbacks.Remove(pipe_id);
            //}
        }

        public static bool Contains(SteamAPICall_t hSteamAPICall)
        {
            return SteamAPICalls.Contains(hSteamAPICall);
        }
    }
}
