using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechnoLogica.RegiX.Adapters.Common.ObjectMapping
{
    /// <summary>
    /// Source types for copy
    /// </summary>
    public enum MapEntrySourceType
    {
        None,
        DataRowCollection,
        DataRows,
        DataColumn,
        Constant,
        Property,
        Collection,
        Object,
        ObjectValue,
        FunctionMap,
        NullableStruct
    }
}
