using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace Bloodthirst.BJson.Tests
{
    public class BJson_Caching_Tests
    {
        internal class TwoInstances
        {
            public SomeClass First { get; set; }
            public SomeClass Second { get; set; }
        }

        internal class TwoLists
        {
            public List<SomeClass> First { get; set; }
            public List<SomeClass> Second { get; set; }
        }

        internal class TwoDictionaries
        {
            public Dictionary<int, SomeClass> First { get; set; }
            public Dictionary<int, SomeClass> Second { get; set; }
        }

        [Test]
        public void CachingInstance()
        {
            SomeClass instance = new SomeClass();

            TwoInstances original = new TwoInstances()
            {
                First = instance,
                Second = instance
            };

            string json = BJsonConverter.ToJson(original);

            TwoInstances copy = BJsonConverter.FromJson<TwoInstances>(json);

            Assert.AreNotEqual(original, copy);
            Assert.AreSame(original.First, original.Second);
            Assert.AreSame(copy.First, copy.Second);
        }

        [Test]
        public void CachingList()
        {
            SomeClass instance = new SomeClass();

            List<SomeClass> lst = new List<SomeClass>() { instance };

            TwoLists original = new TwoLists()
            {
                First = lst,
                Second = lst
            };

            string json = BJsonConverter.ToJson(original);

            TwoLists copy = BJsonConverter.FromJson<TwoLists>(json);

            Assert.AreNotEqual(original, copy);
            Assert.AreSame(original.First, original.Second);
            Assert.AreSame(copy.First, copy.Second);
        }

        [Test]
        public void CachingDictionaries()
        {
            SomeClass instance = new SomeClass();

            Dictionary<int, SomeClass> dict = new Dictionary<int, SomeClass>()
            {
                {1, instance}
            };

            TwoDictionaries original = new TwoDictionaries()
            {
                First = dict,
                Second = dict
            };

            BJsonSettings settings = new BJsonSettings() { Formated = true };

            string json = BJsonConverter.ToJson(original , settings);

            TwoDictionaries copy = BJsonConverter.FromJson<TwoDictionaries>(json);

            Assert.AreNotEqual(original, copy);
            Assert.AreSame(original.First, original.Second);
            Assert.AreSame(copy.First, copy.Second);
        }
    }
}
