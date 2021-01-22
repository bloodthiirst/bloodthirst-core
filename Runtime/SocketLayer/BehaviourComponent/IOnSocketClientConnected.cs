using System;

namespace Bloodthirst.Socket
{
    public interface IOnSocketClientConnected<TClient, TIdentifier> where TClient : SocketClient<TIdentifier> where TIdentifier : IComparable<TIdentifier>
    {
        void OnSocketClientConnected(TClient socketClient);
    }
}
