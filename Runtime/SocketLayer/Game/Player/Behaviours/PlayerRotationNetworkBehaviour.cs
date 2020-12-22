using Assets.Scripts.SocketLayer.Game.Player.Models;
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
    public class PlayerRotationNetworkBehaviour : GUIDNetworkBehaviourBase, IPlayerSpawnServer, IPlayerSpawnSuccess
    {
        public bool IsDoneServerSpawn { get; set; }

        private PlayerRotationPacketClientProcessor playerRotationClient;

        private PlayerRotationPacketServerProcessor playerRotationServer;

        private INetworkSerializer<Guid> identifier;

        private PlayerRotation networkRotation;

        private void Awake()
        {
            identifier = new IdentityGUIDNetworkSerializer();

            playerRotationClient = NetworkEntity.GetClient<PlayerRotationPacketClientProcessor, PlayerRotation>();

            playerRotationServer = NetworkEntity.GetServer<PlayerRotationPacketServerProcessor, PlayerRotation>();

            playerRotationClient.OnPacketParsedUnityThread += OnRotationClient;

            playerRotationServer.OnPacketParsedUnityThread += OnRotationServer;
        }

        private void OnRotationServer(PlayerRotation rot, Guid from, ConnectedClientSocket socket)
        {
            networkRotation = rot;

            transform.rotation = Quaternion.Euler(0, rot.RotationValue, 0);

            byte[] rotPacket = PacketBuilder.BuildPacket(NetworkID, rot, identifier, BaseNetworkSerializer<PlayerRotation>.Instance);

            SocketServer.BroadcastUDP(rotPacket, id => !id.Equals(NetworkID));
        }

        private void Update()
        {
            if (!IsPlayer)
                return;

            networkRotation.RotationValue = transform.rotation.eulerAngles.y;

            byte[] inputPacket = PacketBuilder.BuildPacket(NetworkID, networkRotation, identifier, BaseNetworkSerializer<PlayerRotation>.Instance);

            if (IsServer || (IsServer && IsClient))
            {
                BroadcastRotationToClients(inputPacket);
            }

            else if (IsClient)
            {
                SocketClient.SendUDP(inputPacket);
            }
        }

        private void OnRotationClient(PlayerRotation rot, Guid from)
        {
            float difference = rot.RotationValue - transform.rotation.eulerAngles.y;

            // if the difference is too high then snap it into place

            if (Math.Abs(difference) > 5)
            {
                transform.rotation = Quaternion.Euler(0, rot.RotationValue, 0);
            }

            // else interpolate

            else
            {
                transform.rotation = Quaternion.Euler(0, rot.RotationValue + difference * 0.5f, 0);
            }

        }

        public void OnPlayerSpawnServer()
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);

            IsDoneServerSpawn = true;
        }

        public void BroadcastRotationToClients(byte[] packet)
        {
            if (IsServer && IsClient)
                SocketServer.BroadcastUDP(packet, id => !id.Equals(SocketClient<Guid>.CurrentNetworkID));
            if (IsServer && !IsClient)
                SocketServer.BroadcastUDP(packet);
        }

        public void OnPlayerSpawnSuccess(Guid identifier)
        {
            PlayerRotation rot = new PlayerRotation() { RotationValue = transform.rotation.eulerAngles.y };

            byte[] rotationPacket = PacketBuilder.BuildPacket(NetworkID, rot, SocketServer.IdentifierSerializer, BaseNetworkSerializer<PlayerRotation>.Instance);

            SocketServer.ClientConnexionManager.ClientConnexions[identifier].SendTCP(rotationPacket);
        }
    }
}
