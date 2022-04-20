using System;

namespace Bloodthirst.Core.AdvancedPool
{
    /// <summary>
    /// Use this attribute on a behaviour class if you wish generate a pool of any prefab that has this behaviour on it 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class GeneratePool : Attribute
    {
    }
}