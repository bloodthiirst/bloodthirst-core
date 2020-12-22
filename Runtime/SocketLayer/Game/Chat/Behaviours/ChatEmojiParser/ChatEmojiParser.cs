using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Chat.Model;
using TMPro;

namespace Assets.Scripts.Chat.Emoji
{
    [CreateAssetMenu(menuName = "ChatEmojiParser/Data")]
    public class ChatEmojiParser : SerializedScriptableObject
    {

        [SerializeField]
        private TMP_SpriteAsset spriteAsset;

        [VerticalGroup]
        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.OneLine)]
        public Dictionary<string, EmojiData> emojiTextToIndex;


        [Button]
        private void Initialize()
        {
            emojiTextToIndex = new Dictionary<string, EmojiData>();

            foreach (var sprite in spriteAsset.spriteCharacterTable)
            {
                EmojiData data = new EmojiData();

                data.index = sprite.glyphIndex;
                data.name = sprite.name;
                data.preview = spriteAsset.spriteGlyphTable[(int)sprite.glyphIndex].sprite;


                emojiTextToIndex.Add("SET_CODE_" + sprite.glyphIndex, data);
            }
        }

    }
}
