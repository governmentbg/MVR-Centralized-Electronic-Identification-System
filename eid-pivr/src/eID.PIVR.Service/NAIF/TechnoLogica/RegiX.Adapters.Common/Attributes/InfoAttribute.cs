using System;
using System.Collections.Generic;
using System.Text;

namespace TechnoLogica.RegiX.Adapters.Common.Attributes
{
    /// <summary>
    /// Attribute used for decoration of adapter's operations. It provides
    /// means for configuring informatino about a given operation.
    /// </summary>
    public class InfoAttribute: Attribute
    {
        /// <summary>
        /// Name of the XSD file of the request
        /// </summary>
        public string RequestXSD { get; private set; }

        /// <summary>
        /// Name of the XSD file of the response
        /// </summary>
        public string ResponseXSD { get; private set; }

        /// <summary>
        /// Name of the Common XSD file/s
        /// </summary>
        public string CommonXSD { get; private set; }

        /// <summary>
        /// XSLT transformation used for  the generation of HTML from the requests's XML
        /// </summary>
        public string RequestXSLT { get; private set; }

        /// <summary>
        /// XSLT transformation used for  the generation of HTML from the response's XML
        /// </summary>
        public string ResponseXSLT { get; private set; }

        /// <summary>
        /// XSLT transformation used for  the generation of FOP from the requests's XML
        /// </summary>
        public string RequestXSLTFOP { get; private set; }

        /// <summary>
        /// XSLT transformation used for  the generation of FOP from the response's XML
        /// </summary>
        public string ResponseXSLTFOP { get; private set; }

        /// <summary>
        /// Sample request XML file
        /// </summary>
        public string SampleRequest { get; private set; }

        /// <summary>
        /// Sample response XML file
        /// </summary>
        public string SampleResponse { get; private set; }

        /// <summary>
        /// MetaData XML used for the generation of the request's fill form
        /// </summary>
        public string MetaDataXML { get; private set; }

        /// <summary>
        /// MetaData XML used for the generation of the response's fill form
        /// </summary>
        public string ResponseMetaDataXML { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="requestXSD">Request XSD</param>
        /// <param name="responseXSD">Response XSD</param>
        /// <param name="commonXSD">Common XSD file/s</param>
        /// <param name="requestXSLT">Request XSLT for HTML generation</param>
        /// <param name="responseXSLT">Response XSLT for HTML generation</param>
        /// <param name="sampleRequest">Sample request file</param>
        /// <param name="sampleResponse">Sample response file</param>
        /// <param name="metaDataXML">Metadat XML for the request</param>
        /// <param name="responseMetaDataXML">Metadata XML for the response</param>
        public InfoAttribute(
            string requestXSD = null, 
            string responseXSD = null, 
            string commonXSD = null, 
            string requestXSLT = null, 
            string responseXSLT = null, 
            string sampleRequest = null, 
            string sampleResponse = null, 
            string metaDataXML = null, 
            string responseMetaDataXML = null) : base()
        {
            RequestXSD = requestXSD;
            ResponseXSD = responseXSD;
            CommonXSD = commonXSD;
            RequestXSLT = requestXSLT;
            ResponseXSLT = responseXSLT;
            SampleRequest = sampleRequest;
            SampleResponse = sampleResponse;
            MetaDataXML = metaDataXML;
            ResponseMetaDataXML = responseMetaDataXML;
        }
    }
}
