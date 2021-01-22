using Bloodthirst.Socket;
using System;

namespace Assets.SocketLayer.BehaviourComponent
{
    public interface ISocketServer<T> where T : IComparable<T>
    {
        ManagedSocketServer<T> SocketServer { get; set; }
    }
}
