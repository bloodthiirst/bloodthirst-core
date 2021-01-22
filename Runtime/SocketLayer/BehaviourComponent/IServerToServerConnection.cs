using Bloodthirst.Socket;
using System;

namespace Assets.Scripts.SocketLayer.BehaviourComponent
{
    public interface IServerToServerConnection<TIdentifier>
        where TIdentifier : IComparable<TIdentifier>
    {
        Tuple<Type, SocketClient<TIdentifier>> SocketToServer();
    }

}
