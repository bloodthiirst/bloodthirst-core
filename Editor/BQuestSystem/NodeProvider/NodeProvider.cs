﻿using Bloodthirst.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Bloodthirst.System.Quest.Editor
{
    public static class NodeProvider
    {
        public struct FactoryRecord
        {
            public Type NodeType { get; set; }
            public Func<INodeType> NodeCreator { get; set; }
            public string NodePath { get; set; }
        }

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
                    NodeMenuPathAttribute attr = t.GetCustomAttributes(typeof(NodeMenuPathAttribute), false).FirstOrDefault() as NodeMenuPathAttribute;

                    string nodePath = attr == null ? t.Name : attr.NodePath;

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

        private static INodeType InstanceCreator(Type t)
        {
            return (INodeType)Activator.CreateInstance(t);
        }
    }
}