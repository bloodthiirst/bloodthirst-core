using Assets.SocketLayer.Serialization.Data;
using Bloodthirst.Socket;
using Bloodthirst.Socket.Serializer;
using System;
using System.Net;

namespace Assets.SocketLayer.Client.Base
{
    public class GUIDGhostSocketClient : SocketClient<Guid>
    {
        private readonly INetworkSerializer<Guid> identitySerializer;

        public GUIDGhostSocketClient(IPAddress ServerAddress, int serverPort) : base(ServerAddress, serverPort)
        {
            identitySerializer = new IdentityGUIDNetworkSerializer();
        }

        public override INetworkSerializer<Guid> IdentitySerializer => identitySerializer;
    }
}
