using Assets.Scripts.Player;
using Assets.Scripts.SocketLayer.Models;
using Assets.SocketLayer.BehaviourComponent.NetworkPlayerEntity;
using Assets.SocketLayer.PacketParser;
using Assets.SocketLayer.Serialization.Data;
using Bloodthirst.Socket;
using Bloodthirst.Socket.Serializer;
using Bloodthirst.Socket.Utils;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Assets.Scripts.SocketLayer.Game.Player
{
    public class PlayerInputNetworkBehaviour : GUIDNetworkBehaviourBase
    {
        private const string H_AXIS = "Horizontal";

        private const string V_AXIS = "Vertical";

        private PlayerInputPacketServerProcessor playerInputServer;

        private PlayerInputPacketClientProcessor playerInputClient;

        [SerializeField]
        private PlayerMovementBehaviour movementBehaviour;

        private INetworkSerializer<Guid> identifier;

        private PlayerInput playerInput;

        [ShowInInspector]
        public PlayerInput PlayerInput => playerInput;

        private void Awake()
        {
            identifier = new IdentityGUIDNetworkSerializer();

            if (IsServer)
            {
                playerInputServer = NetworkEntity.GetServer<PlayerInputPacketServerProcessor, PlayerInput>();

                playerInputServer.OnPacketParsedUnityThread += OnServerInput;

            }

            if (IsClient)
            {
                playerInputClient = NetworkEntity.GetClient<PlayerInputPacketClientProcessor, PlayerInput>();
                playerInputClient.OnPacketParsedUnityThread += OnClientInput;
            }
        }


        private void Update()
        {
            // only set input if you are the current player
            if (IsPlayer)
            {
                short x = (short)Input.GetAxisRaw(H_AXIS);

                short z = (short)Input.GetAxisRaw(V_AXIS);

                playerInput = new PlayerInput(x, z);
            }

            byte[] inputPacket = PacketBuilder.BuildPacket(NetworkID, playerInput, identifier, BaseNetworkSerializer<PlayerInput>.Instance);

            // if server and client at the same time
            // tehn send to the rest of the clients
            if (IsServer && IsClient && IsPlayer)
            {
                SocketServer.BroadcastUDP(inputPacket, id => !id.Equals(NetworkID));
            }
            // if only server then send then broadcast to all the clients
            else if (IsServer && !IsClient)
            {
                SocketServer.BroadcastUDP(inputPacket);
            }
            // if client only then send to sever to propagate
            else if (!IsServer && IsClient)
            {
                SocketClient.SendUDP(inputPacket);
            }
        }

        #region network input
        private void OnServerInput(PlayerInput input, Guid guid, ConnectedClientSocket socket)
        {
            playerInput = input;
        }

        private void OnClientInput(PlayerInput playerInput, Guid guid)
        {
            this.playerInput = playerInput;
        }
        #endregion


    }
}
