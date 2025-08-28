#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
#endif
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField]
        private bool isPreload = default;

        [SerializeField]
        private int preloadValue = default;

        private Queue<AudioSource> freeAudioSources = new Queue<AudioSource>();

        private List<AudioSource> busyAudioSources = new List<AudioSource>();

        private List<AudioSource> standaloneAudioSources = new List<AudioSource>();

        private AudioSource currentAudio;

        protected void Awake()
        {
            Clear();

            if (isPreload)
                Preload();
        }

#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        #if ODIN_INSPECTOR[Button]#endif
#endif
        private void Clear()
        {
            // clear standalone

            for (int i = standaloneAudioSources.Count - 1; i >= 0; i--)
            {
#if UNITY_EDITOR
                DestroyImmediate(standaloneAudioSources[i]);
#else
                Destroy(standaloneAudioSources[i]);
#endif
            }

            // clear busy

            for (int i = busyAudioSources.Count - 1; i >= 0; i--)
            {
#if UNITY_EDITOR
                DestroyImmediate(busyAudioSources[i]);
#else
                Destroy(busyAudioSources[i]);
#endif
            }

            // clear free

            while (freeAudioSources.Count != 0)
            {
#if UNITY_EDITOR
                DestroyImmediate(freeAudioSources.Dequeue());
#else
                Destroy(freeAudioSources.Dequeue());
#endif
            }
        }

        private void Preload()
        {
            for (int i = 0; i < preloadValue; i++)
            {
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();

                ResetAudioSource(audioSource);

                freeAudioSources.Enqueue(audioSource);
            }
        }

        private void ResetAudioSource(AudioSource audioSource)
        {
            audioSource.volume = 1;
            audioSource.loop = false;
            audioSource.outputAudioMixerGroup = null;
            audioSource.playOnAwake = false;
            audioSource.clip = null;
        }

        private void Update()
        {
            for (int i = busyAudioSources.Count - 1; i >= 0; i--)
            {
                currentAudio = busyAudioSources[i];

                // if the audio stopped

                if (!currentAudio.isPlaying && currentAudio.time >= currentAudio.clip.length)
                {
                    busyAudioSources.RemoveAt(i);

                    freeAudioSources.Enqueue(currentAudio);
                }
            }
        }

        public AudioSource GetStandaloneAudioSource()
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();

            standaloneAudioSources.Add(audioSource);

            ResetAudioSource(audioSource);

            return audioSource;
        }

        public void RemoveStandaloneAudioSource(AudioSource audioSource)
        {
            // if item was found and removed

            if (standaloneAudioSources.Remove(audioSource))
            {
                freeAudioSources.Enqueue(audioSource);
            }
        }

        private AudioSource GetFreeAudioSource()
        {
            if (freeAudioSources.Count == 0)
            {
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();

                return audioSource;
            }

            return freeAudioSources.Dequeue();
        }

#if ODIN_INSPECTOR
        #if ODIN_INSPECTOR[Button]#endif
#endif
        public AudioSource PlayOnShot(AudioClip audioClip)
        {
            if (audioClip == null)
                return null;

            AudioSource audioSource = GetFreeAudioSource();

            ResetAudioSource(audioSource);

            audioSource.clip = audioClip;

            audioSource.Play();
            busyAudioSources.Add(audioSource);

            return audioSource;
        }

    }
}
