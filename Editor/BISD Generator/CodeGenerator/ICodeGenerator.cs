using System;
using UnityEngine;

namespace Bloodthirst.Core.BISD.CodeGeneration
{
    public interface ICodeGenerator
    {
        bool ShouldInject(Container<Type> TypeList, Container<TextAsset> TextList);
        void InjectGeneratedCode(Container<Type> TypeList, Container<TextAsset> TextList);
    }
}
