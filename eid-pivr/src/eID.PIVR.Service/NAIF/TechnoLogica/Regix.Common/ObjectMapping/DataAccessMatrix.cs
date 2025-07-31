using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using TechnoLogica.RegiX.Common.ObjectMapping;

namespace TechnoLogica.RegiX.Common.ObjectMapping
{
    /// <summary>
    /// Access matrix
    /// </summary>
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.ComponentModel.DescriptionAttribute("Матрица за достъп")]
    public partial class DataAccessMatrix : AccessMatrixType
    {
        /// <summary>
        /// ID field
        /// </summary>
        private string idField = "AccessMatrix";

        /// <summary>
        /// ID attribute
        /// </summary>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public DataAccessMatrix()
            : base()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="matrix">Access matrix</param>
        /// <param name="type">For type</param>
        public DataAccessMatrix(AccessMatrix matrix, Type type) : base(matrix, type) { }
    }

    /// <summary>
    /// Acess matrix type
    /// </summary>
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute]
    [System.Xml.Serialization.XmlRootAttribute]
    public partial class AccessMatrixType
    {
        /// <summary>
        /// if the access field is filled
        /// </summary>
        private bool hasAccessField;

        /// <summary>
        /// Name field
        /// </summary>
        private string nameField;

        /// <summary>
        /// Properties
        /// </summary>
        private List<AMPropertyType> propertiesField;

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public AccessMatrixType()
        {
            this.propertiesField = new List<AMPropertyType>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="matrix">Access matrix</param>
        /// <param name="type">For type</param>
        public AccessMatrixType(AccessMatrix matrix, Type type)
        {
            if (matrix.AMEntry != null)
            {
                this.HasAccess = matrix.AMEntry.HasAccess;
                this.Name = type.Name;
                propertiesField = new List<AMPropertyType>();
                foreach (var property in matrix.AMEntry.AccessMatrix)
                {
                    propertiesField.AddRange(RecursiveMapAccessMatrix(String.Empty, property));
                }
            }
        }

        /// <summary>
        /// Creates List&lt;AMPropertyType&gt; from the provided data
        /// </summary>
        /// <param name="parentPropertyName">Parent property name</param>
        /// <param name="source">Source property characteristics</param>
        /// <returns>The created List&lt;AMPropertyType&gt; object</returns>
        public List<AMPropertyType> RecursiveMapAccessMatrix(string parentPropertyName, KeyValuePair<string, AMEntry> source)
        {
            List<AMPropertyType> result = new List<AMPropertyType>();
            AMPropertyType rootEntry = new AMPropertyType();
            rootEntry.Name = (String.IsNullOrEmpty(parentPropertyName) ? parentPropertyName : parentPropertyName + "." ) + source.Key;
            rootEntry.HasAccess = source.Value.HasAccess;
            result.Add(rootEntry);
            if (source.Value.AccessMatrix != null)
            {
                foreach (var sourceProperty in source.Value.AccessMatrix)
                {
                    result.AddRange(RecursiveMapAccessMatrix(rootEntry.Name, sourceProperty));
                }
            }
            return result;
        }

        /// <summary>
        /// If the access is permitted
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        [System.ComponentModel.DescriptionAttribute("Дали достъпът е позволен")]
        public bool HasAccess
        {
            get
            {
                return this.hasAccessField;
            }
            set
            {
                this.hasAccessField = value;
            }
        }

        /// <summary>
        /// Name of the object
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        [System.ComponentModel.DescriptionAttribute("Наименование на обекта")]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <summary>
        /// Properties
        /// </summary>
        [System.Xml.Serialization.XmlArrayAttribute(Order = 2)]
        [System.Xml.Serialization.XmlArrayItemAttribute("Property", IsNullable = false)]
        [System.ComponentModel.DescriptionAttribute("Характеристики")]
        public List<AMPropertyType> Properties
        {
            get
            {
                return this.propertiesField;
            }
            set
            {
                this.propertiesField = value;
            }
        }
    }

    /// <summary>
    /// Description of the access to the characteristics of an object
    /// </summary>
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute]
    [System.Xml.Serialization.XmlRootAttribute]
    public partial class AMPropertyType
    {
        /// <summary>
        /// Field for the has access property
        /// </summary>
        private bool hasAccessField;

        /// <summary>
        /// Name field
        /// </summary>
        private string nameField;

        /// <summary>
        /// If the access to the current characteristic is permitted
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        [System.ComponentModel.DescriptionAttribute("Дали достъпът до текущата характеристика е позволен")]
        public bool HasAccess
        {
            get
            {
                return this.hasAccessField;
            }
            set
            {
                this.hasAccessField = value;
            }
        }

        /// <summary>
        /// Name
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        [System.ComponentModel.DescriptionAttribute("Наименование")]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    //[Serializable]
    //[XmlRoot("AccessMatrix")]
    //public class SerializableEntry
    //{
    //    public SerializableEntry()
    //    {
    //        this.AllEntries = new List<SerializableEntry>();
    //        this.Leafs = new List<SerializableEntry>();
    //    }

    //    public SerializableEntry(AccessMatrix matrix, Type type)
    //        : this()
    //    {
    //        this.Type = type;
    //        this.Name = type.Name;
    //        this.AccessMatrix = matrix;
    //        this.FillEntriesFromMatrix();
    //    }

    //    public bool HasAccess { get; set; }
    //    public string Name { get; set; }

    //    private bool IsRoot { get; set; }
    //    private Type Type { get; set; }
    //    private AccessMatrix AccessMatrix { get; set; }
    //    private List<SerializableEntry> AllEntries { get; set; }

    //    [XmlArray("SerializableEntries")]
    //    [XmlArrayItem("SerializableEntry")]
    //    public List<SerializableEntry> Leafs { get; set; }

    //    private void FillEntriesFromMatrix()
    //    {
    //        var root = this.AccessMatrix.AMEntry;
    //        this.IsRoot = true;
    //        this.RecursiveMapFromAccessMatrix(this, root);
    //    }

    //    private void RecursiveMapFromAccessMatrix(SerializableEntry serializableEntry, AMEntry entry)
    //    {
    //        serializableEntry.HasAccess = entry.HasAccess;

    //        if (entry.HasAccess)
    //        {
    //            if (entry.AccessMatrix != null)
    //            {
    //                foreach (var item in entry.AccessMatrix)
    //                {
    //                    AMEntry currentEntry = item.Value;
    //                    SerializableEntry newSerializableEntry = new SerializableEntry();

    //                    serializableEntry.AllEntries.Add(newSerializableEntry);
    //                    newSerializableEntry.Name = item.Key;
    //                    if (serializableEntry.Name != null && serializableEntry.IsRoot == false)
    //                    {
    //                        newSerializableEntry.Name = serializableEntry.Name + "." + newSerializableEntry.Name;
    //                    }

    //                    //recursive walk through the tree graph
    //                    this.RecursiveMapFromAccessMatrix(newSerializableEntry, currentEntry);
    //                }
    //            }
    //            else
    //            {
    //                serializableEntry.Leafs = null;
    //                this.Leafs.Add(serializableEntry);
    //            }
    //        }
    //    }

    //    public static AccessMatrix GenerateAccessMatrix(string xml)
    //    {
    //        SerializableEntry entry = new SerializableEntry();

    //        using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
    //        {
    //            reader.MoveToContent();
    //            switch (reader.Name)
    //            {
    //                case "AccessMatrix":
    //                    var obj = new XmlSerializer(typeof(SerializableEntry)).Deserialize(reader);
    //                    entry = (SerializableEntry)obj;
    //                    break;
    //                default:
    //                    throw new NotSupportedException("Unexpected: " + reader.Name);
    //            }
    //        }

    //        Type classType = AppDomain.CurrentDomain.GetAssemblies()
    //                .SelectMany(t => t.GetTypes())
    //                .Where(t => t.IsClass && t.Name == entry.Name)
    //                .FirstOrDefault();

    //        return AccessMatrix.CreateForType(classType);
    //    }
    //}
}
