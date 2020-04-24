using Bloodthirst.Socket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.SocketLayer.BehaviourComponent
{
    public interface ISocketServer<T> where T : IComparable<T>
    {
        ManagedSocketServer<T> SocketServer { get; set; }
    }
}
