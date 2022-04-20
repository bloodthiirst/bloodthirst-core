using System;
using UnityEngine;

namespace Bloodthirst.Core.BISD.CodeGeneration
{
    public interface ICodeGenerator
    {
        bool ShouldInject(BISDInfoContainer container);
        void InjectGeneratedCode(BISDInfoContainer container);
    }
}
