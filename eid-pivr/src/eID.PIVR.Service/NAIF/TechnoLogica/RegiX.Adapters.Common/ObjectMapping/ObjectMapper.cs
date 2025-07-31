using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using FastMember;
using TechnoLogica.RegiX.Common.ObjectMapping;
using TechnoLogica.RegiX.Common.Utils;

namespace TechnoLogica.RegiX.Adapters.Common.ObjectMapping
{

    /// <summary>
    /// Клас извършващ копиране на данните съдържани в обект към друг обект
    /// </summary>
    /// <typeparam name="S">Изходен тип обект, от който се извличат данните</typeparam>
    /// <typeparam name="T">Тип обект, в който се копират данните</typeparam>
    public class ObjectMapper<S, T> : Mapper<T>
        where S : class
        where T : class
    {
        /// <summary>
        /// Корен на характеристиките на изходния обект
        /// </summary>
        protected SourceEntry SourceEntryRoot { get; set; }

        /// <summary>
        /// Изходен обект
        /// </summary>
        private S Source { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="accessMatrix">Матрица на достъпа</param>
        public ObjectMapper(AccessMatrix accessMatrix)
            : base(accessMatrix)
        {
            SourceEntryRoot = new SourceEntry(typeof(S));
            CreateSourceTree(SourceEntryRoot, typeof(S));
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        public ObjectMapper()
            : base()
        {
            SourceEntryRoot = new SourceEntry(typeof(S));
            CreateSourceTree(SourceEntryRoot, typeof(S));
        }

        /// <summary>
        /// Метод извършващ копирането на данни от източника (обект от тип <typeparamref name="S"/>) към целта (Обект от тип <typeparamref name="T"/>)
        /// </summary>
        /// <param name="source">Обект източник на данните</param>
        /// <param name="destination">Обект, в който копираме данните</param>
        public override void Map(object source, T destination)
        {
            Source = source as S;
            Map(source, destination, MapEntryRoot, new List<object>());
        }

        /// <summary>
        /// Изгражда йерархичната структура за източника на обекта
        /// </summary>
        /// <param name="sourceEntry">Корен на йерархията описваща изходния обект</param>
        /// <param name="type">Тип на обекта</param>
        private void CreateSourceTree(SourceEntry sourceEntry, Type type)
        {
            type = MapEntry.GetNonCollectionType(type);
            TypeAccessor typeAccessor = TypeAccessor.Create(type);
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                SourceEntry childSourceEntry = new SourceEntry(propertyInfo.PropertyType, typeAccessor);
                childSourceEntry.PropertyType = propertyInfo.PropertyType;
                childSourceEntry.PropertyName = propertyInfo.Name;
                sourceEntry.ChildProperties.Add(propertyInfo.Name, childSourceEntry);
                if (propertyInfo.PropertyType.Namespace != "System" &&
                    propertyInfo.PropertyType.Namespace != "System.Xml")
                {
                    CreateSourceTree(childSourceEntry, propertyInfo.PropertyType);
                }
            }
        }

        /// <summary>
        /// Извършва същиноското копиране на данните
        /// </summary>
        /// <param name="source">Изходен обект</param>
        /// <param name="destination">Целеви обект</param>
        /// <param name="mapEntry">Информация за съответствието</param>
        /// <param name="sourceStack">Stack с използваните до момента обекти</param>
        private void Map(object source, object destination, MapEntry mapEntry, List<object> sourceStack)
        {
            sourceStack.Add(source);
            if (mapEntry.ChildProperties != null && source != null && destination != null)
            {
                foreach (KeyValuePair<string, MapEntry> keyValuePair in mapEntry.ChildProperties)
                {
                    MapEntry childMapEntry = keyValuePair.Value;
                    if (childMapEntry.DataSource != null &&
                        childMapEntry.SourceType == MapEntrySourceType.Collection)
                    {

                        object collectionSource = source;
                        Type invokeArgumentType = childMapEntry.DataSource.GetType().GetGenericArguments()[0];
                        collectionSource = GetFunctionSource(sourceStack, collectionSource, invokeArgumentType);

                        var invokeArgument = invokeArgumentType.DynamicCast(collectionSource);
                        dynamic dataSource = childMapEntry.DataSource;
                        IEnumerable<object> objectsSource =
                            dataSource
                                .Invoke(invokeArgument);

                        if (objectsSource != null)
                        {
                            object list =
                            objectsSource
                                .Select(
                                   (d) =>
                                   {
                                       object o = childMapEntry.CreateNew();
                                       Map(d, o, childMapEntry, sourceStack);
                                       return o;
                                   }
                                )
                                .ToStrongList(childMapEntry.NonCollectionType);
                            childMapEntry.Set(destination, list);
                        }
                    }
                    else if (childMapEntry.DataSource != null &&
                             childMapEntry.SourceType == MapEntrySourceType.Property)
                    {
                        MapProperty(mapEntry, source, destination, childMapEntry);
                    }
                    else if (childMapEntry.DataSource != null &&
                             childMapEntry.SourceType == MapEntrySourceType.NullableStruct)
                    {
                        SourceEntry sourceEntry = (childMapEntry.DataSource as SourceEntry);
                        object objectValue = sourceEntry.TypeAccessor[source, sourceEntry.PropertyName];
                        childMapEntry.Set(destination, (objectValue == null)? DBNull.Value : objectValue);
                    }
                    else if (
                        childMapEntry.DataSource != null &&
                        childMapEntry.SourceType == MapEntrySourceType.Constant)
                    {
                        childMapEntry.Set(destination, childMapEntry.DataSource);
                    }
                    else if (
                        childMapEntry.DataSource != null &&
                        childMapEntry.SourceType == MapEntrySourceType.Object)
                    {
                        SourceEntry sourceEntry = (childMapEntry.DataSource as SourceEntry);
                        object childObject = sourceEntry.TypeAccessor[source, sourceEntry.PropertyName];
                        Map(childObject, childMapEntry.Get(destination), childMapEntry, sourceStack);
                    }
                    else if (
                        childMapEntry.SourceType == MapEntrySourceType.ObjectValue)
                    {
                        object o = childMapEntry.CreateNew();
                        childMapEntry.Set(destination, o);
                        Map(source, o, childMapEntry, sourceStack);
                    }
                    else if (
                        childMapEntry.SourceType == MapEntrySourceType.FunctionMap)
                    {
                        object functionSource = source;
                        Type invokeArgumentType = childMapEntry.DataSource.GetType().GetGenericArguments()[0];
                        functionSource = GetFunctionSource(sourceStack, functionSource, invokeArgumentType);

                        var invokeArgument = invokeArgumentType.DynamicCast(functionSource);
                        dynamic function = childMapEntry.DataSource;
                        object result =
                            function
                                .Invoke(invokeArgument);
                        childMapEntry.Set(destination, result);
                        Map(source, result, childMapEntry, sourceStack);
                    }
                    else
                    {
                        if (!typeof(IEnumerable).IsAssignableFrom(destination.GetType()))
                        {
                            Map(source, childMapEntry.Get(destination), childMapEntry, sourceStack);
                        }
                    }
                }
            }
            sourceStack.RemoveAt(sourceStack.Count - 1);
        }

        protected  virtual void MapProperty(MapEntry parentMapEntry, object source, object destination, MapEntry childMapEntry)
        {
            SourceEntry sourceEntry = (childMapEntry.DataSource as SourceEntry);
            object objectValue = sourceEntry.TypeAccessor[source, sourceEntry.PropertyName];
            childMapEntry.Set(destination, objectValue);
        }

        /// <summary>
        /// Извлича източника на данни за прилагане на функция
        /// </summary>
        /// <param name="sourceStack">Стек с използвани до момента обекти</param>
        /// <param name="functionSource">Текущия аргумнет</param>
        /// <param name="invokeArgumentType">Тип на аргумента на функцията</param>
        /// <returns>Извлеченият аргумент</returns>
        private object GetFunctionSource(List<object> sourceStack, object functionSource, Type invokeArgumentType)
        {
            if (invokeArgumentType != sourceStack[sourceStack.Count - 1].GetType())
            {
                for (int i = sourceStack.Count - 2; i >= 0; i--)
                {
                    if (invokeArgumentType == sourceStack[i].GetType())
                    {
                        functionSource = sourceStack[i];
                        break;
                    }
                }
            }
            return functionSource;
        }

        /// <summary>
        /// Добавя съответствие между характеристики
        /// </summary>
        /// <typeparam name="R">Тип на характеристиката</typeparam>
        /// <param name="destinationProperty">Израз до характеристиката цел</param>
        /// <param name="sourceProperty">Израз до характеристиката източник</param>
        public void AddPropertyMap<R>(Expression<Func<T, R>> destinationProperty, Expression<Func<S, R>> sourceProperty)
        {
            Stack<string> propertyChain = GetPropertyChain(destinationProperty);
            Stack<string> sourceChain = GetPropertyChain(sourceProperty);
            SetAMEntrySource(propertyChain, sourceChain);
        }


        /// <summary>
        /// Добавя съответствие между характеристики. Специализиран за Nullable структури
        /// </summary>
        /// <typeparam name="ST">Тип на характеристиката</typeparam>
        /// <param name="destinationProperty">Израз до характеристиката цел</param>
        /// <param name="sourceProperty">Израз до характеристиката източник</param>
        public void AddPropertyMap<ST>(Expression<Func<T, ST>> destinationProperty, Expression<Func<S, ST?>> sourceProperty)
            where ST : struct
        {
            Stack<string> propertyChain = GetPropertyChain(destinationProperty);
            Stack<string> sourceChain = GetPropertyChain(sourceProperty);
            SetAMEntrySource(propertyChain, sourceChain, MapEntrySourceType.NullableStruct);
        }

        /// <summary>
        /// Добавя съответствие между обекти
        /// </summary>
        /// <typeparam name="R">Тип на характеристиката</typeparam>
        /// <param name="destinationProperty">Израз до характеристиката цел</param>
        /// <param name="sourceProperty">Израз до характеристиката източник</param>
        public void AddObjectMap<R, R1>(Expression<Func<T, R1>> destinationObject, Expression<Func<S, R>> sourceObject)
        {
            Stack<string> propertyChain = GetPropertyChain(destinationObject);
            Stack<string> sourceChain = GetPropertyChain(sourceObject);
            SetAMEntryObjectSource(propertyChain, sourceChain);
        }
        
        /// <summary>
        /// Добавя съотвествие между колекции
        /// </summary>
        /// <typeparam name="O">Тип на обекта източник</typeparam>
        /// <param name="destinationProperty">Израз сочещ целевата колекция</param>
        /// <param name="sourceProperty">Израз сочещ колекцията източник</param>
        public void AddCollectionMap<O>(Expression<Func<T, IEnumerable<object>>> destinationProperty, Func<O, IEnumerable<object>> sourceProperty)
        {
            Stack<string> propertyChain = GetPropertyChain(destinationProperty);
            SetAMEntryCollectionSource(propertyChain, sourceProperty);
        }

        /// <summary>
        /// Добавя съответсвие между израз и функция
        /// </summary>
        /// <typeparam name="O">Тип на обект вход за функцията</typeparam>
        /// <typeparam name="R">Тип на съотвестваща характеристика</typeparam>
        /// <param name="destinationProperty">Израз сочещ целевата колекция</param>
        /// <param name="sourceProperty">Функция източник</param>
        public void AddFunctionMap<O, R>(Expression<Func<T, R>> destinationProperty, Func<O, R> sourceProperty)
        {
            Stack<string> propertyChain = GetPropertyChain(destinationProperty);
            SetAMEntryFunctionSource<O,R>(propertyChain, sourceProperty);
        }

        /// <summary>
        /// Добавя съответсвие между израз и функция
        /// </summary>
        /// <typeparam name="O">Тип на обект вход за функцията</typeparam>
        /// <typeparam name="R">Тип на съотвестваща характеристика</typeparam>
        /// <param name="destinationProperty">Израз сочещ целевата колекция</param>
        /// <param name="sourceProperty">Функция източник</param>
        public void AddFunctionMapNull<O, R>(Expression<Func<T, R>> destinationProperty, Func<O, R?> sourceProperty) where R: struct
        {
            Stack<string> propertyChain = GetPropertyChain(destinationProperty);
            SetAMEntryFunctionSource<O, R?>(propertyChain, sourceProperty);
        }

        /// <summary>
        /// Задава стойност на AMEntry по подадените параметри
        /// </summary>
        /// <param name="propertyChain">Път до характеристика цел</param>
        /// <param name="sourceChain">Път до характеристика източник</param>
        private void SetAMEntrySource(Stack<string> propertyChain, Stack<string> sourceChain, MapEntrySourceType entrySourceType = MapEntrySourceType.Property)
        {
            MapEntry currentMapEntry = GetMapEntry(propertyChain);
            SourceEntry sourceEntry = GetSourceEntry(sourceChain);
            if (currentMapEntry != null)
            {
                currentMapEntry.DataSource = sourceEntry;
                currentMapEntry.SourceType = entrySourceType;
            }
        }

        /// <summary>
        /// Задава стойност на AMEntry по подадените параметри
        /// </summary>
        /// <param name="propertyChain">Път до характеристика цел</param>
        /// <param name="sourceChain">Път до характеристика източник</param>
        private void SetAMEntryObjectSource(Stack<string> propertyChain, Stack<string> sourceChain)
        {
            MapEntry currentMapEntry = GetMapEntry(propertyChain);
            if (sourceChain.Count == 0)
            {
                //В случай, че няма достъп в АМ до някой root обект, коригира проблема с NullReferenceException
                if (currentMapEntry != null)
                {
                    // В случай че Map-Ваме директно от Source обекта
                    currentMapEntry.SourceType = MapEntrySourceType.ObjectValue;
                }
            }
            else
            {
                SourceEntry sourceEntry = GetSourceEntry(sourceChain);
                if (currentMapEntry != null)
                {
                    currentMapEntry.DataSource = sourceEntry;
                    currentMapEntry.SourceType = MapEntrySourceType.Object;
                }
            }
        }

        /// <summary>
        /// Задава стойност на AMEntry по подадените параметри
        /// </summary>
        /// <typeparam name="O">Тип на обекти източник за подаваната функция</typeparam>
        /// <param name="propertyChain">Път до характеристика цел</param>
        /// <param name="sourceExpression">Функция за извличане на данни от изходния обект</param>
        private void SetAMEntryCollectionSource<O>(Stack<string> propertyChain, Func<O, IEnumerable<object>> sourceExpression)
        {
            MapEntry currentMapEntry = GetMapEntry(propertyChain);
            if (currentMapEntry != null)
            {
                currentMapEntry.DataSource = sourceExpression;
                currentMapEntry.SourceType = MapEntrySourceType.Collection;
            }
        }

        /// <summary>
        /// Задава стойност на AMEntry по подадените параметри
        /// </summary>
        /// <typeparam name="O">Тип на обекти източник за подаваната функция</typeparam>
        /// <typeparam name="R">Тип на връщания обект от функцията</typeparam>
        /// <param name="propertyChain">Път до характеристиката цел</param>
        /// <param name="sourceExpression">Функция извличаща данни от изходния обект</param>
        private void SetAMEntryFunctionSource<O,R>(Stack<string> propertyChain, Func<O, R> sourceExpression)
        {
            MapEntry currentMapEntry = GetMapEntry(propertyChain);
            if (currentMapEntry != null)
            {
                currentMapEntry.DataSource = sourceExpression;
                currentMapEntry.SourceType = MapEntrySourceType.FunctionMap;
            }
        }

        /// <summary>
        /// Извлича SourceEntry по път до характеристика
        /// </summary>
        /// <param name="sourceChain">Път до характеристика</param>
        /// <returns>Извлеченото SourceEntry</returns>
        private SourceEntry GetSourceEntry(Stack<string> sourceChain)
        {
            SourceEntry currentSourceEntry = SourceEntryRoot;
            while (sourceChain.Count > 0)
            {
                if (currentSourceEntry.ChildProperties.ContainsKey(sourceChain.Peek()))
                {
                    currentSourceEntry = currentSourceEntry.ChildProperties[sourceChain.Peek()];
                }
                else
                {
                    return null;
                }
                sourceChain.Pop();
            }
            return currentSourceEntry;
        }
        
        /// <summary>
        /// Клас описващ изходния обект
        /// </summary>
        public class SourceEntry
        {
            /// <summary>
            /// Име на характеристика
            /// </summary>
            public string PropertyName { get; set; }

            /// <summary>
            /// Тип на характеристика
            /// </summary>
            public Type PropertyType { get; set; }

            /// <summary>
            /// Вложени характеристики
            /// </summary>
            public Dictionary<string, SourceEntry> ChildProperties { get; set; }

            /// <summary>
            /// Обект за бърз достъп до характеристика
            /// </summary>
            public TypeAccessor TypeAccessor { get; private set; }
            
            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="type">Тип на характеристика</param>
            public SourceEntry(Type type)
            {
                TypeAccessor = TypeAccessor.Create(type);
                ChildProperties = new Dictionary<string, SourceEntry>();                
            }

            /// <summary>
            /// Констурктор
            /// </summary>
            /// <param name="type">Тип на характеристика</param>
            /// <param name="typeAccessor">Обект за бърз достъп до характеристика</param>
            public SourceEntry(Type type, TypeAccessor typeAccessor)
            {
                TypeAccessor = typeAccessor;
                ChildProperties = new Dictionary<string, SourceEntry>();
            }
        }
    }

    /// <summary>
    /// Допълнения
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Методи извършващ същинското Cast-ване
        /// </summary>
        /// <typeparam name="T">Тип, към който Cast-ваме</typeparam>
        /// <param name="o">Обект, който cast-ваме</param>
        /// <returns>Cast-натия обект</returns>
        private static T Cast<T>(dynamic o)
        { return (T)o; }
                        
        /// <summary>
        /// извършва Cast на обекта към типа, за който извикваме операцията
        /// </summary>
        /// <param name="T">Тип, за който извършваме операцията</param>
        /// <param name="o">Обект който Cast-ваме</param>
        /// <returns>Cast-натия обект</returns>
        /// <exception cref="System.InvalidCastException"></exception>
        public static dynamic DynamicCast(this Type T, dynamic o)
        {
            return typeof(Extensions).GetMethod("Cast", BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(T).Invoke(null, new object[] { o });
        }
    }
}
