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

namespace Bloodthirst.Editor.Assembly
{
    public static class AssemblyReloadHook
    {
        private const string SESSION_STATE_KEY = "AFTER_ASSEMBLY_RELOAD";

        private static string defaultEmptyValue = JsonConvert.SerializeObject(new List<CallbackData>());

        private static List<CallbackData> Get()
        {
            string oldList = SessionState.GetString(SESSION_STATE_KEY, defaultEmptyValue);
            List<CallbackData> actionList = JsonConvert.DeserializeObject<List<CallbackData>>(oldList);

            return actionList;
        }

        private static void Set(List<CallbackData> callbackDatas)
        {
            string json = JsonConvert.SerializeObject(callbackDatas);
            SessionState.SetString(SESSION_STATE_KEY, json);
        }

        [DidReloadScripts]
        private static void AfterAssemblyReload()
        {
            List<CallbackData> lst = Get();

            foreach(CallbackData a in lst)
            {
                MethodInfo m = a.ContainingType.GetMethod(a.MethodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

                m.Invoke(null , null);
            }

            lst.Clear();

            Set(lst);
        }

        public static void Add(Action methodToCall)
        {
            List<CallbackData> l = Get();

            l.Add(new CallbackData()
            {
                ContainingType = methodToCall.Method.DeclaringType,
                MethodName = methodToCall.Method.Name
            });

            Set(l);
            
        }

        private struct CallbackData
        {
            public Type ContainingType { get; set; }
            public string MethodName { get; set; }
        }
    }
}
