using Assets.SocketLayer.BehaviourComponent;
using Bloodthirst.Core.UnitySingleton;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Events;

namespace Bloodthirst.Socket.Core
{
    public enum SERVICE_QUERY
    {
        REFLECTION,
        SCAN_COMPONENTS
    }

    public abstract class NetworkServerEntityBase<TIdentifier> : SerializedUnitySingleton<NetworkServerEntityBase<TIdentifier>> , ISocketServerInjector<TIdentifier> where TIdentifier : IComparable<TIdentifier>
    {
        [ShowInInspector]
        public static bool IsServer { get; private set; }

        [SerializeField]
        private int Port => SocketConfig.Instance.ServerPort;

        [ShowInInspector]
        protected ManagedSocketServer<TIdentifier> socketServer;

        public ManagedSocketServer<TIdentifier> SocketServer
        {
            get
            {
                return socketServer;
            }
        }

        [ShowInInspector]
        protected int ActivePlayerCount => socketServer == null ? 0 : socketServer.ManagedClientsCount;

        [ShowInInspector]
        protected int WaitingClientCount => socketServer == null ? 0 : socketServer.AnonymousClientsCount;


        #region ISocketServerInjector

        ManagedSocketServer<TIdentifier> ISocketServerInjector<TIdentifier>.SocketServer => SocketServer;

        public void InjectSocketServer(IEnumerable<ISocketServer<TIdentifier>> socketServers)
        {
            foreach(ISocketServer<TIdentifier> socketServer in socketServers)
            {
                socketServer.SocketServer = SocketServer;
            }
        }

        #endregion

        [SerializeField]
        public UnityEvent OnSeverStarted;

        protected override void Awake()
        {
            base.Awake();
        }

        protected abstract ManagedSocketServer<TIdentifier> CreateServer();

        [Button]
        public void StartServer()
        {

            if(socketServer != null)
            {
                socketServer.Stop();
            }

            socketServer = CreateServer();

            SocketServer.OnAnonymousClientConnected -= OnAnonymousConnected;
            SocketServer.OnAnonymousClientConnected += OnAnonymousConnected;

            SocketServer.OnManagedClientConnected -= OnManagedConnected;
            socketServer.OnManagedClientConnected += OnManagedConnected;

            SocketServer.OnAnonymousMessage -= OnAnonymousMessage;
            SocketServer.OnAnonymousMessage += OnAnonymousMessage;

            SocketServer.OnManagedClientMessage -= OnManagedMessage;
            SocketServer.OnManagedClientMessage += OnManagedMessage;

            SocketServer.OnClientDisconnected -= OnClientDisconnected;
            SocketServer.OnClientDisconnected += OnClientDisconnected;

            SocketServer.Start();

            OnSeverStarted?.Invoke();

            IsServer = true;
        }

        public void InjectSocketServer()
        {
            ISocketServer<TIdentifier>[] socketServers = GetComponentsInChildren<ISocketServer<TIdentifier>>();

            InjectSocketServer(socketServers);
        }

        protected abstract void OnClientDisconnected(TIdentifier id, ConnectedClientSocket connectedClient);

        protected abstract void OnManagedMessage(TIdentifier id, ConnectedClientSocket from, byte[] data, PROTOCOL protocol);

        protected abstract void OnAnonymousMessage(ConnectedClientSocket from, byte[] data, PROTOCOL protocol);

        protected abstract void OnManagedConnected(TIdentifier id, ConnectedClientSocket connectedClient);

        protected abstract void OnAnonymousConnected(ConnectedClientSocket anon);

        protected virtual void ShutdownServer()
        {
            socketServer?.Stop();

            socketServer = null;

            IsServer = false;
        }

        private void OnDisable()
        {
            ShutdownServer();
        }

        private void OnDestroy()
        {
            ShutdownServer();
        }

        private void OnApplicationQuit()
        {
            ShutdownServer();
        }

    }
}
