using Bloodthirst.Core.TreeList;
using System;

namespace Bloodthirst.System.CommandSystem
{
    public struct CommandNodeBuilder
    {
        private readonly TreeLeaf<int, CommandSettings> leaf;
        private readonly Func<int> getID;

        public CommandNodeBuilder(TreeLeaf<int, CommandSettings> leaf, Func<int> getID)
        {
            this.leaf = leaf;
            this.getID = getID;
        }

        public CommandNodeBuilder Append(ICommandBase command, bool interruptBatchOnFail = false)
        {
            return Append(new CommandSettings() { Command = command });
        }

        public CommandNodeBuilder Append(CommandSettings commandSettings)
        {
            TreeLeaf<int, CommandSettings> newLeaf = new TreeLeaf<int, CommandSettings>();
            newLeaf.LeafKey = getID();
            newLeaf.Value = commandSettings;

            leaf.AddSubLeaf(newLeaf);


            return new CommandNodeBuilder(newLeaf, getID);
        }
    }
}
