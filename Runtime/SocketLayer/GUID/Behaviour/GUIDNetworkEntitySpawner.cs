using Assets.Scripts.SocketLayer.BehaviourComponent;
using Assets.Scripts.SocketLayer.Commands;
using Assets.SocketLayer.BehaviourComponent.NetworkPlayerEntity;
using Assets.SocketLayer.Client.Base;
using Bloodthirst.Core.SceneManager;
using Bloodthirst.System.CommandSystem;
using System;
using UnityEngine;

namespace Assets.SocketLayer.BehaviourComponent
{
    public class GUIDNetworkEntitySpawner : NetworkEntitySpawnerBase<Guid>
    {
        private static bool IsServer => SocketConfig.Instance.IsServer;

        private static bool IsClient => SocketConfig.Instance.IsClient;

        [SerializeField]
        private GUIDNetworkPlayerEntity playerPrefab;

        protected override NetworkPlayerEntityBase<Guid> PlayerPrefab => playerPrefab;

        [SerializeField]
        private GUIDNetworkClientPlayerPacketProcessor clientPlayersProcessor;

        [SerializeField]
        private GUIDNetworkServerPlayerPacketProcessor serverPlayersProcessor;

        [SerializeField]
        private GUIDNetworkServerEntity networkServer;

        [SerializeField]
        private GUIDNetworkClientEntity networkClient;

        [SerializeField]
        private SceneManagerProviderBehaviourBase sceneManagerProvider;

        public void Remove(Guid identifer)
        {
            NetworkPlayerEntityBase<Guid> playerGO = null;

            if (IsClient)
            {
                NetworkPlayerEntityBase<Guid> player = clientPlayersProcessor.RemoveNetworkEntityClient(identifer);

                if (player != null)
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

            // move to gameplay scene

            sceneManagerProvider.GetSceneManager().AddToScene(networkEntity.gameObject);

            // rename

            networkEntity.NetworkID = identifier;

            networkEntity.gameObject.name = "Network Entity - ID : " + identifier.ToString();

            if (IsClient)
            {
                clientPlayersProcessor.AddNetworkEntityClient(identifier, networkEntity);

                // inject socket client

                // make a unique client injection point for all socketclients 
                // ex : ClientInjecor.Inject(networkEntity);

                //NetworkInjectionProvider.InjectSockets(networkEntity.gameObject);


                ISocketClient<GUIDSocketClient, Guid>[] socketClients = networkEntity.GetComponentsInChildren<ISocketClient<GUIDSocketClient, Guid>>();

                networkClient.InjectSocketClient(socketClients);

            }

            if (IsServer)
            {
                serverPlayersProcessor.AddNetworkEntityServer(identifier, networkEntity);

                // inject socket client

                ISocketServer<Guid>[] socketServers = networkEntity.GetComponentsInChildren<ISocketServer<Guid>>();

                networkServer.InjectSocketServer(socketServers);
            }


            var batch = CommandManagerBehaviour.AppendBatch<CommandBatchQueue>(this, true);

            batch.Append(new NetworkPlayerPostSpawnInitializationCommand(networkEntity.gameObject));

            return networkEntity;
        }


    }
}
