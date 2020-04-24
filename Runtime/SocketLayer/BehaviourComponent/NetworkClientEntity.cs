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
    public abstract class NetworkClientEntityBase<TIdentifier> : SerializedUnitySingleton<NetworkClientEntityBase<TIdentifier>>, ISocketClientInjector<TIdentifier> where TIdentifier : IComparable<TIdentifier>
    {
        [ShowInInspector]
        public static bool IsClient { get; private set; }

        [ShowInInspector]
        private SocketClient<TIdentifier> socketClient;

        #region connexion info

        [ReadOnly]
        protected string ServerAddress => SocketConfig.Instance.ServerAddress;

        [ReadOnly]
        protected int ServerPort => SocketConfig.Instance.ServerPort;

        [ShowIf(nameof(socketClient), Value = null)]
        public int portTCP => socketClient.PortTCP;

        [ShowIf(nameof(socketClient), Value = null)]
        public int portUDP => socketClient.PortUDP;

        #endregion

        #region ISocketClientInjector

        public SocketClient<TIdentifier> SocketClient
        {
            get
            {
                return socketClient;
            }
        }

        public void InjectSocketClient(IEnumerable<ISocketClient<TIdentifier>> socketClients)
        {
            foreach (ISocketClient<TIdentifier> socketClient in socketClients)
            {
                socketClient.SocketClient = SocketClient;
            }
        }

        #endregion

        [SerializeField]
        public UnityEvent OnClientConnected;

        protected override void Awake()
        {
            base.Awake();
        }

        private void OnEnable()
        {
            if (OnClientConnected != null && socketClient != null)
            {
                socketClient.OnConnect -= OnClientConnected.Invoke;
                socketClient.OnConnect += OnClientConnected.Invoke;
            }
        }

        protected abstract SocketClient<TIdentifier> CreateClient();


        [Button]
        [DisableIf(nameof(IsClient), Value = false)]
        public void Connect()
        {
            socketClient = CreateClient();

            // hook up the OnConnected event

            SocketClient.OnConnect -= InvokeClientConnectedEvent;
            SocketClient.OnConnect += InvokeClientConnectedEvent;

            // connect to the server

            SocketClient.Connect();
        }


        private void InvokeClientConnectedEvent()
        {
            IsClient = true;
            ThreadCommandProcessor.Append(new ThreadCommandMainAction(() => OnClientConnected?.Invoke()));
        }

        public void InjectSocketClient()
        {
            ISocketClient<TIdentifier>[] socketClients = GetComponentsInChildren<ISocketClient<TIdentifier>>();

            InjectSocketClient(socketClients);
        }


        [Button]
        [EnableIf(nameof(IsClient), Value = true)]
        public void Disconnect()
        {
            if (socketClient != null)
            {
                socketClient.OnConnect -= InvokeClientConnectedEvent;
            }

            socketClient?.Disconnect();
            socketClient = null;

            IsClient = false;
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
