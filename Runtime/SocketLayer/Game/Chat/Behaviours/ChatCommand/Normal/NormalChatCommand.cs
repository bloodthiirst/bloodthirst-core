using Assets.Scripts.ChatUI;
using Assets.Scripts.SocketLayer.Components;
using Assets.Scripts.SocketLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Chat.ChatCommand
{
    public class NormalChatCommand : ChatCommandBase<NormalCommandData>
    {

        public override void ExecuteCommand(NormalCommandData data , ChatMessageNetworkBehaviour Player)
        {
            // configure the text chat

            ChatEntryBehaviour chatGo = ChatManagerUI.Instance.AppendMessage();

            string content = new StringBuilder()
                .Append(data.From)
                .Append(" : ")
                .Append(data.Content)
                .ToString();

            chatGo.SetText(content);
            chatGo.SetColor(Color.white);
        }

        protected override bool IsValid(ChatMessage chatMessage, out NormalCommandData CommandData)
        {
            if (chatMessage.Content.StartsWith("/"))
            {
                CommandData = null;
                return false;
            }

            CommandData = new NormalCommandData()
            {
                From = chatMessage.From,
                Content = chatMessage.Content
            };

            return true;
        }
    }
}
