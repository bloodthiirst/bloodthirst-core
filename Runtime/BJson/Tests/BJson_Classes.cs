using System;

namespace Bloodthirst.BJson.Tests
{
    internal enum SomeEnum
    {
        First,
        Second,
        Third
    }

    [Flags]
    internal enum SomeFlags
    {
        None,
        One,
        Two,
        Three
    }
    
    internal class SomeClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SomeEnum SomeEnum { get; set; }
    }

    internal abstract class SomeBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    internal interface ISomeInterface
    {
        public int InterfaceProp { get; set; }
    }

    internal class SomeConcreteAbstract : SomeBase
    {
        public int Points { get; set; }
    }

    internal class SomeConcreteInterface : ISomeInterface
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int InterfaceProp { get; set; }
    }
}
