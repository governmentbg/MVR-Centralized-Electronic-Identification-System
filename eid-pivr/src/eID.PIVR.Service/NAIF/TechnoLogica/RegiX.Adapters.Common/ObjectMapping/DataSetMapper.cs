using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Linq.Expressions;
using TechnoLogica.RegiX.Common.Utils;
using TechnoLogica.RegiX.Common.ObjectMapping;

namespace TechnoLogica.RegiX.Adapters.Common.ObjectMapping
{
    /// <summary>
    /// Defines mapping from DataSet to an object
    /// </summary>
    /// <typeparam name="T">Type of the target object</typeparam>
    public class DataSetMapper<T> : Mapper<T>
        where T : class
    {
        /// <summary>
        /// Root function used for the initial retrieval of the source object
        /// </summary>
        private Func<DataSet, object> _objectInitializer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="accessMatrix">Access matrix</param>
        public DataSetMapper(AccessMatrix accessMatrix)
            : base(accessMatrix)
        {
        }

        /// <summary>
        /// Executes the mapping from the DataSet to the target object (object of type <typeparamref name="T"/>)
        /// </summary>
        /// <param name="source">DataSet source</param>
        /// <param name="destination">Target object</param>
        public override void Map(object source, T destination)
        {
            if (_objectInitializer != null)
            {
                source = _objectInitializer.Invoke(source as DataSet);
            }
            Map(source, destination, MapEntryRoot);
        }

        /// <summary>
        /// Adds initializing fucntion (used in case when the source is not DataSet but other, for example DataRow or DataTable)
        /// </summary>
        /// <param name="objectInitializer">Initializing function (retrieves the source object from a given DataSet)</param>
        public void AddDataSetObjectInitializer(Func<DataSet, object> objectInitializer)
        {
            _objectInitializer = objectInitializer;
        }
        
        /// <summary>
        /// Executes the actual mapping
        /// </summary>
        /// <param name="source">Source (DataSet, DataTable or DataRow</param>
        /// <param name="destination">Target object</param>
        /// <param name="mapEntry">Mapping context</param>
        private void Map(object source, object destination, MapEntry mapEntry)
        {
            if (mapEntry.ChildProperties != null && source != null)
            {
                foreach (KeyValuePair<string, MapEntry> keyValuePair in mapEntry.ChildProperties)
                {
                    MapEntry childMapEntry = keyValuePair.Value;
                    if (childMapEntry.DataSource != null &&
                        (childMapEntry.SourceType == MapEntrySourceType.DataRows ||
                        childMapEntry.SourceType == MapEntrySourceType.DataRowCollection)
                        )
                    {
                        IEnumerable<DataRow> dataRows = null;
                        if (childMapEntry.SourceType == MapEntrySourceType.DataRowCollection && source is DataSet)
                        {
                            dataRows = (childMapEntry.DataSource as Func<DataSet, DataRowCollection>).Invoke(source as DataSet).Cast<DataRow>();
                        }
                        else if (childMapEntry.SourceType == MapEntrySourceType.DataRowCollection)
                        {
                            dataRows = (childMapEntry.DataSource as Func<DataRow, DataRowCollection>).Invoke(source as DataRow).Cast<DataRow>();
                        }
                        else
                        {
                            dataRows = (childMapEntry.DataSource as Func<DataRow, IEnumerable<DataRow>>).Invoke(source as DataRow);
                        }

                        if (source is DataSet)
                        {
                            object list =
                                dataRows
                                   // .AsParallel() причинява разбъркване на данните
                                    .Select(
                                        (dr) =>
                                        {
                                            object o = childMapEntry.CreateNew();
                                            Map(dr, o, childMapEntry);
                                            return o;
                                        }
                                    ).
                                    ToStrongList(childMapEntry.NonCollectionType);
                            childMapEntry.Set(destination, list);
                        }
                        else
                        {
                            object list =
                                    dataRows
                                    .Select(
                                        (dr) =>
                                        {
                                            object o = childMapEntry.CreateNew();
                                            Map(dr, o, childMapEntry);
                                            return o;
                                        }
                                    ).
                                    ToStrongList(childMapEntry.NonCollectionType);
                            childMapEntry.Set(destination, list);
                        }
                    }
                    else if (childMapEntry.DataSource != null && 
                             childMapEntry.SourceType == MapEntrySourceType.DataColumn)
                    {
                        object columnValue = (source as DataRow)[childMapEntry.DataSource as string];
                        childMapEntry.Set(destination, columnValue);
                    }
                    else if (childMapEntry.DataSource != null &&
                        childMapEntry.SourceType == MapEntrySourceType.Constant)
                    {
                        childMapEntry.Set(destination, childMapEntry.DataSource);
                    }
                    else if (childMapEntry.DataSource != null &&
                        childMapEntry.SourceType == MapEntrySourceType.FunctionMap &&
                        source is DataRow)
                    {
                        dynamic function = childMapEntry.DataSource;
                        object result =
                            function
                                .Invoke(source as DataRow);
                        childMapEntry.Set(destination, result);
                    }
                    else
                    {
                        Map(source, childMapEntry.Get(destination), childMapEntry);
                    }
                }
            }
        }

        /// <summary>
        /// Adds mapping between DataSet's data and object's properties
        /// </summary>
        /// <typeparam name="R">Type of the target's object property</typeparam>
        /// <param name="destinationProperty">Object's property to map data to </param>
        /// <param name="objectsSource">Function retrieving data from the source DataSet</param>
        public void AddDataSetMap<R>(Expression<Func<T, R>> destinationProperty, Func<DataSet, DataRowCollection> objectsSource)
        {
            Stack<string> propertyChain = GetPropertyChain(destinationProperty);
            SetMapEntrySource(propertyChain, objectsSource);
        }

        /// <summary>
        /// Sets the value of the MapEntry related to he target object's property
        /// </summary>
        /// <param name="propertyChain">Path to the property</param>
        /// <param name="objectsSource">Function retrieving the source data</param>
        private void SetMapEntrySource(Stack<string> propertyChain, Func<DataSet, DataRowCollection> objectsSource)
        {
            MapEntry currentMapEntry = GetMapEntry(propertyChain);
            if (currentMapEntry != null)
            {
                currentMapEntry.DataSource = objectsSource;
                currentMapEntry.SourceType = MapEntrySourceType.DataRowCollection;
            }
        }

        /// <summary>
        /// Sets the value of the MapEntry related to he target object's property
        /// </summary>
        /// <param name="propertyChain">Path to the property</param>
        /// <param name="objectsSource">Function retrieving the source data</param>
        private void SetMapEntrySource(Stack<string> propertyChain, Func<DataRow, DataRowCollection> objectsSource)
        {
            MapEntry currentMapEntry = GetMapEntry(propertyChain);
            if (currentMapEntry != null)
            {
                currentMapEntry.DataSource = objectsSource;
                currentMapEntry.SourceType = MapEntrySourceType.DataRowCollection;
            }
        }

        /// <summary>
        /// Sets the value of the MapEntry related to he target object's property
        /// </summary>
        /// <param name="propertyChain">Path to the property</param>
        /// <param name="objectsSource">Function retrieving the source data</param>
        private void SetMapEntrySource(Stack<string> propertyChain, Func<DataRow, IEnumerable<DataRow>> objectsSource)
        {
            MapEntry currentMapEntry = GetMapEntry(propertyChain);
            if (currentMapEntry != null)
            {
                currentMapEntry.DataSource = objectsSource;
                currentMapEntry.SourceType = MapEntrySourceType.DataRows;
            }
        }

        /// <summary>
        /// Sets the value of the MapEntry related to he target object's property
        /// </summary>
        /// <typeparam name="R">Type of the target property</typeparam>
        /// <param name="destinationProperty">Expression pointing to the target's property</param>
        /// <param name="objectsSource">Function retrieving the source data</param>
        public void AddDataRowMap<R>(Expression<Func<T, R>> destinationProperty, Func<DataRow, IEnumerable<DataRow>> objectsSource)
        {
            Stack<string> propertyChain = GetPropertyChain(destinationProperty);
            SetMapEntrySource(propertyChain, objectsSource);
        }

        /// <summary>
        /// Sets the value of the MapEntry related to the property of the target object
        /// </summary>
        /// <typeparam name="R">Type of the property</typeparam>
        /// <param name="destinationProperty">Expression targeting the object's property</param>
        /// <param name="objectsSource">Function retrieving the source date</param>
        public void AddDataRowMap<R>(Expression<Func<T, R>> destinationProperty, Func<DataRow, DataRowCollection> objectsSource)
        {
            Stack<string> propertyChain = GetPropertyChain(destinationProperty);
            SetMapEntrySource(propertyChain, objectsSource);
        }

        /// <summary>
        /// Sets the value of the MapEntry related to the property of the target object
        /// </summary>
        /// <typeparam name="R">Type of the property</typeparam>
        /// <param name="destinationProperty">Expression targeting the object's property</param>
        /// <param name="objectsSource">Function retrieving the source date</param>
        public void AddFunctionMap<T, R>(Expression<Func<T, R>> destinationProperty, Func<DataRow, R> sourceProperty)
        {
            Stack<string> propertyChain = GetPropertyChain(destinationProperty);
            SetAMEntryFunctionSource<R>(propertyChain, sourceProperty);
        }

        /// <summary>
        /// Sets the value of the MapEntry related to the property of the target object
        /// </summary>
        /// <typeparam name="R">Type of the object retrieved from the DataRow</typeparam>
        /// <param name="propertyChain">Expression targeting the object's property</param>
        /// <param name="objectsSource">Function retrieving the source data</param>
        private void SetAMEntryFunctionSource<R>(Stack<string> propertyChain, Func<DataRow, R> sourceExpression)
        {
            MapEntry currentMapEntry = GetMapEntry(propertyChain);
            if (currentMapEntry != null)
            {
                currentMapEntry.DataSource = sourceExpression;
                currentMapEntry.SourceType = MapEntrySourceType.FunctionMap;
            }
        }

        /// <summary>
        /// Sets the value of the MapEntry related to the property of the target object
        /// </summary>
        /// <param name="propertyChain">Expression targeting the object's property</param>
        /// <param name="columnName">Column name</param>
        /// <param name="converterFunction">Function transforming the data retrieved from the row</param>
        private void SetAMEntrySource(Stack<string> propertyChain, string columnName, Func<object, object> converterFunction = null)
        {
            MapEntry currentMapEntry = GetMapEntry(propertyChain);
            if (currentMapEntry != null)
            {
                currentMapEntry.DataSource = columnName;
                currentMapEntry.SourceType = MapEntrySourceType.DataColumn;
                currentMapEntry.ConverterFunction = converterFunction;
            }
        }

        /// <summary>
        /// Sets mapping between target's property and value in source DataRow
        /// </summary>
        /// <typeparam name="R">Type of the object's property</typeparam>
        /// <param name="destinationProperty">Expression targeting the object's property</param>
        /// <param name="columnName">Name of the column source</param>
        public void AddDataColumnMap<R>(Expression<Func<T, R>> destinationProperty, string columnName)
        {
            Stack<string> propertyChain = GetPropertyChain(destinationProperty);
            SetAMEntrySource(propertyChain, columnName);
        }

        /// <summary>
        /// Sets mapping between target's property and value in source DataRow
        /// </summary>
        /// <typeparam name="R">Type of the object's property</typeparam>
        /// <param name="destinationProperty">Expression targeting the object's property</param>
        /// <param name="columnName">Name of the column source</param>
        /// <param name="converterFunction">Converting function</param>
        public void AddDataColumnMap<R>(Expression<Func<T, R>> destinationProperty, string columnName, Func<object, object> converterFunction)
        {
            Stack<string> propertyChain = GetPropertyChain(destinationProperty);
            SetAMEntrySource(propertyChain, columnName, converterFunction);
        }

        /// <summary>
        /// Converts the given value to the target type
        /// </summary>
        /// <param name="value">Value to be converted</param>
        /// <param name="targetType">Target type</param>
        /// <returns>Converted object</returns>
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
            else if (targetType == typeof(UInt32))
            {
                return Convert.ToUInt32(value);
            }
            else if (targetType == typeof(Single))
            {
                return Convert.ToSingle(value);
            }
            return value;
        }
    }
}
