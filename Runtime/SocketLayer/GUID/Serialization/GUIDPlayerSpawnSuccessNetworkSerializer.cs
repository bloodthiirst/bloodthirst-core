using Bloodthirst.Socket.Models;
using Bloodthirst.Socket.Serializer;
using Bloodthirst.Socket.Utils;
using System;

namespace Bloodthirst.Socket.Serialization.Data
{
    public class GUIDPlayerSpawnSuccessNetworkSerializer : BaseNetworkSerializer<GUIDPlayerSpawnSuccess>
    {
        public override GUIDPlayerSpawnSuccess Deserialize(byte[] packet)
        {
            return new GUIDPlayerSpawnSuccess()
            {
                ClientThePlayerSpawnedIn = new Guid(packet.SubArray(0, 16)),
                SpawnedPlayerID = new Guid(packet.SubArray(16, 16))
            };
        }

        public override byte[] Serialize(GUIDPlayerSpawnSuccess identifier)
        {
            byte[] data = new byte[32];

            Array.Copy(identifier.ClientThePlayerSpawnedIn.ToByteArray(), 0, data, 0, 16);
            Array.Copy(identifier.SpawnedPlayerID.ToByteArray(), 0, data, 16, 16);

            return data;
        }
    }
}
