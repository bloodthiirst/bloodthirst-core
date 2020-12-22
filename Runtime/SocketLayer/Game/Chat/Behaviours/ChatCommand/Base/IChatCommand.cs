using Assets.Scripts.SocketLayer.Components;
using Assets.Scripts.SocketLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Chat.ChatCommand
{
    public interface IChatCommand
    {
        bool ExecuteCommand(ChatMessage command);
        ChatMessageNetworkBehaviour Player { get; set; }
    }
}
