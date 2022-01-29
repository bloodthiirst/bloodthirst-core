using System.Collections.Generic;

namespace Bloodthirst.Editor.BInspector
{
    public struct DrawerContext
    {
        public List<IValueDrawer> AllDrawers { get; set; }
        public int IndentationLevel { get; set; }
    }
}
