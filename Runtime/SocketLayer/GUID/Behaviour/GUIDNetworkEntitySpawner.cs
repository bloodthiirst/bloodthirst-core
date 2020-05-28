using Assets.Scripts.SocketLayer.BehaviourComponent;
using Assets.SocketLayer.BehaviourComponent.NetworkPlayerEntity;
using Bloodthirst.Socket;
using System;
using UnityEngine;

namespace Assets.SocketLayer.BehaviourComponent
{
    public class GUIDNetworkEntitySpawner : NetworkEntitySpawnerBase<Guid>
    {
        private static bool IsServer => BasicSocketServer.IsServer;

        private static bool IsClient => SocketClient<Guid>.IsClient;

        [SerializeField]
        private GUIDNetworkPlayerEntity playerPrefab = default;

        protected override NetworkPlayerEntityBase<Guid> PlayerPrefab => playerPrefab;

        [SerializeField]
        private GUIDNetworkClientPlayerPacketProcessor clientPlayersProcessor = default;

        [SerializeField]
        private GUIDNetworkServerPlayerPacketProcessor serverPlayersProcessor = default;

        [SerializeField]
        private GUIDNetworkServerEntity networkServer = default;

        [SerializeField]
        private GUIDNetworkClientEntity networkClient = default;


        public void Remove(Guid identifer)
        {
            NetworkPlayerEntityBase<Guid> playerGO = null;

            if (IsClient)
            {
                NetworkPlayerEntityBase<Guid> player = clientPlayersProcessor.RemoveNetworkEntityClient(identifer);

                if(player != null)
                {
                    playerGO = player;
                }
            }

            if (IsServer)
            {
                NetworkPlayerEntityBase<Guid> player = serverPlayersProcessor.RemoveNetworkEntityServer(identifer);

                if (player != null)
                {
                    playerGO = player;
                }
            }

            Destroy(playerGO.gameObject);
        }

        public NetworkPlayerEntityBase<Guid> Add(Guid identifier)
        {
            NetworkPlayerEntityBase<Guid> networkEntity = Instantiate(playerPrefab);

            // rename

            networkEntity.NetworkID = identifier;

            networkEntity.gameObject.name = "Network Entity - ID : " + identifier.ToString();

            if (IsClient)
            {
                clientPlayersProcessor.AddNetworkEntityClient(identifier, networkEntity);

                // inject socket client

                ISocketClient<Guid>[] socketClients = networkEntity.GetComponentsInChildren<ISocketClient<Guid>>();

                networkClient.InjectSocketClient(socketClients);

                // init client side

                IPlayerSpawnClient[] onSpawnClient = networkEntity.GetComponentsInChildren<IPlayerSpawnClient>();

                foreach (IPlayerSpawnClient onSpawn in onSpawnClient)
                {
                    onSpawn.OnPlayerSpawnClient();
                }
            }

            if (IsServer)
            {
                serverPlayersProcessor.AddNetworkEntityServer(identifier, networkEntity);

                // inject socket client

                ISocketServer<Guid>[] socketServers = networkEntity.GetComponentsInChildren<ISocketServer<Guid>>();

                networkServer.InjectSocketServer(socketServers);

                // init server side

                IPlayerSpawnServer[] onSpawnServer = networkEntity.GetComponentsInChildren<IPlayerSpawnServer>();

                foreach (IPlayerSpawnServer onSpawn in onSpawnServer)
                {
                    onSpawn.OnPlayerSpawnServer();
                }
            }



            return networkEntity;
        }


    }
}
