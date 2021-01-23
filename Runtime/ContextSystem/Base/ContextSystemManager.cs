﻿using Bloodthirst.Core.PersistantAsset;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif
using UnityEngine;

namespace Bloodthirst.System.ContextSystem
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class ContextSystemManager : SingletonScriptableObject<ContextSystemManager>
    {
        [SerializeField]
        public List<ScriptableObject> AllContextInstance;

        [SerializeField]
        private List<List<IContextInstance>> contextLayers;
        public List<List<IContextInstance>> ContextLayers { get => contextLayers; set => contextLayers = value; }

        public static event Action<int, IContextInstance> OnContextAdded;

        public static event Action<int, IContextInstance> OnContextRemoved;

        public static event Action OnContextChanged;

#if UNITY_EDITOR

        static ContextSystemManager()
        {
            EditorApplication.playModeStateChanged -= InitContextSystem;
            EditorApplication.playModeStateChanged += InitContextSystem;
        }


        private static void InitContextSystem(PlayModeStateChange obj)
        {
            Instance.Initialize();
        }

        [Button]
        public void Initialize()
        {
            if (ContextLayers == null)
            {
                ContextLayers = new List<List<IContextInstance>>();
                return;
            }

            ContextLayers.Clear();

            ReloadContexts();
        }

        [DidReloadScripts(SingletonScriptableObjectInit.SINGLETONS_RELOAD)]
        public static void ReloadContexts()
        {

            if (Instance.AllContextInstance == null)
                Instance.AllContextInstance = new List<ScriptableObject>();
            else
                Instance.AllContextInstance.Clear();

            ScriptableObject[] allContexts = Resources.LoadAll("Singletons", typeof(IContextInstance)).Cast<ScriptableObject>().Where(so => so != null).ToArray();

            for (int i = 0; i < allContexts.Length; i++)
            {
                Instance.AllContextInstance.Add(allContexts[i]);
            }

            if (Instance.ContextLayers == null)
                Instance.ContextLayers = new List<List<IContextInstance>>();
        }


#endif
        private static void CheckSize(int contextLayer, int indexInLayer = -1)
        {
            int missingLayers = (contextLayer + 1) - Instance.contextLayers.Count;
            // make sure we have enough layers
            for (int i = 0; i < missingLayers; i++)
            {
                Instance.contextLayers.Add(new List<IContextInstance>());
            }

            if (indexInLayer == -1)
                return;

            int missingIndex = (indexInLayer + 1) - Instance.contextLayers[contextLayer].Count;
            // make sure we have enough context slots
            for (int i = 0; i < missingIndex; i++)
            {
                Instance.contextLayers[contextLayer].Add(null);
            }
        }

        public static int LayerCount(int contextLayer)
        {
            CheckSize(contextLayer);

            return Instance.ContextLayers[contextLayer].Count;
        }

        public static void AddContext(int contextLayer, IContextInstance contextInstance)
        {
            CheckSize(contextLayer);

            if (Instance.ContextLayers[contextLayer].Contains(contextInstance))
                return;

            Instance.ContextLayers[contextLayer].Add(contextInstance);

            OnContextAdded?.Invoke(contextLayer, contextInstance);
            OnContextChanged?.Invoke();
        }

        public static void SetContext(IContextInstance contextInstance, int contextLayer, int indexInLayer)
        {
            CheckSize(contextLayer, indexInLayer);

            Instance.ContextLayers[contextLayer][indexInLayer] = contextInstance;

            OnContextChanged?.Invoke();
        }

        public static void RemoveContext(int contextLayer, IContextInstance contextInstance)
        {
            CheckSize(contextLayer);

            if (Instance.ContextLayers[contextLayer].Remove(contextInstance))
            {
                OnContextRemoved?.Invoke(contextLayer, contextInstance);
            }
        }



        public static void RepalceContext(int contextLayer, IContextInstance oldcontext, IContextInstance newContext)
        {
            if (!HasContext(contextLayer, oldcontext))
                return;

            int oldIndex = Instance.ContextLayers[contextLayer].IndexOf(oldcontext);
            Instance.ContextLayers[contextLayer][oldIndex] = newContext;

            OnContextRemoved?.Invoke(contextLayer, oldcontext);
            OnContextAdded?.Invoke(contextLayer, newContext);
        }
        public static bool HasContext(int contextLayer, IContextInstance context)
        {
            CheckSize(contextLayer, 0);
            return Instance.ContextLayers[contextLayer].Contains(context);
        }
    }
}
