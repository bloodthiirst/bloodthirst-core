using Sirenix.OdinInspector;
using System.Diagnostics;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Assets.SocketLayer.BehaviourComponent
{
    public class NetworkServerTimer : MonoBehaviour
    {
        [SerializeField]
        private GUIDNetworkServerEntity networkServer;

        private Stopwatch serverTimer;

        /// <summary>
        /// Time elasped in milliseconds
        /// </summary>
        [SerializeField]
        [ReadOnly]
        private long timeElapsed;

        /// <summary>
        /// Time elasped in milliseconds
        /// </summary>
        public long TimeElapsed => serverTimer.ElapsedMilliseconds;

        private void OnValidate()
        {
            networkServer = GetComponent<GUIDNetworkServerEntity>();
        }

        private void Awake()
        {
            timeElapsed = 0;

            serverTimer = new Stopwatch();

            if (networkServer == null)
            {
                networkServer = GetComponent<GUIDNetworkServerEntity>();
            }

#if UNITY_EDITOR
            EditorApplication.pauseStateChanged += OnPauseChanged;
#endif
        }

#if UNITY_EDITOR
        private void OnPauseChanged(PauseState pauseState)
        {
            if (!GUIDNetworkServerEntity.IsServer)
                return;

            if (pauseState == PauseState.Paused)
                serverTimer.Stop();
            else
                serverTimer.Start();
        }
#endif

        private void Update()
        {
            timeElapsed = serverTimer.ElapsedMilliseconds;
        }

        public void ResetServerTimer()
        {
            serverTimer.Restart();
        }

        private void OnDestroy()
        {
            serverTimer.Stop();
        }


        private void OnApplicationQuit()
        {
#if UNITY_EDITOR
            EditorApplication.pauseStateChanged -= OnPauseChanged;
#endif
            serverTimer.Stop();


        }
    }
}
