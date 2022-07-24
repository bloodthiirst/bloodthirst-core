using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.CustomComponent
{
    public class TabUI : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TabUI, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlChildElementDescription allowedChildren = new UxmlChildElementDescription(typeof(TabElement));
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get 
                {
                    yield return allowedChildren; 
                }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                TabUI tab = ve as TabUI;
                tab.CheckForTabs();
            }
        }

        private const string UXML_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/CustomComponent/TabUI/TabUI.uxml";
        private const string USS_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/CustomComponent/TabUI/TabUI.uss";

        public int CurrentTabIndex { get; private set; }

        public override VisualElement contentContainer => TabContainer;

        private List<TabButton> _AllButtons { get; set; } = new List<TabButton>();
        private List<TabContent> _AllContents { get; set; } = new List<TabContent>();
        private VisualElement TabContainer => this.Q<VisualElement>(nameof(TabContainer));
        private VisualElement ContentContainer => this.Q<VisualElement>(nameof(ContentContainer));

        public TabUI()
        {
            SetupElement();
            InitializeUI();
        }

        private void SetupElement()
        {
            // import USS
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);

            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
            visualTree.CloneTree(this);

            styleSheets.Add(styleSheet);
            styleSheets.Add(EditorConsts.GlobalStyleSheet);
        }

        public void Select(int tabIndex)
        {
            CurrentTabIndex = tabIndex;

            ContentContainer.Clear();

            TabContent newContent = _AllContents[tabIndex];

            ContentContainer.Add(newContent);

            for (int i = 0; i < _AllButtons.Count; i++)
            {
                TabButton curr = _AllButtons[i];

                curr.UnSelected();
            }

            _AllButtons[tabIndex].Select();
        }

        private void InitializeUI()
        {
            CheckForTabs();
            TabContainer.RegisterCallback<GeometryChangedEvent>(HandleContentChanged);
            TabContainer.RegisterCallback<AttachToPanelEvent>(HandleAttachedToPanel);
        }

        private void HandleAttachedToPanel(AttachToPanelEvent evt)
        {
            CheckForTabs();
        }

        private void HandleContentChanged(GeometryChangedEvent evt)
        {
            CheckForTabs();
        }

        public void CheckForTabs()
        {
            List<TabButton> newTabs = new List<TabButton>();
            List<TabContent> newContents = new List<TabContent>();
            int index = 0;

            int i = 0;

            // the goal of this loop is to check if we have any "TabElement" children that haven't been converted to a "TabButton"-"TabContent" pair
            // if we find any , we convert them into a TabButton and TabContent and we remove the TabElement since it's now "Setup" correctly
            while (i < contentContainer.childCount)
            {
                VisualElement curr = contentContainer[i];

                // if we found an "uncoverted" tab
                // we do the setup
                if (curr is TabElement t)
                {
                    // create button
                    TabButton newTab = new TabButton(this, t.Title, index);

                    // copy content
                    TabContent tabContent = new TabContent(this, index);

                    // we take the content form the TabElement and we move it to the TabContent
                    while(t.contentContainer.childCount != 0)
                    {
                        VisualElement tabC = t.contentContainer[0];

                        // NOTE : apprentyl the "Add" method already removes the element from the previous parent
                        // so no need to do something like : oldParent.Remove(child) ; newParent.Add(child)
                        // we just add directly
                        tabContent.Add(tabC);        
                    }

                    newTabs.Add(newTab);
                    newContents.Add(tabContent);

                    index++;

                    contentContainer.RemoveAt(i);
                }

                else
                {
                    i++;
                }


            }


            // we add all the buttons to the "button" header
            // and to a buttons collection 
            foreach (TabButton b in newTabs)
            {
                _AllButtons.Add(b);
                TabContainer.Add(b);
            }

            // add to a contents collection
            // then the THIS tabUI will decide which tab content to show
            foreach (TabContent c in newContents)
            {
                _AllContents.Add(c);
            }
        }
    }
}
