using System;
using System.Collections.Generic;
using System.Linq;
using TechnoLogica.RegiX.Common;

namespace TechnoLogica.RegiX.Adapters.Common.Utils
{
    public static class TypeExtensions
    {
        public static List<Type> AdapterBaseInterfaces =
            new List<Type>()
            {
                typeof(IAdapterService),
                typeof(IAdapterServiceNETCore),
                typeof(IAdapterServiceWCF),
                typeof(IAsynchronousAdapterService)
            };

        public static List<Type> APIBaseInterfaces =
            new List<Type>()
            {
                typeof(IAPIService),
                typeof(IAdapterServiceNETCore),
                typeof(IAdapterServiceWCF),
                typeof(IAsynchronousAdapterService)
            };

        public static Type GetAdapterServiceInterface(this Type type)
        {
            return type
                .GetInterfaces()
                .Where(
                    i => !AdapterBaseInterfaces.Exists( abi => abi == i) 
                )
                .Single();
        }

        public static Type GetAPIServiceInterface(this Type type)
        {
            return type
                .GetInterfaces()
                .Where(
                    i => !APIBaseInterfaces.Exists(abi => abi == i)
                )
                .Single();
        }
    }
}
