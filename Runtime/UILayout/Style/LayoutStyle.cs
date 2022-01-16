using System;
using UnityEngine;

namespace Bloodthirst.Core.UILayout
{
    [Serializable]
    public class LayoutStyle
    {
        [SerializeField]
        private DisplayType displayType = new DisplayType(DisplayKeyword.INLINE);

        [SerializeField]
        private LengthUnit width;

        [SerializeField]
        private LengthUnit height;

        [SerializeField]
        private LengthUnit paddingLeft;

        [SerializeField]
        private LengthUnit paddingTop;

        [SerializeField]
        private LengthUnit paddingRight;

        [SerializeField]
        private LengthUnit paddingBottom;

        [SerializeField]
        private LengthUnit marginLeft;

        [SerializeField]
        private LengthUnit marginTop;

        [SerializeField]
        private LengthUnit marginRight;

        [SerializeField]
        private LengthUnit marginBottom;

        public DisplayType DisplayType { get => displayType; set => displayType = value; }
        public LengthUnit Width { get => width; set => width = value; }
        public LengthUnit Height { get => height; set => height = value; }
        public LengthUnit PaddingLeft { get => paddingLeft; set => paddingLeft = value; }
        public LengthUnit PaddingTop { get => paddingTop; set => paddingTop = value; }
        public LengthUnit PaddingRight { get => paddingRight; set => paddingRight = value; }
        public LengthUnit PaddingBottom { get => paddingBottom; set => paddingBottom = value; }
        public LengthUnit MarginLeft { get => marginLeft; set => marginLeft = value; }
        public LengthUnit MarginTop { get => marginTop; set => marginTop = value; }
        public LengthUnit MarginRight { get => marginRight; set => marginRight = value; }
        public LengthUnit MarginBottom { get => marginBottom; set => marginBottom = value; }
    }
}