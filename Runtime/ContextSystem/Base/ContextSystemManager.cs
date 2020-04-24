using Bloodthirst.Core.PersistantAsset;
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
        private List<IContextInstance> contextSet;
        public List<IContextInstance> ContextSet { get => contextSet; set => contextSet = value; }

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
            if (ContextSet == null)
            {
                ContextSet = new List<IContextInstance>();
                return;
            }

            ContextSet.Clear();

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

            if (Instance.ContextSet == null)
                Instance.ContextSet = new List<IContextInstance>();
        }
#endif

        public static void AddContext(IContextInstance contextInstance)
        {
            if (Instance.ContextSet.Contains(contextInstance))
                return;

            Instance.ContextSet.Add(contextInstance);
        }

        public static void SetContext(IContextInstance contextInstance, int index)
        {
            Instance.ContextSet[index] = contextInstance;
        }

        public static void RemoveContext(IContextInstance contextInstance)
        {
            Instance.ContextSet.Remove(contextInstance);
        }

        public static void RepalceContext(IContextInstance oldcontext, IContextInstance newContext)
        {
            if (!Instance.ContextSet.Contains(oldcontext))
            {
                AddContext(oldcontext);
            }

            RemoveContext(oldcontext);
            AddContext(newContext);
        }
        public static bool HasContext(IContextInstance context)
        {
            return Instance.ContextSet.Contains(context);
        }
    }
}
