using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    public class GameStateSaveInstance : SerializedScriptableObject
    {
        [SerializeField]
        private string title;
        public string Title { get => title; set => title = value; }

        [SerializeField]
        private Dictionary<ISavableInstanceProvider, List<ISavableGameSave>> gameDatas;

        public Dictionary<ISavableInstanceProvider, List<ISavableGameSave>> GameDatas { get => gameDatas; set => gameDatas = value; }

    }
}