using Assets.Scripts.SocketLayer.Components;
using Assets.Scripts.SocketLayer.Models;

namespace Assets.Scripts.Chat.ChatCommand
{
    public abstract class ChatCommandBase<T> : IChatCommand
    {
        public ChatMessageNetworkBehaviour Player { get; set; }

        protected abstract bool IsValid(ChatMessage command, out T CommandData);

        public bool Process(ChatMessage command)
        {
            T data = default;

            if (!IsValid(command, out data))
                return false;

            ExecuteCommand(data, Player);
            return true;
        }

        public abstract void ExecuteCommand(T data, ChatMessageNetworkBehaviour Player);

        public bool ExecuteCommand(ChatMessage command)
        {
            return Process(command);
        }
    }
}
