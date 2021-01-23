using Bloodthirst.Socket.Core;
using Bloodthirst.Socket.Serialization.Data;
using Bloodthirst.Socket.Serializer;
using Bloodthirst.Socket.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Socket.BehaviourComponent
{
    public class GUIDGhostNetworkServerPacketInterceptor : NetworkServerPacketInterceptorBase<Guid>
    {
        private INetworkSerializer<Guid> identitySerializer;

        [ShowInInspector]
        private List<IPacketServerProcessor<Guid>> packetProcessors;


        private void Awake()
        {
            identitySerializer = new IdentityGUIDNetworkSerializer();

            packetProcessors = new List<IPacketServerProcessor<Guid>>();

            GetComponentsInChildren(packetProcessors);
        }

        protected override void OnAnonymousMessage(ConnectedClientSocket connectedClient, byte[] packet, PROTOCOL protocol)
        {
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
                packetProcessors[i].ProcessPacket(type, connectedClient, identifier, data);
            }

        }

        protected override void OnManagedClientMessage(Guid identifier, ConnectedClientSocket connectedClient, byte[] packet, PROTOCOL protocol)
        {
            uint type;

            byte[] data;

            if (!PacketBuilder.UnpackPacket(packet, out type, out identifier, out data, identitySerializer))
            {
                Debug.Log("Parsing failed");
                return;
            }

            for (int i = 0; i < packetProcessors.Count; i++)
            {
                packetProcessors[i].ProcessPacket(type, connectedClient, identifier, data);
            }
        }

        protected override void OnServerClientMessge(ConnectedClientSocket connectedClient, byte[] packet, PROTOCOL protocol)
        {
            Debug.Log("server client msg");

            Guid identifier;

            uint type;

            byte[] data;

            if (!PacketBuilder.UnpackPacket(packet, out type, out identifier, out data, identitySerializer))
            {
                Debug.Log("Parsing failed");
                return;
            }

            for (int i = 0; i < packetProcessors.Count; i++)
            {
                packetProcessors[i].ProcessPacket(type, connectedClient, identifier, data);
            }
        }
    }
}
