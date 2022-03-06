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
        private GlobalObjectId objectId;

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

            objectId = default(GlobalObjectId);

            if (resultPath.Value is UnityEngine.Object unityObj)
            {
                objectId = GlobalObjectId.GetGlobalObjectIdSlow(unityObj); 
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

            if (objectId.Equals(default(GlobalObjectId)) )
            {
                ValuePicker.Display(false);
                return;
            }

            UnityEngine.Object find = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(objectId);

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