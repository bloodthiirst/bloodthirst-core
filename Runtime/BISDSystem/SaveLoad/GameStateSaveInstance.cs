#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
#if ODIN_INSPECTOR
    public class GameStateSaveInstance : SerializedScriptableObject
#else
    public class GameStateSaveInstance : ScriptableObject
#endif
    {
        [SerializeField]
        private string title;
        public string Title { get => title; set => title = value; }

        [SerializeField]
        private Dictionary<ISavableInstanceProvider, List<ISavableGameSave>> gameDatas;

        public Dictionary<ISavableInstanceProvider, List<ISavableGameSave>> GameDatas { get => gameDatas; set => gameDatas = value; }

    }
}