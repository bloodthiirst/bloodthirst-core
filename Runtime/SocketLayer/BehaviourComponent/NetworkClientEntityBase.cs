using Assets.Scripts.SocketLayer.BehaviourComponent;
using Assets.SocketLayer.BehaviourComponent;
using Assets.SocketLayer.Client.Base;
using Bloodthirst.Core.ThreadProcessor;
using Bloodthirst.Core.UnitySingleton;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Events;

namespace Bloodthirst.Socket.Core
{
    public abstract class NetworkClientEntityBase<TClient, TIdentifier> : 
        UnitySingleton<NetworkClientEntityBase<TClient, TIdentifier>>,
        ISocketClientInjector<TClient, TIdentifier>,
        INetworkInjector
        
        where TClient : SocketClient<TIdentifier>
        where TIdentifier : IComparable<TIdentifier>
    {
        [ShowInInspector]
        public static bool IsClient => SocketConfig.Instance.IsClient;

        [ShowInInspector]
        private TClient socketClient;

        #region connexion info

        [ReadOnly]
        protected string ServerAddress => SocketConfig.Instance.ServerAddress;

        [ReadOnly]
        protected int ServerPort => SocketConfig.Instance.WorldServerPort;

        [ShowIf(nameof(socketClient), Value = null)]
        public int portTCP => socketClient.PortTCP;

        [ShowIf(nameof(socketClient), Value = null)]
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

        protected override void Awake()
        {
            base.Awake();

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



        [Button]
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

            ThreadCommandProcessor.Append(new ThreadCommandMainAction(InjectSocketClient));


            // trigger the events hooked in the editor

            ThreadCommandProcessor.Append(new ThreadCommandMainAction(OnConnectedUnityThread));
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


        [Button]
        [EnableIf(nameof(IsClient), Value = true)]
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
