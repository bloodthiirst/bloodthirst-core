using Bloodthirst.Models;
using Bloodthirst.Socket;
using Bloodthirst.Socket.Core;
using Bloodthirst.Socket.Serialization;
using Bloodthirst.Socket.Serializer;
using Bloodthirst.Socket.Utils;
using Bloodthirst.System.CommandSystem;
using System;
using System.Collections.Generic;

namespace Bloodthirst.Scripts.SocketLayer.Commands
{
    public class SendGUIDConnectionInfoCommand : CommandBase<SendGUIDConnectionInfoCommand>
    {
        private readonly GUIDAndPrefabPath playerId;

        private readonly List<GUIDAndPrefabPath> existingPlayers;

        private readonly ConnectedClientSocket playerSocket;

        private INetworkSerializer<Guid> guidSerializer;

        private INetworkSerializer<GUIDConnectionInfo> connectionInfoSerializer;

        public SendGUIDConnectionInfoCommand(GUIDAndPrefabPath playerId, List<GUIDAndPrefabPath> existingPlayers, ConnectedClientSocket playerSocket)
        {
            this.playerId = playerId;
            this.existingPlayers = existingPlayers;
            this.playerSocket = playerSocket;
            connectionInfoSerializer = SerializerProvider.Get<GUIDConnectionInfo>();
            guidSerializer = SerializerProvider.Get<Guid>();
        }

        public override void OnStart()
        {
        }


        public override void OnTick(float delta)
        {
            List<Guid> allPlayers = GUIDNetworkServerEntity.Instance.SocketServer.GetManagedClients();


            GUIDConnectionInfo connectionInfo = new GUIDConnectionInfo()
            {
                PlayerNetworkID = playerId,
                ExistingPlayers = existingPlayers
            };

            // send the game info to the new player

            byte[] packet = PacketBuilder.BuildPacket(GUIDIdentifier.DefaultClientID, connectionInfo, guidSerializer, connectionInfoSerializer);

            playerSocket.SendTCP(packet);

            Success();
        }

        public override void OnEnd()
        {
        }

    }
}
