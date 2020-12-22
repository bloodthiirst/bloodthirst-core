using Assets.Models;
using Assets.SocketLayer.Client.Base;
using Assets.SocketLayer.PacketParser;
using Assets.SocketLayer.PacketParser.Base;
using Bloodthirst.Socket;
using Bloodthirst.Socket.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.SocketLayer.BehaviourComponent.NetworkPlayerEntity
{
    public abstract class GUIDNetworkBehaviourBase : NetworkBehaviourBase<Guid>, ISocketClient< GUIDSocketClient , Guid>, ISocketServer<Guid>
    {
        public ManagedSocketServer<Guid> SocketServer { get; set; }
        public GUIDSocketClient SocketClient { get; set; }

        private IPlayerSpawnSuccess[] spawnSuccess;

        private GUIDPlayerSpawnSuccessPacketServerProcessor GUIDPlayerSpawnSuccess;

        private void Awake()
        {
            if (!IsServer)
                return;

            spawnSuccess = GetComponentsInChildren<IPlayerSpawnSuccess>();

            GUIDPlayerSpawnSuccess = NetworkEntity.GetServer<GUIDPlayerSpawnSuccessPacketServerProcessor, GUIDPlayerSpawnSuccess>();

            GUIDPlayerSpawnSuccess.OnPacketParsedUnityThread += OnGUIDPlayerSpawnSuccess;
        }

        private void OnGUIDPlayerSpawnSuccess(GUIDPlayerSpawnSuccess spawnInfo, Guid from, ConnectedClientSocket socket)
        {
            foreach(IPlayerSpawnSuccess spawn in spawnSuccess) 
            {
                spawn.OnPlayerSpawnSuccess(spawnInfo.SpawnNetworkID);
            }
        }

        private void Start()
        {
            if (IsClient && !IsServer)
            {
                GUIDPlayerSpawnSuccess playerSpawnSuccess = new GUIDPlayerSpawnSuccess() { SpawnNetworkID = NetworkID };

                byte[] spawnPacket = PacketBuilder.BuildPacket(NetworkID, playerSpawnSuccess, SocketClient.IdentitySerializer, BaseNetworkSerializer<GUIDPlayerSpawnSuccess>.Instance);

                SocketClient.SendTCP(spawnPacket);
            }
        }
    }
}
