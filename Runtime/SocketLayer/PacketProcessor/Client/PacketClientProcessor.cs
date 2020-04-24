using Assets.Scripts;
using Assets.Scripts.NetworkCommand;
using Bloodthirst.Core.ThreadProcessor;
using Bloodthirst.Socket.PacketParser;
using Bloodthirst.Socket.Serializer;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.SocketLayer.PacketParser
{
    public abstract class PacketClientProcessor<TType, TIdentitfier> : PacketClientProcessorBase<TIdentitfier> where TIdentitfier : IComparable<TIdentitfier>
    {
#if UNITY_EDITOR
        [ShowInInspector]
        private readonly string typeName = typeof(TType).Name;
#endif
        private static readonly uint typeHash = HashUtils.StringToHash(typeof(TType).Name);

        public event Action<TType, TIdentitfier> OnPacketParsedUnityThread;

        public event Action<TType, TIdentitfier> OnPacketParsedThreaded;

        protected PacketClientProcessor() : base(typeHash)
        {
        }

        /// <summary>
        /// abstract method used to treat the data and apply some check ups before sending it to the client
        /// </summary>
        /// <param name="data"></param>
        public abstract bool Validate(ref TType data);

        /// <summary>
        /// Serializer used for the data type
        /// </summary>
        public abstract INetworkSerializer<TType> DataSerializer { get; }

        /// <summary>
        /// Serializer used for the identifier type
        /// </summary>
        public abstract INetworkSerializer<TIdentitfier> IdentifierSerializer { get; }

        public override void Process(TIdentitfier from , byte[] packet)
        {    

            TType data = DataSerializer.Deserialize(packet);

            if (!Validate(ref data))
                return;

            // trigger unity thread events

            if (OnPacketParsedUnityThread != null)
            {
                ThreadCommandProcessor.Append(new ThreadCommandMainAction(() => OnPacketParsedUnityThread?.Invoke(data, from)));
            }

            // trigger threaded events

            if (OnPacketParsedThreaded != null)
            {
                OnPacketParsedThreaded?.Invoke(data, from);
            }
        }
    }
}
