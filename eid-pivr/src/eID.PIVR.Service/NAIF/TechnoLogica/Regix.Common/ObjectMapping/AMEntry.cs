using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace TechnoLogica.RegiX.Common.ObjectMapping
{
    /// <summary>
    /// Access matrix key value pair
    /// </summary>
    [XmlRoot("AMKeyValue", Namespace = "http://egov.bg/RegiX/ObjectMapping")]
    [XmlType(Namespace = "http://egov.bg/RegiX/ObjectMappingr")]
    public class AMKeyValue
    {
        [XmlElement]
        public string Key { get; set; }

        [XmlElement]
        public AMEntry Value { get; set; }
    }

    /// <summary>
    /// Hierarchy object describing the access to the properties of a given object
    /// </summary>
    [Serializable]
    [XmlRoot("AMEntry", Namespace = "http://egov.bg/RegiX/ObjectMapping")]
    [XmlType(Namespace = "http://egov.bg/RegiX/ObjectMappingr")]
    public class AMEntry
    {
        /// <summary>
        /// If the access to the given element is permitted
        /// </summary>
        [XmlElement]
        public bool HasAccess { get; set; }

        /// <summary>
        /// Dictionary containing the acces matrix for the current level
        /// </summary>
        [XmlIgnore]
        public Dictionary<string, AMEntry> AccessMatrix { get; set; }

        /// <summary>
        /// Access matrix entry
        /// </summary>
        [XmlElement]
        public AMKeyValue[] AccessMatrixElement
        {
            get
            {
                if (AccessMatrix != null)
                {
                    return AccessMatrix.Select(am => new AMKeyValue() { Key = am.Key, Value = am.Value }).ToArray();
                }
                else
                {
                    return null;
                }
                
            }
            set
            {
                if (value != null)
                {
                    AccessMatrix = new Dictionary<string, AMEntry>();
                    value.
                        Select(
                            v =>
                            {
                                AccessMatrix.Add(
                                    v.Key,
                                    new AMEntry()
                                    {
                                        HasAccess = v.Value.HasAccess,
                                        AccessMatrixElement = v.Value.AccessMatrixElement
                                    }
                                );
                                return 0;
                            }
                        ).ToArray();
                }
            }
        }

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public AMEntry()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hasAccess">If the access is permitted</param>
        /// <param name="am">Access matrix entries</param>
        public AMEntry(bool hasAccess, Dictionary<string, AMEntry> am)
        {
            HasAccess = hasAccess;
            AccessMatrix = am;
        }

        /// <summary>
        /// Creates an AMEntry object from the given Dictionary
        /// </summary>
        /// <param name="propertyAccess">Dictionary describing the properties (and if the access to these properties is permitted)
        /// of an object</param>
        /// <returns>The created AMEntry object</returns>
        public static AMEntry LoadFromDictionary(Dictionary<string, bool> propertyAccess)
        {
            string[] amLines = propertyAccess.Keys.OrderBy(s => s).ToList().ToArray();            
            int line = 0;
            AMEntry result = new AMEntry() { HasAccess = true, AccessMatrix = GetAM(0, ref line, amLines, propertyAccess) };
            return result;            
        }

        /// <summary>
        /// Method constructing the hierachy structure describing the access to properties of a given object
        /// </summary>
        /// <param name="level">Current level</param>
        /// <param name="line">Line in the list with characteristics</param>
        /// <param name="amLine">Array with characteristics (should be provided in alphabetic order)</param>
        /// <param name="propertyAccess">Information for the characteristics (if access is permitted)</param>
        /// <returns>The constructed hierachy structure</returns>
        private static Dictionary<string, AMEntry> GetAM(int level, ref int line, string[] amLine, Dictionary<string, bool> propertyAccess)
        {
            Dictionary<string, AMEntry> result = new Dictionary<string, AMEntry>();
            while (line < amLine.Length)
            {
                string[] amLineCells = amLine[line].Split('.');
                if (amLineCells.Length - 1 < level)
                {
                    return result;
                }
                else if ( amLineCells.Length - 1 == level )
                {
                    result.Add(amLineCells[level], new AMEntry(propertyAccess[amLine[line]], null));
                    line++;
                }
                else
                {
                    result.Last().Value.AccessMatrix = GetAM(level + 1, ref line, amLine, propertyAccess);
                }
            }
            return result;
        }
    }
}
