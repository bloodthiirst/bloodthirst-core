using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using static Bloodthirst.Core.Utils.StringExtensions;

namespace Bloodthirst.Core.BISD.CodeGeneration
{
    public class ObservableFieldsCodeGenerator : ICodeGenerator
    {
        /// <summary>
        /// Path to the template text file
        /// </summary>
        private const string STATE_PROPERTY_TEMPALTE = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "BISD Generator/CodeGenerator/Template.StateProperty.cs.txt";

        /// <summary>
        /// The start of the part of the script to inject the auto-generated observables
        /// </summary>
        private const string NAMESPACE_START_CONST = @"//NAMESPACE_START";

        /// <summary>
        /// The end of the part of the script to inject the auto-generated observables
        /// </summary>
        private const string NAMESPACE_END_CONST = @"//NAMESPACE_END";

        /// <summary>
        /// The start of the part of the script to inject the auto-generated observables
        /// </summary>
        private const string PROPS_START_CONST = @"//OBSERVABLES_START";

        /// <summary>
        /// The end of the part of the script to inject the auto-generated observables
        /// </summary>
        private const string PROPS_END_CONST = @"//OBSERVABLES_END";

        private string[] filterForNamespaces = new string[]
        {
            "System"
        };

        public bool ShouldInject(Container<Type> TypeList, Container<TextAsset> TextList)
        {
            bool mustRegenerate = false;

            // fields of state
            FieldInfo[] fields = GetObseravableFields(TypeList);

            // instance vars
            PropertyInfo[] props = GetObservableProps(TypeList);

            // events
            EventInfo[] events = GetObservableEvents(TypeList);
            MethodInfo[] methods = GetObservableMethods(TypeList);

            // check if we need to regenerate the code

            // we check for double the count since every observable has 2 properties
            // 1 getter and setter that trigger event
            // 1 setter that doesn't notify

            if (fields.Length * 3 != props.Length)
            {
                return true;
            }

            if (fields.Length != events.Length)
            {
                return true;
            }

            if (fields.Length != methods.Length)
            {
                return true;
            }

            else
            {
                foreach (FieldInfo field in fields)
                {
                    string propertyName = FieldFormatedName(field);

                    string eventName = $"On{propertyName}Changed";
                    string triggerName = $"Trigger{propertyName}Changed";

                    // search for observer with valid name
                    // property check
                    PropertyInfo property = props.FirstOrDefault(o => o.Name.Equals(propertyName));

                    if (property == null)
                    {
                        mustRegenerate = true;
                        break;
                    }

                    // property check
                    EventInfo eventProp = events.FirstOrDefault(o => o.Name.Equals(eventName));

                    if (eventProp == null)
                    {
                        mustRegenerate = true;
                        break;
                    }

                    // property check
                    MethodInfo method = methods.FirstOrDefault(o => o.Name.Equals(triggerName));

                    if (method == null)
                    {
                        mustRegenerate = true;
                        break;
                    }
                }
            }

            // if dont have to regenerate , go to next type

            return mustRegenerate;
        }

        private static MethodInfo[] GetObservableMethods(Container<Type> TypeList)
        {
            //observers
            return TypeList.Instance
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.GetCustomAttribute<ObservableAttribute>() != null)
                .ToArray();
        }
        private static PropertyInfo[] GetObservableProps(Container<Type> TypeList)
        {
            //observers
            return TypeList.Instance
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.GetCustomAttribute<ObservableAttribute>() != null)
                .ToArray();
        }

        private static EventInfo[] GetObservableEvents(Container<Type> TypeList)
        {
            //observers
            return TypeList.Instance
                .GetEvents(BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.GetCustomAttribute<ObservableAttribute>() != null)
                .ToArray();
        }

        public void InjectGeneratedCode(Container<Type> TypeList, Container<TextAsset> TextList)
        {
            FieldInfo[] fields = GetObseravableFields(TypeList);

            // do the generation

            // separate the parts

            /* Generate something like this
            public event Action<PlayerInstance> OnHealthChanged;

            public float HealthProp
            {
                get => state.health;
                set
                {
                    state.health = value;
                    OnHealthChanged?.Invoke(this);
                }
            }
            */

            string oldScript = TextList.Instance.text;

            #region write the properties for the observables in the state
            List<Tuple<SECTION_EDGE, int, int>> propsSections = oldScript.StringReplaceSection(PROPS_START_CONST, PROPS_END_CONST);

            int padding = 0;

            for (int i = 0; i < propsSections.Count - 1; i++)
            {
                Tuple<SECTION_EDGE, int, int> start = propsSections[i];
                Tuple<SECTION_EDGE, int, int> end = propsSections[i + 1];

                // if we have correct start and end
                // then do the replacing
                if (start.Item1 == SECTION_EDGE.START && end.Item1 == SECTION_EDGE.END)
                {
                    var replacementText = new StringBuilder();

                    replacementText.Append(Environment.NewLine);
                    replacementText.Append(Environment.NewLine);

                    replacementText.Append("\t").Append("\t")
                        .Append("#region state accessors")
                        .Append(Environment.NewLine)
                        .Append(Environment.NewLine);


                    foreach (FieldInfo field in fields)
                    {
                        string templateText = AssetDatabase.LoadAssetAtPath<TextAsset>(STATE_PROPERTY_TEMPALTE).text;

                        templateText = templateText.Replace("[INSTANCE_TYPE]", TypeList.Instance.Name);

                        templateText = templateText.Replace("[FIELD_NICE_NAME]", FieldFormatedName(field));

                        templateText = templateText.Replace("[FIELD_TYPE]", field.FieldType.GetNiceName());

                        templateText = templateText.Replace("[FIELD]", field.Name);

                        replacementText.Append(templateText)
                        .Append(Environment.NewLine)
                        .Append(Environment.NewLine);
                    }


                    replacementText.Append("\t").Append("\t").Append("#endregion state accessors");

                    replacementText
                    .Append(Environment.NewLine)
                    .Append(Environment.NewLine)
                    .Append("\t")
                    .Append("\t");

                    oldScript = oldScript.ReplaceBetween(start.Item3, end.Item2, replacementText.ToString());

                    int oldTextLength = end.Item2 - start.Item3;

                    padding += replacementText.Length - oldTextLength;
                }
            }
            #endregion

            #region write the properties for the observables in the state
            List<Tuple<SECTION_EDGE, int, int>> namespaceSections = oldScript.StringReplaceSection(NAMESPACE_START_CONST, NAMESPACE_END_CONST);

            padding = 0;

            for (int i = 0; i < namespaceSections.Count - 1; i++)
            {
                Tuple<SECTION_EDGE, int, int> start = namespaceSections[i];
                Tuple<SECTION_EDGE, int, int> end = namespaceSections[i + 1];

                // if we have correct start and end
                // then do the replacing
                if (start.Item1 == SECTION_EDGE.START && end.Item1 == SECTION_EDGE.END)
                {
                    StringBuilder replacementText = new StringBuilder();

                    HashSet<string> namespaceAdded = new HashSet<string>();

                    replacementText
                        .Append(Environment.NewLine)
                        .Append(Environment.NewLine);

                    foreach (FieldInfo field in fields)
                    {
                        if (field.FieldType.Namespace == null)
                            continue;

                        if (filterForNamespaces.Contains(field.FieldType.Namespace))
                            continue;

                        if (!namespaceAdded.Add(field.FieldType.Namespace))
                            continue;

                        replacementText.Append($"using {field.FieldType.Namespace};")
                        .Append(Environment.NewLine);
                    }


                    replacementText
                    .Append(Environment.NewLine);

                    oldScript = oldScript.ReplaceBetween(start.Item3, end.Item2, replacementText.ToString());

                    int oldTextLength = end.Item2 - start.Item3;

                    padding += replacementText.Length - oldTextLength;
                }
            }
            #endregion


            // save
            File.WriteAllText(AssetDatabase.GetAssetPath(TextList.Instance), oldScript);

            // set dirty
            EditorUtility.SetDirty(TextList.Instance);

            //
            Debug.Log($"Type regenerated [{ TypeList.ModelName }]");
        }

        private static FieldInfo[] GetObseravableFields(Container<Type> TypeList)
        {
            // fields
            return TypeList.State
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(f => !f.Name.Equals("data"))
                .Where(f => !f.Name.Equals("id"))
                .Where(f => f.GetCustomAttribute<ObservableAttribute>() != null)
                .ToArray();
        }

        private static string FieldFormatedName(FieldInfo field)
        {
            StringBuilder sb = new StringBuilder(field.GetNiceName());
            sb[0] = Char.ToUpper(sb[0]);
            return sb.ToString();
        }
    }
}
