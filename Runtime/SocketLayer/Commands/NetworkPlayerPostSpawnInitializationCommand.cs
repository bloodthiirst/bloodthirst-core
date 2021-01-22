using Bloodthirst.Socket.Core;
using Bloodthirst.System.CommandSystem;
using UnityEngine;

namespace Assets.Scripts.SocketLayer.Commands
{
    public class NetworkPlayerPostSpawnInitializationCommand : CommandBase<NetworkPlayerPostSpawnInitializationCommand>
    {
        /// <summary>
        /// Instance of the spawned player
        /// </summary>
        private readonly GameObject networkPlayerEntity;

        private IOnPlayerSpawnedClient[] clientInitializables;

        private IOnPlayerSpawnedServer[] serverInitializables;

        public NetworkPlayerPostSpawnInitializationCommand(GameObject networkPlayerEntity)
        {
            this.networkPlayerEntity = networkPlayerEntity;
        }

        public override void OnStart()
        {
            // execute on spawn for server if needed

            if (SocketConfig.Instance.IsServer)
            {
                serverInitializables = networkPlayerEntity.gameObject.GetComponents<IOnPlayerSpawnedServer>();

                foreach (IOnPlayerSpawnedServer init in serverInitializables)
                {
                    init.OnPlayerSpawnedServer();
                }
            }

            // execute on spawn for client if needed

            if (SocketConfig.Instance.IsClient)
            {
                clientInitializables = networkPlayerEntity.gameObject.GetComponents<IOnPlayerSpawnedClient>();

                foreach (IOnPlayerSpawnedClient init in clientInitializables)
                {
                    init.OnPlayerSpawnedClient();
                }
            }
        }


        public override void OnTick(float delta)
        {
            // client

            if (SocketConfig.Instance.IsClient)
            {
                foreach (IOnPlayerSpawnedClient init in clientInitializables)
                {
                    if (!init.IsDoneClientSpawn)
                        return;
                }
            }

            // server

            if (SocketConfig.Instance.IsServer)
            {
                foreach (IOnPlayerSpawnedServer init in serverInitializables)
                {
                    if (!init.IsDoneServerSpawn)
                        return;
                }
            }

            Success();
        }
    }
}
