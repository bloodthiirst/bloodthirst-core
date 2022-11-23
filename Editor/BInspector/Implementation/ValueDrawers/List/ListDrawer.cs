using System;
using System.Collections;
using UnityEditor;
using UnityEngine.UIElements;
using Bloodthirst.Core.Utils;

namespace Bloodthirst.Editor.BInspector
{
    public class ListDrawer : ValueDrawerBase
    {
        private struct AddElementData
        {
            public IValueDrawer Value { get; set; }
            public IDictionary Dictionary { get; set; }
        }

        private const string LIST_PATH_UXML = "Packages/com.bloodthirst.bloodthirst-core/Editor/BInspector/Implementation/ValueDrawers/List/ListDrawer.uxml";
        private const string PATH_USS = "Packages/com.bloodthirst.bloodthirst-core/Editor/BInspector/Implementation/ValueDrawers/List/ListDrawer.uss";

        private static VisualTreeAsset uxmlAsset => AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(LIST_PATH_UXML);
        private static StyleSheet ussAsset => AssetDatabase.LoadAssetAtPath<StyleSheet>(PATH_USS);
        public Type ElementType { get; private set; }

        public Button AddElementBtn { get; private set; }

        private TypeSelector TypeSelector { get; set; }
        private VisualElement UIHeader { get; set; }
        private VisualElement UIContainer { get; set; }
        private VisualElement UIList { get; set; }
        public VisualElement AddElementContainer { get; private set; }
        public IValueDrawer AddElementDrawer { get; private set; }
        public VisualElement ElementsContainer { get; private set; }
        private LayoutContext cachedLayoutContext;

        public override object DefaultValue()
        {
            return null;
        }

        public override void Tick()
        {
            // todo ; make the tick work correctly then port it to the dictionary
            // check for added/remove elements
            IList currentList = (IList)ValueProvider.Get();

            if (currentList == null)
                return;

            bool countChanged = currentList.Count != SubDrawers.Count;

            // if the list reference changed
            // or count changed
            // redraw everything
            if (currentList != DrawerValue || countChanged)
            {
                DrawerValue = currentList;

                Clean();
                Draw(cachedLayoutContext);
                return;
            }

            for (int i = SubDrawers.Count - 1; i >= 0; i--)
            {
                IValueDrawer s = SubDrawers[i];

                // if same reference
                if (s.ValueProvider.Get() == currentList[i])
                {
                    s.Tick();
                }
                else
                {
                    // clean up
                    SubDrawers.Remove(s);
                    ElementsContainer.Remove(s.DrawerContainer);
                    s.Destroy();

                    // redraw
                    GenerateElementAtIndex(i);
                }

            }

        }

        protected override void GenerateDrawer(LayoutContext layoutContext)
        {
            UIHeader = new VisualElement();
            UIHeader.AddToClassList("row");
            UIHeader.AddToClassList("grow-1");
            UIHeader.Add(new VisualElement() { name = ValueDrawerBase.VALUE_LABEL_CONTAINER_CLASS });

            // load dictionary ui
            UIContainer = new VisualElement();
            UIContainer.AddToClassList("column");
            UIContainer.AddToClassList("grow-1");
            UIContainer.AddToClassList("shrink-1");

            DrawerContainer.AddToClassList("column");
            DrawerContainer.Add(UIHeader);
            DrawerContainer.Add(UIContainer);

            cachedLayoutContext = layoutContext;

            // get the dictionary types
            Type[] typeArgs = ReflectionUtils.GetMemberType(ValueProvider.MemberInfo).GenericTypeArguments;

            ElementType = typeArgs[0];

            // init uis
            Draw(layoutContext);
        }

        private void InstanceCreator()
        {
            Type type = ReflectionUtils.GetMemberType(ValueProvider.MemberInfo);

            TypeSelector = new TypeSelector(type);
            TypeSelector.AddToClassList("grow-1");

            Type currentType = ValueProvider.Get() == null ? null : ValueProvider.Get().GetType();
            TypeSelector.SetValueWithoutNotify(currentType);

            TypeSelector.UnregisterValueChangedCallback(HandleValueTypeChanged);
            TypeSelector.RegisterValueChangedCallback(HandleValueTypeChanged);

            UIHeader.Add(TypeSelector);
        }

        private void HandleValueTypeChanged(ChangeEvent<Type> evt)
        {
            Type newType = evt.newValue;

            object newInstance = null;

            if (newType != null)
            {
                newInstance = Activator.CreateInstance(newType);
            }

            DrawerValue = evt.newValue;
            ValueProvider.Set(newInstance);
            TriggerOnValueChangedEvent();

            Clean();
            Draw(cachedLayoutContext);
        }

        protected override void PostLayout() { }

        private void AddElementSection()
        {
            // setup add element
            AddElementDrawer = ValueDrawerProvider.Get(ElementType);

            // value setup
            ValueProviderGeneric elementInfo = GetAddValueDrawerInfo();


            ValueDrawerUtils.DoLayout(AddElementDrawer, this, elementInfo);


            AddElementContainer.Add(AddElementDrawer.DrawerContainer);

            // add btn
            AddElementBtn.RegisterCallback<ClickEvent>(HandleAddElementClick);
        }

        private ValueProviderGeneric GetAddValueDrawerInfo()
        {
            return new ValueProviderGeneric(
                            // state
                            null,

                            new ValuePath() { PathType = PathType.CUSTOM },

                            // getter
                            () =>
                            {
                                return AddElementDrawer.DrawerValue;
                            },
                            // setter
                            (newValue) =>
                            {
                                AddElementDrawer.DrawerValue = newValue;
                            },
                            // type
                            ElementType,

                            // index
                            null,

                            null
                            );
        }

        private void Clean()
        {
            if (UIList != null)
            {
                AddElementBtn.UnregisterCallback<ClickEvent>(HandleAddElementClick);

                UIContainer.Remove(UIList);
                UIList = null;
            }

            if (TypeSelector != null)
            {
                TypeSelector.UnregisterValueChangedCallback(HandleValueTypeChanged);
                UIHeader.Remove(TypeSelector);
                TypeSelector = null;
            }


            for (int i = SubDrawers.Count - 1; i >= 0; i--)
            {
                IValueDrawer s = SubDrawers[i];

                // clean up
                SubDrawers.RemoveAt(i);
                ElementsContainer.Remove(s.DrawerContainer);
                s.Destroy();
            }

            AddElementContainer = null;
            AddElementBtn = null;
            ElementsContainer = null;
        }

        private void Draw(LayoutContext layoutContext)
        {
            InstanceCreator();

            if (ValueProvider.Get() != null)
            {
                GenerateSubElements(layoutContext);
            }
        }

        private void GenerateSubElements(LayoutContext layoutContext)
        {
            UIList = uxmlAsset.CloneTree();
            UIList.styleSheets.Add(ussAsset);

            UIContainer.Add(UIList);

            AddElementContainer = UIList.Q<VisualElement>(nameof(AddElementContainer));
            AddElementBtn = UIList.Q<Button>(nameof(AddElementBtn));

            ElementsContainer = UIList.Q<VisualElement>(nameof(ElementsContainer));

            IList list = (IList)ValueProvider.Get();

            ElementsContainer.Clear();

            AddElementSection();

            for (int i = 0; i < list.Count; i++)
            {
                GenerateElementAtIndex(i);
            }
        }

        private void GenerateElementAtIndex(int i)
        {
            ListElementDrawer elem = new ListElementDrawer();

            ValueDrawerUtils.DoLayout(elem, this, GetValueDrawerInfo(i));

            SubDrawers.Insert(i, elem);

            ElementsContainer.Insert(i, elem.DrawerContainer);
        }

        private ValueProviderGeneric GetValueDrawerInfo(int index)
        {
            IList list = (IList)ValueProvider.Get();

            return new ValueProviderGeneric(
                                // index
                                index,

                                 new ValuePath() { PathType = PathType.LIST_ENTRY, ListIndex = index },

                                // getter
                                () =>
                                {
                                    return list[index];
                                },
                                // setter
                                (newValue) =>
                                {
                                    list[index] = newValue;

                                },
                                // type
                                ElementType,

                                // parent
                                list,

                                null
                                );
        }
        private void HandleAddElementClick(ClickEvent evt)
        {
            IList list = (IList)ValueProvider.Get();

            list.Add(AddElementDrawer.ValueProvider.Get());

            Clean();
            Draw(cachedLayoutContext);
        }

        public override void Destroy()
        {
            Clean();
        }
    }
}