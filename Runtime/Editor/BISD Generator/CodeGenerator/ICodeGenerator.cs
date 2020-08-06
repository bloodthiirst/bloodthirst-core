using System;
using UnityEngine;

namespace Packages.com.bloodthirst.bloodthirst_core.Runtime.Editor.BISD_Generator.CodeGenerator
{
    public interface ICodeGenerator
    {
        bool ShouldInject(Container<Type> TypeList, Container<TextAsset> TextList);
        void InjectGeneratedCode(Container<Type> TypeList, Container<TextAsset> TextList);
    }
}
