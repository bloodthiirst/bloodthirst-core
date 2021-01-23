using UnityEngine;

namespace Bloodthirst.Scripts.SocketLayer.BehaviourComponent
{
    public interface INetworkInjector
    {
        void InjectSocket(GameObject gameObject);
    }
}
