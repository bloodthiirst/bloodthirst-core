using Bloodthirst.Socket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.SocketLayer.BehaviourComponent
{
    public interface ISocketClient<T> where T : IComparable<T>
    {
        SocketClient<T> SocketClient { get; set; }
    }
}
