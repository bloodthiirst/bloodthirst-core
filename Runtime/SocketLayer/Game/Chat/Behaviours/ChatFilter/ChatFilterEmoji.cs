using Assets.Scripts.Chat.ChatFilter.Base;
using Assets.Scripts.Chat.Emoji;
using Sirenix.OdinInspector;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Chat.ChatFilter
{
    public class ChatFilterEmoji : ChatFilterBase
    {


        [SerializeField]
        private ChatEmojiParser emojiParser;


#if UNITY_EDITOR

        [PreviewField]
        [ShowInInspector]
        private Sprite preview
        {
            get
            {
                if (!emojiParser.emojiTextToIndex.ContainsKey(OriginalText))
                {
                    return null;
                }

                return emojiParser.emojiTextToIndex[OriginalText].preview;
            }
        }
#endif

        public override string ReplacementText
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("<sprite=").Append(emojiParser.emojiTextToIndex[OriginalText].index).Append(">");

                return sb.ToString();
            }
        }

    }
}
