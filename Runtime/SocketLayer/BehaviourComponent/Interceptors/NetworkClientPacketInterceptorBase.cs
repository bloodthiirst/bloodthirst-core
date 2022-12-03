using Bloodthirst.Socket.Core;
#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using System;
using UnityEngine;

namespace Bloodthirst.Socket.BehaviourComponent
{
    public abstract class NetworkClientPacketInterceptorBase<TClient, TIdentifier> : MonoBehaviour,
        ISocketClient<TClient, TIdentifier>,
        IOnSocketClientConnected<TClient, TIdentifier>

        where TClient : SocketClient<TIdentifier>
        where TIdentifier : IComparable<TIdentifier>
    {
        #if ODIN_INSPECTOR[ShowInInspector]#endif
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
