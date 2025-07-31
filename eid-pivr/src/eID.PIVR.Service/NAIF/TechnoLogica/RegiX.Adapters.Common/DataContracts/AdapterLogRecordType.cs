using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using TechnoLogica.RegiX.Common.DataContracts;
using TechnoLogica.RegiX.Common.ObjectMapping;

namespace TechnoLogica.RegiX.Adapters.Common.DataContracts
{
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://egov.bg/RegiX/AdapterLogRecord")]
    [XmlRootAttribute(Namespace = "http://egov.bg/RegiX/AdapterLogRecord", IsNullable = false)]
    public class AdapterLogRecordType
    {
        [XmlElement]
        public XmlElement Argument { get; set; }

        [XmlElement]
        public XmlElement Response { get; set; }

        [XmlElement]
        public AccessMatrix AccessMatrix { get; set; }

        [XmlElement]
        public AdapterAdditionalParameters AdditionalParameters { get; set; }

        [XmlElement]
        public decimal ApiServiceCallId { get; set; }

        [XmlIgnore]
        public bool ApiServiceCallIdSpecified { get; set; }

        [XmlElement]
        public string ErrorMessage { get; set; }
    }
}
