using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BrightstarDB.DynamicSystem.Tests
{
    [TestClass]
    public class SystemTests
    {
        private const string ConnectionString = "type=embedded;storesdirectory=c:\\brightstar";

        [TestMethod]
        public void TestCreateObjectSystem()
        {
            var storeId = Guid.NewGuid().ToString();
            var dos = new DynamicObjectSystem(ConnectionString, storeId);
            Assert.IsNotNull(dos);
        }

        [TestMethod]
        public void TestNoTypesExist()
        {
            var storeId = Guid.NewGuid().ToString();
            var dos = new DynamicObjectSystem(ConnectionString, storeId);
            var typeCount = dos.Types.Count();
            Assert.AreEqual(0, typeCount);
        }

        [TestMethod]
        public void TestAssertType()
        {
            var storeId = Guid.NewGuid().ToString();
            var dos = new DynamicObjectSystem(ConnectionString, storeId);
            var typeCount = dos.Types.Count();
            Assert.AreEqual(0, typeCount);

            dos.AssertType("Person", new string[] { "age", "name", "hometown" });

            typeCount = dos.Types.Count();
            Assert.AreEqual(1, typeCount);
        }

        [TestMethod]
        public void TestGetType()
        {
            var storeId = Guid.NewGuid().ToString();
            var dos = new DynamicObjectSystem(ConnectionString, storeId);
            var typeCount = dos.Types.Count();
            Assert.AreEqual(0, typeCount);
            dos.AssertType("Person", new string[] { "age", "name", "hometown" });
            var type = dos.GetType("Person");
        }

        [TestMethod]
        public void TestEnumerateTypeAllowedProperties()
        {
            var storeId = Guid.NewGuid().ToString();
            var dos = new DynamicObjectSystem(ConnectionString, storeId);
            var typeCount = dos.Types.Count();
            Assert.AreEqual(0, typeCount);
            dos.AssertType("Person", new string[] { "age", "name", "hometown" });
            var type = dos.GetType("Person");

            var properties = type.ds__properties as IEnumerable<object>;
            Assert.AreEqual(3, properties.ToList().Count());

            Assert.IsTrue(properties.Contains("age"));
            Assert.IsTrue(properties.Contains("name"));
            Assert.IsTrue(properties.Contains("hometown"));
        }

        [TestMethod]
        public void TestCreateUntypedDynamic()
        {
            var storeId = Guid.NewGuid().ToString();
            var dos = new DynamicObjectSystem(ConnectionString, storeId);
            var dyna1 = dos.CreateNewDynamicObject();
            dos.SaveChanges();
            Assert.IsNotNull(dyna1.Id);
        }

        [TestMethod]
        public void TestCreateTypedDynamic()
        {
            var storeId = Guid.NewGuid().ToString();
            var dos = new DynamicObjectSystem(ConnectionString, storeId);
            var person = dos.AssertType("Person", new string[] { "age", "name", "hometown" });
            var bob = dos.CreateNewDynamicObject("Person");
            Assert.AreEqual(person.Id.FirstOrDefault(), bob.ds__type.FirstOrDefault().Id.FirstOrDefault());
        }
    }
}
