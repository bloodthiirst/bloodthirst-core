using System;

namespace Assets.Scripts.SocketLayer.Models
{
    [Serializable]
    public struct ChatMessage
    {
        public string From { get; set; }
        public string Content { get; set; }
    }
}
