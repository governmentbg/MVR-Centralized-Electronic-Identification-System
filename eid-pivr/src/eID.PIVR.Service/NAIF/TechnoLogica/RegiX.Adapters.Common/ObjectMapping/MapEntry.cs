using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using FastMember;

namespace TechnoLogica.RegiX.Adapters.Common.ObjectMapping
{
    /// <summary>
    /// Class representing the mapping between a source and a target object
    /// </summary>
    public class MapEntry
    {
        /// <summary>
        /// Name of target property
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Name of a property describing the presence of data in other property (for the example  PropertyA the name of the describing property should be PropertyASpecified)
        /// </summary>
        public string SpecifiedPropertyName { get; set; }

        /// <summary>
        /// Property type
        /// </summary>
        public Type PropertyType { get; set; }

        /// <summary>
        /// Dictionary describing nested objects
        /// </summary>
        public Dictionary<string, MapEntry> ChildProperties { get; set; }

        /// <summary>
        /// Source data object
        /// </summary>
        public object DataSource { get; set; }

        /// <summary>
        /// Type of the source data object
        /// </summary>
        public MapEntrySourceType SourceType { get; set; }

        /// <summary>
        /// Object for quick access to the properties
        /// </summary>
        public TypeAccessor TypeAccessor { get; private set; }

        /// <summary>
        /// Type of the main object (for example if the object is of type string[], the main object is of type string
        /// </summary>
        public Type NonCollectionType { get; private set; }

        /// <summary>
        /// TypeAccessor for creating objects of the main object type
        /// </summary>
        public TypeAccessor TypeCreator { get; private set; }

        /// <summary>
        /// Converting function
        /// </summary>
        public Func<object, object> ConverterFunction { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type of the object for the MapEntry</param>
        public MapEntry(Type type)
        {
            TypeAccessor = TypeAccessor.Create(type);
            NonCollectionType = GetNonCollectionType(type);
            ChildProperties = new Dictionary<string, MapEntry>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type of the object for the MapEntry</param>
        /// <param name="typeAccessor">TypeAccessor for quick property access</param>
        public MapEntry(Type type, TypeAccessor typeAccessor)
        {
            TypeAccessor = typeAccessor;
            NonCollectionType = GetNonCollectionType(type);
            ChildProperties = new Dictionary<string, MapEntry>();
        }

        /// <summary>
        /// Extracts the value of a property accessed through the TypeAccessor for the given object
        /// </summary>
        /// <param name="obj">Object to retrieved the property's value from </param>
        /// <returns>Retrieved property value</returns>
        public object Get(Object obj)
        {
            return TypeAccessor[obj, PropertyName];
        }

        /// <summary>
        /// Sets value of the given object
        /// </summary>
        /// <param name="obj">Object we are assigning value to (throught tye TypeAccessor)</param>
        /// <param name="value">Value to assign</param>
        internal void Set(object obj, object value)
        {
            if (value != System.DBNull.Value && value != null)
            {
                if (ConverterFunction == null)
                {
                    TypeAccessor[obj, PropertyName] = ConvertToType(value, PropertyType);
                }
                else
                {
                    var convertedValue = ConverterFunction.Invoke(value);
                    if (convertedValue != System.DBNull.Value && convertedValue != null)
                    {
                        TypeAccessor[obj, PropertyName] = ConvertToType(convertedValue, PropertyType);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(SpecifiedPropertyName))
                        {
                            TypeAccessor[obj, SpecifiedPropertyName] = false;
                            return;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(SpecifiedPropertyName))
                {
                    TypeAccessor[obj, SpecifiedPropertyName] = true;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(SpecifiedPropertyName))
                {
                    TypeAccessor[obj, SpecifiedPropertyName] = false;
                }
            }
        }

        /// <summary>
        /// Converts the given value to the desired type
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="targetType">Target type</param>
        /// <returns>Converted value</returns>
        private static object ConvertToType(object value, Type targetType)
        {

            if (value != null && value.GetType() == typeof(TimeSpan))
            {
                return Convert.ToString(value);
            }
            if (value != null && value.GetType() == typeof(double) && targetType == typeof(string))
            {
                return Convert.ToString(value);
            }
            if (value != null && typeof(IFormattable).IsAssignableFrom(targetType))
            {
                // Определяне на NumberDecimalSeparator
                double test;
                NumberFormatInfo format = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
                if (Double.TryParse(value.ToString(), NumberStyles.Number,
                                    new NumberFormatInfo { NumberDecimalSeparator = "," }, out test))
                {
                    format.NumberDecimalSeparator = ",";
                }

                if (targetType == typeof(UInt32))
                {
                    return Convert.ToUInt32(value);
                }
                if (targetType == typeof(Int32))
                {
                    return Convert.ToInt32(value);
                }
                if (targetType == typeof(Single))
                {
                    return Convert.ToSingle(value, format);
                }
                if (targetType == typeof(Double))
                {
                    return Convert.ToDouble(value, format);
                }
                if (targetType == typeof(Decimal))
                {
                    return Convert.ToDecimal(value, format);
                }
            }
            if (targetType.IsEnum)
            {
                return Enum.Parse(targetType, value.ToString(), true);
            }
            return value;
        }

        /// <summary>
        /// Retrieves the main object type for a given object
        /// </summary>
        /// <param name="type">Complex type</param>
        /// <returns>Retrieved main object</returns>
        public static Type GetNonCollectionType(Type type)
        {
            if (type.IsGenericType)
            {
                return type.GetGenericArguments()[0];
            }
            else if (type.IsArray)
            {
                return type.GetElementType();
            }
            else
            {
                return type;
            }
        }

        /// <summary>
        /// Creates and object using the TypeCreate
        /// </summary>
        /// <returns>The created object</returns>
        public object CreateNew()
        {
            if (TypeCreator == null)
            {
                TypeCreator = TypeAccessor.Create(NonCollectionType);
            }
            return TypeCreator.CreateNew();
        }
    }
}
