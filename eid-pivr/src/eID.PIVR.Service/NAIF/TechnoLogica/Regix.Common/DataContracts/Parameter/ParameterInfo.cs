using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace TechnoLogica.RegiX.Common.DataContracts.Parameter
{
    /// <summary>
    /// Adapter Parameter
    /// </summary>
    [Serializable]
    public class ParameterInfo
    {
        /// <summary>
        /// Parameter's owner assembly
        /// </summary>
        [XmlIgnore]
        public virtual Assembly OwnerAssembly { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Key
        /// </summary>
        public virtual string Key { get; set; }

        /// <summary>
        /// Current Value
        /// </summary>
        public virtual string CurrentValue { get; set; }

        /// <summary>
        /// Sets the value of the parameter
        /// </summary>
        /// <param name="value">New value</param>
        public virtual void SetValue(string value)
        {
        }
    }
}
