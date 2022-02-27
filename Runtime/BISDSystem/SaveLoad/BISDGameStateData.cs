using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    public class BISDGameStateData : SerializedScriptableObject
    {
        [SerializeField]
        private string title;
        public string Title { get => title; set => title = value; }

        [SerializeField]
        private Dictionary<ISavableSpawnInfo, List<ISavableGameSave>> gameDatas;

        public Dictionary<ISavableSpawnInfo, List<ISavableGameSave>> GameDatas { get => gameDatas; set => gameDatas = value; }

    }
}