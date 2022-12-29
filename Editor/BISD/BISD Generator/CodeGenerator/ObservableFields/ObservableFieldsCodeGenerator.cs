using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
using Bloodthirst.Editor.CodeGenerator;
#if ODIN_INSPECTOR
using Sirenix.Utilities;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

namespace Bloodthirst.Core.BISD.CodeGeneration
{
    public class ObservableFieldsCodeGenerator : ICodeGenerator
    {
        /// <summary>
        /// Path to the template text file
        /// </summary>
        private const string STATE_PROPERTY_TEMPALTE = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "BISD/BISD Generator/CodeGenerator/ObservableFields/Template.StateProperty.cs.txt";

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

        private StringBuilder sb = new StringBuilder();

        public bool ShouldInject(BISDInfoContainer typeInfo)
        {
            bool mustRegenerate = false;

            using (ListPool<FieldInfo>.Get(out List<FieldInfo> fields))
            using (ListPool<PropertyInfo>.Get(out List<PropertyInfo> props))
            using (ListPool<EventInfo>.Get(out List<EventInfo> events))
            using (ListPool<MethodInfo>.Get(out List<MethodInfo> methods))
            {
                // fields of state
                fields.AddRange(GetObseravableFields(typeInfo));

                // instance vars
                props.AddRange(GetObservableProps(typeInfo));

                // events
                events.AddRange(GetObservableEvents(typeInfo));

                // methods
                methods.AddRange(GetObservableMethods(typeInfo));

                // check if we need to regenerate the code

                // we check for double the count since every observable has 2 properties
                // 1 getter and setter that trigger event
                // 1 setter that doesn't notify

                if (fields.Count * 3 != props.Count)
                {
                    return true;
                }

                if (fields.Count != events.Count)
                {
                    return true;
                }

                if (fields.Count != methods.Count)
                {
                    return true;
                }


                foreach (FieldInfo field in fields)
                {
                    string propertyName = FieldFormatedName(field);
                    string eventName = $"On{propertyName}Changed";
                    string triggerName = $"Trigger{propertyName}Changed";

                    // search for observer with valid name
                    if (!props.Has(p => p.Name == propertyName))
                    {
                        mustRegenerate = true;
                        break;
                    }

                    if (!events.Has(p => p.Name == eventName))
                    {
                        mustRegenerate = true;
                        break;
                    }

                    if (!methods.Has(p => p.Name == triggerName))
                    {
                        mustRegenerate = true;
                        break;
                    }
                }
            }

            // if dont have to regenerate , go to next field
            return mustRegenerate;
        }

        public void InjectGeneratedCode(BISDInfoContainer typeInfo)
        {
            using (ListPool<FieldInfo>.Get(out List<FieldInfo> fields))
            {
                fields.AddRange(GetObseravableFields(typeInfo));

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
                string oldScript = typeInfo.InstancePartial.TextAsset.text;
                string propTemplate = AssetDatabase.LoadAssetAtPath<TextAsset>(STATE_PROPERTY_TEMPALTE).text;

                TemplateCodeBuilder scriptBuilder = new TemplateCodeBuilder(oldScript);

                // write properties
                {
                    ITextSection propertiesScriptSection = scriptBuilder
                        .CreateSection(new StartEndTextSection(PROPS_START_CONST, PROPS_END_CONST));
                    propertiesScriptSection
                        .AddWriter(new ReplaceAllTextWriter(string.Empty))
                        .AddWriter(new AppendTextWriter(Environment.NewLine));

                    for (int i = 0; i < fields.Count; i++)
                    {
                        FieldInfo field = fields[i];
                        TemplateCodeBuilder propertiesBuilder = new TemplateCodeBuilder(propTemplate);

                        propertiesBuilder
                            .CreateSection(new EntireTextSection())
                            .AddWriter(new ReplaceTermTextWriter("[INSTANCE_TYPE]", typeInfo.InstanceMain.TypeRef.Name))
                            .AddWriter(new ReplaceTermTextWriter("[FIELD_NICE_NAME]", FieldFormatedName(field)))
                            .AddWriter(new ReplaceTermTextWriter("[FIELD_TYPE]", TypeUtils.GetNiceName(field.FieldType)))
                            .AddWriter(new ReplaceTermTextWriter("[FIELD]", field.Name));

                        string propertyCode = propertiesBuilder.Build();

                        propertiesScriptSection.AddWriter(new AppendTextWriter(propertyCode));
                    }

                    propertiesScriptSection
                        .AddWriter(new AppendTextWriter(Environment.NewLine))
                        .AddWriter(new AppendTextWriter("\t\t"));
                }

                // write namespaces
                {
                    ITextSection namespacesScriptSection = scriptBuilder
                        .CreateSection(new StartEndTextSection(NAMESPACE_START_CONST, NAMESPACE_END_CONST));

                    namespacesScriptSection
                        .AddWriter(new ReplaceAllTextWriter(string.Empty))
                        .AddWriter(new AppendTextWriter(Environment.NewLine));

                    using (HashSetPool<string>.Get(out HashSet<string> namespaceAdded))
                    {

                        for (int i = 0; i < fields.Count; i++)
                        {
                            FieldInfo field = fields[i];

                            string namespaceAsString = field.FieldType.Namespace;

                            if (string.IsNullOrEmpty(namespaceAsString))
                                continue;

                            if (filterForNamespaces.Contains(namespaceAsString))
                                continue;

                            if (!namespaceAdded.Add(field.FieldType.Namespace))
                                continue;

                            namespacesScriptSection.AddWriter(new AppendTextWriter($"using {field.FieldType.Namespace};\n"));
                        }
                    }
                }

                string newScriptText = scriptBuilder.Build();

                // save
                File.WriteAllText(AssetDatabase.GetAssetPath(typeInfo.InstancePartial.TextAsset), newScriptText);

                // set dirty
                EditorUtility.SetDirty(typeInfo.InstancePartial.TextAsset);

                //
                Debug.Log($"model type affected [{typeInfo.ModelName}]");
            }

        }

        private static IEnumerable<MethodInfo> GetObservableMethods(BISDInfoContainer typeInfo)
        {
            //observers
            return typeInfo.InstanceMain.TypeRef
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.GetCustomAttribute<ObservableAttribute>() != null);
        }

        private static IEnumerable<PropertyInfo> GetObservableProps(BISDInfoContainer typeInfo)
        {
            //observers
            return typeInfo.InstanceMain.TypeRef
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.GetCustomAttribute<ObservableAttribute>() != null);
        }

        private static IEnumerable<EventInfo> GetObservableEvents(BISDInfoContainer typeInfo)
        {
            //observers
            return typeInfo.InstanceMain.TypeRef
                .GetEvents(BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.GetCustomAttribute<ObservableAttribute>() != null);
        }

        private static IEnumerable<FieldInfo> GetObseravableFields(BISDInfoContainer TypeList)
        {
            // fields
            return TypeList.State.TypeRef
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(f => !f.Name.Equals("data"))
                .Where(f => !f.Name.Equals("id"))
                .Where(f => f.GetCustomAttribute<ObservableAttribute>() != null);
        }

        private string FieldFormatedName(FieldInfo field)
        {
            sb.Clear();
            sb.Append(field.Name.Replace("_", ""));
            sb[0] = Char.ToUpper(sb[0]);
            return sb.ToString();
        }
    }
}
