using Bloodthirst.Socket.Core;
using Bloodthirst.Socket.Identifier;
using Bloodthirst.Socket.Serialization.Data;
using Bloodthirst.Socket.Serializer;
using System;
using System.Net;

namespace Bloodthirst.Socket
{
    public class GUIDGhostManagedSocketServer : ManagedSocketServer<Guid>
    {

        private readonly INetworkSerializer<Guid> identifierProcessor;


        public GUIDGhostManagedSocketServer(IPAddress IP, int Port) : base(IP, Port, new GUIDComparer())
        {
            identifierProcessor = new IdentityGUIDNetworkSerializer();
        }

        public override INetworkSerializer<Guid> IdentifierSerializer => identifierProcessor;

        public override Guid GenerateClientIdentifier()
        {
            return GUIDIdentifier.GenerateClientIdentifier();
        }

        public override Guid GenerateServerIdentifier()
        {
            return GUIDIdentifier.GenerateServerIdentifier();
        }
    }
}
