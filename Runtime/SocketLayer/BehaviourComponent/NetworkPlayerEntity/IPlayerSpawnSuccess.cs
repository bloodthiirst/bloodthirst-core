using System;

namespace Assets.SocketLayer.BehaviourComponent.NetworkPlayerEntity
{
    public interface IPlayerSpawnSuccess
    {
        void OnPlayerSpawnSuccess(Guid identifier);
    }
}
