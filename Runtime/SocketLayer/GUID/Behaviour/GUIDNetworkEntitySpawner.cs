using Bloodthirst.Core.SceneManager;
using Bloodthirst.Core.ServiceProvider;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using Bloodthirst.Scripts.SocketLayer.BehaviourComponent;
using Bloodthirst.Scripts.SocketLayer.Commands;
using Bloodthirst.Socket.BehaviourComponent.NetworkPlayerEntity;
using Bloodthirst.Socket.Client.Base;
using Bloodthirst.System.CommandSystem;
using System;
using UnityEngine;

namespace Bloodthirst.Socket.BehaviourComponent
{
    public class GUIDNetworkEntitySpawner : NetworkEntitySpawnerBase<Guid> , IAwakePass
    {
        private static bool IsServer => SocketConfig.Instance.IsServer;

        private static bool IsClient => SocketConfig.Instance.IsClient;

        [SerializeField]
        private GUIDNetworkClientPlayerPacketProcessor clientPlayersProcessor;

        [SerializeField]
        private GUIDNetworkServerPlayerPacketProcessor serverPlayersProcessor;

        [SerializeField]
        private GUIDNetworkServerEntity networkServer;

        [SerializeField]
        private GUIDNetworkClientEntity networkClient;

        [SerializeField]
        private SceneManagerProviderBehaviourBase sceneManagerProvider = null;

        [SerializeField]
        private PrefabInstanceProviderBehaviourBase prefabInstanceProvider = null;

        private CommandManager _commandManager;

        private void Execute()
        {
            _commandManager = BProviderRuntime.Instance.GetSingleton<CommandManager>();
        }

        void IAwakePass.Execute()
        {
            Execute();
        }

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

            GUIDNetworkPlayerEntity playerEntity = playerGO.GetComponent<GUIDNetworkPlayerEntity>();

            prefabInstanceProvider.RemovePrefabInstance(playerEntity);
        }

        public NetworkPlayerEntityBase<Guid> Add(Guid identifier, string prefabPath)
        {
            GUIDNetworkPlayerEntity go = prefabInstanceProvider.GetPrefabInstance<GUIDNetworkPlayerEntity>(prefabPath);

            NetworkPlayerEntityBase<Guid> networkEntity = go.GetComponent<NetworkPlayerEntityBase<Guid>>();

            Debug.Assert(networkEntity != null, "networkEntity is null");

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


            var batch = _commandManager.AppendBatch<CommandBatchQueue>(this, true);

            batch.Append(new NetworkPlayerPostSpawnInitializationCommand(networkEntity.gameObject));

            return networkEntity;
        }


    }
}
