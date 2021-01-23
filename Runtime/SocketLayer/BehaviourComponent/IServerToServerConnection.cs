using Bloodthirst.Socket;
using System;

namespace Bloodthirst.Scripts.SocketLayer.BehaviourComponent
{
    public interface IServerToServerConnection<TIdentifier>
        where TIdentifier : IComparable<TIdentifier>
    {
        Tuple<Type, SocketClient<TIdentifier>> SocketToServer();
    }

}
