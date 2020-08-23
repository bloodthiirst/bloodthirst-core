using Bloodthirst.Core.PersistantAsset;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Bloodthirst.Core.BISD.CodeGeneration
{
    public class BISDGeneratorOnReloadScripts
    {
        private static readonly string[] filterFiles =
        {
            "BISDTag",
            "BISDGeneratorOnReloadScripts"
        };

        [DidReloadScripts(SingletonScriptableObjectInit.BISD_OBSERVABLE_GENERATOR)]
        public static void OnReloadScripts()
        {
            // code generators
            List<ICodeGenerator> codeGenerators = new List<ICodeGenerator>()
            {
                new ObservableFieldsCodeGenerator()
            };

            // get models info
            Dictionary<string, Container<Type>> TypeList = null;
            Dictionary<string, Container<TextAsset>> TextList = null;

            ExtractBISDInfo(ref TypeList, ref TextList);

            string[] models = TypeList.Keys.ToArray();

            // run thorugh the models to apply the changes
            foreach (string model in models)
            {
                Container<Type> typeList = TypeList[model];

                Container<TextAsset> textList = TextList[model];

                foreach (ICodeGenerator generator in codeGenerators)
                {
                    if (generator.ShouldInject(typeList, textList))
                    {
                        generator.InjectGeneratedCode(typeList, textList);
                    }
                }
            }

            // save the changes
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        private static string FieldToObserverName(FieldInfo field)
        {
            StringBuilder sb = new StringBuilder(field.GetNiceName());
            sb.Append("Prop");
            sb[0] = Char.ToUpper(sb[0]);
            return sb.ToString();
        }


        /// <summary>
        /// Extracts info about the classes of that follow the BISD pattern
        /// </summary>
        /// <param name="TypeList"></param>
        /// <param name="TextList"></param>
        private static void ExtractBISDInfo(ref Dictionary<string, Container<Type>> TypeList, ref Dictionary<string, Container<TextAsset>> TextList)
        {
            // get 
            var allTypes = AppDomain.CurrentDomain.GetAssemblies().Select(asm => asm.GetTypes()).SelectMany(t => t).ToList();

            List<TextAsset> textAssets = FindTextAssets()
                .Where(t => !filterFiles.Contains(t.name))
                .Where(t => !t.name.EndsWith(".cs"))
                .Where(t => t.text.Contains("BISDTag"))
                .ToList();

            // key : model name
            // value : types of => behaviour , instance , state , data
            TypeList = new Dictionary<string, Container<Type>>();

            // key : model name
            // value : text files of => behaviour , instance , state , data
            TextList = new Dictionary<string, Container<TextAsset>>();

            foreach (TextAsset text in textAssets)
            {
                // local vars

                Container<Type> tupleType = null;

                Container<TextAsset> tupleText = null;


                string model = null;

                // start comparing

                if (text.name.EndsWith("Behaviour"))
                {
                    model = text.name.Remove(text.name.Length - 9);

                    // type
                    if (!TypeList.TryGetValue(model, out tupleType))
                    {
                        tupleType = new Container<Type>();
                        TypeList.Add(model, tupleType);
                    }

                    Type behaviour = allTypes.FirstOrDefault(t => t.Name.Equals(text.name));

                    tupleType.Behaviour = behaviour;

                    // text

                    if (!TextList.TryGetValue(model, out tupleText))
                    {
                        tupleText = new Container<TextAsset>();
                        TextList.Add(model, tupleText);
                    }

                    tupleText.Behaviour = text;
                }

                if (text.name.EndsWith("Instance"))
                {
                    model = text.name.Remove(text.name.Length - 8);


                    // type
                    if (!TypeList.TryGetValue(model, out tupleType))
                    {
                        tupleType = new Container<Type>();
                        TypeList.Add(model, tupleType);
                    }

                    Type instance = allTypes.FirstOrDefault(t => t.Name.Equals(text.name));

                    tupleType.Instance = instance;

                    // text

                    if (!TextList.TryGetValue(model, out tupleText))
                    {
                        tupleText = new Container<TextAsset>();
                        TextList.Add(model, tupleText);
                    }

                    tupleText.Instance = text;
                }

                if (text.name.EndsWith("State"))
                {
                    model = text.name.Remove(text.name.Length - 5);


                    // type
                    if (!TypeList.TryGetValue(model, out tupleType))
                    {
                        tupleType = new Container<Type>();
                        TypeList.Add(model, tupleType);
                    }

                    Type state = allTypes.FirstOrDefault(t => t.Name.Equals(text.name));

                    tupleType.State = state;

                    // text

                    if (!TextList.TryGetValue(model, out tupleText))
                    {
                        tupleText = new Container<TextAsset>();
                        TextList.Add(model, tupleText);
                    }

                    tupleText.State = text;

                }
                if (text.name.EndsWith("Data"))
                {
                    model = text.name.Remove(text.name.Length - 4);


                    // type
                    if (!TypeList.TryGetValue(model, out tupleType))
                    {
                        tupleType = new Container<Type>();
                        TypeList.Add(model, tupleType);
                    }

                    Type data = allTypes.FirstOrDefault(t => t.Name.Equals(text.name));

                    tupleType.Data = data;

                    // text

                    if (!TextList.TryGetValue(model, out tupleText))
                    {
                        tupleText = new Container<TextAsset>();
                        TextList.Add(model, tupleText);
                    }

                    tupleText.Data = text;
                }

                tupleType.ModelName = model;
                tupleText.ModelName = model;

            }
        }


        public static List<TextAsset> FindTextAssets()
        {
            List<TextAsset> assets = new List<TextAsset>();
            string[] guids = AssetDatabase.FindAssets("t:TextAsset");
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }

    }
}
