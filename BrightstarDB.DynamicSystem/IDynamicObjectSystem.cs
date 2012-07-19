using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrightstarDB.DynamicSystem
{
    interface IDynamicObjectSystem
    {
        /// <summary>
        /// Returns an enumeration of all the 'types' defined in the dynamic system.
        /// </summary>
        IEnumerable<dynamic> Types { get; }

        /// <summary>
        /// Either creates a new dynamic object to represent the specified type or updates the instanceProperties
        /// property of the existing type.
        /// </summary>
        /// <param name="name">The unique name of the type</param>
        /// <param name="instanceProperties">A list of strings that defined the allowed properties on an instance of this type</param>
        /// <returns></returns>
        dynamic AssertType(string name, IEnumerable<string> instanceProperties);

        /// <summary>
        /// Looks up the type by name and returns it. Returns null if the type does not exist.
        /// </summary>
        /// <param name="name">The name of the type</param>
        /// <returns>A dynamic object for the type or null.</returns>
        dynamic GetType(string name);

        /// <summary>
        /// Creates a new persistent and typed dynamic object. 
        /// Throws an exception if the named type does not exist.
        /// </summary>
        /// <returns>A new dynamic object of the specified type.</returns>
        dynamic CreateNewDynamicObject(string typeName);

        /// <summary>
        /// Deletes the object with the specified identity. 
        /// </summary>
        /// <param name="id">The identity of the object to delete</param>
        void DeleteObject(string id);

        /// <summary>
        /// Commits all changes. 
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Returns an enumeration of all dynamic instances of the specified type.
        /// </summary>
        /// <param name="typeName">The type name</param>
        /// <returns>Enumeration of all instances of the type.</returns>
        IEnumerable<dynamic> GetInstancesOfType(string typeName);

        /// <summary>
        /// Uses a SPARQL query to locate a set of objects.
        /// </summary>
        /// <param name="sparqlQuery">The sparql query to execute. Note that the query must have only a single result variable that is 
        /// the id of the object.</param>
        /// <returns>A collection of dynamic objects that meet the query.</returns>
        IEnumerable<dynamic> FindByQuery(string sparqlQuery);

    }
}
