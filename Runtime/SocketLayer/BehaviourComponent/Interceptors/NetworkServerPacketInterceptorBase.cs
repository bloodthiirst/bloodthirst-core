using Bloodthirst.Socket;
using Bloodthirst.Socket.Core;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Assets.SocketLayer.BehaviourComponent
{
    public abstract class NetworkServerPacketInterceptorBase<TIdentifier> : MonoBehaviour, ISocketServer<TIdentifier>, IOnSocketServerConnected<ManagedSocketServer<TIdentifier>, TIdentifier> where TIdentifier : IComparable<TIdentifier>
    {
        public ManagedSocketServer<TIdentifier> SocketServer { get; set; }

        public void OnSocketServerConnected(ManagedSocketServer<TIdentifier> socketClient)
        {
            SocketServer.OnAnonymousMessage -= OnAnonymousMessage;
            SocketServer.OnAnonymousMessage += OnAnonymousMessage;

            SocketServer.OnManagedClientMessage -= OnManagedClientMessage;
            SocketServer.OnManagedClientMessage += OnManagedClientMessage;

            SocketServer.OnServerMessage -= OnServerClientMessge;
            SocketServer.OnServerMessage += OnServerClientMessge;
        }

        protected abstract void OnServerClientMessge(ConnectedClientSocket connectedClient, byte[] data, PROTOCOL protocol);

        protected abstract void OnAnonymousMessage(ConnectedClientSocket connectedClient, byte[] data, PROTOCOL protocol);

        protected abstract void OnManagedClientMessage(TIdentifier identifier, ConnectedClientSocket connectedClient, byte[] data, PROTOCOL protocol);

        private void OnDestroy()
        {
            if (SocketServer == null)
                return;

            SocketServer.OnAnonymousMessage -= OnAnonymousMessage;

            SocketServer.OnManagedClientMessage -= OnManagedClientMessage;

            SocketServer.OnServerMessage -= OnServerClientMessge;

        }


    }
}
