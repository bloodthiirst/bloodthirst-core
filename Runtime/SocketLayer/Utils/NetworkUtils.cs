using Bloodthirst.Socket.Serializer;
using System;
using System.IO;
using System.IO.Compression;

namespace Assets.Scripts.NetworkCommand
{
    public static class NetworkUtils
    {
        public static byte[] Compress(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream();
            GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true);
            zip.Write(buffer, 0, buffer.Length);
            zip.Close();
            ms.Position = 0;

            MemoryStream outStream = new MemoryStream();

            byte[] compressed = new byte[ms.Length];
            ms.Read(compressed, 0, compressed.Length);

            byte[] gzBuffer = new byte[compressed.Length + 4];
            Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);

            zip.Dispose();
            ms.Dispose();

            return gzBuffer;
        }

        public static byte[] Decompress(byte[] gzBuffer)
        {
            MemoryStream ms = new MemoryStream();
            int msgLength = BitConverter.ToInt32(gzBuffer, 0);
            ms.Write(gzBuffer, 4, gzBuffer.Length - 4);

            byte[] buffer = new byte[msgLength];

            ms.Position = 0;
            GZipStream zip = new GZipStream(ms, CompressionMode.Decompress);
            zip.Read(buffer, 0, buffer.Length);

            zip.Dispose();
            ms.Dispose();


            return buffer;
        }

        public static byte[] Combine(byte[] first, byte[] second, byte[] third)
        {
            byte[] ret = new byte[first.Length + second.Length + third.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            Buffer.BlockCopy(third, 0, ret, first.Length + second.Length,
                             third.Length);
            return ret;
        }

        public static byte[] GetCompressedData<TData , TKey>(TData Data, TKey From, INetworkSerializer<TData> serializer , INetworkSerializer<TKey> identifierProcessor)
        {
            // type hash
            // 4 bytes
            byte[] typeHash = BitConverter.GetBytes(HashUtils.StringToHash(typeof(TData).Name));

            // data content
            // variable size
            byte[] data = serializer.Serialize(Data);

            /// from identifier
            byte[] from = identifierProcessor.Serialize(From);

            // hash + from + content
            byte[] combinedData = Combine(typeHash, from, data);

            // compress data
            byte[] compressedBytes = Compress(combinedData);

            // return comressed data
            return compressedBytes;
        }

        public static byte[] GetCompressedData<TData, TKey>(TData Data, TKey From, INetworkSerializer<TKey> identifierProcessor)
        {
            // create default serializer
            INetworkSerializer<TData> serializer = SerializerProvider.Get<TData>();

            // type hash
            // 4 bytes
            byte[] typeHash = BitConverter.GetBytes(HashUtils.StringToHash(typeof(TData).Name));

            // data content
            // variable size
            byte[] data = serializer.Serialize(Data);

            /// from identifier
            byte[] from = identifierProcessor.Serialize(From);

            // hash + from + content
            byte[] combinedData = Combine(typeHash, from, data);

            // compress data
            byte[] compressedBytes = Compress(combinedData);

            // return comressed data
            return compressedBytes;
        }


        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static T[] SubArray<T>(this T[] data, int index)
        {
            int length = data.Length - index;

            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static bool TryGetTypeFromJSON<TKey> (byte[] packet, out uint type , out TKey from, out byte[] data , INetworkSerializer<TKey> identifierProcessor)
        {
            // decompress the data
            byte[] decompressed = Decompress(packet);

            // get data type
            type = BitConverter.ToUInt32(decompressed , 0);

            // get from identifier
            from = identifierProcessor.Deserialize( decompressed.SubArray(identifierProcessor.StartIndex, identifierProcessor.Length) );
            
            // get the actual data
            data = decompressed.SubArray(identifierProcessor.Length + 4, decompressed.Length - (identifierProcessor.Length + 4) );

            return true;

        }

    }
}
