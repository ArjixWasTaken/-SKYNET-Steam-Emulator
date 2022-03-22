﻿using Core.Interface;
using SKYNET.Interface;
using System;

//[Map("STEAMMUSIC_INTERFACE_VERSION")]
//[Map("SteamMusic")]
public class SteamMusic : IBaseInterface, ISteamMusic
{
    public bool BIsEnabled(IntPtr _)
    {
        return true;
    }

    public bool BIsPlaying(IntPtr _)
    {
        return true;
    }

    public AudioPlayback_Status GetPlaybackStatus(IntPtr _)
    {
        return AudioPlayback_Status.AudioPlayback_Undefined;
    }

    public float GetVolume(IntPtr _)
    {
        return 0;
    }

    public void Pause(IntPtr _)
    {
        //
    }

    public void Play(IntPtr _)
    {
        //
    }

    public void PlayNext(IntPtr _)
    {
        //
    }

    public void PlayPrevious(IntPtr _)
    {
        //
    }

    public void SetVolume(float flVolume)
    {
        //
    }
}