using System;
using UnityEngine;

namespace Bloodthirst.Core.BISD.CodeGeneration
{
    public interface ICodeGenerator
    {
        bool ShouldInject(Container container);
        void InjectGeneratedCode(Container container);
    }
}
