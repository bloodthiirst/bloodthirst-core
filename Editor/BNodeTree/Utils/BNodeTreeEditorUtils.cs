using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.System.Quest.Editor
{
    internal static class BNodeTreeEditorUtils
    {
        public const string EDITOR_BASE_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/BNodeTree";

        /// <summary>
        /// Color returned when nothing is returned
        /// </summary>
        internal static Color nothingColor = Color.white;

        /// <summary>
        /// Color returned for enum types
        /// </summary>
        internal static Color customEnumColor = Color.blue;
        
        /// <summary>
        /// Color returned if the type doesn't exist in the known types list
        /// </summary>
        internal static Color notFoundColor = new Color(255, 0, 255);

        internal static Dictionary<Type, Color> typeToColor;

        internal static IReadOnlyDictionary<Type, Color> TypeToColor => typeToColor;

        public static Color GetColor(Type nodeReturn)
        {
            if(nodeReturn == null)
            {
                return nothingColor;
            }

            if(nodeReturn.IsEnum)
            {
                return customEnumColor;
            }

            if(typeToColor.TryGetValue(nodeReturn , out var col))
            {
                return col;
            }

            return nothingColor;
        }

        static BNodeTreeEditorUtils()
        {
            typeToColor = new Dictionary<Type, Color>()
            {
                { typeof(void)      ,   new Color(13, 28, 110)  },
                { typeof(float)     ,   new Color(73, 10, 112)  },
                { typeof(int)       ,   new Color(22, 54, 199)  },
                { typeof(bool)       ,   new Color(2, 172, 181)  },
                { typeof(Vector3)   ,   new Color(102, 199, 22)  },
                { typeof(Vector2)   ,   new Color(217, 230, 48) },
            };
        }

        public static IReadOnlyList<PortBaseElement> GetPortsUI(this NodeBaseElement nodeType, PORT_DIRECTION direction, PORT_TYPE type)
        {
            if (direction == PORT_DIRECTION.INPUT && type == PORT_TYPE.CONST)
            {
                return nodeType.InputsConst;
            }
            else if (direction == PORT_DIRECTION.INPUT && type == PORT_TYPE.VARIABLE)
            {
                return nodeType.InputsVariable;
            }
            else if (direction == PORT_DIRECTION.OUTPUT && type == PORT_TYPE.CONST)
            {
                return nodeType.OutputsConst;
            }

            return nodeType.OutputsVariable;
        }

    }
}
