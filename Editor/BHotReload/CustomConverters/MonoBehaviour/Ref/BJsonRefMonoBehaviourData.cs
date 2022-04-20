using Bloodthirst.Core.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.BJson
{
    internal struct BJsonRefMonoBehaviourData
    {
        public string ScenePath { get; set; }
        public string GameObjectName { get; set; }
        public List<int> SceneGameObjectIndex { get; set; }
        public int ComponentIndex { get; set; }
        public Type ComponentType { get; set; }

        public static BJsonRefMonoBehaviourData CreateRef(object component)
        {
            Component casted = (Component)component;


            UnityEngine.SceneManagement.Scene scene = casted.gameObject.scene;

            Component[] allComps = casted.gameObject.GetComponents<Component>();

            Type compType = casted.GetType();
            int compIndex = allComps.IndexOf(casted);
            List<int> scenePath = new List<int>();

            Transform curr = casted.transform;

            while (curr != null)
            {
                scenePath.Add(curr.transform.GetSiblingIndex());
                curr = curr.transform.parent;
            }

            scenePath.Reverse();

            return new BJsonRefMonoBehaviourData()
            {
                ScenePath = scene.path,
                ComponentIndex = compIndex,
                ComponentType = compType,
                GameObjectName = casted.gameObject.name,
                SceneGameObjectIndex = scenePath
            };
        }

        public static object LoadRef(BJsonRefMonoBehaviourData componentRef, BJsonSettings settings)
        {
            UnityObjectContext ctx = (UnityObjectContext)settings.CustomContext;

            GameObject[] gos = ctx.allSceneObjects[componentRef.ScenePath];

            GameObject curr = gos[componentRef.SceneGameObjectIndex[0]];

            for (int i = 1; i < componentRef.SceneGameObjectIndex.Count; i++)
            {
                curr = curr.transform.GetChild(componentRef.SceneGameObjectIndex[i]).gameObject;
            }

            Component[] allComp = curr.GetComponents<Component>();

            Component currComp = allComp[componentRef.ComponentIndex];

            Type compType = currComp.GetType();

            return currComp;
        }
    }
}