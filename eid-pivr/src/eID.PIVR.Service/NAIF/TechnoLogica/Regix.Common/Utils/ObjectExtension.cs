using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TechnoLogica.RegiX.Common.Utils
{
    /// <summary>
    /// Extension methods for Object
    /// </summary>
    public static class ObjectExtension
    {
        /// <summary>
        /// Serializes an object and returns string
        /// </summary>
        /// <param name="obj">The object that contains the data to be serialized</param>
        /// <returns>The serialized object</returns>
        public static string Serialize(this Object obj)
        {
            if (obj != null)
            {
                using (MemoryStream ms = new MemoryStream())
                using (StreamReader sr = new StreamReader(ms))
                {
                    DataContractSerializer serializer = new DataContractSerializer(obj.GetType());
                    serializer.WriteObject(ms, obj);
                    ms.Seek(0, SeekOrigin.Begin);
                    return sr.ReadToEnd();
                }
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Serializes an object to XML and returns string
        /// </summary>
        /// <param name="obj">The object that contains the data to be serialized</param>
        /// <returns>The serialized object</returns>
        public static string XmlSerialize(this Object obj)
        {
            if (obj != null)
            {
                using (MemoryStream ms = new MemoryStream())
                using (StreamReader sr = new StreamReader(ms))
                {
                    XmlSerializer serializer = new XmlSerializer(obj.GetType());
                    serializer.Serialize(ms, obj);
                    ms.Seek(0, SeekOrigin.Begin);
                    return sr.ReadToEnd();
                }
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Serializes an object to JSON and returns string
        /// </summary>
        /// <param name="obj">The object that contains the data to be serialized</param>
        /// <returns>The serialized object</returns>
        public static string JsonSerialize(this Object obj)
        {
            if (obj != null)
            {
                using (MemoryStream ms = new MemoryStream())
                using (StreamReader sr = new StreamReader(ms))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                    serializer.WriteObject(ms, obj);
                    ms.Position = 0;
                    return sr.ReadToEnd();
                }
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the value of the passed property (if Specified property exists and its value is false DBNull.Value is returned).
        /// Only first level properties are supported
        /// </summary>
        /// <typeparam name="TObject">The object's type</typeparam>
        /// <typeparam name="T">The property's type</typeparam>
        /// <param name="instance">Instance of an object</param>
        /// <param name="propAccessExpression">Expression to the desired property</param>
        /// <returns>The extracted value</returns>
        public static object GetValueOrNull<TObject, T>(this TObject instance, Expression<Func<TObject, T>> propAccessExpression, bool isFromDb = true)
        {
            string propertyName = (propAccessExpression.Body as MemberExpression).Member.Name;
            Type declaryingType = (propAccessExpression.Body as MemberExpression).Member.DeclaringType;
            PropertyInfo specifiedProperty = declaryingType.GetProperty(propertyName + "Specified");
            PropertyInfo property = declaryingType.GetProperty(propertyName);
            if (instance == null || (specifiedProperty != null && !(bool)specifiedProperty.GetValue(instance, null)))
            {
                return isFromDb ? System.DBNull.Value : null;
            }
            else
            {
                object result = property.GetValue(instance, null);
                if (result != null)
                {
                    return result;
                }
                else
                {
                    return isFromDb ? System.DBNull.Value : null;
                }
            }
        }
    }
}
