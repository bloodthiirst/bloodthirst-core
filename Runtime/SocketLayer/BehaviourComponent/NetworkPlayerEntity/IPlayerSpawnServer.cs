using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.SocketLayer.BehaviourComponent.NetworkPlayerEntity
{
    public interface IPlayerSpawnServer
    {
        bool IsDoneServerSpawn { get; set; }
        void OnPlayerSpawnServer();
    }
}
