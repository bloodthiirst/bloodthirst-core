using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.CustomComponent
{
    public class TabElement : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TabElement, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription m_TitleAttr = new UxmlStringAttributeDescription { name = "label", defaultValue = "Tab Title" };
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                TabElement tabElem = ve as TabElement;

                tabElem.Title = m_TitleAttr.GetValueFromBag(bag, cc);
            }
        }

        public string Title { get; set; }

        public int TabIndex { get; set; }

        public TabElement()
        {
            
        }
    }
}
