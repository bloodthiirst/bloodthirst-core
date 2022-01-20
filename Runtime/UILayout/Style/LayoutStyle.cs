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
        private PositionType positionType = new PositionType(PositionKeyword.DISPLAY_MODE);

        [SerializeField]
        private SizeUnit width;

        [SerializeField]
        private SizeUnit height;

        [SerializeField]
        private PivotUnit pivot;

        public DisplayType DisplayType { get => displayType; set => displayType = value; }
        public PositionType PositionType { get => positionType; set => positionType = value; }
        public SizeUnit Width { get => width; set => width = value; }
        public SizeUnit Height { get => height; set => height = value; }
        public PivotUnit Pivot { get => pivot; set => pivot = value; }
    }
}