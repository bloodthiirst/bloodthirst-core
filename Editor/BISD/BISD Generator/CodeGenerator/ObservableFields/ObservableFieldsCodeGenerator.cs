using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
using Bloodthirst.Editor.CodeGenerator;
#if ODIN_INSPECTOR
using Sirenix.Utilities;
#endif
using System;
using System.CodeDom.Compiler;
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

            // fields of state
            FieldInfo[] fields = GetObseravableFields(typeInfo);

            // instance vars
            PropertyInfo[] props = GetObservableProps(typeInfo);

            // events
            EventInfo[] events = GetObservableEvents(typeInfo);
            MethodInfo[] methods = GetObservableMethods(typeInfo);

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

        private static MethodInfo[] GetObservableMethods(BISDInfoContainer typeInfo)
        {
            //observers
            return typeInfo.InstanceMain.TypeRef
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.GetCustomAttribute<ObservableAttribute>() != null)
                .ToArray();
        }
        private static PropertyInfo[] GetObservableProps(BISDInfoContainer typeInfo)
        {
            //observers
            return typeInfo.InstanceMain.TypeRef
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.GetCustomAttribute<ObservableAttribute>() != null)
                .ToArray();
        }
        private static EventInfo[] GetObservableEvents(BISDInfoContainer typeInfo)
        {
            //observers
            return typeInfo.InstanceMain.TypeRef
                .GetEvents(BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.GetCustomAttribute<ObservableAttribute>() != null)
                .ToArray();
        }

        public void InjectGeneratedCode(BISDInfoContainer typeInfo)
        {
            FieldInfo[] fields = GetObseravableFields(typeInfo);

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

                foreach (FieldInfo field in fields)
                {
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
                    .AddWriter(new AppendTextWriter("\t"));
            }

            // write namespaces
            {
                ITextSection namespacesScriptSection = scriptBuilder
                    .CreateSection(new StartEndTextSection(NAMESPACE_START_CONST, NAMESPACE_END_CONST));
                namespacesScriptSection
                    .AddWriter(new ReplaceAllTextWriter(string.Empty))
                    .AddWriter(new AppendTextWriter(Environment.NewLine));

                HashSet<string> namespaceAdded = new HashSet<string>();

                foreach (FieldInfo field in fields)
                {
                    if (field.FieldType.Namespace == null)
                        continue;

                    if (filterForNamespaces.Contains(field.FieldType.Namespace))
                        continue;

                    if (!namespaceAdded.Add(field.FieldType.Namespace))
                        continue;

                    namespacesScriptSection.AddWriter(new AppendTextWriter($"using {field.FieldType.Namespace};\n"));
                }
            }

            oldScript = scriptBuilder.Build();

            // save
            File.WriteAllText(AssetDatabase.GetAssetPath(typeInfo.InstancePartial.TextAsset), oldScript);

            // set dirty
            EditorUtility.SetDirty(typeInfo.InstancePartial.TextAsset);

            //
            Debug.Log($"model type affected [{typeInfo.ModelName}]");
        }

        private static FieldInfo[] GetObseravableFields(BISDInfoContainer TypeList)
        {
            // fields
            return TypeList.State.TypeRef
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(f => !f.Name.Equals("data"))
                .Where(f => !f.Name.Equals("id"))
                .Where(f => f.GetCustomAttribute<ObservableAttribute>() != null)
                .ToArray();
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
