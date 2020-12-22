using Assets.Scripts;
using Bloodthirst.Socket;
using System;
using System.Collections.Generic;
using System.Net;

namespace Bloodthirst.Socket
{
	public class ServerConnexionManager<T> where T : IComparable<T>
	{
		private Dictionary<uint, ConnectedClientSocket> serverConnections;

		private Dictionary<uint , Type> ServerHashToTypeMap;

		public IReadOnlyDictionary<uint, ConnectedClientSocket> ServerConnexions => serverConnections;

		public ServerConnexionManager()
		{
			serverConnections = new Dictionary<uint, ConnectedClientSocket>();
		}

		public void InjectServerTypes(IEnumerable<Type> types)
		{
			ServerHashToTypeMap = new Dictionary<uint, Type>();

			foreach (Type t in types)
			{
				uint serverHash = HashUtils.StringToHash(t.Name);

				ServerHashToTypeMap.Add(serverHash, t);
			}
		}

		public void Add(uint serverHash , ConnectedClientSocket clientInfo)
		{
			serverConnections.Add(serverHash, clientInfo);
		}

		public bool ContainsHash(uint serverHesh)
		{
			return ServerHashToTypeMap.ContainsKey(serverHesh);
		}

		public void Remove<TServer>() where TServer : ManagedSocketServer<T>
		{
			uint serverHash = HashUtils.StringToHash(typeof(TServer).Name);

			// disconnect TCP
			serverConnections.Remove(serverHash);
		}

		public void Remove(uint serverHash)
		{
			serverConnections.Remove(serverHash);
		}

		public bool TryGet<TServer>( out ConnectedClientSocket socketClient) where TServer : ManagedSocketServer<T>
		{
			uint serverHash = HashUtils.StringToHash(typeof(TServer).Name);

			if (!serverConnections.ContainsKey(serverHash))
			{
				socketClient = null;

				return false;
			}

			socketClient = serverConnections[serverHash];

			return true;
		}

		public void Clear()
		{
			serverConnections.Clear();
		}
	}
}
