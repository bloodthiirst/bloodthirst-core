using Assets.Scripts;
using Bloodthirst.Socket.Serializer;
using System;

namespace Bloodthirst.Socket.Utils
{
    public static class PacketBuilder
    {
        public static byte[] BuildPacket<TIdentifier, TData>(TIdentifier identifier, TData data, INetworkSerializer<TIdentifier> IdSerializer, INetworkSerializer<TData> dataSerializer) where TIdentifier : IComparable<TIdentifier>
        {
            // type

            byte[] type = BitConverter.GetBytes(HashUtils.StringToHash(typeof(TData).Name));

            // from

            byte[] from = IdSerializer.Serialize(identifier);

            // content

            byte[] content = dataSerializer.Serialize(data);

            // pre-compression

            byte[] nonCompressed = NetworkUtils.Combine(type, from, content);

            //return NetworkUtils.Compress(nonCompressed);
            return nonCompressed;

        }

        public static bool UnpackPacket<TIdentifier>(byte[] packet, out uint type, out TIdentifier from, out byte[] data, INetworkSerializer<TIdentifier> IdSerializer)
        {
            // decompress the data
            //byte[] decompressed = NetworkUtils.Decompress(packet);

            // get data type
            type = BitConverter.ToUInt32(packet, 0);

            // get from identifier
            from = IdSerializer.Deserialize(packet.SubArray(4, 16));

            // type int + from guid
            int headerSize = 4 + 16;

            // get the actual data
            data = packet.SubArray(headerSize, packet.Length - headerSize);

            return true;
        }
    }
}
