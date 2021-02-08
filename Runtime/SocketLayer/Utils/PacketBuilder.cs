using Bloodthirst.Socket.Serialization;
using Bloodthirst.Socket.Serializer;
using Bloodthirst.Utils;
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

            return NetworkUtils.Compress(nonCompressed);
        }

        public static byte[] BuildPacket<TIdentifier, TData>(TIdentifier identifier, TData data) where TIdentifier : IComparable<TIdentifier>
        {
            // serializers

            INetworkSerializer<TIdentifier> IdSerializer = SerializerProvider.Get<TIdentifier>();

            INetworkSerializer<TData> dataSerializer = SerializerProvider.Get<TData>();

            return BuildPacket(identifier, data, IdSerializer, dataSerializer);
        }

        public static bool UnpackPacket<TIdentifier>(byte[] packet, out uint type, out TIdentifier from, out byte[] data)
        {
            // get serializer
            INetworkSerializer<TIdentifier> IdSerializer = SerializerProvider.Get<TIdentifier>();

            return UnpackPacket(packet, out type, out from, out data, IdSerializer);
        }

        public static bool UnpackPacket<TIdentifier>(byte[] packet, out uint type, out TIdentifier from, out byte[] data, INetworkSerializer<TIdentifier> IdSerializer)
        {
            // decompress the data
            byte[] decompressed = NetworkUtils.Decompress(packet);

            // get data type
            type = BitConverter.ToUInt32(decompressed, 0);

            // get from identifier
            from = IdSerializer.Deserialize(decompressed.SubArray(4, 16));

            // type int + from guid
            int headerSize = 4 + 16;

            // get the actual data
            data = decompressed.SubArray(headerSize, decompressed.Length - headerSize);

            return true;
        }
    }
}
