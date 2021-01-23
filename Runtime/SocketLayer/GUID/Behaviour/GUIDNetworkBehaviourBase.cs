using Bloodthirst.Socket.Client.Base;
using Bloodthirst.Socket.Core;
using System;

namespace Bloodthirst.Socket.BehaviourComponent.NetworkPlayerEntity
{
    public abstract class GUIDNetworkBehaviourBase : NetworkBehaviourBase<Guid>, ISocketClient<GUIDSocketClient, Guid>, ISocketServer<Guid>
    {
        public ManagedSocketServer<Guid> SocketServer { get; set; }
        public GUIDSocketClient SocketClient { get; set; }
    }
}
