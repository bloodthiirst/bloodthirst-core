using Bloodthirst.Socket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.SocketLayer.BehaviourComponent
{
    public interface IServerToServerConnection<TIdentifier>
        where TIdentifier : IComparable<TIdentifier>
    {
        Tuple<Type, SocketClient<TIdentifier>> SocketToServer();
    }

}
