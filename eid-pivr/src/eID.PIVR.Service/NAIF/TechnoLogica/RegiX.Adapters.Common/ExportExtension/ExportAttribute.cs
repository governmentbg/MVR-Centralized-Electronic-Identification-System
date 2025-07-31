using System;

namespace TechnoLogica.RegiX.Adapters.Common.ExportExtension
{
    /// <summary>
    /// Helper attribute - allows the usage of a type in export with its full name as a contract name
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public class ExportFullNameAttribute : System.ComponentModel.Composition.ExportAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contractName">Contract name type</param>
        /// <param name="interfaceType">Interface type</param>
        public ExportFullNameAttribute(Type contractName, Type interfaceType) : base(contractName.FullName, interfaceType)
        {
        }
    }

    /// <summary>
    /// Helper attribute - allows the usage of a type in export with its name as a contract name
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public class ExportSimpleNameAttribute : System.ComponentModel.Composition.ExportAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contractName">Contract name type</param>
        /// <param name="interfaceType">Interface type</param>
        public ExportSimpleNameAttribute(Type contractName, Type interfaceType) : base(contractName.Name, interfaceType)
        {
        }
    }

    /// <summary>
    /// Helper attribute - allows the usage of a type in import with its full name as a contract name
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class ImportAttribute : System.ComponentModel.Composition.ImportAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contractName">Conract name type</param>
        /// <param name="interfaceType">Interface type</param>
        public ImportAttribute(Type contractName, Type interfaceType) : base(contractName.FullName, interfaceType)
        {
        }
    }
}
