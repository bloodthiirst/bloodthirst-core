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
    public class SendGUIDNewPlayerConnectedCommand : CommandBase<SendGUIDConnectionInfoCommand>
    {
        private readonly Guid playerId;

        private INetworkSerializer<Guid> guidSerializer;

        private INetworkSerializer<GUIDNewPlayerConnected> newPlayerSerializer;

        public SendGUIDNewPlayerConnectedCommand(Guid playerId)
        {
            this.playerId = playerId;
            newPlayerSerializer = SerializerProvider.Get<GUIDNewPlayerConnected>();
            guidSerializer = SerializerProvider.Get<Guid>();
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

            byte[] newPlayerPacket = PacketBuilder.BuildPacket(GUIDIdentifier.DefaultClientID, newPlayer, guidSerializer, newPlayerSerializer);


            foreach (KeyValuePair<Guid, ConnectedClientSocket> kv in GUIDNetworkServerEntity.Instance.SocketServer.ClientConnexionManager.ClientConnexions)
            {
                if (kv.Key.Equals(playerId))
                    continue;

                kv.Value.SendTCP(newPlayerPacket);
            }

            Success();
        }

        public override void OnEnd()
        {
        }

    }
}
