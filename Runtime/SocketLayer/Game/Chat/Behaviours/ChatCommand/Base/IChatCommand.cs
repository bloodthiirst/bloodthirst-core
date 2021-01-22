using Assets.Scripts.SocketLayer.Components;
using Assets.Scripts.SocketLayer.Models;

namespace Assets.Scripts.Chat.ChatCommand
{
    public interface IChatCommand
    {
        bool ExecuteCommand(ChatMessage command);
        ChatMessageNetworkBehaviour Player { get; set; }
    }
}
