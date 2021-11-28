using Bloodthirst.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bloodthirst.Editor.BInspector
{
    internal static class TypeDataProvider
    {

        private static readonly Dictionary<Type, TypeData> _types = new Dictionary<Type,TypeData>();

        public static TypeData Get(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type passed in null");
            }

            if(_types.TryGetValue(type , out TypeData data))
            {
                return data;
            }

            data = new TypeData(type);
            _types.Add(type, data);

            return data;
        }
    }

}
