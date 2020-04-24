using Bloodthirst.Socket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.SocketLayer.BehaviourComponent
{
    public interface ISocketClientInjector<T> where T : IComparable<T>
    {
        SocketClient<T> SocketClient { get; }

        void InjectSocketClient(IEnumerable<ISocketClient<T>> socketClients);
    }
}
