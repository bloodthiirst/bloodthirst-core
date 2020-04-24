using Bloodthirst.Socket;
using Bloodthirst.Socket.Core;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Assets.SocketLayer.BehaviourComponent
{
    public abstract class NetworkServerPacketInterceptorBase<TIdentifier> : MonoBehaviour where TIdentifier : IComparable<TIdentifier>
    {
        [ShowInInspector]
        private ISocketServerInjector<TIdentifier> networkServer;

        private void OnValidate()
        {
            networkServer = GetComponent<ISocketServerInjector<TIdentifier>>();
        }

        private void Start()
        {
            if (networkServer == null)
            {
                networkServer = GetComponent<ISocketServerInjector<TIdentifier>>();
            }
        }

        public void OnServerStarted()
        {
            OnServerStarted(networkServer.SocketServer);
        }

        private void OnServerStarted(ManagedSocketServer<TIdentifier> socketServer)
        {
            socketServer.OnAnonymousMessage -= OnAnonymousMessage;
            socketServer.OnAnonymousMessage += OnAnonymousMessage;

            socketServer.OnManagedClientMessage -= OnManagedClientMessage;
            socketServer.OnManagedClientMessage += OnManagedClientMessage;
        }

        protected abstract void OnAnonymousMessage(ConnectedClientSocket connectedClient, byte[] data, PROTOCOL protocol);

        protected abstract void OnManagedClientMessage(TIdentifier identifier, ConnectedClientSocket connectedClient, byte[] data, PROTOCOL protocol);

        private void OnDestroy()
        {
            if (networkServer.SocketServer == null)
                return;

            networkServer.SocketServer.OnAnonymousMessage -= OnAnonymousMessage;

            networkServer.SocketServer.OnManagedClientMessage -= OnManagedClientMessage;

            networkServer.SocketServer.OnServerStarted -= OnServerStarted;
        }
    }
}
