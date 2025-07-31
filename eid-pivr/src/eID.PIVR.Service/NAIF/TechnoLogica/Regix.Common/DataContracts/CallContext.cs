using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TechnoLogica.RegiX.Common.Utils;

namespace TechnoLogica.RegiX.Common.DataContracts
{

    /// <summary>
    /// Caller context
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TechnoLogica.RegiX.Common.TransportObject")]
    public class CallContext
    {
        /// <summary>
        /// URI of the administration service's instance or organization procedure
        /// </summary>
        [XmlElement]
        [DataMember]
        public string ServiceURI { get; set; }

        /// <summary>
        /// Type of the service, for wich the operation is requested
        /// Examples:
        /// За административна услуга: типа на услугата;
        /// За проверовъчна дейност: вида на дейността;
        /// За друга причина за използване: описание на причината;
        /// </summary>
        [XmlElement]
        [DataMember]
        public string ServiceType { get; set; }

        /// <summary>
        /// Employee identifier - for example email address as used in the AD of the administration or part of ЕИСУЧРДА
        /// The identifier should be checked by the client's information system
        /// </summary>
        [XmlElement(IsNullable = true)]
        [DataMember]
        public string EmployeeIdentifier { get; set; }

        /// <summary>
        /// Employee names
        /// </summary>
        [XmlElement(IsNullable = true)]
        [DataMember]
        public string EmployeeNames { get; set; }

        /// <summary>
        /// Oprtional employeed identifier - for example employee id
        /// </summary>
        [XmlElement(IsNullable = true)]
        [DataMember]
        public string EmployeeAditionalIdentifier { get; set; }

        /// <summary>
        /// Employee position in the organization
        /// </summary>
        [XmlElement(IsNullable = true)]
        [DataMember]
        public string EmployeePosition { get; set; }

        /// <summary>
        /// Administration identifier (oID or eAuth)
        /// </summary>
        [XmlElement(IsNullable = true)]
        [DataMember]
        public string AdministrationOId { get; set; }

        /// <summary>
        /// Name of the administration
        /// </summary>
        [XmlElement(IsNullable = true)]
        [DataMember]
        public string AdministrationName { get; set; }

        /// <summary>
        /// Optional identifier of the person responsible for the operation.
        /// This property should be set when the operation request is not initiated 
        /// by an employee but from an automated information system.
        /// The responsbile person could be the executive officer of the organization, 
        /// using the information system
        /// </summary>
        [XmlElement(IsNullable = true)]
        [DataMember]
        public string ResponsiblePersonIdentifier { get; set; }

        /// <summary>
        /// Law reason
        /// The law reason could be derived from regulations or could be based on agreement between the parties
        /// The property should contain the law reason or should refer to a registered one
        /// </summary>
        [XmlElement]
        [DataMember]
        public string LawReason { get; set; }

        /// <summary>
        /// Additional remarks
        /// </summary>
        [XmlElement(IsNullable = true)]
        [DataMember]
        public string Remark { get; set; }

        /// <inheritdoc>
        public override string ToString()
        {
            return this.JsonSerialize();
        }
    }
}
