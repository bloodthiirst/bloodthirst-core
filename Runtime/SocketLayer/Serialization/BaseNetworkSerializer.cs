using System;

namespace Bloodthirst.Socket.Serializer
{
    /// <summary>
    /// Base class to inherit to create a serializer for a certain type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseNetworkSerializer<T> : INetworkSerializer<T>
    {
        private static Type type => typeof(T);

        public Type Type => type;

        public abstract T Deserialize(byte[] data);

        public abstract byte[] Serialize(T t);

        byte[] INetworkSerializer.SerializeInternal(object identifier)
        {
            return Serialize((T)identifier);
        }

        object INetworkSerializer.DeserializeInternal(byte[] data)
        {
            return Deserialize(data);
        }
    }
}
