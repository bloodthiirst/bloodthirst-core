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
    public class ScreamChatCommand : ChatCommandBase<ScreamCommandData>
    {

        public override void ExecuteCommand(ScreamCommandData data , ChatMessageNetworkBehaviour Player)
        {
            // configure the text chat

            ChatEntryBehaviour chatGo = ChatManagerUI.Instance.AppendMessage();

            string content = new StringBuilder()
                .Append(data.From)
                .Append(" [Screams] : ")
                .Append(data.Content)
                .ToString();

            chatGo.SetColor(Color.red);
            chatGo.SetText(content);

            // configure the bubble

            ChatBubbleBehaviour bubble = Player.ChatBubble;
            bubble.Say(data.Content);
            bubble.SetColor(Color.red);
        }

        protected override bool IsValid(ChatMessage chatMessage, out ScreamCommandData CommandData)
        {
            if (!chatMessage.Content.StartsWith("/l"))
            {
                CommandData = null;
                return false;
            }

            string text = chatMessage.Content.Substring(2);

            CommandData = new ScreamCommandData()
            {
                From = chatMessage.From,
                Content = text
            };

            return true;
        }
    }
}
