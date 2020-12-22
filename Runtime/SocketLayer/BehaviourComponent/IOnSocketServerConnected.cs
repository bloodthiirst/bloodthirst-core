using System;

namespace Bloodthirst.Socket
{
    public interface IOnSocketServerConnected<TServer , TIdentifier> where TServer : ManagedSocketServer<TIdentifier> where TIdentifier : IComparable<TIdentifier>
    {
        void OnSocketServerConnected(TServer socketClient);
    }
}
