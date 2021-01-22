using Assets.Scripts.ChatUI;
using Assets.Scripts.SocketLayer.Components;
using Assets.Scripts.SocketLayer.Models;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Chat.ChatCommand
{
    public class SayChatCommand : ChatCommandBase<SayCommandData>
    {
        public override void ExecuteCommand(SayCommandData data, ChatMessageNetworkBehaviour Player)
        {
            // configure the text chat

            ChatEntryBehaviour chatGo = ChatManagerUI.Instance.AppendMessage();

            string content = new StringBuilder()
                .Append(data.From)
                .Append(" : ")
                .Append(data.Content)
                .ToString();

            chatGo.SetColor(Color.white);
            chatGo.SetText(content);

            ChatBubbleBehaviour bubble = Player.ChatBubble;
            bubble.Say(data.Content);
            bubble.SetColor(Color.white);
        }

        protected override bool IsValid(ChatMessage chatMessage, out SayCommandData CommandData)
        {
            if (!chatMessage.Content.StartsWith("/s"))
            {
                CommandData = null;
                return false;
            }

            string text = chatMessage.Content.Substring(2);

            CommandData = new SayCommandData()
            {
                From = chatMessage.From,
                Content = text
            };

            return true;
        }
    }
}
