using System;

namespace Bloodthirst.Socket.Serializer
{
    public interface INetworkSerializer<TData> : INetworkSerializer
    {
        byte[] Serialize(TData identifier);

        TData Deserialize(byte[] data);
    }

    public interface INetworkSerializer
    {
        Type Type { get; }
        byte[] SerializeInternal(object identifier);
        object DeserializeInternal(byte[] data);
    }
}
