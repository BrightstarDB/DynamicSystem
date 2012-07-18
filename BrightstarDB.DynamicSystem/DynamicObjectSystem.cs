using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BrightstarDB.Client;
using BrightstarDB.Dynamic;

namespace BrightstarDB.DynamicSystem
{
    /// <summary>
    /// The dynamic object system allows for the creation and persistence of types and instances.
    /// Both types and instances are data driven.
    /// </summary>
    public class DynamicObjectSystem
    {
        private readonly string _connectionString;
        private readonly string _storeId;
        private readonly DynamicStore _dynamicStore;
        private const string UriPrefix = "http://www.brightstardb.com/dynamic/system/";

        public DynamicObjectSystem(string connectionString, string storeId)
        {
            _connectionString = connectionString;
            _storeId = storeId;
            var dataObjectContext = BrightstarService.GetDataObjectContext(_connectionString);
            var dynamicContext = new BrightstarDynamicContext(dataObjectContext);
            if (dynamicContext.DoesStoreExist(_storeId))
            {
                _dynamicStore = dynamicContext.OpenStore(_storeId, new Dictionary<string, string>() { { "ds", UriPrefix } });
            }
            else
            {
                _dynamicStore = dynamicContext.CreateStore(_storeId, new Dictionary<string, string>() { { "ds", UriPrefix } });
            }                
        }

        private const string TypesQuery = @"select ?id where { ?id <http://www.brightstardb.com/dynamic/system/type> ""type""^^<http://www.w3.org/2001/XMLSchema#string> }";
        private const string GetTypeByName = @"select ?id where {{ ?id <http://www.brightstardb.com/dynamic/system/name> ""{0}""^^<http://www.w3.org/2001/XMLSchema#string> . ?id <http://www.brightstardb.com/dynamic/system/type> ""type""^^<http://www.w3.org/2001/XMLSchema#string> }}";

        public IEnumerable<dynamic> Types
        {
            get { return _dynamicStore.BindObjectsWithSparql(TypesQuery); }
        }

        public dynamic AssertType(string name, IEnumerable<string> instanceProperties)
        {
            // check if one exists already
            var type = _dynamicStore.BindObjectsWithSparql(String.Format(GetTypeByName, name)).FirstOrDefault();
            if (type != null)
            {
                // update instance properties
                type.ds__properties = instanceProperties;
            } else
            {
                // create new type with instance properties
                type = _dynamicStore.MakeNewObject(UriPrefix);
                type.ds__properties = instanceProperties;
                type.ds__name = name;
                type.ds__type = "type";
            }
            _dynamicStore.SaveChanges();

            return type;
        }

        public dynamic GetType(string name)
        {
            return _dynamicStore.BindObjectsWithSparql(String.Format(GetTypeByName, name)).FirstOrDefault();           
        }

        public void DeleteObject(string id)
        {
            var dataObjectContext = BrightstarService.GetDataObjectContext(_connectionString + ";storename=" + _storeId);
            var store = dataObjectContext.OpenStore(_storeId);
            store.GetDataObject(id).Delete();
            store.SaveChanges();
        }

        /// <summary>
        /// Creates a new persistent but untyped dynamic object.
        /// </summary>
        /// <returns>A new dynamic object</returns>
        public dynamic CreateNewDynamicObject()
        {
            return _dynamicStore.MakeNewObject(UriPrefix);
        }

        /// <summary>
        /// Creates a new persistent and typed dynamic object.
        /// </summary>
        /// <returns>A new dynamic object</returns>
        public dynamic CreateNewDynamicObject(string typeName)
        {
            var t = GetType(typeName);
            if (t == null) throw new Exception("Type does not exist");

            var d = _dynamicStore.MakeNewObject(UriPrefix);
            d.ds__type = t;
            return d;
        }

        public void SaveChanges()
        {
            _dynamicStore.SaveChanges();
        }
    }
}
