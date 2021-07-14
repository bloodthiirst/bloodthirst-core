using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.System.Quest.Editor
{
    public class PortInfoBaseElement
    {
        private const string UXML_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/BQuestSystem/PortInfo/PortInfoBaseElement.uxml";
        private const string USS_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/BQuestSystem/PortInfo/PortInfoBaseElement.uss";

        private VisualElement PortInfoRoot { get; set; }
        public VisualElement VisualElement => PortInfoRoot;
        private VisualElement BorderActive => PortInfoRoot.Q<VisualElement>(nameof(BorderActive));
        private VisualElement BorderSelected => PortInfoRoot.Q<VisualElement>(nameof(BorderSelected));
        private VisualElement FieldsContainer => PortInfoRoot.Q<VisualElement>(nameof(FieldsContainer));
        private Label NoFieldsAvailable => PortInfoRoot.Q<Label>(nameof(NoFieldsAvailable));
        private Label PortName => PortInfoRoot.Q<Label>(nameof(PortName));

        public PortBaseElement PortBase { get; }
        public IPortType PortType { get; set; }

        public bool IsShowingInfo { get; private set; }

        private List<IBindableUI> BindableUIs { get; set; }

        public PortInfoBaseElement(PortBaseElement port, IPortType portType)
        {
            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);

            StyleSheet customUss = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);

            TemplateContainer templateContainer = visualTree.CloneTree();
            PortInfoRoot = templateContainer.Q<VisualElement>(nameof(PortInfoRoot));
            PortInfoRoot.styleSheets.Add(customUss);

            BorderActive.pickingMode = PickingMode.Ignore;
            BorderSelected.pickingMode = PickingMode.Ignore;

            // bindable
            BindableUIs = new List<IBindableUI>();

            // parent node ui
            PortBase = port;

            // port instance
            PortType = portType;

            PortName.text = PortType.PortName;

            if (ValidMembers().Count == 0)
            {
                NoFieldsAvailable.style.display = DisplayStyle.Flex;
                FieldsContainer.style.display = DisplayStyle.None;
            }
            else
            {
                NoFieldsAvailable.style.display = DisplayStyle.None;
                FieldsContainer.style.display = DisplayStyle.Flex;

                SetupFields();

                FixLabels();
            }

            PortInfoRoot.MarkDirtyRepaint();
        }

        private IEnumerable<MemberInfo> GetMembers()
        {
            foreach (PropertyInfo f in PortType.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                yield return f;
            }

            foreach (FieldInfo f in PortType.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                yield return f;
            }
        }

        public List<MemberInfo> ValidMembers()
        {
            IEnumerable<MemberInfo> allInterfaceMembers = PortType.GetType().GetInterfaces().SelectMany( i => i.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));

            IEnumerable<MemberInfo> members = GetMembers()

                .Where(m =>
                    {
                        List<MemberInfo> mem = allInterfaceMembers.Where(i => i.Name == m.Name).ToList();

                        if (mem.Count == 0)
                            return true;

                        return mem.FirstOrDefault(im => im != null && im.GetCustomAttribute<IgnoreBindableAttribute>() == null) != null;
                    })

                .Where(m => !m.Name.EndsWith("__BackingField"));

            List<MemberInfo> lst = members.ToList();

            return lst;
        }

        private void SetupFields()
        {
            List<MemberInfo> members = ValidMembers();


            foreach (MemberInfo mem in members)
            {
                IBindableUIFactory factory = BindableUIProvider.UIFactory.FirstOrDefault(f => f.CanBind(mem));

                if (factory == null)
                    continue;

                IBindableUI bindable = factory.CreateUI();

                BindableUIs.Add(bindable);

                bindable.Setup(PortType, mem);
                FieldsContainer.Add(bindable.VisualElement);

            }
        }


        private void FixLabels()
        {
            if (BindableUIs.Count == 0)
                return;

            // max label length
            int maxLabelLength = BindableUIs.Max(l => l.MemberInfo.Name.Length);



            // ratio picked
            float fontRatio = 70 / 10f;

            float labelWidth = fontRatio * maxLabelLength;

            foreach (IBindableUI ui in BindableUIs)
            {
                foreach (Label l in ui.VisualElement.Query<Label>().Build().ToList())
                {
                    l.style.width = new StyleLength(labelWidth);
                }
            }
        }


        public void AfterAddToCanvas()
        {
            PortInfoRoot.RegisterCallback<ClickEvent>(OnClick);
            PortInfoRoot.RegisterCallback<ContextClickEvent>(OnRightClick);
        }

        public void BeforeRemoveFromCanvas()
        {
            PortInfoRoot.UnregisterCallback<ClickEvent>(OnClick);
            PortInfoRoot.UnregisterCallback<ContextClickEvent>(OnRightClick);
        }

        private void OnRightClick(ContextClickEvent evt)
        {

        }

        private void OnClick(ClickEvent evt)
        {

        }
    }
}
