using Bloodthirst.JsonUnityObject;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    public class BInspectorBehaviour: JsonMonoBehaviour
    {
        [SerializeField]
        private int health;

        [SerializeField]
        private ScriptableObject someRef;
    }
}
