﻿using Assets.SocketLayer.Identifier;
using Assets.SocketLayer.Serialization.Data;
using Bloodthirst.Socket.Serializer;
using System;
using System.Net;

namespace Bloodthirst.Socket
{
    public class GUIDManagedSocketServer : ManagedSocketServer<Guid>
    {

        private readonly INetworkSerializer<Guid> identifierProcessor;


        public GUIDManagedSocketServer(IPAddress IP, int Port ) : base(IP, Port , new GUIDComparer() )
        {
            identifierProcessor = new IdentityGUIDNetworkSerializer();
        }

        public override INetworkSerializer<Guid> IdentifierSerializer => identifierProcessor;

        public override Guid GenerateIdentifer()
        {
            return Guid.NewGuid();
        }
    }
}
