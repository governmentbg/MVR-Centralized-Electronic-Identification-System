using FastMember;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using TechnoLogica.RegiX.Common.ObjectMapping;

namespace TechnoLogica.RegiX.Adapters.Common.ObjectMapping
{
    public class SelfMapper<S> : ObjectMapper<S, S>
        where S: class
    {
        public SelfMapper(AccessMatrix accessMatrix) : base(accessMatrix)
        {
            AddMapping(SourceEntryRoot, MapEntryRoot, typeof(S));
        }


        private void AddMapping(SourceEntry sourceEntry, MapEntry mapEntry, Type parentType)
        {
            if (
                mapEntry.PropertyType != null &&
                mapEntry.PropertyType != typeof(string) && 
                (mapEntry.PropertyType.IsArray || typeof(IEnumerable).IsAssignableFrom(mapEntry.PropertyType)))
            {   
                PropertyInfo pi = parentType.GetProperty(mapEntry.PropertyName);
                var i = Expression.Parameter(parentType, "i");

                var property = Expression.Property(i, pi);
                var lambdaExpression =
                    Expression.Lambda(property, new ParameterExpression[] { i });                

                mapEntry.DataSource = lambdaExpression.Compile();
                mapEntry.SourceType = MapEntrySourceType.Collection;
            }
            else if(mapEntry.ChildProperties != null && mapEntry.ChildProperties.Count > 0)
            {
                mapEntry.DataSource = sourceEntry;
                mapEntry.SourceType = MapEntrySourceType.Object;
            }
            else
            {
                mapEntry.DataSource = sourceEntry;
                mapEntry.SourceType = MapEntrySourceType.Property;
            }

            foreach(var childMapEntry in mapEntry.ChildProperties?.Keys)
            {
                AddMapping(sourceEntry.ChildProperties[childMapEntry], mapEntry.ChildProperties[childMapEntry], sourceEntry.PropertyType ?? parentType);
            }
        }

        protected override void MapProperty(MapEntry parentMapEntry, object source, object destination, MapEntry childMapEntry)
        {
            bool specifiedMissing = string.IsNullOrEmpty(childMapEntry.SpecifiedPropertyName);
            if (specifiedMissing ||
                (bool)childMapEntry.TypeAccessor[source, childMapEntry.SpecifiedPropertyName]
                )
            {
                base.MapProperty(parentMapEntry, source, destination, childMapEntry);
                if (!specifiedMissing)
                {
                    childMapEntry.TypeAccessor[destination, childMapEntry.SpecifiedPropertyName] = true;
                }
            }
        }
    }
}
