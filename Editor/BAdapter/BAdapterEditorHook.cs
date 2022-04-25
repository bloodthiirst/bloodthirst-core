using Bloodthirst.Runtime.BAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Bloodthirst.Utils.EditorOpenTracker
{
    [InitializeOnLoad]
    public class BAdapterEditorHook
    {
        private static Type[] adapters { get; set; }
        private static Dictionary<Type,Type> typeToAdapter { get; set; }
        private static Dictionary<Type,Type> adapterToType { get; set; }

        static BAdapterEditorHook()
        {
            adapters = TypeCache.GetTypesWithAttribute<BAdapterForAttribute>().ToArray();
            typeToAdapter = new Dictionary<Type, Type>();
            adapterToType = new Dictionary<Type, Type>();

            foreach (Type adapter in adapters)
            {
                BAdapterForAttribute attrData = adapter.GetCustomAttribute<BAdapterForAttribute>();

                typeToAdapter.Add(attrData.AdapterForType, adapter); 
                adapterToType.Add(adapter, attrData.AdapterForType); 
            }    

            ObjectFactory.componentWasAdded -= HandleComponentAdded;
            ObjectFactory.componentWasAdded += HandleComponentAdded;

            EditorApplication.quitting -= OnEditorQuiting;
            EditorApplication.quitting += OnEditorQuiting;
        }


        private static void HandleComponentAdded(UnityEngine.Component component)
        {
            Type t = component.GetType();

            if (adapters.Contains(t))
                return;

            if(!typeToAdapter.TryGetValue(t , out Type adapterType))
                return;

            component.gameObject.AddComponent(adapterType);
        }

        private static void OnEditorQuiting()
        {
            ObjectFactory.componentWasAdded -= HandleComponentAdded;
            EditorApplication.quitting -= OnEditorQuiting;
        }
    }
}
