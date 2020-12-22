using Assets.Scripts.Player;
using Assets.Scripts.SocketLayer.Models;
using Assets.SocketLayer.BehaviourComponent;
using Assets.SocketLayer.BehaviourComponent.NetworkPlayerEntity;
using Assets.SocketLayer.PacketParser;
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

namespace Assets.Scripts.SocketLayer.Game.Player
{
    public class PlayerInputNetworkBehaviour : GUIDNetworkBehaviourBase
    {
        private const string H_AXIS = "Horizontal";

        private const string V_AXIS = "Vertical";

        private PlayerInputPacketServerProcessor playerInputServer;

        private PlayerInputPacketClientProcessor playerInputClient;

        private PlayerMovementBehaviour playerMovement;

        private PlayerPositionNetworkBehaviour positionNetwork;

        private INetworkSerializer<Guid> identifier;

        private PlayerInput playerInput;

        [ShowInInspector]
        public PlayerInput PlayerInput => playerInput;

        private PlayerInput cachedInput;

        private void Awake()
        {
            playerMovement = GetComponent<PlayerMovementBehaviour>();

            positionNetwork = GetComponent<PlayerPositionNetworkBehaviour>();

            identifier = new IdentityGUIDNetworkSerializer();

            if (IsClient)
            {
                playerInputClient = NetworkEntity.GetClient<PlayerInputPacketClientProcessor, PlayerInput>();
                playerInputClient.OnPacketParsedUnityThread += OnClientInput;
            }

            if (IsServer)
            {
                playerInputServer = NetworkEntity.GetServer<PlayerInputPacketServerProcessor, PlayerInput>();
                playerInputServer.OnPacketParsedUnityThread += OnServerInput;
            }
        }

        private void Update()
        {
            if (!IsPlayer)
                return;

            short x = (short)Input.GetAxisRaw(H_AXIS);

            short z = (short)Input.GetAxisRaw(V_AXIS);

            playerInput = new PlayerInput(x, z);

            byte[] inputPacket = PacketBuilder.BuildPacket(NetworkID, playerInput, identifier , BaseNetworkSerializer<PlayerInput>.Instance);

            if (IsServer && !IsClient)
            {
                SocketServer.BroadcastUDP(inputPacket);
            }
            else if(IsServer && IsClient)
            {
                SocketServer.BroadcastUDP(inputPacket, id => !id.Equals(SocketClient<Guid>.CurrentNetworkID));
            }

            else if (IsClient)
            {
                SocketClient.SendUDP(inputPacket);
            }

            cachedInput = playerInput;
        }



        #region network input
        private void OnServerInput(PlayerInput input, Guid guid , ConnectedClientSocket socket)
        {
            playerInput = input;

            byte[] inputPacket = PacketBuilder.BuildPacket(NetworkID, playerInput, identifier, BaseNetworkSerializer<PlayerInput>.Instance);

            if (IsServer && !IsClient)
            {
                SocketServer.BroadcastUDP(inputPacket);
            }
            else if (IsServer && IsClient)
            {
                SocketServer.BroadcastUDP(inputPacket, id => !id.Equals(SocketClient<Guid>.CurrentNetworkID));
            }
        }

        private void OnClientInput(PlayerInput input, Guid guid)
        {
            // save the client input

            playerInput = input;
        }

        #endregion

    }
}
