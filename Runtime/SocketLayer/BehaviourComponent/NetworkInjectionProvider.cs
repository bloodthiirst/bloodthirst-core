using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.SocketLayer.BehaviourComponent
{

    public static class NetworkInjectionProvider
    {
        private static HashSet<INetworkInjector> networkInjectors;

        private static HashSet<INetworkInjector> NetworkInjectors
        {
            get
            {
                if (networkInjectors == null)
                {
                    networkInjectors = new HashSet<INetworkInjector>();
                }

                return networkInjectors;
            }
        }

        static NetworkInjectionProvider()
        {
            networkInjectors = new HashSet<INetworkInjector>();
        }

        public static void Add<T>(T injector) where T : INetworkInjector
        {
            NetworkInjectors.Add(injector);
        }

        public static void InjectSockets(GameObject gameObject)
        {
            for (int i = 0; i < NetworkInjectors.Count; i++)
            {
                NetworkInjectors.ElementAt(i).InjectSocket(gameObject);
            }
        }
    }
}
