using Bloodthirst.Socket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.SocketLayer.BehaviourComponent
{
    public interface ISocketClient<TClient, TIdentifier> where TClient : SocketClient<TIdentifier> where TIdentifier : IComparable<TIdentifier>
    {
        TClient SocketClient { get; set; }
    }
}
