using Newtonsoft.Json;
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
    public class BaseNetworkSerializer<T> : INetworkSerializer<T>
    {
        private static Type type => typeof(T);

        private static BaseNetworkSerializer<T> instance;

        public static BaseNetworkSerializer<T> Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BaseNetworkSerializer<T>();
                }

                return instance;
            }
        }

        public T Deserialize(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);

            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                Debug.Break();
                Debug.LogError(ex.Message);
            }

            return default;
        }

        public byte[] Serialize(T t)
        {
            string JSONString = JsonConvert.SerializeObject(t);

            return Encoding.UTF8.GetBytes(JSONString);
        }
    }
}
