using Assets.SocketLayer.PacketParser;
using Assets.SocketLayer.PacketParser.Base;
using Assets.SocketLayer.Serialization.Data;
using Bloodthirst.Socket;
using Bloodthirst.Socket.Serializer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.SocketLayer.BehaviourComponent.NetworkPlayerEntity
{
    public class PlayerPositionNetworkBehaviour : GUIDNetworkBehaviourBase , IPlayerSpawnServer , IPlayerSpawnSuccess
    {
        public bool IsDoneServerSpawn { get; set; }

        private PlayerPostionPacketClientProcessor playerPostionClient;

        private INetworkSerializer<Guid> identifier;

        private Vector3 cachedPosition;

        private void Awake()
        {
            identifier = new IdentityGUIDNetworkSerializer();

            playerPostionClient = NetworkEntity.GetClient<PlayerPostionPacketClientProcessor, Vector3>();
            playerPostionClient.OnPacketParsedUnityThread += OnPositionClient;
        }

        private void OnPositionClient(Vector3 pos, Guid from)
        {
            Vector3 difference = pos - transform.position;

            // if the difference is too high then snap it into place

            if(difference.magnitude > 1)
            {
                transform.position = pos;
            }

            // else interpolate

            else
            {
                transform.position += difference * 0.1f;
            }
            
        }

        private void Update()
        {
            if (IsClient && !IsServer)
                return;

            cachedPosition = transform.position;

            byte[] Packet = PacketBuilder.BuildPacket(NetworkID, cachedPosition, identifier, Vector3NetworkSerializer.Instance);

            if (IsServer && !IsClient)
            {
                SocketServer.BroadcastUDP(Packet);
            }
            else if (IsServer && IsClient)
            {
                SocketServer.BroadcastUDP(Packet, id => !id.Equals(SocketClient<Guid>.CurrentNetworkID));
            }
        }

        public void OnPlayerSpawnServer()
        {
            Vector3 initPos = Vector3.zero;

            Vector3 offset = new Vector3(0, 2, 0);

            Ray upRay = new Ray(Vector3.down * 100, Vector3.up);
            Ray downRay = new Ray(Vector3.up * 100, Vector3.down);


            // check if ground is on top

            List<RaycastHit> hits = new List<RaycastHit>();

            hits.AddRange(Physics.RaycastAll(upRay));
            hits.AddRange(Physics.RaycastAll(downRay));


            foreach (RaycastHit h in hits)
            {
                if (h.collider.gameObject.CompareTag("Terrain"))
                {
                    initPos = h.point;
                    break;
                }
            }

            Vector3 onFloorValue = initPos + offset;

            transform.position = onFloorValue;

            IsDoneServerSpawn = true;
        }

        public void BroadcastPositionToClientsUDP()
        {
            byte[] positionPacket = PacketBuilder.BuildPacket(NetworkID, transform.position, SocketServer.IdentifierSerializer, Vector3NetworkSerializer.Instance);

            if (IsServer && IsClient)
                SocketServer.BroadcastUDP(positionPacket, id => !id.Equals( SocketClient<Guid>.CurrentNetworkID ));
            if (IsServer && !IsClient)
                SocketServer.BroadcastUDP(positionPacket);
        }

        public void OnPlayerSpawnSuccess(Guid identifier)
        {
            byte[] positionPacket = PacketBuilder.BuildPacket(NetworkID, transform.position, SocketServer.IdentifierSerializer, BaseNetworkSerializer<Vector3>.Instance);

            SocketServer.ClientConnexionManager.ClientConnexions[identifier].SendTCP(positionPacket);
        }
    }
}
