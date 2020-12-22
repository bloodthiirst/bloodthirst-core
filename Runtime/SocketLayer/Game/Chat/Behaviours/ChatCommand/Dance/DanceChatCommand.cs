using Assets.Scripts.ChatUI;
using Assets.Scripts.SocketLayer.Components;
using Assets.Scripts.SocketLayer.Game.Player.Behaviours;
using Assets.Scripts.SocketLayer.Models;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Chat.ChatCommand
{
    public class DanceChatCommand : ChatCommandBase<DanceCommandData>
    {
        private readonly Color textColor;

        public DanceChatCommand() : base()
        {
            textColor = new Color(255, 145, 0);
        }

        public override void ExecuteCommand(DanceCommandData data, ChatMessageNetworkBehaviour Player)
        {
            // configure the text chat

            ChatEntryBehaviour chatGo = ChatManagerUI.Instance.AppendMessage();

            string content = new StringBuilder()
                .Append(data.From)
                .Append(" is dancing")
                .ToString();

            chatGo.SetText(content);
            chatGo.SetColor(textColor);

            Player.GetComponent<PlayerAnimationBehaviour>().TriggerDance();
        }

        protected override bool IsValid(ChatMessage chatMessage, out DanceCommandData CommandData)
        {
            if (!chatMessage.Content.Trim().ToLower().Equals("/dance"))
            {
                CommandData = null;
                return false;
            }

            CommandData = new DanceCommandData()
            {
                From = chatMessage.From
            };

            return true;
        }
    }
}
