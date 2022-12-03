#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using System;
using System.Collections.Generic;

namespace Bloodthirst.Socket
{
    public class ClientConnexionManager<T> where T : IComparable<T>
    {
        #if ODIN_INSPECTOR[ShowInInspector]#endif
        private Dictionary<T, ConnectedClientSocket> clientConnexions;

        public IReadOnlyDictionary<T, ConnectedClientSocket> ClientConnexions => clientConnexions;

        public ClientConnexionManager()
        {
            clientConnexions = new Dictionary<T, ConnectedClientSocket>();
        }

        public ClientConnexionManager(IEqualityComparer<T> equalityComparer)
        {
            clientConnexions = new Dictionary<T, ConnectedClientSocket>(equalityComparer);
        }

        public void Add(T id, ConnectedClientSocket clientInfo)
        {
            clientConnexions.Add(id, clientInfo);
        }


        public void Remove(T id)
        {
            // disconnect TCP
            clientConnexions.Remove(id);
        }


        public bool TryGet(T id, out ConnectedClientSocket socketClient)
        {
            if (!clientConnexions.ContainsKey(id))
            {
                socketClient = null;

                return false;
            }

            socketClient = clientConnexions[id];

            return true;
        }

        public void Clear()
        {
            clientConnexions.Clear();
        }
    }
}
