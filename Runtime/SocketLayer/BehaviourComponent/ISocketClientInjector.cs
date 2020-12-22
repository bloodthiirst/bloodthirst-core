using Bloodthirst.Socket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.SocketLayer.BehaviourComponent
{
    public interface ISocketClientInjector<TClient ,TIdentifer>
        where TClient : SocketClient<TIdentifer>
        where TIdentifer : IComparable<TIdentifer>
    {
        TClient SocketClient { get; }

        void InjectSocketClient(IEnumerable<ISocketClient<TClient , TIdentifer>> socketClients);
    }
}
