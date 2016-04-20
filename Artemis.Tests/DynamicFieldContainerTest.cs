/*  NEEDS TO BE REWRITTEN

using Artemis.Engine;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Artemis.Tests
{
    [TestClass]
    public class DynamicFieldContainerTest
    {

        [TestMethod]
        public void DynamicFields()
        {
            var container = new DynamicFieldContainer();

            container.Set<int>("A", 1);
            container.Set<int>("B", 15);
            container.Set<string>("Name", "Michael Chiklis");

            Assert.AreEqual(1, container.Get<int>("A"));
            Assert.AreEqual(15, container.Get<int>("B"));
            Assert.AreEqual("Michael Chiklis", container.Get<string>("Name"));
        }

        [TestMethod]
        public void DynamicFieldsWithGetters()
        {
            var container = new DynamicFieldContainer();
            var random = new Random().Next();

            container.SetGetter<int>("A", () => 5);
            container.SetGetter<int>("B", () => random + 8);

            Assert.AreEqual(5, container.Get<int>("A"));
            Assert.AreEqual(random + 8, container.Get<int>("B"));
        }

        [TestMethod]
        public void DynamicFieldsWithSetters()
        {
            var container = new DynamicFieldContainer();
            var list = new List<int>();

            container.SetSetter<int>("NextListItem", i => list.Add(i));
            container.Set<int>("NextListItem", 1);
            CollectionAssert.Contains(list, 1);
        }

        [TestMethod]
        public void DynamicFieldsWithGettersAndSetters()
        {
            var container = new DynamicFieldContainer();
            var list = new List<int>();
            container.SetGetterAndSetter<int>("A", () => list.LastOrDefault(), i => list.Add(i));

            Assert.AreEqual(0, container.Get<int>("A"));
            container.Set("A", 3);
            Assert.AreEqual(3, container.Get<int>("A"));
        }

        [TestMethod, ExpectedException(typeof(DynamicFieldException))]
        public void DynamicFieldWithMissingGetterThrowsException()
        {
            var container = new DynamicFieldContainer();
            var doesnt_exist = container.Get<int>("A");
        }

        [TestMethod, ExpectedException(typeof(DynamicFieldException))]
        public void DynamicFieldWithSetterAndNoGetterThrowsExceptionUponGet()
        {
            var container = new DynamicFieldContainer();
            container.SetSetter<int>("B", i => { }); // Setter does nothing.
            var doesnt_exist = container.Get<int>("B");
        }

        [TestMethod, ExpectedException(typeof(DynamicFieldException))]
        public void DynamicFieldWithGetterAndNoSetterThrowsExceptionUponSet()
        {
            var container = new DynamicFieldContainer();
            container.SetGetter<int>("C", () => 5);
            container.Set<int>("C", 7);
        }

        [HasDynamicProperties]
        private class Foo : ArtemisObject
        {
            [DynamicPropertyAttribute]
            public int A { get; set; }

            public Foo() : base() { }
        }

        [TestMethod]
        public void DynamicProperties()
        {
            var foo = new Foo();

            foo.A = 7;
            foo.Fields.Set<int>("A", 5);

            Assert.AreEqual(5, foo.A);
            Assert.AreEqual(5, foo.Fields.Get<int>("A"));
        }
    }
}
 * 
 */
