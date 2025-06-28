using System;
using System.Collections;
using System.Linq;
using Bloodthirst.Core.Utils;
using NUnit.Framework;
using UnityEngine;

public class GeneralCSharpTests
{
    class EventTestClass
    {
        public event Action MyEvent;
    
        public Action AsAction()
        {
            return MyEvent;
        }
    }

    class NullTestClass : MonoBehaviour
    {
        public int num;
    }

    [Test]
    public void EventSubsrciptionTests()
    {
        EventTestClass instance = new EventTestClass();

        Action callback = () => { Debug.Log("I got called"); };
        Action callback2 = () => { Debug.Log("I got called 2"); };

        Action initialEvt = instance.AsAction();

        // initial empty event check
        {
            Action evt = instance.AsAction();
            Assert.IsTrue(evt == null);
        }


        // subscrible once
        {
            instance.MyEvent += callback;
            Action evt = instance.AsAction();

            Delegate[] delegates = evt.GetInvocationList();
            Assert.IsTrue(delegates.Length == 1);
        }

        // check duplicate
        {
            Action evt = instance.AsAction();
            Assert.IsFalse(GameObjectUtils.HasDuplicateSubscriptions(evt));
        }

        // notice how the old "initialEvt" stay null even after we subscribe
        {
            Assert.IsTrue(initialEvt == null);
        }

        // subscrible for a second time
        {
            instance.MyEvent += callback;
            Action evt = instance.AsAction();

            Delegate[] delegates = evt.GetInvocationList();
            Assert.IsTrue(delegates.Length == 2);
        }

        // check duplicate
        {
            Action evt = instance.AsAction();
            Assert.IsTrue(GameObjectUtils.HasDuplicateSubscriptions(evt));
        }

        // unsubscrible once
        {
            instance.MyEvent -= callback;
            Action evt = instance.AsAction();

            Delegate[] delegates = evt.GetInvocationList();
            Assert.IsTrue(delegates.Length == 1);
        }

        // empty event check
        {
            instance.MyEvent -= callback;
            Action evt = instance.AsAction();

            Assert.IsTrue(evt == null);
        }
    }

    [Test]
    public void NullConditionalCheckTests()
    {
        // null-operator test
        // same results for "?." and "??" operators
        {
            GameObject go = new GameObject();

            NullTestClass destroyedCmp = go.AddComponent<NullTestClass>();
            NullTestClass aliveCmp = go.AddComponent<NullTestClass>();
            UnityEngine.Object.DestroyImmediate(destroyedCmp);

            NullTestClass result = destroyedCmp ?? aliveCmp;

            Assert.IsTrue(result != aliveCmp);

            UnityEngine.Object.DestroyImmediate(go);
        }
    }


    [Test]
    public void NullCheckTests()
    {
        GameObject go = new GameObject();
        object castedBeforeDestroy = go;
        UnityEngine.Object.DestroyImmediate(go);
        object castedAfterDestroy = go;

        // go
        {
            Assert.IsTrue(go == null);
            Assert.IsFalse(go is null);
            Assert.IsFalse(object.ReferenceEquals(go, null));
            Assert.IsTrue(go.Equals(null));
            Assert.IsFalse(object.Equals(go, null));
        }

        // castedBeforeDestroy
        {
            Assert.IsFalse(castedBeforeDestroy == null);
            Assert.IsFalse(castedBeforeDestroy is null);
            Assert.IsFalse(object.ReferenceEquals(castedBeforeDestroy, null));
            Assert.IsTrue(castedBeforeDestroy.Equals(null));
            Assert.IsFalse(object.Equals(castedBeforeDestroy, null));
        }

        // castedAfterDestroy
        {
            Assert.IsFalse(castedAfterDestroy == null);
            Assert.IsFalse(castedAfterDestroy is null);
            Assert.IsFalse(object.ReferenceEquals(castedAfterDestroy, null));
            Assert.IsTrue(castedAfterDestroy.Equals(null));
            Assert.IsFalse(object.Equals(castedAfterDestroy, null));
        }
    }
}
