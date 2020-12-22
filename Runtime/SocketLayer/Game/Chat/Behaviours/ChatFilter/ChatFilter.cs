using Assets.Scripts.Chat.ChatFilter.Base;
using Bloodthirst.Core.PersistantAsset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Chat.ChatFilter
{
    public class ChatFilter : SingletonScriptableObject<ChatFilter>
    {
        public List<ChatFilterBase> chatFilterBases;
    }
}
