using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace TechnoLogica.RegiX.Common.Utils
{
    /// <summary>
    /// Extension methods for String
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// Reads the string and returns the deserialized object
        /// </summary>
        /// <typeparam name="T">The type of the object to be deserialized</typeparam>
        /// <param name="serializedObject">The string to be deserialized</param>
        /// <returns>The deserialized object</returns>
        public static T Deserialize<T>(this string serializedObject)
            where T : class
        {
            return serializedObject.Deserialize(typeof(T)) as T;
        }

        /// <summary>
        /// Reads the string and returns the deserialized object
        /// </summary>
        /// <param name="serializedObject">The string to be deserialized</param>
        /// <param name="type">The type of the object to be deserialized</param>
        /// <returns>The deserialized object</returns>
        public static object Deserialize(this string serializedObject, Type type)
        {
            if (string.IsNullOrEmpty(serializedObject))
            {
                return null;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractSerializer serializer = new DataContractSerializer(type);
                Byte[] bytes = Encoding.UTF8.GetBytes(serializedObject);
                ms.Write(bytes, 0, bytes.Length);
                ms.Seek(0, SeekOrigin.Begin);
                return serializer.ReadObject(ms);
            }
        }

        /// <summary>
        /// Deserializes the provided string to the desired object
        /// </summary>
        /// <param name="serializedObject">Serialized object</param>
        /// <param name="type">Type of the serialized object</param>
        /// <returns>Deserialzed object</returns>
        public static object XmlDeserialize(this string serializedObject, Type type)
        {
            if (string.IsNullOrEmpty(serializedObject))
            {
                return null;
            }
            using (StringReader sr = new StringReader(serializedObject))
            {
                XmlSerializer serializer = new XmlSerializer(type);
                return serializer.Deserialize(sr);
            }
        }

        /// <summary>
        /// Reads the string and returns the deserialized object
        /// </summary>
        /// <typeparam name="T">The type of the object to be deserialized</typeparam>
        /// <param name="serializedObject">The string to be deserialized</param>
        /// <returns>The deserialized object</returns>
        public static T XmlDeserialize<T>(this string serializedObject)
            where T : class
        {
            return serializedObject.XmlDeserialize(typeof(T)) as T;
        }

        /// <summary>
        /// Reports the index of the nth occurrence of the specified Unicode character in this string.
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="value">Searched character</param>
        /// <param name="n">Number of occurance</param>
        /// <returns>Found index</returns>
        public static int NthIndexOf(this string input, char value, int n)
        {
            if (value == '\0')//default of char is '\0'
            {
                throw new ArgumentNullException();
            }

            if (n < 1)
            {
                throw new ArgumentOutOfRangeException();
            }

            char valueChar = Convert.ToChar(value);
            int position = -1;
            int positionCounter = 0;

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == valueChar)
                {
                    positionCounter++;
                }

                if (positionCounter == n)
                {
                    position = i;
                    break;
                }
            }

            return position;
        }

        /// <summary>
        /// Creates XMLElement from the provided string input
        /// </summary>
        /// <param name="input">Input to be converted to XMLElement</param>
        /// <returns>The resultant XMLElement</returns>
        public static XmlElement ToXmlElement(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }
            else
            {
                XmlDocument document = new XmlDocument();
                document.LoadXml(input);
                return document.DocumentElement;
            }
        }
    }
}
