using UnityEngine;

namespace Assets.Scripts.SocketLayer.BehaviourComponent
{
    public interface INetworkInjector
    {
        void InjectSocket(GameObject gameObject);
    }
}
