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
            Assert.IsNotNull(type);
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
        public void TestCreateTypedDynamicObject()
        {
            var storeId = Guid.NewGuid().ToString();
            var dos = new DynamicObjectSystem(ConnectionString, storeId);
            var person = dos.AssertType("Person", new string[] { "age", "name", "hometown" });
            var bob = dos.CreateNewDynamicObject("Person");

            // check that the id of the type of the instance is the same as the type we passed in.
            Assert.AreEqual(person.Identity, bob.ds__type[0].Identity);
        }

        [TestMethod]
        public void TestGetInstancesOfType()
        {
            var storeId = Guid.NewGuid().ToString();
            var dos = new DynamicObjectSystem(ConnectionString, storeId);
            var person = dos.AssertType("Person", new string[] { "age", "name", "hometown" });
            var bob = dos.CreateNewDynamicObject("Person");
            dos.SaveChanges();

            var instancesOfPerson = dos.GetInstancesOfType("Person");
            Assert.AreEqual(1, instancesOfPerson.Count());
        }

        [TestMethod]
        public void TestDeleteObject()
        {
            var storeId = Guid.NewGuid().ToString();
            var dos = new DynamicObjectSystem(ConnectionString, storeId);
            var person = dos.AssertType("Person", new string[] { "age", "name", "hometown" });
            var bob = dos.CreateNewDynamicObject("Person");
            dos.SaveChanges();

            var instancesOfPerson = dos.GetInstancesOfType("Person");
            Assert.AreEqual(1, instancesOfPerson.Count());

            // delete object
            dos.DeleteObject(bob.Identity);
            dos.SaveChanges();

            // check bob is gone
            instancesOfPerson = dos.GetInstancesOfType("Person");
            Assert.AreEqual(0, instancesOfPerson.Count());
        }

        [TestMethod]
        public void TestFindByQuery()
        {
            var storeId = Guid.NewGuid().ToString();
            var dos = new DynamicObjectSystem(ConnectionString, storeId);
            var person = dos.AssertType("Person", new string[] { "age", "name", "hometown" });
            var bob = dos.CreateNewDynamicObject("Person");
            bob.ds__name = "Bob";
            var bill = dos.CreateNewDynamicObject("Person");
            bill.ds__name = "Bill";
            dos.SaveChanges();

            var queryResult = dos.FindByQuery("select ?id where { ?id <" + DynamicObjectSystem.UriPrefix + "name> \"Bob\"^^<http://www.w3.org/2001/XMLSchema#string> }" );
            Assert.AreEqual(1, queryResult.Count());
        }

    }
}
