using Bloodthirst.Core.Utils;
using Bloodthirst.Runtime.BNodeTree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bloodthirst.Editor.BNodeTree
{
    /// <summary>
    /// class responsible for providing the nodes to be created
    /// </summary>
    public static class NodeProvider
    {
        /// <summary>
        /// Contains the info needed for node creation though the context menu
        /// </summary>
        public struct FactoryRecord
        {
            /// <summary>
            /// Reference to the node type
            /// </summary>
            public Type NodeType { get; set; }

            /// <summary>
            /// lambda that returns a new instance of node of type <see cref="FactoryRecord.NodeType"/>
            /// </summary>
            public Func<INodeType> NodeCreator { get; set; }

            /// <summary>
            /// The path to follow in the context menu to create the node
            /// </summary>
            public string NodePath { get; set; }
        }

        /// <summary>
        /// Take the base node type as a parameter and returns all the node sub-types that user can create
        /// </summary>
        /// <param name="nodeBaseType"></param>
        /// <returns></returns>
        public static Dictionary<Type,FactoryRecord> GetNodeFactory(Type nodeBaseType)
        {

            Dictionary<Type, FactoryRecord> nodeFactory = new Dictionary<Type, FactoryRecord>();

            nodeFactory = TypeUtils.AllTypes
                .Where(t => !t.IsAbstract)
                .Where(t => !t.IsInterface)
                .Where(t => !t.IsGenericType)
                .Where(t => TypeUtils.IsSubTypeOf(t, nodeBaseType))
                .Select(t =>
                {
                    NodeMenuPathAttribute pathAttr = t.GetCustomAttributes(typeof(NodeMenuPathAttribute), false).FirstOrDefault() as NodeMenuPathAttribute;
                    NodeNameAttribute nameAttr = t.GetCustomAttributes(typeof(NodeNameAttribute), false).FirstOrDefault() as NodeNameAttribute;

                    string nodePath = pathAttr == null ? string.Empty : pathAttr.NodePath;
                    nodePath += "/";
                    nodePath += nameAttr == null ? t.Name : nameAttr.Name;

                    FactoryRecord rec = new FactoryRecord()
                    {
                        NodeType = t,
                        NodeCreator = () => (INodeType)Activator.CreateInstance(t),
                        NodePath = nodePath
                    };

                    return rec;
                })
                .ToDictionary((kv) => kv.NodeType);

            return nodeFactory;
        }
    }
}
