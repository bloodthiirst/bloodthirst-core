using System.Collections.Generic;

namespace Bloodthirst.Editor.BInspector
{
    public struct LayoutContext
    {
        public List<IValueDrawer> AllDrawers { get; set; }
        public int IndentationLevel { get; set; }
    }
}
