namespace Bloodthirst.Socket.Serializer
{
    public interface INetworkSerializer<TData>
    {
        byte[] Serialize(TData identifier);

        TData Deserialize(byte[] data);
    }
}
