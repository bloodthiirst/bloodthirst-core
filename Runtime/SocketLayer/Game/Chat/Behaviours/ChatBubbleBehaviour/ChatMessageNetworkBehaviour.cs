using Assets.Scripts.Chat.ChatCommand;
using Assets.Scripts.SocketLayer.Game.Chat.Processors;
using Assets.Scripts.SocketLayer.Models;
using Assets.SocketLayer.BehaviourComponent.NetworkPlayerEntity;
using Bloodthirst.Core.Utils;
using Bloodthirst.Socket;
using Bloodthirst.Socket.Serializer;
using Bloodthirst.Socket.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.SocketLayer.Components
{
    public class ChatMessageNetworkBehaviour : GUIDNetworkBehaviourBase
    {
        [SerializeField]
        private ChatBubbleBehaviour chatBubble;

        [ShowInInspector]
        private ChatMessagePacketClientProcessor chatMessageClient;

        [ShowInInspector]
        private ChatMessagePacketServerProcessor chatMessageServer;

        [ShowInInspector]
        private List<IChatCommand> chatCommands;

        public ChatBubbleBehaviour ChatBubble => chatBubble;

        private IEnumerable<IChatCommand> QueryChatCommandServices()
        {
            List<Type> serviceTypes = TypeUtils.AllTypes
            .Where(t => t.IsClass)
            .Where(t => !t.IsAbstract)
            .Where(t => t.GetInterfaces().Contains(typeof(IChatCommand)))
            .ToList();

            foreach (Type t in serviceTypes)
            {
                IChatCommand chatCommand = (IChatCommand)Activator.CreateInstance(t);
                chatCommand.Player = this;

                yield return chatCommand;
            }
        }

        private void Awake()
        {
            chatCommands = QueryChatCommandServices().ToList();


            if (ChatBubble == null)
            {
                chatBubble = GetComponent<ChatBubbleBehaviour>();
            }


            chatMessageServer = NetworkEntity.GetServer<ChatMessagePacketServerProcessor, ChatMessage>();

            chatMessageServer.OnPacketParsedUnityThread -= OnChatMessageServer;
            chatMessageServer.OnPacketParsedUnityThread += OnChatMessageServer;

            chatMessageClient = NetworkEntity.GetClient<ChatMessagePacketClientProcessor, ChatMessage>();

            chatMessageClient.OnPacketParsedUnityThread -= OnChatMessageClient;
            chatMessageClient.OnPacketParsedUnityThread += OnChatMessageClient;
        }

        private void Start()
        {
            if (IsPlayer)
            {
                ChatManagerUI.Instance.Setup(this);
            }
        }

        private void ProcessChat(ChatMessage msg)
        {
            for (int i = 0; i < chatCommands.Count; i++)
            {
                if (chatCommands[i].ExecuteCommand(msg))
                {
                    Debug.Log("A Chat command was found");
                    return;
                }
            }

            Debug.Log("No appropriate command was found");
        }

        private void OnChatMessageClient(ChatMessage msg, Guid from)
        {
            ProcessChat(msg);
        }

        private void OnChatMessageServer(ChatMessage msg, Guid from, ConnectedClientSocket socket)
        {
            // if the player is server-side only

            byte[] msgPacket = PacketBuilder.BuildPacket(from, msg, SocketServer.IdentifierSerializer, BaseNetworkSerializer<ChatMessage>.Instance);

            ProcessChat(msg);

            if (IsPlayer)
            {
                SocketServer.BroadcastTCP(msgPacket, (id) => !id.Equals(from));
            }
            else
            {
                SocketServer.BroadcastTCP(msgPacket);
            }

        }

        public void SendChatMessage(string from, string content)
        {
            ChatMessage chatMessage = new ChatMessage()
            {
                From = from,
                Content = content
            };

            byte[] msgPacket = PacketBuilder.BuildPacket(NetworkID, chatMessage, SocketClient.IdentitySerializer, BaseNetworkSerializer<ChatMessage>.Instance);

            SocketClient.SendTCP(msgPacket);
        }
    }
}
