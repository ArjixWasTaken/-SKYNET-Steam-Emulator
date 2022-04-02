﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyHook;
using SKYNET;
using SKYNET.Helper;
using SKYNET.Manager;
using SKYNET.Types;

namespace SKYNET.Hook.Handles
{
    public partial class SteamInternal : BaseHook
    {
        public override bool Installed { get; set; }

        public unsafe override void Install()
        {
            // SteamInternal Handles
            base.Install<SteamInternal_FindOrCreateUserInterfaceDelegate>("SteamInternal_FindOrCreateUserInterface", _SteamInternal_FindOrCreateUserInterface, new SteamInternal_FindOrCreateUserInterfaceDelegate(SteamInternal_FindOrCreateUserInterface));
            base.Install<SteamInternal_FindOrCreateGameServerInterfaceDelegate>("SteamInternal_FindOrCreateGameServerInterface", _SteamInternal_FindOrCreateGameServerInterfaceDelegate, new SteamInternal_FindOrCreateGameServerInterfaceDelegate(SteamInternal_FindOrCreateGameServerInterface));
            base.Install<SteamInternal_CreateInterfaceDelegate>("SteamInternal_CreateInterface", _SteamInternal_CreateInterfaceDelegate, new SteamInternal_CreateInterfaceDelegate(SteamInternal_CreateInterface));
            base.Install<SteamInternal_GameServer_InitDelegate>("SteamInternal_GameServer_Init", _SteamInternal_GameServer_InitDelegate, new SteamInternal_GameServer_InitDelegate(SteamInternal_GameServer_Init));
            //base.Install<SteamInternal_ContextInitDelegate>("SteamInternal_ContextInit", _SteamInternal_ContextInitDelegate, new SteamInternal_ContextInitDelegate(SteamInternal_ContextInit));
        }

        public IntPtr SteamInternal_FindOrCreateUserInterface(int hSteamUser, [MarshalAs(UnmanagedType.LPStr)] string pszVersion)
        {
            Write($"SteamInternal_FindOrCreateUserInterface {pszVersion}");
            return InterfaceManager.FindOrCreateInterface(pszVersion); 
        }

        public IntPtr SteamInternal_FindOrCreateGameServerInterface(int hSteamUser, [MarshalAs(UnmanagedType.LPStr)] string pszVersion)
        {
            Write($"SteamInternal_FindOrCreateGameServerInterface {pszVersion}");
            return InterfaceManager.FindOrCreateInterface(pszVersion);
        }

        public IntPtr SteamInternal_CreateInterface([MarshalAs(UnmanagedType.LPStr)] string pszVersion)
        {
            Write($"SteamInternal_CreateInterface {pszVersion}");
            return InterfaceManager.FindOrCreateInterface(pszVersion);
        }

        public bool SteamInternal_GameServer_Init(IntPtr unIP, IntPtr usPort, IntPtr usGamePort, IntPtr usQueryPort, IntPtr eServerMode, [MarshalAs(UnmanagedType.LPStr)] string pchVersionString)
        {
            Write($"SteamInternal_GameServer_Init {pchVersionString}");
            return true;
        }

        public unsafe IntPtr SteamInternal_ContextInit(void* c_contextPointer)
        {
            Main.Write("SteamInternal_ContextInit");

            return SteamEmulator.ContextAddress;

            ContextInitData* CreatedContext = (ContextInitData*)c_contextPointer;

            if (CreatedContext->Context.SteamClient() != SteamEmulator.SteamClient.MemoryAddress)
            {
                Main.Write("SteamInternal_ContextInit initializing");
                CreatedContext->Context.Init();
                CreatedContext->counter = 1;
            }

        }

        public struct ContextInitData
        {
            public uint counter;
            public CSteamApiContext Context;
        }

        public override void Write(object v)
        {
            Main.Write("SteamInternal", v);
        }
    }
}
