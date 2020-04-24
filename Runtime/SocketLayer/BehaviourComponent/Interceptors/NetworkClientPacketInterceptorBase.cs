using Assets.Scripts;
using Assets.SocketLayer.PacketParser;
using Bloodthirst.Socket;
using Bloodthirst.Socket.Core;
using Bloodthirst.Socket.PacketParser;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.SocketLayer.BehaviourComponent
{
    public abstract class NetworkClientPacketInterceptorBase<TIdentifier> : MonoBehaviour where TIdentifier : IComparable<TIdentifier>
    {
        [ShowInInspector]
        private ISocketClientInjector<TIdentifier> networkClient;

        private void OnValidate()
        {
            networkClient = GetComponent<ISocketClientInjector<TIdentifier>>();
        }

        private void Start()
        {
            if (networkClient == null)
            {
                networkClient = GetComponent<ISocketClientInjector<TIdentifier>>();
            }

        }

        public void OnClientStarted()
        {
            networkClient.SocketClient.OnMessage -= OnMessage;
            networkClient.SocketClient.OnMessage += OnMessage;
        }

        protected abstract void OnMessage(SocketClient<TIdentifier> socketClient, byte[] packet, PROTOCOL protocol);

        private void OnDestroy()
        {
            networkClient.SocketClient.OnMessage -= OnMessage;

            networkClient.SocketClient.OnConnect -= OnClientStarted;
        }
    }
}
