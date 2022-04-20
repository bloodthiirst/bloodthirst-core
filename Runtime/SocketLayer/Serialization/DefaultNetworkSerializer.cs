using Bloodthirst.BJson;
using System;
using System.Text;
using UnityEngine;

namespace Bloodthirst.Socket.Serializer
{
    /// <summary>
    /// Basic network serialized using the bytes of JSON string
    /// Does not apply compression
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DefaultNetworkSerializer<T> : BaseNetworkSerializer<T>
    {
        public override T Deserialize(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);

            try
            {
                return BJsonConverter.FromJson<T>(json);
            }
            catch (Exception ex)
            {
                Debug.Break();
                Debug.LogError(ex.Message);
            }

            return default;
        }

        public override byte[] Serialize(T t)
        {
            string JSONString = BJsonConverter.ToJson(t);

            return Encoding.UTF8.GetBytes(JSONString);
        }
    }
}
