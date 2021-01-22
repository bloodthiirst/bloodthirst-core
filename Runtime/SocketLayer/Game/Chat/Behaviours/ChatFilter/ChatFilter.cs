using Assets.Scripts.Chat.ChatFilter.Base;
using Bloodthirst.Core.PersistantAsset;
using System.Collections.Generic;

namespace Assets.Scripts.Chat.ChatFilter
{
    public class ChatFilter : SingletonScriptableObject<ChatFilter>
    {
        public List<ChatFilterBase> chatFilterBases;
    }
}
