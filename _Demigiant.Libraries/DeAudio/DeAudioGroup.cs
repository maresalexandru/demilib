﻿// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/21 18:43
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;
using DG.DeAudio.Core;
using DG.DeAudio.Events;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

namespace DG.DeAudio
{
    /// <summary>
    /// Audio group, connected to an AudioMixerGroup
    /// </summary>
    [System.Serializable]
    public class DeAudioGroup : IDisposable
    {
        public AudioMixerGroup mixerGroup;
        public DeAudioGroupId id;
        public bool recycle = true;
        /// <summary>Max AudioSources for this group. DO NOT change this at runtime</summary>
        public int maxSources = -1;
        public int preallocate = 0;
        public float fooVolume = 1;

        [System.NonSerialized] public List<DeAudioSource> sources; // Sources per each group
        public float volume {
            get { return fooVolume; }
            set { SetVolume(value); }
        }

        bool _disposed;
        GameObject _sourcesContainer;
        Tween _fadeTween;

        // ▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬
        // INIT

        internal void Init(Transform container)
        {
            _sourcesContainer = new GameObject(id.ToString());
            _sourcesContainer.transform.parent = container;
            sources = maxSources >= 0 ? new List<DeAudioSource>(maxSources) : new List<DeAudioSource>();
            if (preallocate > 0) {
                for (int i = 0; i < preallocate; ++i) sources.Add(new DeAudioSource(this, _sourcesContainer));
            }
        }

        #region Public Methods

        /// <summary>
        /// Play the given sound with the given options
        /// </summary>
        public DeAudioSource Play(AudioClip clip, float volume = 1, bool loop = false)
        {
            DeAudioSource source = GetAvailableSource();
            if (source == null) {
                if (DeAudioManager.I.logInfo) Debug.Log(DeAudioManager.LogPrefix + "Clip can't be played because all sources are busy");
                return null;
            }
            source.Play(clip, volume, loop);
            return source;
        }

        /// <summary>
        /// Stop all sounds for this group
        /// </summary>
        public void Stop()
        { IterateOnAllSources(OperationType.Stop); }
        /// <summary>Stop all sounds for this group that are using the given clip</summary>
        public void Stop(AudioClip clip)
        { IterateOnAllSources(OperationType.StopByClip, clip); }

        #region Tweens

        /// <summary>Fades out this group's volume</summary>
        public void FadeOut(float duration = 1.5f, bool ignoreTimeScale = true, bool stopOnComplete = true, TweenCallback onComplete = null)
        { FadeTo(0, duration, ignoreTimeScale, stopOnComplete, onComplete); }
        /// <summary>Fades in this group's volume</summary>
        public void FadeIn(float duration = 1.5f, bool ignoreTimeScale = true, TweenCallback onComplete = null)
        { FadeTo(1, duration, ignoreTimeScale, false, onComplete); }
        /// <summary>Fades this group's volume to the given value</summary>
        public void FadeTo(float to, float duration = 1.5f, bool ignoreTimeScale = true, TweenCallback onComplete = null)
        { FadeTo(to, duration, ignoreTimeScale, false, onComplete); }
        internal void FadeTo(float to, float duration, bool ignoreTimeScale, bool stopOnComplete, TweenCallback onComplete)
        {
            _fadeTween.Kill();
            _fadeTween = DOTween.To(() => volume, x => volume = x, to, duration)
                .SetTarget(this).SetUpdate(ignoreTimeScale).SetEase(Ease.Linear);
            if (stopOnComplete) _fadeTween.OnStepComplete(Stop);
            if (onComplete != null) _fadeTween.OnComplete(onComplete);
        }

        #endregion

        #endregion

        #region Methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing) {
                _fadeTween.Kill();
                int len = sources.Count;
                for (int i = 0; i < len; ++i) sources[i].Dispose();
                sources.Clear();
            }
            _disposed = true;
        }

        // Internal so it can be accessed by the editor at runtime
        internal void SetVolume(float volume)
        {
            fooVolume = volume;
            DeAudioNotificator.DispatchDeAudioEvent(DeAudioEventType.GroupVolumeChange, this);
        }

        // Either:
        // - returns an existing non playing source if available
        // - creates a new source if all existing are busy and maxSources allows it
        // - return NULL if none of the previous options work
        DeAudioSource GetAvailableSource()
        {
            int len = sources.Count;
            for (int i = 0; i < len; ++i) {
                DeAudioSource s = sources[i];
                if (s.isFree) return s;
            }
            // No free sources...
            if (maxSources < 0 || len < maxSources) {
                // Create new source
                DeAudioSource s = new DeAudioSource(this, _sourcesContainer);
                sources.Add(s);
                return s;
            } else if (recycle) {
                // Recycle oldest source
                return GetOldestSource();
            } else {
                // All sources busy and can't be recycled
                return null;
            }
        }

        void IterateOnAllSources(OperationType operationType, AudioClip clip = null)
        {
            int len = sources.Count;
            for (int i = 0; i < len; ++i) {
                DeAudioSource s = sources[i];
                switch (operationType) {
                case OperationType.Stop:
                    s.Stop();
                    break;
                case OperationType.StopByClip:
                    if (s.clip == clip) s.Stop();
                    break;
                }
            }
        }

        #region Helpers

        DeAudioSource GetOldestSource()
        {
            int len = sources.Count;
            float time = float.MaxValue;
            DeAudioSource res = null;
            for (int i = 0; i < len; ++i) {
                DeAudioSource s = sources[i];
                if (s.playTime >= time) continue;
                res = s;
                time = s.playTime;
            }
            return res;
        }

        #endregion

        #endregion
    }
}