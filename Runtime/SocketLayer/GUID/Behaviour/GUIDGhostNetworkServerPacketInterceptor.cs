using Assets.SocketLayer.Identifier;
using Assets.SocketLayer.PacketParser.Base;
using Assets.SocketLayer.Serialization.Data;
using Bloodthirst.Socket;
using Bloodthirst.Socket.Core;
using Bloodthirst.Socket.Serializer;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.SocketLayer.BehaviourComponent
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

            for(int i = 0; i < packetProcessors.Count; i++)
            {
                packetProcessors[i].ProcessPacket(type, connectedClient , identifier, data);
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
