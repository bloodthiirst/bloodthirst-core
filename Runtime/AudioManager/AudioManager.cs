using Bloodthirst.Core.UnitySingleton;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.Audio
{
    public class AudioManager : UnitySingleton<AudioManager>
    {
        [SerializeField]
        private bool isPreload = default;

        [SerializeField]
        private int preloadValue = default;

        private Queue<AudioSource> freeAudioSources;

        private Queue<AudioSource> FreeAudioSources
        {
            get
            {
                if (freeAudioSources == null)
                {
                    freeAudioSources = new Queue<AudioSource>();
                }

                return freeAudioSources;
            }
        }

        private List<AudioSource> busyAudioSources;

        private List<AudioSource> BusyAudioSources
        {
            get
            {
                if (busyAudioSources == null)
                {
                    busyAudioSources = new List<AudioSource>();
                }

                return busyAudioSources;
            }
        }

        private AudioSource currentAudio;

        protected override void Awake()
        {
            if (isPreload)
                Preload();
        }

        [Button]
        private void Clear()
        {
            // clear busy

            for(int i = BusyAudioSources.Count - 1; i >= 0; i--)
            {
#if UNITY_EDITOR
                DestroyImmediate(BusyAudioSources[i]);
#else
                Destroy(BusyAudioSources[i]);
#endif
            }

            // clear free

            while (FreeAudioSources.Count != 0)
            {
#if UNITY_EDITOR
                DestroyImmediate(FreeAudioSources.Dequeue());
#else
                Destroy(FreeAudioSources.Dequeue());
#endif
            }
        }

        private void Preload()
        {
            for(int i = 0; i < preloadValue; i++)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();

                FreeAudioSources.Enqueue(source);
            }
        }

        private void Update()
        {
            int i = BusyAudioSources.Count;


            while (i != 0)
            {
                currentAudio = BusyAudioSources[i];

                // if the audio stopped

                if (!currentAudio.isPlaying)
                {
                    BusyAudioSources.RemoveAt(i);

                    FreeAudioSources.Enqueue(currentAudio);
                }

                i--;
            }
        }

        private AudioSource GetFreeAudioSource()
        {
            if (FreeAudioSources.Count == 0)
            {
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();

                return audioSource;
            }

            return FreeAudioSources.Dequeue();
        }

        [Button]
        public AudioSource PlayOnShot(AudioClip audioClip)
        {
            if (audioClip == null)
                return null;

            AudioSource audioSource = GetFreeAudioSource();

            audioSource.clip = audioClip;

            audioSource.Play();

            BusyAudioSources.Add(audioSource);

            return audioSource;
        }

    }
}
