using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Bloodthirst.Editor.BHotReload
{
    [InitializeOnLoad]
    public static class BHotReload
    {
        static BHotReload()
        {
            AssemblyReloadEvents.beforeAssemblyReload += HandleBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += HandleAfterAssemblyReload;
        }

        private static void HandleBeforeAssemblyReload()
        {

        }

        private static void HandleAfterAssemblyReload()
        {
            
        }


    }
}
