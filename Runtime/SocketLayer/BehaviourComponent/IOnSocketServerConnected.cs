using Bloodthirst.Socket.Core;
using System;

namespace Bloodthirst.Socket.BehaviourComponent
{
    public interface IOnSocketServerConnected<TServer, TIdentifier> where TServer : ManagedSocketServer<TIdentifier> where TIdentifier : IComparable<TIdentifier>
    {
        void OnSocketServerConnected(TServer socketClient);
    }
}
