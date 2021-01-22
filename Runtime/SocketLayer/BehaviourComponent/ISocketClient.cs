using Bloodthirst.Socket;
using System;

namespace Assets.SocketLayer.BehaviourComponent
{
    public interface ISocketClient<TClient, TIdentifier> where TClient : SocketClient<TIdentifier> where TIdentifier : IComparable<TIdentifier>
    {
        TClient SocketClient { get; set; }
    }
}
