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
    public class SendGUIDNewPlayerConnectedCommand : CommandBase<SendGUIDConnectionInfoCommand>
    {
        private readonly Guid playerId;

        private readonly INetworkSerializer<Guid> identifier;

        public SendGUIDNewPlayerConnectedCommand(Guid playerId)
        {
            this.playerId = playerId;
            this.identifier = GUIDNetworkServerEntity.Instance.SocketServer.IdentifierSerializer;
        }

        public override void OnStart()
        {      
        }


        public override void OnTick(float delta)
        {
            GUIDNewPlayerConnected newPlayer = new GUIDNewPlayerConnected()
            { 
                NetworkID = playerId
            };

            byte[] newPlayerPacket = PacketBuilder.BuildPacket(GUIDIdentifier.DefaultClientID, newPlayer, identifier, BaseNetworkSerializer<GUIDNewPlayerConnected>.Instance);


            foreach (KeyValuePair<Guid, ConnectedClientSocket> kv in GUIDNetworkServerEntity.Instance.SocketServer.ClientConnexionManager.ClientConnexions)
            {
                if (kv.Key.Equals(playerId))
                    continue;

                kv.Value.SendTCP(newPlayerPacket);
            }

            CommandState = COMMAND_STATE.SUCCESS;
            IsDone = true;
        }

        public override void OnEnd()
        {
        }

    }
}
