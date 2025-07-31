using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace TechnoLogica.RegiX.Common.Utils
{
    /// <summary>
    /// Extension methods for XmlElement
    /// </summary>
    public static class XmlElementExtensions
    {
        /// <summary>
        /// Deserializes from the provided XMLElement
        /// </summary>
        /// <param name="serializedObject">Serialized object</param>
        /// <param name="type">Type of the object</param>
        /// <returns>Deserialized object</returns>
        public static object Deserialize(this XmlElement serializedObject, Type type)
        {
            if (serializedObject == null)
            {
                return null;
            }
            using (StringReader sr = new StringReader(serializedObject.OuterXml))
            {
                XmlSerializer serializer = new XmlSerializer(type);
                return serializer.Deserialize(sr);
            }
        }

        /// <summary>
        /// Generic method for deserialization from XMLElement
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="serializedObject">Serialized object</param>
        /// <returns>Deserialized object</returns>
        public static T Deserialize<T>(this XmlElement serializedObject)
            where T : class
        {
            object result = serializedObject.Deserialize(typeof(T));
            return result as T;
        }
    }
}
