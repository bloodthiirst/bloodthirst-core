using Bloodthirst.Core.Utils;
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BSearch
{
    public class BSearchResultPath : VisualElement
    {
        private const string UXML_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/BSearch/UI/Result/BSearchResultPath.uxml";
        private ResultPath resultPath;
        private int index;
        private int instanceID;

        private Label ValueIndex => this.Q<Label>(nameof(ValueIndex));
        private Label ValueName => this.Q<Label>(nameof(ValueName));
        private Label ValuePath => this.Q<Label>(nameof(ValuePath));
        private Label ValueType => this.Q<Label>(nameof(ValueType));
        private ObjectField ValuePicker => this.Q<ObjectField>(nameof(ValuePicker));

        public BSearchResultPath()
        {
            AddToClassList("shrink-0");
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH).CloneTree(this);
        }

        public void Setup(ResultPath resultPath, int index)
        {
            this.resultPath = resultPath;
            this.index = index;

            instanceID = -1;

            if (resultPath.Value is UnityEngine.Object unityObj)
            {
                instanceID = unityObj.GetInstanceID();
            }

            Refresh();
        }

        public void Refresh()
        {
            ValueIndex.text = index.ToString();
            ValueName.text = resultPath.ValueName;

            string path = resultPath.ValuePath.ToString();

            if (resultPath.ValuePath == FieldType.COLLECTION)
            {
                path += "[" + resultPath.Index + "]";
            }
            ValuePath.text = path;
            ValueType.text = TypeUtils.GetNiceName(resultPath.Value.GetType());
            ValuePicker.objectType = typeof(UnityEngine.Object);

            if (instanceID == -1)
            {
                ValuePicker.Display(false);
                return;
            }

            UnityEngine.Object find = EditorUtility.InstanceIDToObject(instanceID);

            if (find == null)
            {
                ValuePicker.Display(false);
            }
            else
            {
                ValuePicker.Display(true);
                ValuePicker.value = find;
            }
        }

    }
}