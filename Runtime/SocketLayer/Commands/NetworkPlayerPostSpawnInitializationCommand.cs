using Assets.SocketLayer.BehaviourComponent;
using Assets.SocketLayer.BehaviourComponent.NetworkPlayerEntity;
using Bloodthirst.Socket;
using Bloodthirst.Socket.Core;
using Bloodthirst.System.CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.SocketLayer.Commands
{
    public class NetworkPlayerPostSpawnInitializationCommand : CommandBase<NetworkPlayerPostSpawnInitializationCommand>
    {
        /// <summary>
        /// Instance of the spawned player
        /// </summary>
        private readonly GUIDNetworkEntitySpawner networkPlayerEntity;

        private IPlayerSpawnClient[] clientInitializables;

        private IPlayerSpawnServer[] serverInitializables;

        public NetworkPlayerPostSpawnInitializationCommand(GUIDNetworkEntitySpawner networkPlayerEntity)
        {
            this.networkPlayerEntity = networkPlayerEntity;
        }

        public override void OnStart()
        {
            if (SocketConfig.Instance.IsServer)
            {
                serverInitializables = networkPlayerEntity.gameObject.GetComponents<IPlayerSpawnServer>();
                
                foreach (IPlayerSpawnServer init in serverInitializables)
                {
                    init.OnPlayerSpawnServer();
                }
            }

            if (SocketConfig.Instance.IsClient)
            {
                clientInitializables = networkPlayerEntity.gameObject.GetComponents<IPlayerSpawnClient>();

                foreach (IPlayerSpawnClient init in clientInitializables)
                {
                    init.OnPlayerSpawnClient();
                }
            }        
        }


        public override void OnTick(float delta)
        {
            // client

            if (SocketConfig.Instance.IsClient)
            {
                foreach (IPlayerSpawnClient init in clientInitializables)
                {
                    if (!init.IsDoneClientSpawn)
                        return;
                }
            }

            // server

            if (SocketConfig.Instance.IsServer)
            {
                foreach (IPlayerSpawnServer init in serverInitializables)
                {
                    if (!init.IsDoneServerSpawn)
                        return;
                }
            }

            CommandState = COMMAND_STATE.SUCCESS;
            IsDone = true;
        }

        public override void OnEnd()
        {
        }

    }
}
