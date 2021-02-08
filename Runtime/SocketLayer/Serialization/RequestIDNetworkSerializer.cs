using Bloodthirst.Socket.Serializer;
using System;

namespace Bloodthirst.Socket.Serialization
{
    /// <summary>
    /// A serializer used for dealing with empty structs marked with the interface <see cref="IEmptyStruct"/>
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public sealed class EmptyStructNetworkSerializer<TData> : BaseNetworkSerializer<TData> where TData : IEmptyStruct
    {
        public override TData Deserialize(byte[] packet)
        {
            return default;
        }

        public override byte[] Serialize(TData identifier)
        {
            return Array.Empty<byte>();
        }
    }
}
