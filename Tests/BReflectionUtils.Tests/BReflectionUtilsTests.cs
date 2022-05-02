using Bloodthirst.Core.Utils;
using NUnit.Framework;

public class BReflectionUtilsTests
{
    private enum SomeEnum
    {
        ONE,
        TWO,
        THREE
    }

    private class SomeClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Salary { get; set; }
    }

    private struct SomePureStruct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Salary { get; set; }
    }
    private struct SomeOtherPureStruct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Salary { get; set; }
    }

    private struct SomePureStructWithClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Salary { get; set; }
        public SomeClass SomeClass { get; set; }
    }
    
    private struct SomePureStructWithOtherPureStruct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Salary { get; set; }
        public SomeOtherPureStruct OtherPureStruct { get; set; }
    }


    [Test]
    public void PureValueTypeTests()
    {
        Assert.IsTrue   (TypeUtils.IsPureValueType(typeof(int)));
        Assert.IsTrue   (TypeUtils.IsPureValueType(typeof(float)));
        Assert.IsTrue   (TypeUtils.IsPureValueType(typeof(double)));
        Assert.IsTrue   (TypeUtils.IsPureValueType(typeof(string)));
        Assert.IsTrue   (TypeUtils.IsPureValueType(typeof(SomeEnum)));
        Assert.IsTrue   (TypeUtils.IsPureValueType(typeof(SomePureStruct)));
        Assert.IsTrue   (TypeUtils.IsPureValueType(typeof(SomePureStructWithOtherPureStruct)));

        Assert.IsFalse  (TypeUtils.IsPureValueType(typeof(SomeClass)));
        Assert.IsFalse  (TypeUtils.IsPureValueType(typeof(SomePureStructWithClass)));
    }
}
