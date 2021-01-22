using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets.Scripts.Chat.ChatFilter.Base
{
    [InlineProperty]
    public abstract class ChatFilterBase
    {
        [SerializeField]
        public string OriginalText;

        public abstract string ReplacementText { get; }
    }
}
