using Assets.Scripts.NetworkCommand;
using Newtonsoft.Json;
using System.Text;

namespace Bloodthirst.Socket.Serializer
{
    /// <summary>
    /// Basic network serialized using the bytes of JSON string
    /// Does not apply compression
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseNetworkSerializer<T> : INetworkSerializer<T>
    {
        private static BaseNetworkSerializer<T>  instance;

        public static BaseNetworkSerializer<T> Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new BaseNetworkSerializer<T>();
                }

                return instance;
            }
        }

        public int StartIndex { get; set; }

        public int Length { get; set; }

        private readonly bool entireArray;

        public BaseNetworkSerializer()
        {
            StartIndex = 0;
            entireArray = true;
        }

        public BaseNetworkSerializer(int startIndex, int length)
        {
            StartIndex = startIndex;
            Length = length;
            entireArray = false;
        }

        public BaseNetworkSerializer(int startIndex)
        {
            StartIndex = startIndex;
            entireArray = true;
        }

        public T Deserialize(byte[] data)
        {
            byte[] subData = entireArray ? data.SubArray(StartIndex) : data.SubArray(StartIndex , Length);

            string json = Encoding.UTF8.GetString(subData);

            return JsonConvert.DeserializeObject<T>(json);
        }

        public byte[] Serialize(T t)
        {
            string JSONString = JsonConvert.SerializeObject(t);

            return Encoding.UTF8.GetBytes(JSONString);
        }
    }
}
