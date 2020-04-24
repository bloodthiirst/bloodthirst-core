using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.SocketLayer.BehaviourComponent.NetworkPlayerEntity
{
    public interface IPlayerSpawnClient
    {
        bool IsDoneClientSpawn { get; set; }
        void OnPlayerSpawnClient();
    }
}
