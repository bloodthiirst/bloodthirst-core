using Bloodthirst.Core.PersistantAsset;
using Bloodthirst.Core.Utils;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.Core.BISD.CodeGeneration
{
    public class BISDGeneratorOnReloadScripts
    {
        /// <summary>
        /// Ending for state class files
        /// </summary>
        private const string STATE_FILE_ENDING = "State";

        /// <summary>
        /// Ending for instance class files
        /// </summary>
        private const string INSTANCE_FILE_ENDING = "Instance";

        /// <summary>
        /// Ending for behaviour class files
        /// </summary>
        private const string BEHAVIOUR_FILE_ENDING = "Behaviour";

        /// <summary>
        /// Ending for data class files
        /// </summary>
        private const string DATA_FILE_ENDING = "Data";

        /// <summary>
        /// Files that shouldn't be scanned for the code generation
        /// </summary>
        private static readonly string[] filterFiles =
        {
            nameof(BISDTag),
            nameof(BISDGeneratorOnReloadScripts)
        };

        [UnityEditor.Callbacks.DidReloadScripts(SingletonScriptableObjectInit.BISD_OBSERVABLE_GENERATOR)]
        public static void OnReloadScripts()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;


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


        /// <summary>
        /// Extracts info about the classes that follow the BISD pattern
        /// </summary>
        /// <param name="TypeList"></param>
        /// <param name="TextList"></param>
        private static void ExtractBISDInfo(ref Dictionary<string, Container<Type>> TypeList, ref Dictionary<string, Container<TextAsset>> TextList)
        {
            IReadOnlyList<Type> allTypes = TypeUtils.AllTypes;

            // find all the scripts that contain the BISDTag
            // which means the scripts that need to be treated
            List<TextAsset> textAssets = EditorUtils.FindTextAssets()
                .Where(t => !filterFiles.Contains(t.name))
                .Where(t => !t.name.EndsWith(".cs"))
                .Where(t => t.text.Contains(nameof(BISDTag)))
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

                if (text.name.EndsWith(BEHAVIOUR_FILE_ENDING))
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

                if (text.name.EndsWith(INSTANCE_FILE_ENDING))
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

                if (text.name.EndsWith(STATE_FILE_ENDING))
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
                if (text.name.EndsWith(DATA_FILE_ENDING))
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



    }
}
