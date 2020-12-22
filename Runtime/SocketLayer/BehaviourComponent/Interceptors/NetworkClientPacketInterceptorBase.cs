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
    public abstract class NetworkClientPacketInterceptorBase<TClient,TIdentifier> : MonoBehaviour , 
        ISocketClient<TClient, TIdentifier> ,
        IOnSocketClientConnected<TClient, TIdentifier>  

        where TClient : SocketClient<TIdentifier>
        where TIdentifier : IComparable<TIdentifier>
    {
        [ShowInInspector]
        public TClient SocketClient { get; set; }

        protected abstract void OnMessage(SocketClient<TIdentifier> socketClient, byte[] packet, PROTOCOL protocol);

        private void OnDestroy()
        {
            if (SocketClient == null)
                return;

            SocketClient.OnMessage -= OnMessage;
        }

        public void OnSocketClientConnected(TClient socketClient)
        {
            SocketClient.OnMessage -= OnMessage;
            SocketClient.OnMessage += OnMessage;
        }
    }
}
