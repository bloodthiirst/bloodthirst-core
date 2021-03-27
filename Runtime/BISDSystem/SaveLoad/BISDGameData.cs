using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    public class BISDGameData : SerializedScriptableObject
    {
        [SerializeField]
        private string title;
        public string Title { get => title; set => title = value; }

        [SerializeField]
        private Dictionary<PrefabIDPair, List<IEntityState>> states;

        public Dictionary<PrefabIDPair, List<IEntityState>> States { get => states; set => states = value; }

    }
}