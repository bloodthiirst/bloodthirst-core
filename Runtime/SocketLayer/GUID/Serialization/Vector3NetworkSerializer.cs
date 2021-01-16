using Assets.Scripts.NetworkCommand;
using Bloodthirst.Socket.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.SocketLayer.Serialization.Data
{
    public class Vector3NetworkSerializer : INetworkSerializer<Vector3>
    {
        private static Vector3NetworkSerializer _instance;

        public static Vector3NetworkSerializer Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new Vector3NetworkSerializer();
                }

                return _instance;
            }
        }

        private const int SIZE_OF_FLOAT = sizeof(float);

        public int StartIndex => 4;

        public int Length => 16;

        public Vector3 Deserialize(byte[] packet)
        {
            byte[] from = packet.SubArray(StartIndex, Length);

            Vector3 data = new Vector3();

            for (int i = 0; i < 3; i++)
            {
                data[i] = BitConverter.ToSingle(from,  i * SIZE_OF_FLOAT);
            }

            return data;
        }

        public byte[] Serialize(Vector3 identifier)
        {
            byte[] data = new byte[SIZE_OF_FLOAT * 3];

            for (int i = 0; i < 3; i++)
            {
                var x = BitConverter.GetBytes(identifier[i]);
                Array.Copy(x, 0 ,data, i * SIZE_OF_FLOAT, SIZE_OF_FLOAT);
            }

            return data;
        }
    }
}
