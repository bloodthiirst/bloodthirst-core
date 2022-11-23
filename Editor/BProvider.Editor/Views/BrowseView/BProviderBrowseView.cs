using Bloodthirst.Editor;
using Bloodthirst.Editor.CustomComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace Bloodthirst.Core.BProvider.Editor
{
    public class BProviderBrowseView : VisualElement
    {
        internal enum BrowseType
        {
            ClassSingletons,
            ClassInstances,

            InterfaceSingletons,
            InterfaceInstances,
        }
        public new class UxmlFactory : UxmlFactory<BProviderBrowseView, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlEnumAttributeDescription<BrowseType> browseType = new UxmlEnumAttributeDescription<BrowseType> { name = "browse-type", defaultValue = BrowseType.ClassInstances };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                BProviderBrowseView providerBrowseView = ve as BProviderBrowseView;

                BrowseType val = browseType.GetValueFromBag(bag, cc);

                providerBrowseView.Type = val;
            }
        }

        private const string UXML_PATH = BProviderEditor.FOLDER_PATH + "Views/BrowseView/BProviderBrowseView.uxml";
        private const string USS_PATH = BProviderEditor.FOLDER_PATH + "Views/BrowseView/BProviderBrowseView.uss";

        public BProviderEditor Editor { get; set; }

        internal BrowseType Type { get; set; }
        internal SearchableList SearchableList => this.Q<SearchableList>(nameof(SearchableList));

        public BProviderBrowseView()
        {
            // import USS
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);

            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
            visualTree.CloneTree(this);

            
            styleSheets.Add(styleSheet);
            if (!styleSheets.Contains(EditorConsts.GlobalStyleSheet))
            {
                styleSheets.Add(EditorConsts.GlobalStyleSheet);
            }
        }

        public void Setup(BProviderEditor editor)
        {
            Editor = editor;
            BProviderAsset asset = (BProviderAsset)editor.ProviderAsset.CurrentValue.Value;

            List<object> allObjs = new List<object>();

            switch (Type)
            {
                case BrowseType.ClassSingletons:
                    {
                        allObjs.AddRange(asset.bProvider.GetSingletons<object>());
                        break;
                    }
                case BrowseType.ClassInstances:
                    {
                        allObjs.AddRange(asset.bProvider.GetInstances<object>());
                        break;
                    }
                case BrowseType.InterfaceSingletons:
                    {
                        allObjs.AddRange(asset.bProvider.GetSingletons<object>());
                        break;
                    }
                case BrowseType.InterfaceInstances:
                    {

                        allObjs.AddRange(asset.bProvider.GetInstances<object>());
                        break;
                    }
            }

            SearchableList.Setup(allObjs, FilterCondition, MakeElement, BindElement, null);
        }

        private void BindElement(VisualElement visualElement, int index)
        {
            BProviderInstanceElement casted = (BProviderInstanceElement)visualElement;

            IndexWrapper actualVal = SearchableList.CurrentValues[index];

            casted.Setup(Editor, actualVal);
        }
        private VisualElement MakeElement()
        {
            return new BProviderInstanceElement();
        }
        private bool FilterCondition(IndexWrapper val, string searchTerm)
        {
            return true;
        }

        internal void Refresh()
        {
            SearchableList.RefreshResults();
        }
    }
}
