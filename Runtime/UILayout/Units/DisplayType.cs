using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bloodthirst.Core.UILayout
{
    public enum DisplayKeyword : int
    {
        INLINE,
        BLOCK,
        NONE
    }

    [Serializable]
    public struct DisplayType
    {
        [SerializeField]
        private DisplayKeyword displayKeyword;

        public DisplayKeyword DisplayKeyword => displayKeyword;
        public DisplayType(DisplayKeyword displayKeyword)
        {
            this.displayKeyword = displayKeyword;
        }
    }
}
