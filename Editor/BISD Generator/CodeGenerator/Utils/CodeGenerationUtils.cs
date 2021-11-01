using Bloodthirst.Core.BISDSystem;
using Bloodthirst.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Bloodthirst.Core.BISD.CodeGeneration
{
    public static class CodeGenerationUtils
    {

        public static void GenerateGameDataInfoFromStateField(MemberInfo mem, out string name, out Type type)
        {
            Type memTyp = ReflectionUtils.GetMemberType(mem);

            List<Type> interfaces = memTyp
                .GetInterfaces()
                .Where(i => i.IsGenericType)
                .Where(i =>
                {
                    Type t = i.GetGenericTypeDefinition();
                    return
                    t == typeof(IList<>) ||
                    t == typeof(ISet<>);
                })
                .Select(i =>
                {
                    Type genArg = i.GetGenericArguments()[0];
                    Type[] types = genArg.GetInterfaces();
                    Type isInstance = types.Contains(typeof(IEntityInstance)) ? genArg : null;

                    return isInstance;
                })
                .Where(i => i != null)
                .ToList();

            // many entity ref
            if (interfaces.Count != 0)
            {
                type = typeof(List<int>);
                name = $"{ mem.Name }_Ids";
                return;
            }

            // single entity ref
            else if (TypeUtils.IsSubTypeOf(ReflectionUtils.GetMemberType(mem), typeof(IEntityInstance)))
            {
                type = typeof(int);
                name = $"{ mem.Name }_Id";
                return;
            }

            // other data
            type = memTyp;
            name = $"{ mem.Name }_Value";
        }
    } 
}
