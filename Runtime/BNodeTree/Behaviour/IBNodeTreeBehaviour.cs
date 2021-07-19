using Bloodthirst.System.Quest.Editor;
using System;

namespace Bloodthirst.System.Quest
{
    /// <summary>
    /// Base class for all the behaviours that have treenode behaviours
    /// </summary>
    public interface IBNodeTreeBehaviour
    {
        NodeTreeData TreeData { get; }

        INodeType ActiveNode { get; }

        /// <summary>
        /// Triggered when the current node changes
        /// </summary>
        event Action<INodeType> OnActiveNodeChanged;
    }
}
