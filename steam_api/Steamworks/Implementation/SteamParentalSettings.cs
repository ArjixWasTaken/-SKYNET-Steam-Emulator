﻿
using SKYNET;
using SKYNET.Steamworks;
using System;
using System.Runtime.InteropServices;

public class SteamParentalSettings : SteamInterface
{
    public bool BIsParentalLockEnabled(IntPtr _)
    {
        Write("boolBIsParentalLockEnabled");
        return false;
    }

    public bool BIsParentalLockLocked(IntPtr _)
    {
        Write("boolBIsParentalLockLocked");
        return false;
    }

    public bool BIsAppBlocked(IntPtr nAppID)
    {
        Write("boolBIsAppBlocked");
        return false;
    }

    public bool BIsAppInBlockList(IntPtr nAppID)
    {
        Write("boolBIsAppInBlockList");
        return false;
    }

    public bool BIsFeatureBlocked(EParentalFeature eFeature)
    {
        Write("boolBIsFeatureBlocked");
        return false;
    }

    public bool BIsFeatureInBlockList(EParentalFeature eFeature)
    {
        Write("boolBIsFeatureInBlockList");
        return false;
    }


    private void Write(string v)
    {
        Main.Write(InterfaceVersion, v);
    }
}