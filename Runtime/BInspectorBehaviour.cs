using Bloodthirst.JsonUnityObject;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Editor.BInspector
{
    public enum SomeEnum
    {
        None,
        First,
        Second,
        Third
    }

    public class BInspectorBehaviour : JsonMonoBehaviour
    {
        [SerializeField]
        private int health;

        [SerializeField]
        private SomeEnum enumValue;

        [SerializeField]
        private ScriptableObject someRef;

        [SerializeField]
        private Dictionary<GameObject, int> dict;

        private void Awake()
        {
            dict = new Dictionary<GameObject, int>();
            dict.Add(gameObject , 5);
        }

    }
}
