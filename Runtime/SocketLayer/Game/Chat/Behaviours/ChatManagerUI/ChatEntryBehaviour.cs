using TMPro;
using UnityEngine;

namespace Assets.Scripts.ChatUI
{
    public class ChatEntryBehaviour : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text content;

        public void SetText(string chatMsg)
        {
            content.SetText(chatMsg);
        }

        public void SetColor(Color color)
        {
            content.color = color;
        }
    }
}
