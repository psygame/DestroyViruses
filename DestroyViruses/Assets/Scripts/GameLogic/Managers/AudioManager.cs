﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DestroyViruses
{
    public class AudioManager : Singleton<AudioManager>
    {
        Dictionary<string, AudioClip> mCached = new Dictionary<string, AudioClip>();

        //音乐播放器
        public AudioSource MusicPlayer;
        //开火音乐
        public AudioSource FireMusicPlayer;
        //音效播放器
        public AudioSource SoundPlayer;

        private string mLastMusic = "";
        //播放音乐
        public void PlayMusic(string name, float volume = 1f, bool loop = true)
        {
            if (mLastMusic != name || !MusicPlayer.isPlaying)
            {
                AudioClip clip = GetClip(name);
                MusicPlayer.clip = clip;
                MusicPlayer.loop = loop;
                MusicPlayer.volume = volume;
                MusicPlayer.Play();
                mLastMusic = name;
            }
        }

        public void StopMusic()
        {
            if (MusicPlayer.isPlaying)
                MusicPlayer.Stop();
        }

        private string mLastFireMusic = "";
        //播放音乐
        public void PlayFireMusic(string name, float volume = 1f, bool loop = true)
        {
            if (mLastFireMusic != name || !FireMusicPlayer.isPlaying)
            {
                AudioClip clip = GetClip(name);
                FireMusicPlayer.clip = clip;
                FireMusicPlayer.loop = loop;
                FireMusicPlayer.volume = volume;
                FireMusicPlayer.Play();
                mLastFireMusic = name;
            }
        }

        public void StopFireMusic()
        {
            if (FireMusicPlayer.isPlaying)
                FireMusicPlayer.Stop();
        }

        //播放音效
        public void PlaySound(string name)
        {
            AudioClip clip = GetClip(name);
            SoundPlayer.clip = clip;
            SoundPlayer.PlayOneShot(clip);
        }

        public void StopSound()
        {
            SoundPlayer.Stop();
        }

        private AudioClip GetClip(string name)
        {
            var path = "Sounds/" + name;
            if (!mCached.TryGetValue(path, out AudioClip clip))
            {
                clip = Resources.Load<AudioClip>(path);
                mCached.Add(path, clip);
            }
            return clip;

        }

        public static void Preload(string name)
        {
            Instance.GetClip(name);
        }
    }
}