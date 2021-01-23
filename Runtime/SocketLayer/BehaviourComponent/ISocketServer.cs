using Bloodthirst.Socket.Core;
using System;

namespace Bloodthirst.Socket.BehaviourComponent
{
    public interface ISocketServer<T> where T : IComparable<T>
    {
        ManagedSocketServer<T> SocketServer { get; set; }
    }
}
