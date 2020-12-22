using Assets.Models;
using Assets.SocketLayer.PacketParser.Base;
using Bloodthirst.Socket;
using Bloodthirst.Socket.Core;
using Bloodthirst.Socket.Serializer;
using Bloodthirst.System.CommandSystem;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.SocketLayer.Commands
{
    public class SendGUIDConnectionInfoCommand : CommandBase<SendGUIDConnectionInfoCommand>
    {
        private readonly Guid playerId;

        private readonly List<Guid> existingPlayers;

        private readonly ConnectedClientSocket playerSocket;

        public SendGUIDConnectionInfoCommand(Guid playerId , List<Guid> existingPlayers , ConnectedClientSocket playerSocket)
        {
            this.playerId = playerId;
            this.existingPlayers = existingPlayers;
            this.playerSocket = playerSocket;
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

            byte[] packet = PacketBuilder.BuildPacket(GUIDIdentifier.DefaultClientID, connectionInfo, GUIDNetworkServerEntity.Instance.SocketServer.IdentifierSerializer, BaseNetworkSerializer<GUIDConnectionInfo>.Instance);

            playerSocket.SendTCP(packet);

            CommandState = COMMAND_STATE.SUCCESS;
            IsDone = true;
        }

        public override void OnEnd()
        {
        }

    }
}
