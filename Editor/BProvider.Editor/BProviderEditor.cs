#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using Bloodthirst.Editor;
using Bloodthirst.Editor.CustomComponent;
#endif

using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using Bloodthirst.Core.Utils;
using System.Collections.Generic;
using Bloodthirst.Core.GameEventSystem;
using System.Linq;

namespace Bloodthirst.Core.BProvider.Editor
{
    public class BProviderEditor : EditorWindow
    {
        public const string FOLDER_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/BProvider.Editor/";
        private const string UXML_PATH = FOLDER_PATH + "BProviderEditor.uxml";
        private const string USS_PATH = FOLDER_PATH + "BProviderEditor.uss";

        [MenuItem("Bloodthirst Tools/BProvider Editor")]
        public static void ShowExample()
        {
            BProviderEditor wnd = GetWindow<BProviderEditor>();
            wnd.titleContent = new GUIContent("BProvider Editor");
        }

        private VisualElement Root { get; set; }

        private EditorCoroutine loadCrtHandle;

        private VisualElement PlayModeContainer => Root.Q<VisualElement>(nameof(PlayModeContainer));
        private VisualElement PlayModeIcon => Root.Q<VisualElement>(nameof(PlayModeIcon));
        private Label PlayModeText => Root.Q<Label>(nameof(PlayModeText));
        private VisualElement MainDashboad => Root.Q<VisualElement>(nameof(MainDashboad));
        private VisualElement EditorProviderAssetContainer => Root.Q<VisualElement>(nameof(EditorProviderAssetContainer));
        internal SearchableDropdown ProviderAsset { get; set; }
        private Button CreateEditorProviderAssetBtn => Root.Q<Button>(nameof(CreateEditorProviderAssetBtn));
        private BProviderBrowseView ClassSingletons => Root.Q<BProviderBrowseView>(nameof(ClassSingletons));
        private BProviderBrowseView InterfaceSingletons => Root.Q<BProviderBrowseView>(nameof(InterfaceSingletons));
        private BProviderBrowseView ClassInstances => Root.Q<BProviderBrowseView>(nameof(ClassInstances));
        private BProviderBrowseView InterfaceInstances => Root.Q<BProviderBrowseView>(nameof(InterfaceInstances));
        private TabUI ProviderTabs => Root.Q<TabUI>(nameof(ProviderTabs));


        private List<IEditorTask<BProviderEditor>> PostLoadTasks { get; set; } = new List<IEditorTask<BProviderEditor>>();

        public void CreateGUI()
        {
            Root = rootVisualElement;

            LoadUI();

            loadCrtHandle = EditorCoroutineUtility.StartCoroutine(CrtPostLoad(), this);

            Initialize();

            ListenEvents();

        }

        private void Initialize()
        {
            List<BProviderAsset> allAssets = EditorUtils.FindAssets<BProviderAsset>();
            ProviderAsset = new SearchableDropdown("Select Datasource", -1, allAssets, this, MakeItem, BindItem, SearchTerm, SelectedAsString);
            EditorProviderAssetContainer.Add(ProviderAsset);

            ProviderTabs.Select(0);

            MainDashboad.Display(false);
            RefreshPlayMode();
        }

        private VisualElement MakeItem()
        {
            return new SearchableDropdownAssetElement();
        }
        private void BindItem(VisualElement element, int index)
        {
            SearchableDropdownAssetElement casted = (SearchableDropdownAssetElement)element;

            IndexWrapper wrapped = ProviderAsset.CurrentDropdown.SearchableList.CurrentValues[index];

            casted.Setup(wrapped);
        }
        private bool SearchTerm(IndexWrapper item, string searchTerm)
        {
            UnityEngine.Object casted = (UnityEngine.Object)item.Value;

            return casted.name.ToLowerInvariant().Contains(searchTerm.ToLowerInvariant());
        }
        private string SelectedAsString(object item)
        {
            UnityEngine.Object casted = (UnityEngine.Object)((IndexWrapper)item).Value;

            return casted.name;
        }

        private void ListenEvents()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= HandleBeforeReload;
            AssemblyReloadEvents.beforeAssemblyReload += HandleBeforeReload;

            AssemblyReloadEvents.afterAssemblyReload -= HandleAfterReload;
            AssemblyReloadEvents.afterAssemblyReload += HandleAfterReload;

            EditorApplication.playModeStateChanged -= HandlePlayModeChanged;
            EditorApplication.playModeStateChanged += HandlePlayModeChanged;

            EditorApplication.update -= HandleUpdate;
            EditorApplication.update += HandleUpdate;

            ProviderAsset.OnValueChanged -= HandleAssetChanged;
            ProviderAsset.OnValueChanged +=  HandleAssetChanged;

            CreateEditorProviderAssetBtn.clickable.clicked -= HandleCreateAssetClicked;
            CreateEditorProviderAssetBtn.clickable.clicked += HandleCreateAssetClicked;
        }

        private void HandleUpdate()
        {
            if (ClassInstances.SearchableList.SearchResult.itemsSource == null)
                return;

            List<BProviderInstanceElement> elems = ClassInstances.Query<BProviderInstanceElement>().Build().ToList();

            foreach(BProviderInstanceElement ui in elems)
            {
                ui.Tick();
            }
        }

        private void RefreshDashboad()
        {
            {
                BProviderBrowseView get = ClassSingletons;
                get.Setup(this);
            }
            {
                BProviderBrowseView get = InterfaceSingletons;
                get.Setup(this);
            }

            {
                BProviderBrowseView get = ClassInstances;
                get.Setup(this);
            }
            {
                BProviderBrowseView get = InterfaceInstances;
                get.Setup(this);
            }
        }

        private void HandleAssetChanged(IndexWrapper indexWrapper)
        {
            MainDashboad.Display(true);
            RefreshDashboad();
        }

        private void HandleCreateAssetClicked()
        {
            string path = EditorUtility.SaveFilePanel("Create Editor Asset", "Assets", "EditorProvider", "asset");

            if (string.IsNullOrEmpty(path))
                return;

            path = EditorUtils.AbsoluteToRelativePath(path);

            BProviderAsset asset = CreateInstance<BProviderAsset>();
            AssetDatabase.CreateAsset(asset, path);

            ProviderAsset.UpdateSource(EditorUtils.FindAssets<BProviderAsset>());

            IndexWrapper createdAsset = ProviderAsset.AllValues.FirstOrDefault(c =>  object.ReferenceEquals( c.Value , asset));

            ProviderAsset.SetCurrentValue(createdAsset.Index);
        }

        private void UnlistenEvents()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= HandleBeforeReload;
            AssemblyReloadEvents.afterAssemblyReload -= HandleAfterReload;
            EditorApplication.playModeStateChanged -= HandlePlayModeChanged;
            EditorApplication.update -= HandleUpdate;

            ProviderAsset.OnValueChanged -= HandleAssetChanged;
            CreateEditorProviderAssetBtn.clickable.clicked -= HandleCreateAssetClicked;

        }

        private void HandlePlayModeChanged(PlayModeStateChange obj)
        {
            RefreshPlayMode();
        }
        private void HandleBeforeReload()
        {

        }

        private void HandleAfterReload()
        {
            RefreshPlayMode();
        }
        private void RefreshPlayMode()
        {
            PlayModeContainer.RemoveFromClassList("green-bg");
            PlayModeContainer.RemoveFromClassList("red-bg");

            PlayModeIcon.RemoveFromClassList("play-btn");
            PlayModeIcon.RemoveFromClassList("pause-btn");

            PlayModeText.text = "Loading ...";

            if (EditorApplication.isPlaying)
            {
                PlayModeContainer.AddToClassList("red-bg");
                PlayModeIcon.AddToClassList("pause-btn");
                PlayModeText.text = "Can't use in play mode";
            }
            else
            {
                PlayModeContainer.AddToClassList("green-bg");
                PlayModeIcon.AddToClassList("play-btn");
                PlayModeText.text = "Usable in editor mode";
            }
        }



        private void LoadUI()
        {
            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);

            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);

            visualTree.CloneTree(Root);

            Root.styleSheets.Add(styleSheet);
            if (!Root.styleSheets.Contains(EditorConsts.GlobalStyleSheet))
            {
                Root.styleSheets.Add(EditorConsts.GlobalStyleSheet);
            }
        }

        public void AddPostLoadTask(IEditorTask<BProviderEditor> editorTask)
        {
            PostLoadTasks.Add(editorTask);
        }

        private IEnumerator CrtPostLoad()
        {
            // wait until layout it built
            yield return new WaitUntil(Root.IsLayoutBuilt);

            // execute post load tasks
            for (int i = PostLoadTasks.Count - 1; i >= 0; i--)
            {
                IEditorTask<BProviderEditor> t = PostLoadTasks[i];

                t.Execute(this);

                PostLoadTasks.RemoveAt(i);
            }

            loadCrtHandle = null;
        }

        private void OnDestroy()
        {
            PostLoadTasks.Clear();
            UnlistenEvents();

            if (loadCrtHandle != null)
            {
                EditorCoroutineUtility.StopCoroutine(loadCrtHandle);
            }

            loadCrtHandle = null;
        }

    }
}