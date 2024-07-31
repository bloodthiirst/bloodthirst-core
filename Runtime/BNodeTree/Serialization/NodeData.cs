#if ODIN_INSPECTOR
using Sirenix.Serialization;
using Sirenix.OdinInspector;

#endif
using UnityEngine;

namespace Bloodthirst.Runtime.BNodeTree
{
    public class NodeData
    {
#if ODIN_INSPECTOR
        [Title("Canvas UI info")]
#endif
        [SerializeField]
        private Vector2 size;

        [SerializeField]
        private Vector2 position;

#if ODIN_INSPECTOR
        [Title("Node Data")]
        [ShowInInspector]
        [OdinSerialize]
#endif

        private INodeType nodeType;

        public Vector2 Size
        {
            get => size;
            set => size = value;
        }

        public Vector2 Position
        {
            get => position;
            set => position = value;
        }

        public INodeType NodeType
        {
            get => nodeType;
            set => nodeType = value;
        }
    }

    public class LinkData
    {
#if ODIN_INSPECTOR

        const string H_GROUP = "H_GROUP";
        const string V_FROM_GROUP = "H_GROUP/V_FROM_GROUP";
        const string V_TO_GROUP = "H_GROUP/V_TO_GROUP";
#endif

        #region from
#if ODIN_INSPECTOR
        [Title("From")]
        [HorizontalGroup(H_GROUP)]
        [VerticalGroup(V_FROM_GROUP)]
#endif
        [SerializeField]
        private int fromNodeIndex;

#if ODIN_INSPECTOR
        [HorizontalGroup(H_GROUP)]
        [VerticalGroup(V_FROM_GROUP)]
#endif
        [SerializeField]
        private int fromPortIndex;

#if ODIN_INSPECTOR
        [HorizontalGroup(H_GROUP)]
        [VerticalGroup(V_FROM_GROUP)]
#endif
        [SerializeField]
        private PORT_DIRECTION fromPortDirection;

#if ODIN_INSPECTOR
        [HorizontalGroup(H_GROUP)]
        [VerticalGroup(V_FROM_GROUP)]
#endif
        [SerializeField]
        private PORT_TYPE fromPortType;

        #endregion

        #region to
#if ODIN_INSPECTOR
        [Title("To")]
        [HorizontalGroup(H_GROUP)]
        [VerticalGroup(V_TO_GROUP)]
#endif
        [SerializeField]
        private int toNodeIndex;

#if ODIN_INSPECTOR
        [HorizontalGroup(H_GROUP)]
        [VerticalGroup(V_TO_GROUP)]
#endif
        [SerializeField]
        private int toPortIndex;

#if ODIN_INSPECTOR
        [HorizontalGroup(H_GROUP)]
        [VerticalGroup(V_TO_GROUP)]
#endif
        [SerializeField]
        private PORT_DIRECTION toPortDirection;

#if ODIN_INSPECTOR
        [HorizontalGroup(H_GROUP)]
        [VerticalGroup(V_TO_GROUP)]
#endif
        [SerializeField]
        private PORT_TYPE toPortType;

        #endregion

#if ODIN_INSPECTOR
        [Title("Link Data")]
#endif
        [SerializeField]
        private ILinkType linkType;

        #region accessors

        private ILinkType LinkType
        {
            get => linkType;
            set => linkType = value;
        }

        public int FromNodeIndex
        {
            get => fromNodeIndex;
            set => fromNodeIndex = value;
        }

        public int FromPortIndex
        {
            get => fromPortIndex;
            set => fromPortIndex = value;
        }

        public int ToNodeIndex
        {
            get => toNodeIndex;
            set => toNodeIndex = value;
        }

        public int ToPortIndex
        {
            get => toPortIndex;
            set => toPortIndex = value;
        }
        public PORT_DIRECTION FromPortDirection
        {
            get => fromPortDirection;
            set => fromPortDirection = value;
        }

        public PORT_TYPE FromPortType
        {
            get => fromPortType;
            set => fromPortType = value;
        }

        public PORT_DIRECTION ToPortDirection
        {
            get => toPortDirection;
            set => toPortDirection = value;
        }

        public PORT_TYPE ToPortType
        {
            get => toPortType;
            set => toPortType = value;
        }

        #endregion
    }
}
