using Bloodthirst.Socket.Core;
using System;
using System.Collections.Generic;

namespace Bloodthirst.Socket.BehaviourComponent
{
    public interface ISocketServerInjector<T> where T : IComparable<T>
    {
        ManagedSocketServer<T> SocketServer { get; }

        void InjectSocketServer(IEnumerable<ISocketServer<T>> socketServers);
    }
}
