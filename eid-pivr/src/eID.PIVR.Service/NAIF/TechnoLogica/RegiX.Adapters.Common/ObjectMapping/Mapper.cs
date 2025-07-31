using System.Linq.Expressions;
using System.Reflection;
using FastMember;
using TechnoLogica.RegiX.Common.ObjectMapping;

namespace TechnoLogica.RegiX.Adapters.Common.ObjectMapping
{
    /// <summary>
    /// Base class fro all mappers
    /// </summary>
    /// <typeparam name="T">Target object's type</typeparam>
    public abstract class Mapper<T>
        where T : class
    {
        /// <summary>
        /// Root of the properties of the target object
        /// </summary>
        protected MapEntry MapEntryRoot { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="accessMatrix">Access matrix</param>
        public Mapper(AccessMatrix accessMatrix)
        {
            CreateMapTree(accessMatrix);
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public Mapper()
        {
            CreateMapTree();
        }

        /// <summary>
        /// Create hierarchy of the properties based on the given access matrix
        /// </summary>
        /// <param name="accessMatrix">Access matrix</param>
        private void CreateMapTree(AccessMatrix accessMatrix)
        {
            MapEntryRoot = new MapEntry(typeof(T));
            CopyMapTree(MapEntryRoot, accessMatrix.AMEntry, typeof(T));
        }
        /// <summary>
        /// Create hierarchy of the properties
        /// </summary>
        private void CreateMapTree()
        {
            MapEntryRoot = new MapEntry(typeof(T));
            CopyMapTree(MapEntryRoot, typeof(T));
        }

        /// <summary>
        /// Creates MapEntry based on the dta from the access matrix
        /// </summary>
        /// <param name="mapEntry">Current MapEntry</param>
        /// <param name="aMEntry">Current AMEntry</param>
        /// <param name="type">Property type</param>
        private void CopyMapTree(MapEntry mapEntry, AMEntry aMEntry, Type type)
        {
            type = MapEntry.GetNonCollectionType(type);
            TypeAccessor typeAccessor = TypeAccessor.Create(type);
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                string name = propertyInfo.PropertyType.Name;
                if (aMEntry != null &&
                    aMEntry.AccessMatrix != null &&
                    aMEntry.AccessMatrix.ContainsKey(propertyInfo.Name) &&
                    aMEntry.AccessMatrix[propertyInfo.Name].HasAccess)
                {
                    MapEntry childMapEntry = new MapEntry(propertyInfo.PropertyType, typeAccessor);
                    childMapEntry.PropertyType = propertyInfo.PropertyType;
                    childMapEntry.PropertyName = propertyInfo.Name;
                    if (properties.Where(pi => pi.Name == propertyInfo.Name + "Specified").Any())
                    {
                        childMapEntry.SpecifiedPropertyName = propertyInfo.Name + "Specified";
                    }
                    mapEntry.ChildProperties.Add(propertyInfo.Name, childMapEntry);
                    CopyMapTree(childMapEntry, aMEntry.AccessMatrix[propertyInfo.Name], propertyInfo.PropertyType);
                }
            }
        }
        /// <summary>
        /// Creates MapEntry
        /// </summary>
        /// <param name="mapEntry">Current MapEntry</param>
        /// <param name="type">Property type</param>
        private void CopyMapTree(MapEntry mapEntry, Type type)
        {
            type = MapEntry.GetNonCollectionType(type);
            TypeAccessor typeAccessor = TypeAccessor.Create(type);
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                string name = propertyInfo.PropertyType.Name;
                MapEntry childMapEntry = new MapEntry(propertyInfo.PropertyType, typeAccessor);
                childMapEntry.PropertyType = propertyInfo.PropertyType;
                childMapEntry.PropertyName = propertyInfo.Name;
                if (properties.Where(pi => pi.Name == propertyInfo.Name + "Specified").Any())
                {
                    childMapEntry.SpecifiedPropertyName = propertyInfo.Name + "Specified";
                }
                mapEntry.ChildProperties.Add(propertyInfo.Name, childMapEntry);
                CopyMapTree(childMapEntry, propertyInfo.PropertyType);
            }
        }

        /// <summary>
        /// Retrieves the path to the property described in the given expression
        /// </summary>
        /// <typeparam name="F">Type of the object which property is accessed</typeparam>
        /// <typeparam name="R">Property type</typeparam>
        /// <param name="propAccessExpression">Expression describing how to access a property</param>
        /// <returns>Path to the property</returns>
        protected Stack<string> GetPropertyChain<F, R>(Expression<Func<F, R>> propAccessExpression)
        {
            Stack<string> result = new Stack<string>();
            Expression expr = propAccessExpression.Body;
            while (expr != null)
            {
                if (expr is MemberExpression)
                {
                    result.Push((expr as MemberExpression).Member.Name);
                    expr = (expr as MemberExpression).Expression;
                }
                else if (expr is MethodCallExpression)
                {
                    expr = (expr as MethodCallExpression).Object;
                }
                else if (expr is BinaryExpression)
                {
                    expr = (expr as BinaryExpression).Left;
                }
                else
                {
                    expr = null;
                }
            }
            return result;
        }

        /// <summary>
        /// Creates new MapEntry based on the given path to the property
        /// </summary>
        /// <param name="propertyChain">Path to the property</param>
        /// <returns>Created map entry</returns>
        protected MapEntry GetMapEntry(Stack<string> propertyChain)
        {
            MapEntry currentMapEntry = MapEntryRoot;
            while (propertyChain.Count > 0)
            {
                if (currentMapEntry.ChildProperties.ContainsKey(propertyChain.Peek()))
                {
                    currentMapEntry = currentMapEntry.ChildProperties[propertyChain.Peek()];
                }
                else
                {
                    return null;
                }
                propertyChain.Pop();
            }
            return currentMapEntry;
        }

        /// <summary>
        /// Adds mapping to a constant object
        /// </summary>
        /// <typeparam name="R">Targe property type</typeparam>
        /// <param name="destinationProperty">Expression accessing the target property</param>
        /// <param name="value">Constant</param>
        public void AddConstantMap<R>(Expression<Func<T, R>> destinationProperty, R value)
        {
            Stack<string> propertyChain = GetPropertyChain(destinationProperty);
            SetAMEntryConstantSource(propertyChain, value);
        }

        /// <summary>
        /// Sets value of the map entry connected to the target's property
        /// </summary>
        /// <typeparam name="C">Type of the constant value</typeparam>
        /// <param name="propertyChain">Path tot he property</param>
        /// <param name="value">Constant value</param>
        private void SetAMEntryConstantSource<C>(Stack<string> propertyChain, C value)
        {
            MapEntry currentMapEntry = GetMapEntry(propertyChain);
            if (currentMapEntry != null)
            {
                currentMapEntry.DataSource = value; ;
                currentMapEntry.SourceType = MapEntrySourceType.Constant;
            }
        }

        /// <summary>
        /// Applies the mapping 
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="destination">Target object</param>
        public abstract void Map(object source, T destination);
    }
}
