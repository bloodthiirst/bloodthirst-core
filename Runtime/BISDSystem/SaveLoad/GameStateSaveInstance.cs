#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using Bloodthirst.JsonUnityObject;
using System.Collections.Generic;
using UnityEngine;
using static Bloodthirst.Core.BISDSystem.SaveLoadManager;

namespace Bloodthirst.Core.BISDSystem
{
#if ODIN_INSPECTOR
    public class GameStateSaveInstance : SerializedScriptableObject
#else
    public class GameStateSaveInstance : JsonScriptableObject
#endif
    {
        [SerializeField]
        private string title;
        public string Title { get => title; set => title = value; }

        [SerializeField]
        private List<SavedEntityEntry> gameDatas;

        public List<SavedEntityEntry> GameDatas { get => gameDatas; set => gameDatas = value; }

    }
}