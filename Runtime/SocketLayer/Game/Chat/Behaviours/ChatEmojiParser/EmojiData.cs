using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets.Scripts.Chat.Model
{
    [InlineProperty]
    public class EmojiData
    {
        [HorizontalGroup("Base")]

        [ShowInInspector]
        [ReadOnly]
        [HideLabel]
        public uint index;

        [HorizontalGroup("Base")]

        [ShowInInspector]
        [ReadOnly]
        [HideLabel]
        public string name;

        [HorizontalGroup("Base")]

        [ShowInInspector]
        [PreviewField]
        [HideLabel]
        [ReadOnly]
        public Sprite preview;
    }
}
