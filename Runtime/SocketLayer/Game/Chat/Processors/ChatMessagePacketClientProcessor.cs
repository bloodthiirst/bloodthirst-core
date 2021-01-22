using Assets.Scripts.SocketLayer.Models;
using Assets.SocketLayer.PacketParser;
using Assets.SocketLayer.Serialization.Data;
using Bloodthirst.Socket.Serializer;
using System;

namespace Assets.Scripts.SocketLayer.Game.Chat.Processors
{
    class ChatMessagePacketClientProcessor : PacketClientProcessor<ChatMessage, Guid>
    {
        public override INetworkSerializer<ChatMessage> DataSerializer { get; }

        public override INetworkSerializer<Guid> IdentifierSerializer { get; }

        public ChatMessagePacketClientProcessor() : base()
        {
            IdentifierSerializer = new IdentityGUIDNetworkSerializer();

            DataSerializer = new BaseNetworkSerializer<ChatMessage>();
        }


        public override bool Validate(ref ChatMessage data)
        {
            return true;
        }
    }
}
