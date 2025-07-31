using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;

namespace TechnoLogica.RegiX.Common.ObjectMapping
{
    /// <summary>
    /// Describes the acessibility to properties of a given object (access matrix).
    /// Consists of hierarchical structure of named/bool values that enables/disable 
    /// the access to a specific element
    /// </summary>
    [Serializable]
    [XmlRoot("AccessMatrix", Namespace = "http://egov.bg/RegiX/ObjectMapping")]
    [XmlType(Namespace = "http://egov.bg/RegiX/ObjectMappingr")]
    public class AccessMatrix
    {
        /// <summary>
        /// Hierarchy root
        /// </summary>
        [XmlElement]
        public AMEntry AMEntry { get; set; }

        /// <summary>
        /// Loads the hierarchy from a dictionay describing the access to the elements
        /// The elements of the dicitonary consists of attribute names and boolean values. For 
        /// elements deeper in the hierarhcy a '.' separator is used to specify the levels.
        /// </summary>
        /// <param name="propertyAccess">Dictionary describing the access to an object</param>
        /// <returns>Created access matrix based on the dictionary</returns>
        public static AccessMatrix LoadFromDictionary(Dictionary<string, bool> propertyAccess)
        {
            AccessMatrix accessMatrix = new AccessMatrix();
            accessMatrix.AMEntry = AMEntry.LoadFromDictionary(propertyAccess);
            return accessMatrix;
        }

        /// <summary>
        /// Creates access matrix with access to all properties of a given type
        /// </summary>
        /// <param name="type">Type of the object to craete access matrix for</param>
        /// <returns>Created access matrix</returns>
        public static AccessMatrix CreateForType(Type type)
        {
            Dictionary<string, bool> propertyAccess = new Dictionary<string, bool>();
            CreatePropertyList(type.GetProperties(), "", propertyAccess);
            return LoadFromDictionary(propertyAccess);
        }

        /// <summary>
        /// Fills dictionary containing all characteristics for a given object. The method is called recusively for complex objects.
        /// </summary>
        /// <param name="properties">Properties of an object</param>
        /// <param name="path">Path to the properties</param>
        /// <param name="propertyAccess">Dictionary containing the result</param>
        private static void CreatePropertyList(PropertyInfo[] properties, string path, Dictionary<string, bool> propertyAccess)
        {
            foreach (PropertyInfo propertyInfo in properties)
            {
                if (!propertyInfo.Name.EndsWith("Specified"))
                {
                    string currentPath = string.IsNullOrEmpty(path) ? propertyInfo.Name : string.Format("{0}.{1}", path, propertyInfo.Name);
                    propertyAccess.Add(currentPath, true);
                    if (propertyInfo.PropertyType.Namespace != "System" &&
                        propertyInfo.PropertyType.Namespace != "System.Xml" &&
                        !(propertyInfo.PropertyType.IsArray &&
                        propertyInfo.PropertyType.GetElementType().Namespace.StartsWith("System")) &&
                        !(propertyInfo.PropertyType.IsGenericType &&
                        propertyInfo.PropertyType.GetGenericArguments()[0].Namespace.StartsWith("System")))
                    {
                        Type childType = propertyInfo.PropertyType;
                        if (propertyInfo.PropertyType.IsGenericType)
                        {
                            childType = childType.GetGenericArguments()[0];
                        }
                        else if (propertyInfo.PropertyType.IsArray )
                        {
                            childType = childType.GetElementType();
                        }
                        CreatePropertyList(childType.GetProperties(), currentPath, propertyAccess);
                    }
                }
            }
        }
    }
}
