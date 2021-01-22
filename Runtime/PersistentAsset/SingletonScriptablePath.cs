using System;

namespace Bloodthirst.Core.PersistantAsset
{
    /// <summary>
    /// 
    /// <para>Custom attribute to define the path of the singleton asset</para>
    /// 
    /// <para> NOTE : "Asset/Resources" is automatically added to the path so start directly with the folder name</para>
    /// 
    /// <para>Example : if the path is "Singleton" then the asset will be at "Asset/Resources/Singleton/[TYPENAME].asset"</para>
    /// 
    /// </summary>

    public class SingletonScriptablePath : Attribute
    {
        public string Path { get; private set; }

        public SingletonScriptablePath(string Path)
        {
            this.Path = Path;
        }
    }
}
