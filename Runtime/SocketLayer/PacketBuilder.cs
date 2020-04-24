using Assets.Scripts;
using Assets.Scripts.NetworkCommand;
using Bloodthirst.Socket.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.SocketLayer.PacketParser.Base
{
    public static class PacketBuilder
    {
        public static byte[] BuildPacket<TIdentifier, TData>(TIdentifier identifier, TData data , INetworkSerializer<TIdentifier> IdSerializer , INetworkSerializer<TData> dataSerializer) where TIdentifier : IComparable<TIdentifier>
        {
            // type

            byte[] type = BitConverter.GetBytes(HashUtils.StringToHash(typeof(TData).Name));

            // from

            byte[] from = IdSerializer.Serialize(identifier);

            // content

            byte[] content = dataSerializer.Serialize(data);

            // pre-compression

            byte[] nonCompressed = NetworkUtils.Combine(type, from, content);

            return NetworkUtils.Compress(nonCompressed);

        }

        public static bool UnpackPacket<TIdentifier>(byte[] packet , out uint type, out TIdentifier from, out byte[] data, INetworkSerializer<TIdentifier> IdSerializer)
        {
            // decompress the data
            byte[] decompressed = NetworkUtils.Decompress(packet);

            // get data type
            type = BitConverter.ToUInt32(decompressed, 0);

            // get from identifier
            from = IdSerializer.Deserialize(decompressed);

            // get the actual data
            data = decompressed.SubArray(IdSerializer.Length + 4, decompressed.Length - (IdSerializer.Length + 4));

            return true;
        }
    }
}
