using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TextCore.Text;

namespace Bloodthirst.BJson.Tests
{
    public class BJson_Basic_Tests
    {
        [Test]
        public void EnumFlags()
        {
            SomeFlags flags = SomeFlags.One | SomeFlags.Two;

            string json = BJsonConverter.ToJson(flags);

            SomeFlags copy = BJsonConverter.FromJson<SomeFlags>(json);

            Assert.AreEqual(flags, copy);
        }
        
        [Test]
        public void TMPEnumFlags()
        {
            FontStyles flags = FontStyles.Normal;

            string json = BJsonConverter.ToJson(flags);

            FontStyles copy = BJsonConverter.FromJson<FontStyles>(json);

            Assert.AreEqual(flags, copy);
        }

        [Test]
        public void ConcreteInstance()
        {
            SomeClass concrete = new SomeClass();

            string json = BJsonConverter.ToJson(concrete);

            SomeClass copy = BJsonConverter.FromJson<SomeClass>(json);

            Assert.AreNotEqual(concrete, copy);
            Assert.AreEqual(concrete.Id, copy.Id);
            Assert.AreEqual(concrete.Name, copy.Name);
        }

        [Test]
        public void AbstractInstance()
        {
            SomeConcreteAbstract concrete = new SomeConcreteAbstract();

            string json = BJsonConverter.ToJson(concrete);

            SomeBase copy = BJsonConverter.FromJson<SomeConcreteAbstract>(json);

            Assert.AreNotEqual(concrete, copy);
            Assert.AreEqual(concrete.Id, copy.Id);
            Assert.AreEqual(concrete.Name, copy.Name);
        }

        [Test]
        public void InterfaceInstance()
        {
            ISomeInterface concrete = new SomeConcreteInterface();

            string json = BJsonConverter.ToJson(concrete);

            ISomeInterface copy = BJsonConverter.FromJson<ISomeInterface>(json);

            Assert.AreNotEqual(concrete, copy);
            Assert.AreEqual(concrete.InterfaceProp, copy.InterfaceProp);
        }

        [Test]
        public void ListConcrete()
        {
            SomeConcreteAbstract concrete = new SomeConcreteAbstract();

            List<SomeConcreteAbstract> lst = new List<SomeConcreteAbstract>();
            lst.Add(concrete);
            lst.Add(concrete);
            lst.Add(concrete);

            string json = BJsonConverter.ToJson(lst);

            List<SomeConcreteAbstract> copy = BJsonConverter.FromJson<List<SomeConcreteAbstract>>(json);

            Assert.AreNotEqual(concrete, copy);
            Assert.AreEqual(lst.Count, copy.Count);

            Assert.AreEqual(lst[0], lst[1]);
            Assert.AreEqual(lst[0], lst[2]);

            Assert.AreEqual(copy[0], copy[1]);
            Assert.AreEqual(copy[0], copy[2]);
        }

        [Test]
        public void ListInterface()
        {
            SomeConcreteAbstract concrete = new SomeConcreteAbstract();

            List<SomeConcreteAbstract> lst = new List<SomeConcreteAbstract>();
            lst.Add(concrete);
            lst.Add(concrete);
            lst.Add(concrete);

            string json = BJsonConverter.ToJson(lst);

            IList copy = BJsonConverter.FromJson<IList>(json);

            Assert.AreNotEqual(concrete, copy);
            Assert.AreEqual(lst.Count, copy.Count);

            Assert.AreEqual(lst[0], lst[1]);
            Assert.AreEqual(lst[0], lst[2]);

            Assert.AreEqual(copy[0], copy[1]);
            Assert.AreEqual(copy[0], copy[2]);
        }
    }
}
