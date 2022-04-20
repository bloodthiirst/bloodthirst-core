using Bloodthirst.Core.Singleton;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.Audio
{
    public class AudioManager : BSingleton<AudioManager>
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

        private List<AudioSource> standaloneAudioSources;

        private List<AudioSource> StandaloneAudioSources
        {
            get
            {
                if (standaloneAudioSources == null)
                {
                    standaloneAudioSources = new List<AudioSource>();
                }

                return standaloneAudioSources;
            }
        }


        private AudioSource currentAudio;

        protected void Awake()
        {
            Clear();

            if (isPreload)
                Preload();
        }

        [Button]
        private void Clear()
        {
            // clear standalone

            for (int i = StandaloneAudioSources.Count - 1; i >= 0; i--)
            {
#if UNITY_EDITOR
                DestroyImmediate(StandaloneAudioSources[i]);
#else
                Destroy(StandaloneAudioSources[i]);
#endif
            }

            // clear busy

            for (int i = BusyAudioSources.Count - 1; i >= 0; i--)
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
            for (int i = 0; i < preloadValue; i++)
            {
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();

                ResetAudioSource(audioSource);

                FreeAudioSources.Enqueue(audioSource);
            }
        }

        private void ResetAudioSource(AudioSource audioSource)
        {
            audioSource.playOnAwake = false;
            audioSource.clip = null;
        }

        private void Update()
        {
            for (int i = BusyAudioSources.Count - 1; i >= 0; i--)
            {

                currentAudio = BusyAudioSources[i];

                // if the audio stopped

                if (!currentAudio.isPlaying)
                {
                    BusyAudioSources.RemoveAt(i);

                    FreeAudioSources.Enqueue(currentAudio);
                }
            }
        }

        public AudioSource GetStandaloneAudioSource()
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();

            StandaloneAudioSources.Add(audioSource);

            ResetAudioSource(audioSource);

            return audioSource;
        }

        public void RemoveStandaloneAudioSource(AudioSource audioSource)
        {
            // if item was found and removed

            if (StandaloneAudioSources.Remove(audioSource))
            {
                FreeAudioSources.Enqueue(audioSource);
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

            ResetAudioSource(audioSource);

            audioSource.clip = audioClip;

            audioSource.Play();
            BusyAudioSources.Add(audioSource);

            return audioSource;
        }

    }
}
