using Assets.Scripts.Chat.ChatFilter.Base;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Chat.ChatFilter
{
    public class ChatFilterCensor : ChatFilterBase
    {
        [SerializeField]
        public char CensorCharacter;


        public override string ReplacementText
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < OriginalText.Length; i++)
                {
                    sb.Append(CensorCharacter);
                }

                return sb.ToString();
            }
        }

    }
}
