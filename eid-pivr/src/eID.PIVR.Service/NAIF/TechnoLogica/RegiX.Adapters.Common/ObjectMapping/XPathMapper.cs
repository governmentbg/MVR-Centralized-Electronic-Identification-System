using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using TechnoLogica.RegiX.Common.ObjectMapping;
using TechnoLogica.RegiX.Common.Utils;

namespace TechnoLogica.RegiX.Adapters.Common.ObjectMapping
{
    public class XPathMapper<T> : Mapper<T>
        where T : class
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="accessMatrix">Матрица за достъп</param>
        public XPathMapper(AccessMatrix accessMatrix)
            : base(accessMatrix)
        {

        }
        /// <summary>
        /// Конструктор
        /// </summary>
        public XPathMapper()
            : base()
        {

        }

        /// <summary>
        /// Извършва съотвествието
        /// </summary>
        /// <param name="source">Източник</param>
        /// <param name="destination">Цел</param>
        public override void Map(object source, T destination)
        {
            if (source != null && 
                source is XmlDocument)
            {
                Map((source as XmlDocument), destination, MapEntryRoot);
            }
            else if (source != null && 
                source is XElement)
            {
                using (XmlReader xmlReader = (source as XElement).CreateReader())
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(xmlReader);
                    Map(xmlDoc, destination, MapEntryRoot);
                }
            }
        }
        
        /// <summary>
        /// Извършва същиноското копиране на данните
        /// </summary>
        /// <param name="source">Изходен обект</param>
        /// <param name="destination">Целеви обект</param>
        /// <param name="mapEntry">Информация за съответствието</param>
        private void Map(XmlNode source, object destination, MapEntry mapEntry)
        {
            if (mapEntry.ChildProperties != null && source != null && destination != null)
            {
                foreach (KeyValuePair<string, MapEntry> keyValuePair in mapEntry.ChildProperties)
                {
                    MapEntry childMapEntry = keyValuePair.Value;
                    if (childMapEntry.DataSource != null &&
                        childMapEntry.SourceType == MapEntrySourceType.Collection)
                    {


                        List<object> result = new List<object>();
                        foreach (XmlNode node in source.SelectNodes(childMapEntry.DataSource as string))
                        {
                            object o = childMapEntry.CreateNew();
                            Map(node, o, childMapEntry);
                            result.Add(o);
                        } 
                        object list = result.ToStrongList(childMapEntry.NonCollectionType);
                        childMapEntry.Set(destination, list);
                    }
                    else if (childMapEntry.DataSource != null &&
                             childMapEntry.SourceType == MapEntrySourceType.Property)
                    {
                        XmlNode node = source.SelectSingleNode((childMapEntry.DataSource as string));
                        if (node != null)
                        {
                            childMapEntry.Set(destination, node.InnerText);
                        }
                    }
                    else if (
                        childMapEntry.DataSource != null &&
                        childMapEntry.SourceType == MapEntrySourceType.Constant)
                    {
                        childMapEntry.Set(destination, childMapEntry.DataSource);
                    }
                    else if (
                        childMapEntry.SourceType == MapEntrySourceType.FunctionMap)
                    {
                        dynamic function = childMapEntry.DataSource;
                        object result =
                            function
                                .Invoke(source);
                        childMapEntry.Set(destination, result);
                        Map(source, result, childMapEntry);
                    }
                    else
                    {
                        Map(source, childMapEntry.Get(destination), childMapEntry);
                    }
                }
            }
        }

        /// <summary>
        /// Добавя съотвествие между колекции
        /// </summary>
        /// <param name="destinationProperty">Израз сочещ целевата колекция</param>
        /// <param name="sourceProperty">XPath извличащ колекция</param>
        public void AddCollectionMap(Expression<Func<T, IEnumerable<object>>> destinationProperty, string xPath)
        {
            Stack<string> propertyChain = GetPropertyChain(destinationProperty);
            SetAMEntryCollectionSource(propertyChain, xPath);
        }

        /// <summary>
        /// Задава стойност на AMEntry по подадените параметри
        /// </summary>
        /// <typeparam name="O">Тип на обекти източник за подаваната функция</typeparam>
        /// <param name="propertyChain">Път до характеристика цел</param>
        /// <param name="sourceExpression">Функция за извличане на данни от изходния обект</param>
        private void SetAMEntryCollectionSource(Stack<string> propertyChain, string xPath)
        {
            MapEntry currentMapEntry = GetMapEntry(propertyChain);
            if (currentMapEntry != null)
            {
                currentMapEntry.DataSource = xPath;
                currentMapEntry.SourceType = MapEntrySourceType.Collection;
            }
        }

        /// <summary>
        /// Добавя съответсвие между израз и функция
        /// </summary>
        /// <typeparam name="R">Тип на съотвестваща характеристика</typeparam>
        /// <param name="destinationProperty">Израз сочещ целевата колекция</param>
        /// <param name="sourceProperty">Функция източник</param>
        public void AddFunctionMap<R>(Expression<Func<T, R>> destinationProperty, Func<XmlNode, R> sourceProperty)
        {
            Stack<string> propertyChain = GetPropertyChain(destinationProperty);
            SetAMEntryFunctionSource(propertyChain, sourceProperty);
        }

        /// <summary>
        /// Добавя съответсвие между израз и функция
        /// </summary>
        /// <typeparam name="R">Тип на съотвестваща характеристика (структура)</typeparam>
        /// <param name="destinationProperty">Израз сочещ целевата колекция</param>
        /// <param name="sourceProperty">Функция източник</param>
        public void AddFunctionMap<R>(Expression<Func<T, R>> destinationProperty, Func<XmlNode, R?> sourceProperty)
            where R:struct
        {
            Stack<string> propertyChain = GetPropertyChain(destinationProperty);
            SetAMEntryFunctionSource(propertyChain, sourceProperty);
        }

        /// <summary>
        /// Задава стойност на AMEntry по подадените параметри
        /// </summary>
        /// <typeparam name="R">Тип на връщания обект от функцията</typeparam>
        /// <param name="propertyChain">Път до характеристиката цел</param>
        /// <param name="sourceExpression">Функция извличаща данни от изходния обект</param>
        private void SetAMEntryFunctionSource<R>(Stack<string> propertyChain, Func<XmlNode, R> sourceProperty)
        {
            MapEntry currentMapEntry = GetMapEntry(propertyChain);
            if (currentMapEntry != null)
            {
                currentMapEntry.DataSource = sourceProperty;
                currentMapEntry.SourceType = MapEntrySourceType.FunctionMap;
            }
        }

        /// <summary>
        /// Добавя съответствие между характеристики
        /// </summary>
        /// <typeparam name="R">Тип на характеристиката</typeparam>
        /// <param name="destinationProperty">Израз до характеристиката цел</param>
        /// <param name="xPath">Израз до характеристиката източник</param>
        /// <param name="converterFunction">Конвертираща функция</param>
        public void AddPropertyMap<R>(Expression<Func<T, R>> destinationProperty, string xPath, Func<object, object> converterFunction = null)
        {
            Stack<string> propertyChain = GetPropertyChain(destinationProperty);
            SetAMEntrySource(propertyChain, xPath, converterFunction);
        }

        /// <summary>
        /// Задава стойност на AMEntry по подадените параметри
        /// </summary>
        /// <param name="propertyChain">Път до характеристика цел</param>
        /// <param name="xPath">Път до характеристика източник</param>
        /// <param name="converterFunction">Конвертираща функция</param>
        private void SetAMEntrySource(Stack<string> propertyChain, string xPath, Func<object, object> converterFunction)
        {
            MapEntry currentMapEntry = GetMapEntry(propertyChain);
            if (currentMapEntry != null)
            {
                currentMapEntry.DataSource = xPath;
                currentMapEntry.ConverterFunction = converterFunction;
                currentMapEntry.SourceType = MapEntrySourceType.Property;
            }
        }


    }
}
