using Bloodthirst.Socket.Client.Base;
using Bloodthirst.Socket.Core;
using Bloodthirst.Socket.Serialization.Data;
using Bloodthirst.Socket.Serializer;
using Bloodthirst.Socket.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Socket.BehaviourComponent
{
    public class GUIDGhostNetworkClientPacketInterceptor : NetworkClientPacketInterceptorBase<GUIDGhostSocketClient, Guid>
    {
        private INetworkSerializer<Guid> identitySerializer;

        private List<IPacketClientProcessor<Guid>> packetProcessors;

        public void Awake()
        {
            identitySerializer = new IdentityGUIDNetworkSerializer();

            packetProcessors = new List<IPacketClientProcessor<Guid>>();

            GetComponentsInChildren(packetProcessors);
        }


        protected override void OnMessage(SocketClient<Guid> socketClient, byte[] packet, PROTOCOL protocol)
        {
            Debug.Log(" GUIDGhostNetworkClientPacketInterceptor received message");

            uint type;

            Guid identifier;

            byte[] data;

            if (!PacketBuilder.UnpackPacket(packet, out type, out identifier, out data, identitySerializer))
            {
                Debug.Log("Parsing failed");
                return;
            }

            for (int i = 0; i < packetProcessors.Count; i++)
            {
                packetProcessors[i].ProcessPacket(type, identifier, data);
            }

        }
    }
}
