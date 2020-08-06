using Assets.Scripts.Game;
using Packages.com.bloodthirst.bloodthirst_core.Runtime.Utils;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using static Packages.com.bloodthirst.bloodthirst_core.Runtime.Utils.StringExtensions;

namespace Packages.com.bloodthirst.bloodthirst_core.Runtime.Editor.BISD_Generator.CodeGenerator
{
    public class ObservableFieldsCodeGenerator : ICodeGenerator
    {
        private const string STATE_PROPERTY_TEMPALTE = "Packages/com.bloodthirst.bloodthirst-core/Runtime/Editor/BISD Generator/CodeGenerator/Template.StateProperty.cs.txt";

        private const string START_CONST = @"//OBSERVABLES_START";
        private const string END_CONT = @"//OBSERVABLES_END";

        public bool ShouldInject(Container<Type> TypeList, Container<TextAsset> TextList)
        {
            /*
            bool mustRegenerate = false;

            // fields
            FieldInfo[] fields = TypeList.State
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(f => !f.Name.Equals("data"))
                .Where(f => !f.Name.Equals("id"))
                .ToArray();

            //observers
            FieldInfo[] observers = TypeList.Instance
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.FieldType.GetInterfaces().Contains(typeof(IObservable)))
                .ToArray();

            // check if we need to regenerate the code

            if (fields.Length != observers.Length)
            {
                mustRegenerate = true;
            }

            else
            {
                foreach (FieldInfo field in fields)
                {
                    string observerName = FieldToObserverName(field);

                    // search for observer with valid name
                    FieldInfo obs = observers.FirstOrDefault(o => o.Name.Equals(observerName));

                    if (obs == null)
                    {
                        mustRegenerate = true;
                        break;
                    }

                    // check if it has the same type
                    if (obs.FieldType != field.FieldType)
                    {
                        mustRegenerate = true;
                        break;
                    }
                }
            }

            // if dont have to regenerate , go to next type

            return mustRegenerate;
            */
            return true;
        }

        public void InjectGeneratedCode(Container<Type> TypeList, Container<TextAsset> TextList)
        {
            // fields
            FieldInfo[] fields = TypeList.State
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(f => !f.Name.Equals("data"))
                .Where(f => !f.Name.Equals("id"))
                .ToArray();

            // do the generation

            // separate the parts

            string oldScript = TextList.Instance.text;

            List<Tuple<SECTION_EDGE, int, int>> sections = oldScript.StringReplaceSection(START_CONST, END_CONT);

            int padding = 0;

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

            for (int i = 0; i < sections.Count - 1; i++)
            {
                Tuple<SECTION_EDGE, int, int> start = sections[i];
                Tuple<SECTION_EDGE, int, int> end = sections[i + 1];

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

            // save
            File.WriteAllText(AssetDatabase.GetAssetPath(TextList.Instance), oldScript);

            // set dirty
            EditorUtility.SetDirty(TextList.Instance);
        }

        private static string FieldFormatedName(FieldInfo field)
        {
            StringBuilder sb = new StringBuilder(field.GetNiceName());
            sb[0] = Char.ToUpper(sb[0]);
            return sb.ToString();
        }
    }
}
