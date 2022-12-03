using Bloodthirst.Core.BProvider;
using Bloodthirst.Core.ThreadProcessor;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using Bloodthirst.Scripts.SocketLayer.BehaviourComponent;
using Bloodthirst.Socket.BehaviourComponent;
#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Bloodthirst.Socket.Core
{
    public abstract class NetworkClientEntityBase<TClient, TIdentifier> : MonoBehaviour,
        ISocketClientInjector<TClient, TIdentifier>,
        INetworkInjector,
        IAwakePass

        where TClient : SocketClient<TIdentifier>
        where TIdentifier : IComparable<TIdentifier>
    {

#if ODIN_INSPECTOR
[ShowInInspector]
#endif

        public static bool IsClient => SocketConfig.Instance.IsClient;


#if ODIN_INSPECTOR
[ShowInInspector]
#endif

        private TClient socketClient;

        #region connexion info


#if ODIN_INSPECTOR
[ReadOnly]
#endif

        protected string ServerAddress => SocketConfig.Instance.ServerAddress;


#if ODIN_INSPECTOR
[ReadOnly]
#endif

        protected int ServerPort => SocketConfig.Instance.WorldServerPort;

#if ODIN_INSPECTOR
        [ShowIf(nameof(socketClient), Value = null)]
#endif
        public int portTCP => socketClient.PortTCP;

#if ODIN_INSPECTOR
        [ShowIf(nameof(socketClient), Value = null)]
#endif
        public int portUDP => socketClient.PortUDP;

#endregion

#region ISocketClientInjector

        public TClient SocketClient
        {
            get
            {
                return socketClient;
            }
        }

        public void InjectSocketClient(IEnumerable<ISocketClient<TClient, TIdentifier>> socketClients)
        {
            foreach (ISocketClient<TClient, TIdentifier> socketClient in socketClients)
            {
                socketClient.SocketClient = SocketClient;
            }
        }

#endregion

        [SerializeField]
        public UnityEvent OnClientConnected;

        public List<IOnSocketClientConnected<TClient, TIdentifier>> OnConnectedBehaviours;

        private ThreadCommandProcessor _threadCommandProcessor;

        void IAwakePass.Execute()
        {
            _threadCommandProcessor = BProviderRuntime.Instance.GetSingleton<ThreadCommandProcessor>();

            OnConnectedBehaviours = new List<IOnSocketClientConnected<TClient, TIdentifier>>();

            GetComponentsInChildren(OnConnectedBehaviours);

            NetworkInjectionProvider.Add(this);
        }

        private void OnEnable()
        {
            if (OnClientConnected != null && socketClient != null)
            {
                socketClient.OnConnect -= OnClientConnected.Invoke;
                socketClient.OnConnect += OnClientConnected.Invoke;
            }
        }

        protected abstract TClient CreateClient();




#if ODIN_INSPECTOR
[Button]
#endif

        //[DisableIf(nameof(IsClient), Value = false)]
        public void Connect()
        {
            socketClient = CreateClient();

            // hook up the OnConnected event

            SocketClient.OnConnect -= OnClientConnectedTriggered;
            SocketClient.OnConnect += OnClientConnectedTriggered;

            // connect to the server

            SocketClient.Connect();
        }


        private void OnClientConnectedTriggered()
        {
            SocketConfig.Instance.IsClient = true;

            // trigger on client connected events
            _threadCommandProcessor.Append(new ThreadCommandMainAction(InjectSocketClient));


            // trigger the events hooked in the editor
            _threadCommandProcessor.Append(new ThreadCommandMainAction(OnConnectedUnityThread));
        }

        private void OnConnectedUnityThread()
        {
            OnClientConnected?.Invoke();
        }

        public void InjectSocket(GameObject gameObject)
        {
            ISocketClient<TClient, TIdentifier>[] socketClients = gameObject.GetComponentsInChildren<ISocketClient<TClient, TIdentifier>>();

            InjectSocketClient(socketClients);
        }

        private void InvokeOnConnectedBehaviours()
        {
            foreach (IOnSocketClientConnected<TClient, TIdentifier> onConnected in OnConnectedBehaviours)
            {
                onConnected.OnSocketClientConnected(SocketClient);
            }
        }

        public void InjectSocketClient()
        {
            // set socket client
            InjectSocket(gameObject);

            // trigger on socket client connected
            InvokeOnConnectedBehaviours();
        }



#if ODIN_INSPECTOR
[Button]
[EnableIf(nameof(IsClient), Value = true)]
#endif
        public void Disconnect()
        {
            if (socketClient != null)
            {
                socketClient.OnConnect -= OnClientConnectedTriggered;
                socketClient?.Disconnect();
                socketClient = null;
            }

            SocketConfig.Instance.IsClient = false;
        }

        private void OnApplicationQuit()
        {
            Disconnect();
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void OnDisable()
        {
            Disconnect();
        }
    }
}
