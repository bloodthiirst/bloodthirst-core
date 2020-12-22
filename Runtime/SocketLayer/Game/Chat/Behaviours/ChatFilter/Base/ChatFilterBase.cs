using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
