using Bloodthirst.Core.BProvider;
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
        private readonly GUIDAndPrefabPath playerId;
        private INetworkSerializer<Guid> guidSerializer;

        private INetworkSerializer<GUIDNewPlayerConnected> newPlayerSerializer;
        private GUIDNetworkServerEntity _guidNetworkServerEntity;

        public SendGUIDNewPlayerConnectedCommand(GUIDAndPrefabPath playerId)
        {
            this.playerId = playerId;
            _guidNetworkServerEntity = BProviderRuntime.Instance.GetSingleton<GUIDNetworkServerEntity>();
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
                GUIDandPrefab = playerId
            };

            byte[] newPlayerPacket = PacketBuilder.BuildPacket(GUIDIdentifier.DefaultClientID, newPlayer, guidSerializer, newPlayerSerializer);


            foreach (KeyValuePair<Guid, ConnectedClientSocket> kv in _guidNetworkServerEntity.SocketServer.ClientConnexionManager.ClientConnexions)
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
