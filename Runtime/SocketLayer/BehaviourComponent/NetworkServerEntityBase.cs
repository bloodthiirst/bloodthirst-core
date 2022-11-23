using Bloodthirst.Socket.BehaviourComponent;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace Bloodthirst.Socket.Core
{
    public enum SERVICE_QUERY
    {
        REFLECTION,
        SCAN_COMPONENTS
    }

    public abstract class NetworkServerEntityBase<TServer, TIdentifier> : MonoBehaviour, 
        ISocketServerInjector<TIdentifier>
        where TServer : ManagedSocketServer<TIdentifier>
        where TIdentifier : IComparable<TIdentifier>
    {
        [ShowInInspector]
        public static bool IsServer => SocketConfig.Instance.IsServer;

        [SerializeField]
        private int Port => SocketConfig.Instance.WorldServerPort;

        [ShowInInspector]
        protected TServer socketServer;

        public TServer SocketServer
        {
            get
            {
                return socketServer;
            }
        }

        [ShowInInspector]
        protected int ActivePlayerCount => socketServer == null ? 0 : socketServer.ManagedClientsCount;

        [ShowInInspector]
        protected int ActiveServerCount => socketServer == null ? 0 : socketServer.ServerConnexionManager.ServerConnexions.Count;


        [ShowInInspector]
        protected int WaitingClientCount => socketServer == null ? 0 : socketServer.AnonymousClientsCount;


        #region ISocketServerInjector

        ManagedSocketServer<TIdentifier> ISocketServerInjector<TIdentifier>.SocketServer => SocketServer;

        public void InjectSocketServer(IEnumerable<ISocketServer<TIdentifier>> socketServers)
        {
            foreach (ISocketServer<TIdentifier> socketServer in socketServers)
            {
                socketServer.SocketServer = SocketServer;
            }
        }

        #endregion

        [SerializeField]
        public UnityEvent OnSeverStarted;

        [ShowInInspector]
        public List<IOnSocketServerConnected<ManagedSocketServer<TIdentifier>, TIdentifier>> OnConnectedBehaviours;

        [ShowInInspector]
        private Dictionary<Type, SocketClient<TIdentifier>> ServerToServerClients;

        protected void Awake()
        {
            OnConnectedBehaviours = new List<IOnSocketServerConnected<ManagedSocketServer<TIdentifier>, TIdentifier>>();

            GetComponentsInChildren(OnConnectedBehaviours);
        }

        public IEnumerable<Type> QueryServerTypes()
        {
            // get all server types

            List<Type> types = Assembly.GetAssembly(typeof(ManagedSocketServer<>))
               .GetTypes()
               .Where(t => t.IsClass)
               .Where(t => t.BaseType != null)
               .Where(t => t.BaseType.IsGenericType)
               .Where(t => t.BaseType.GetGenericTypeDefinition() == typeof(ManagedSocketServer<>))
               .Where(t => t != typeof(TServer))
               .ToList();

            // get the right server types

            foreach (Type t in types)
            {
                Type socketType = t.BaseType;

                Type genericParam = socketType.GetGenericArguments()[0];

                if (genericParam != typeof(TIdentifier))
                    continue;

                yield return t;
            }
        }


        protected abstract TServer CreateServer();

        [Button]
        public void StartServer()
        {

            if (socketServer != null)
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

            // inject the server into the components

            InjectSocketServer();

            // trigger on connceted behaviours

            OnSocketServerConnectedTriggered();

            // trigger the in editor event

            OnSeverStarted?.Invoke();

            SocketConfig.Instance.IsServer = true;
        }

        public void InjectSocketServer()
        {
            ISocketServer<TIdentifier>[] socketServers = GetComponentsInChildren<ISocketServer<TIdentifier>>();

            InjectSocketServer(socketServers);
        }

        private void OnSocketServerConnectedTriggered()
        {
            foreach (IOnSocketServerConnected<ManagedSocketServer<TIdentifier>, TIdentifier> onConnected in OnConnectedBehaviours)
            {
                onConnected.OnSocketServerConnected(SocketServer);
            }
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

            SocketConfig.Instance.IsServer = false;
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
