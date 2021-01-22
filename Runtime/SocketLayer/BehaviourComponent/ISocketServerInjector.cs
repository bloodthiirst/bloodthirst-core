using Bloodthirst.Socket;
using System;
using System.Collections.Generic;

namespace Assets.SocketLayer.BehaviourComponent
{
    public interface ISocketServerInjector<T> where T : IComparable<T>
    {
        ManagedSocketServer<T> SocketServer { get; }

        void InjectSocketServer(IEnumerable<ISocketServer<T>> socketServers);
    }
}
