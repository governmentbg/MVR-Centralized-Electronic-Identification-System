using System.Globalization;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using eID.PIVR.Contracts.Commands;
using eID.PIVR.Contracts.Results;
using eID.PIVR.Service.Extensions;
using eID.PIVR.Service.NAIF.OtherServices;
using eID.PIVR.Service.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TechnoLogica.RegiX.Adapters.Common.ObjectMapping;
using TechnoLogica.RegiX.Common.ObjectMapping;
using TechnoLogica.RegiX.Common.Utils;
using NAIFImpl = NAIF;

namespace eID.PIVR.Service;

public class NAIFService : BaseService, INAIFService
{
    private readonly ILogger<NAIFService> _logger;
    private readonly HttpClient _httpClient;
    private readonly IDistributedCache _cache;
    private readonly ExternalRegistersCacheOptions _cacheOptions;

    public NAIFService(ILogger<NAIFService> logger, HttpClient httpClient, IDistributedCache cache, IOptions<ExternalRegistersCacheOptions> cacheOptions)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _cacheOptions = (cacheOptions ?? throw new ArgumentNullException(nameof(cacheOptions))).Value;
    }

    public async Task<ServiceResult<RegiXSearchResult>> GetForeignIdentityV2Async(MVRGetForeignIdentityV2 command)
    {
        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        // Validation
        //var validator = new RegiXSearchCommandValidator();
        //var validationResult = await validator.ValidateAsync(message);
        //if (!validationResult.IsValid)
        //{
        //    return BadRequest<RegiXSearchResult>(validationResult.Errors);
        //}

        #region Request
        TechnoLogica.RegiX.MVRERChAdapterV2.Request request = new TechnoLogica.RegiX.MVRERChAdapterV2.Request();
        request.Header.DateTime = System.DateTime.Now;
        request.Header.Operation = "0002";
        request.Header.OrganizationUnit = "CSEI-PIVR";
        request.Header.SystemID = "CSEI";
        request.Header.UserName = $"CorrelationId={command.CorrelationId}";

        //request.Header.OrganizationUnit = additionalParameters.OrganizationUnit;
        //request.Header.CallerIPAddress = additionalParameters.ClientIPAddress;
        //request.Header.CallContext = additionalParameters.CallContext != null ? additionalParameters.CallContext.ToString() : null;

        ////BiometricFlag - дали консуматорът има достъп до подпис и снимка
        request.Header.BiometricDataFlagSpecified = true;
        request.Header.BiometricDataFlag = false;

        TechnoLogica.RegiX.MVRERChAdapterV2.RequestDataTypeFC fc = new TechnoLogica.RegiX.MVRERChAdapterV2.RequestDataTypeFC();
        fc.Item = command.Identifier;
        switch (command.IdentifierType)
        {
            case Contracts.Enums.IdentifierType.EGN:
                fc.ItemElementName = TechnoLogica.RegiX.MVRERChAdapterV2.ItemChoiceType.PID;
                break;
            case Contracts.Enums.IdentifierType.LNCh:
                fc.ItemElementName = TechnoLogica.RegiX.MVRERChAdapterV2.ItemChoiceType.ID;
                break;
        }

        request.RequestData.Item = fc;
        #endregion

        string response;
        try
        {
            var client = new NAIFImpl.IntSyncPortTypeClient();
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = false;
            doc.LoadXml(request.XmlSerialize());

            foreach (XmlNode node in doc)
            {
                if (node.NodeType == XmlNodeType.XmlDeclaration)
                {
                    doc.RemoveChild(node);
                    break;
                }
            }

            var callResponse = await client.CallAsync(new NAIFImpl.Call { Call1 = doc.OuterXml.Trim() });
            response = callResponse.Message;
            _logger.LogInformation("Call to NAIF {CommandName} succeeded.", nameof(MVRGetForeignIdentityV2));
            _logger.LogDebug("NAIF {CommandName} Response {ResponseStr}", nameof(MVRGetForeignIdentityV2), response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Call to NAIF {CommandName} failed.", nameof(MVRGetForeignIdentityV2));
            TechnoLogica.RegiX.MVRERChAdapterV2.ForeignIdentityInfoResponseType tempresult = new TechnoLogica.RegiX.MVRERChAdapterV2.ForeignIdentityInfoResponseType();
            tempresult.ReturnInformations = new TechnoLogica.RegiX.MVRERChAdapterV2.ReturnInformation();
            tempresult.ReturnInformations.ReturnCode = "1111";
            tempresult.ReturnInformations.Info = "Call unsuccessful.";

            return new ServiceResult<RegiXSearchResult>
            {
                StatusCode = System.Net.HttpStatusCode.BadGateway,
                Error = "NAIF connection failed.",
                Result = new FailedNAIFResult
                {
                    Response = new Dictionary<string, object> { { "ForeignIdentityInfoResponse", tempresult } }
                }
            };
        }
        #region Result

        TechnoLogica.RegiX.MVRERChAdapterV2.ForeignIdentityInfoResponseType result = new TechnoLogica.RegiX.MVRERChAdapterV2.ForeignIdentityInfoResponseType();
        try
        {
            XmlDocument resultXmlDoc = new XmlDocument();
            resultXmlDoc.LoadXml(response);
            XPathMapper<TechnoLogica.RegiX.MVRERChAdapterV2.ForeignIdentityInfoResponseType> mapper = CreateForeignIdentityInfoMapV2();
            mapper.Map(resultXmlDoc, result);

            _logger.LogInformation("NAIF {NAIFResponseType} mapping succeeded.", nameof(TechnoLogica.RegiX.MVRERChAdapterV2.ForeignIdentityInfoResponseType));
            return new ServiceResult<RegiXSearchResult>
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Result = new NAIFResult
                {
                    Response = new Dictionary<string, object> { { "ForeignIdentityInfoResponse", result } }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during parsing {NAIFResponseType}.", nameof(TechnoLogica.RegiX.MVRERChAdapterV2.ForeignIdentityInfoResponseType));
            TechnoLogica.RegiX.MVRERChAdapterV2.ForeignIdentityInfoResponseType tempresult = new TechnoLogica.RegiX.MVRERChAdapterV2.ForeignIdentityInfoResponseType();
            tempresult.ReturnInformations = new TechnoLogica.RegiX.MVRERChAdapterV2.ReturnInformation();
            tempresult.ReturnInformations.ReturnCode = "1111";
            tempresult.ReturnInformations.Info = response;
            ObjectMapper<TechnoLogica.RegiX.MVRERChAdapterV2.ForeignIdentityInfoResponseType, TechnoLogica.RegiX.MVRERChAdapterV2.ForeignIdentityInfoResponseType> selfmapper = CreateForeignIdentitySelfMapperV2();
            selfmapper.Map(tempresult, result);

            return new ServiceResult<RegiXSearchResult>
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Error = "Error during parsing.",
                Result = new FailedNAIFResult
                {
                    Response = new Dictionary<string, object> { { "ForeignIdentityInfoResponse", tempresult } }
                }
            };
        }

        #endregion
    }

    private static ObjectMapper<TechnoLogica.RegiX.MVRERChAdapterV2.ForeignIdentityInfoResponseType, TechnoLogica.RegiX.MVRERChAdapterV2.ForeignIdentityInfoResponseType> CreateForeignIdentitySelfMapperV2()
    {
        var am = AccessMatrix.CreateForType(typeof(TechnoLogica.RegiX.MVRERChAdapterV2.ForeignIdentityInfoResponseType));
        ObjectMapper<TechnoLogica.RegiX.MVRERChAdapterV2.ForeignIdentityInfoResponseType, TechnoLogica.RegiX.MVRERChAdapterV2.ForeignIdentityInfoResponseType> mapper = new ObjectMapper<TechnoLogica.RegiX.MVRERChAdapterV2.ForeignIdentityInfoResponseType, TechnoLogica.RegiX.MVRERChAdapterV2.ForeignIdentityInfoResponseType>(am);

        mapper.AddObjectMap((o) => o.ReturnInformations, (c) => c.ReturnInformations);
        mapper.AddPropertyMap((o) => o.ReturnInformations.ReturnCode, (c) => c.ReturnInformations.ReturnCode);
        mapper.AddPropertyMap((o) => o.ReturnInformations.Info, (c) => c.ReturnInformations.Info);

        return mapper;
    }
    private XPathMapper<TechnoLogica.RegiX.MVRERChAdapterV2.ForeignIdentityInfoResponseType> CreateForeignIdentityInfoMapV2()
    {
        var am = AccessMatrix.CreateForType(typeof(TechnoLogica.RegiX.MVRERChAdapterV2.ForeignIdentityInfoResponseType));
        XPathMapper<TechnoLogica.RegiX.MVRERChAdapterV2.ForeignIdentityInfoResponseType> mapper = new XPathMapper<TechnoLogica.RegiX.MVRERChAdapterV2.ForeignIdentityInfoResponseType>(am);

        mapper.AddPropertyMap(d => d.ReturnInformations.ReturnCode, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ReturnInformation']/*[local-name()='ReturnCode']");
        mapper.AddPropertyMap(d => d.ReturnInformations.Info, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ReturnInformation']/*[local-name()='Info']");

        mapper.AddPropertyMap(d => d.EGN, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='Data']/*[local-name()='PIN']");
        mapper.AddPropertyMap(d => d.LNCh, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='Data']/*[local-name()='PN']");
        mapper.AddPropertyMap(d => d.PersonNames.FirstName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='Data']/*[local-name()='Names']/*[local-name()='Cyrillic']/*[local-name()='First']");
        mapper.AddPropertyMap(d => d.PersonNames.Surname, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='Data']/*[local-name()='Names']/*[local-name()='Cyrillic']/*[local-name()='Surname']");
        mapper.AddPropertyMap(d => d.PersonNames.FamilyName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='Data']/*[local-name()='Names']/*[local-name()='Cyrillic']/*[local-name()='Family']");
        mapper.AddPropertyMap(d => d.PersonNames.FirstNameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='Data']/*[local-name()='Names']/*[local-name()='Latin']/*[local-name()='First']");
        mapper.AddPropertyMap(d => d.PersonNames.SurnameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='Data']/*[local-name()='Names']/*[local-name()='Latin']/*[local-name()='Surname']");
        mapper.AddPropertyMap(d => d.PersonNames.LastNameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='Data']/*[local-name()='Names']/*[local-name()='Latin']/*[local-name()='Family']");

        // от МВР казаха, че датата на раждане на чужденец е стринг (може да е попълнена само година например)
        //mapper.AddFunctionMap(d => d.BirthDate, node =>
        //{
        //    XmlNode dateNode =
        //           node.SelectSingleNode(
        //               "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='Data']/*[local-name()='BirthDate']");
        //    if (dateNode != null && !string.IsNullOrWhiteSpace(dateNode.InnerText))
        //    {
        //        return DateTime.ParseExact(dateNode.InnerText, "yyyy-MM-dd", CultureInfo.InvariantCulture.DateTimeFormat);
        //    }
        //    return null;
        //});

        mapper.AddPropertyMap(d => d.BirthDate, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='Data']/*[local-name()='BirthDate']");

        mapper.AddFunctionMap(d => d.DeathDate, node =>
        {
            XmlNode dateNode =
                   node.SelectSingleNode("./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='DeathDate']");
            if (dateNode != null && !string.IsNullOrWhiteSpace(dateNode.InnerText))
            {
                return DateTime.ParseExact(dateNode.InnerText, "yyyy-MM-dd", CultureInfo.InvariantCulture.DateTimeFormat);
            }
            return null;
        });
        mapper.AddPropertyMap(d => d.BirthPlace.CountryName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='BirthPlace']/*[local-name()='Country']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.BirthPlace.CountryNameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='BirthPlace']/*[local-name()='Country']/*[local-name()='Latin']");
        mapper.AddFunctionMap(d => d.BirthPlace.CountryCode, node =>
        {

            XmlNode n = node.SelectSingleNode("./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='BirthPlace']/*[local-name()='Country']");
            if (n == null || string.IsNullOrWhiteSpace(n.InnerText)) return null;
            XmlAttribute attr = n.Attributes["code"];
            if (attr != null)
            {
                return attr.Value;
            }
            return null;
        });
        mapper.AddPropertyMap(d => d.BirthPlace.TerritorialUnitName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='BirthPlace']/*[local-name()='TerritorialUnitName']");
        mapper.AddPropertyMap(d => d.BirthPlace.DistrictName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='BirthPlace']/*[local-name()='DistrictName']");
        mapper.AddPropertyMap(d => d.BirthPlace.MunicipalityName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='BirthPlace']/*[local-name()='MunicipalityName']");

        mapper.AddPropertyMap(d => d.GenderName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='Data']/*[local-name()='Gender']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.GenderNameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='Data']/*[local-name()='Gender']/*[local-name()='Latin']");

        mapper.AddCollectionMap(d => d.NationalityList, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='Data']/*[local-name()='NationalityList']/*[local-name()='Nationality']");
        mapper.AddFunctionMap(d => d.NationalityList[0].NationalityCode, node =>
        {
            XmlAttribute attr = node.Attributes["code"];
            if (attr != null)
            {
                return attr.Value;
            }
            return null;
        });
        mapper.AddPropertyMap(d => d.NationalityList[0].NationalityName, "./*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.NationalityList[0].NationalityNameLatin, "./*[local-name()='Latin']");

        mapper.AddCollectionMap(d => d.Statuses, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Statuses']");
        mapper.AddPropertyMap(d => d.Statuses[0].Status.StatusName, "./*[local-name()='Status']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.Statuses[0].Status.StatusNameLatin, "./*[local-name()='Status']/*[local-name()='Latin']");
        mapper.AddFunctionMap(d => d.Statuses[0].Status.DateFrom, node =>
        {
            XmlNode dateNode =
                   node.SelectSingleNode("./*[local-name()='DateFrom']");
            if (dateNode != null && !string.IsNullOrWhiteSpace(dateNode.InnerText))
            {
                return DateTime.ParseExact(dateNode.InnerText, "yyyy-MM-dd", CultureInfo.InvariantCulture.DateTimeFormat);
            }
            return null;
        });
        mapper.AddFunctionMap(d => d.Statuses[0].Status.DateTo, node =>
        {
            XmlNode dateNode =
                   node.SelectSingleNode("./*[local-name()='DateTo']");
            if (dateNode != null && !string.IsNullOrWhiteSpace(dateNode.InnerText))
            {
                return DateTime.ParseExact(dateNode.InnerText, "yyyy-MM-dd", CultureInfo.InvariantCulture.DateTimeFormat);
            }
            return null;
        });

        //new
        mapper.AddPropertyMap(d => d.Employer, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Statuses']/*[local-name()='Employer']");
        mapper.AddPropertyMap(d => d.Position, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Statuses']/*[local-name()='Position']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.PositionLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Statuses']/*[local-name()='Position']/*[local-name()='Latin']");

        mapper.AddPropertyMap(d => d.Statuses[0].StatusLawReason.Status, "./*[local-name()='LegalBasis']/*[local-name()='Cyrillic']");
        //"./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Statuses']/*[local-name()='LegalBasis']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.Statuses[0].StatusLawReason.StatusLatin, "./*[local-name()='LegalBasis']/*[local-name()='Latin']");
        //"./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Statuses']/*[local-name()='LegalBasis']/*[local-name()='Latin']");

        //mapper.AddPropertyMap(d => d.StatusLawReason.Code, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Statuses']/*[local-name()='LegalBasis']/*[@code]");
        mapper.AddFunctionMap(d => d.Statuses[0].StatusLawReason.Code, node =>
        {
            XmlNode n = node.SelectSingleNode("./*[local-name()='LegalBasis']");
            //("./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Statuses']/*[local-name()='LegalBasis']");
            if (n == null || string.IsNullOrWhiteSpace(n.InnerText)) return null;
            XmlAttribute attr = n.Attributes["code"];
            if (attr != null)
            {
                return attr.Value;
            }
            return null;
        });
        mapper.AddFunctionMap(d => d.Statuses[0].StatusDocument.StatusDocumentDate, node =>
        {
            XmlNode dateNode =
                   node.SelectSingleNode("./*[local-name()='Document']/*[local-name()='RegDate']");
            if (dateNode != null && !string.IsNullOrWhiteSpace(dateNode.InnerText))
            {
                return DateTime.ParseExact(dateNode.InnerText, "yyyy-MM-dd", CultureInfo.InvariantCulture.DateTimeFormat);
            }
            return null;
        });
        mapper.AddPropertyMap(d => d.Statuses[0].StatusDocument.DocumentNumber, "./*[local-name()='Document']/*[local-name()='RegNumber']");
        mapper.AddPropertyMap(d => d.Statuses[0].StatusDocument.DocumentType, "./*[local-name()='Document']/*[local-name()='DocumentType']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.Statuses[0].StatusDocument.DocumentTypeLatin, "./*[local-name()='Document']/*[local-name()='DocumentType']/*[local-name()='Latin']");
        mapper.AddPropertyMap(d => d.Statuses[0].Category.CategoryLatin, "./*[local-name()='Category']/*[local-name()='Latin']");
        //mapper.AddPropertyMap(d => d.StatusLawReason.Code, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Statuses']/*[local-name()='Category']/*[@code]");

        mapper.AddFunctionMap(d => d.Statuses[0].Category.Code, node =>
        {
            XmlNode n = node.SelectSingleNode("./*[local-name()='Category']");
            if (n == null || string.IsNullOrWhiteSpace(n.InnerText)) return null;
            XmlAttribute attr = n.Attributes["code"];
            if (attr != null)
            {
                return attr.Value;
            }
            return null;
        });
        mapper.AddPropertyMap(d => d.Statuses[0].Category.CategoryCyrillic, "./*[local-name()='Category']/*[local-name()='Cyrillic']");

        //mapper.AddPropertyMap(d => d.Category[0]. , "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Statuses']/*[local-name()='Category']/*[local-name()='Cyrillic']");

        //permanent address
        mapper.AddPropertyMap(d => d.PermanentAddress.DistrictName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Permanent']/*[local-name()='District']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.PermanentAddress.DistrictNameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Permanent']/*[local-name()='District']/*[local-name()='Latin']");
        mapper.AddPropertyMap(d => d.PermanentAddress.MunicipalityName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Permanent']/*[local-name()='Municipality']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.PermanentAddress.MunicipalityNameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Permanent']/*[local-name()='Municipality']/*[local-name()='Latin']");
        mapper.AddPropertyMap(d => d.PermanentAddress.SettlementName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Permanent']/*[local-name()='Settlement']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.PermanentAddress.SettlementNameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Permanent']/*[local-name()='Settlement']/*[local-name()='Latin']");
        mapper.AddFunctionMap(d => d.PermanentAddress.SettlementCode, node =>
        {

            XmlNode n = node.SelectSingleNode("./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Permanent']/*[local-name()='Settlement']");
            if (n == null || string.IsNullOrWhiteSpace(n.InnerText)) return null;
            XmlAttribute attr = n.Attributes["code"];
            if (attr != null)
            {
                return attr.Value;
            }
            return null;
        });

        mapper.AddPropertyMap(d => d.PermanentAddress.LocationName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Permanent']/*[local-name()='Location']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.PermanentAddress.LocationNameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Permanent']/*[local-name()='Location']/*[local-name()='Latin']");
        mapper.AddFunctionMap(d => d.PermanentAddress.LocationCode, node =>
        {

            XmlNode n = node.SelectSingleNode("./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Permanent']/*[local-name()='Location']");
            if (n == null || string.IsNullOrWhiteSpace(n.InnerText)) return null;
            XmlAttribute attr = n.Attributes["code"];
            if (attr != null)
            {
                return attr.Value;
            }
            return null;
        });
        mapper.AddPropertyMap(d => d.PermanentAddress.BuildingNumber, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Permanent']/*[local-name()='BuildingNumber']");
        mapper.AddPropertyMap(d => d.PermanentAddress.Entrance, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Permanent']/*[local-name()='Entrance']");
        mapper.AddPropertyMap(d => d.PermanentAddress.Floor, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Permanent']/*[local-name()='Floor']");
        mapper.AddPropertyMap(d => d.PermanentAddress.Apartment, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Permanent']/*[local-name()='Apartment']");

        //temporary address
        mapper.AddPropertyMap(d => d.TemporaryAddress.DistrictName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Temporary']/*[local-name()='District']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.TemporaryAddress.DistrictNameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Temporary']/*[local-name()='District']/*[local-name()='Latin']");
        mapper.AddPropertyMap(d => d.TemporaryAddress.MunicipalityName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Temporary']/*[local-name()='Municipality']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.TemporaryAddress.MunicipalityNameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Temporary']/*[local-name()='Municipality']/*[local-name()='Latin']");
        mapper.AddPropertyMap(d => d.TemporaryAddress.SettlementName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Temporary']/*[local-name()='Settlement']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.TemporaryAddress.SettlementNameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Temporary']/*[local-name()='Settlement']/*[local-name()='Latin']");
        mapper.AddFunctionMap(d => d.TemporaryAddress.SettlementCode, node =>
        {

            XmlNode n = node.SelectSingleNode("./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Temporary']/*[local-name()='Settlement']");
            if (n == null || string.IsNullOrWhiteSpace(n.InnerText)) return null;
            XmlAttribute attr = n.Attributes["code"];
            if (attr != null)
            {
                return attr.Value;
            }
            return null;
        });

        mapper.AddPropertyMap(d => d.TemporaryAddress.LocationName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Temporary']/*[local-name()='Location']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.TemporaryAddress.LocationNameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Temporary']/*[local-name()='Location']/*[local-name()='Latin']");
        mapper.AddFunctionMap(d => d.TemporaryAddress.LocationCode, node =>
        {

            XmlNode n = node.SelectSingleNode("./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Temporary']/*[local-name()='Location']");
            if (n == null || string.IsNullOrWhiteSpace(n.InnerText)) return null;
            XmlAttribute attr = n.Attributes["code"];
            if (attr != null)
            {
                return attr.Value;
            }
            return null;
        });
        mapper.AddPropertyMap(d => d.TemporaryAddress.BuildingNumber, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Temporary']/*[local-name()='BuildingNumber']");
        mapper.AddPropertyMap(d => d.TemporaryAddress.Entrance, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Temporary']/*[local-name()='Entrance']");
        mapper.AddPropertyMap(d => d.TemporaryAddress.Floor, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Temporary']/*[local-name()='Floor']");
        mapper.AddPropertyMap(d => d.TemporaryAddress.Apartment, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Temporary']/*[local-name()='Apartment']");

        //abroad address
        mapper.AddPropertyMap(d => d.PermanentAddressAbroad.NationalityName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Abroad']/*[local-name()='Country']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.PermanentAddressAbroad.NationalityNameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Abroad']/*[local-name()='Country']/*[local-name()='Latin']");
        mapper.AddFunctionMap(d => d.PermanentAddressAbroad.NationalityCode, node =>
        {

            XmlNode n = node.SelectSingleNode("./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Abroad']/*[local-name()='Country']");
            if (n == null || string.IsNullOrWhiteSpace(n.InnerText)) return null;
            XmlAttribute attr = n.Attributes["code"];
            if (attr != null)
            {
                return attr.Value;
            }
            return null;
        });
        mapper.AddPropertyMap(d => d.PermanentAddressAbroad.SettlementName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Abroad']/*[local-name()='Settlement']");
        mapper.AddPropertyMap(d => d.PermanentAddressAbroad.Street, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Abroad']/*[local-name()='Street']");

        mapper.AddPropertyMap(d => d.IdentityDocument.DocumentType, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Identity']/*[local-name()='DocumentType']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.IdentityDocument.DocumentTypeLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Identity']/*[local-name()='DocumentType']/*[local-name()='Latin']");
        mapper.AddPropertyMap(d => d.IdentityDocument.IdentityDocumentNumber, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Identity']/*[local-name()='IdentityNumber']");
        mapper.AddFunctionMap(p => p.IdentityDocument.IssueDate, node =>
        {
            XmlNode dateNode =
                   node.SelectSingleNode(
                       "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Identity']/*[local-name()='IssueDate']");
            if (dateNode != null && !string.IsNullOrWhiteSpace(dateNode.InnerText))
            {
                return DateTime.ParseExact(dateNode.InnerText, "yyyy-MM-dd", CultureInfo.InvariantCulture.DateTimeFormat);
            }
            return null;
        });
        mapper.AddPropertyMap(d => d.IdentityDocument.IssuePlace, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Identity']/*[local-name()='IssuePlace']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.IdentityDocument.IssuePlaceLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Identity']/*[local-name()='IssuePlace']/*[local-name()='Latin']");
        mapper.AddPropertyMap(d => d.IdentityDocument.IssuerName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Identity']/*[local-name()='IssuerName']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.IdentityDocument.IssuerNameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Identity']/*[local-name()='IssuerName']/*[local-name()='Latin']");

        // Актуален статус на документ
        mapper.AddPropertyMap(d => d.IdentityDocument.StatusCyrillic, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Identity']/*[local-name()='Status']/*[local-name()='Status']/*[local-name()='Cyrillic']");
        //StatusDate 
        mapper.AddFunctionMap(d => d.IdentityDocument.StatusDate, node =>
        {
            XmlNode dateNode =
                   node.SelectSingleNode(
                       "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Identity']/*[local-name()='Status']/*[local-name()='StatusDate']");
            if (dateNode != null && !string.IsNullOrWhiteSpace(dateNode.InnerText))
            {
                return DateTime.ParseExact(dateNode.InnerText, "yyyy-MM-dd", CultureInfo.InvariantCulture.DateTimeFormat);
            }
            return null;
        });
        //ReasonCyrillic
        mapper.AddPropertyMap(d => d.IdentityDocument.StatusReasonCyrillic, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Identity']/*[local-name()='Status']/*[local-name()='Reason']/*[local-name()='Cyrillic']");
        //RPTypeOfPermit
        mapper.AddPropertyMap(d => d.IdentityDocument.RPTypeOfPermit, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Identity']/*[local-name()='RPTypeOfPermit']");
        //RPRemarks
        mapper.AddCollectionMap(d => d.IdentityDocument.RPRemarks, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Identity']/*[local-name()='RPRemarks']/*[local-name()='RPRemark']");
        mapper.AddFunctionMap(d => d.IdentityDocument.RPRemarks, node =>
        {
            XmlNode remarks = node.SelectSingleNode(
                        "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Identity']/*[local-name()='RPRemarks']");

            if (remarks != null)
            {
                List<string> restrictionNode = remarks.ChildNodes.Cast<XmlNode>()
                    .Where(x => x.LocalName == "RPRemark")
                    .Select(x => x.InnerText)
                    .ToList();
                return restrictionNode;
            }
            else
            {
                return null;
            }

        });


        // Актуален статус на документ - travel
        mapper.AddPropertyMap(d => d.TravelDocument.StatusCyrillic, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Travel']/*[local-name()='Status']/*[local-name()='Status']/*[local-name()='Cyrillic']");
        //StatusDate - travel
        mapper.AddFunctionMap(d => d.TravelDocument.StatusDate, node =>
        {
            XmlNode dateNode =
                   node.SelectSingleNode(
                       "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Travel']/*[local-name()='Status']/*[local-name()='StatusDate']");
            if (dateNode != null && !string.IsNullOrWhiteSpace(dateNode.InnerText))
            {
                return DateTime.ParseExact(dateNode.InnerText, "yyyy-MM-dd", CultureInfo.InvariantCulture.DateTimeFormat);
            }
            return null;
        });
        //ReasonCyrillic - travel
        mapper.AddPropertyMap(d => d.TravelDocument.StatusReasonCyrillic, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Travel']/*[local-name()='Status']/*[local-name()='Reason']/*[local-name()='Cyrillic']");

        mapper.AddFunctionMap(d => d.IdentityDocument.ValidDate, node =>
        {
            XmlNode dateNode =
                   node.SelectSingleNode(
                       "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Identity']/*[local-name()='ValidDate']");
            if (dateNode != null && !string.IsNullOrWhiteSpace(dateNode.InnerText))
            {
                return DateTime.ParseExact(dateNode.InnerText, "yyyy-MM-dd", CultureInfo.InvariantCulture.DateTimeFormat);
            }
            return null;
        });

        //travel document
        mapper.AddPropertyMap(d => d.TravelDocument.DocumentType, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Travel']/*[local-name()='DocumentType']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.TravelDocument.DocumentTypeLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Travel']/*[local-name()='DocumentType']/*[local-name()='Latin']");
        mapper.AddPropertyMap(d => d.TravelDocument.TravelDocumentSeries, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Travel']/*[local-name()='TravelDocumentSeries']");
        mapper.AddPropertyMap(d => d.TravelDocument.TravelDocumentNumber, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Travel']/*[local-name()='TravelDocumentNumber']");
        mapper.AddFunctionMap(p => p.TravelDocument.IssueDate, node =>
        {
            XmlNode dateNode =
                   node.SelectSingleNode(
                       "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Travel']/*[local-name()='IssueDate']");
            if (dateNode != null && !string.IsNullOrWhiteSpace(dateNode.InnerText))
            {
                return DateTime.ParseExact(dateNode.InnerText, "yyyy-MM-dd", CultureInfo.InvariantCulture.DateTimeFormat);
            }
            return null;
        });
        mapper.AddPropertyMap(d => d.TravelDocument.IssuePlace, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Travel']/*[local-name()='IssuePlace']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.TravelDocument.IssuePlaceLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Travel']/*[local-name()='IssuePlace']/*[local-name()='Latin']");
        mapper.AddPropertyMap(d => d.TravelDocument.IssuerName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Travel']/*[local-name()='IssuerName']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.TravelDocument.IssuerNameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Travel']/*[local-name()='IssuerName']/*[local-name()='Latin']");

        mapper.AddFunctionMap(d => d.TravelDocument.ValidDate, node =>
        {
            XmlNode dateNode =
                   node.SelectSingleNode(
                       "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Travel']/*[local-name()='ValidDate']");
            if (dateNode != null && !string.IsNullOrWhiteSpace(dateNode.InnerText))
            {
                return DateTime.ParseExact(dateNode.InnerText, "yyyy-MM-dd", CultureInfo.InvariantCulture.DateTimeFormat);
            }
            return null;
        });

        mapper.AddFunctionMap(d => d.Height, node =>
        {
            XmlNode n = node.SelectSingleNode("./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='Height']");
            if (n != null && !string.IsNullOrWhiteSpace(n.InnerText))
            {
                return double.Parse(n.InnerText, CultureInfo.InvariantCulture.NumberFormat);
            }
            return null;
        });

        mapper.AddPropertyMap(d => d.EyesColor, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='EyesColor']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.EyesColorLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='EyesColor']/*[local-name()='Latin']");

        mapper.AddFunctionMap<byte[]>(d => d.Picture, node =>
        {
            XmlNode blobXML = node.SelectSingleNode("./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='Picture']");
            if (blobXML != null && !string.IsNullOrWhiteSpace(blobXML.InnerText))
            {
                return Convert.FromBase64String(blobXML.InnerText);
            }
            return null;
        });

        mapper.AddFunctionMap<byte[]>(d => d.IdentitySignature, node =>
        {
            XmlNode blobXML = node.SelectSingleNode("./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='IdentitySignature']");
            if (blobXML != null && !string.IsNullOrWhiteSpace(blobXML.InnerText))
            {
                return Convert.FromBase64String(blobXML.InnerText);
            }
            return null;
        });

        return mapper;
    }

    public async Task<ServiceResult<RegiXSearchResult>> GetPersonalIdentityV2Async(MVRGetPersonalIdentityV2 command)
    {
        #region Request
        TechnoLogica.RegiX.MVRBDSAdapterV2.Request request = new TechnoLogica.RegiX.MVRBDSAdapterV2.Request();
        request.Header.DateTime = System.DateTime.Now;
        request.Header.Operation = "0001";
        request.Header.OrganizationUnit = "CSEI-PIVR";
        request.Header.SystemID = "CSEI";
        request.Header.UserName = $"CorrelationId={command.CorrelationId}";

        //request.Header.OrganizationUnit = additionalParameters.OrganizationUnit;
        //request.Header.CallerIPAddress = additionalParameters.ClientIPAddress;
        //request.Header.CallContext = additionalParameters.CallContext != null ? additionalParameters.CallContext.ToString() : null;

        ////BiometricFlag - дали консуматорът има достъп до подпис и снимка
        request.Header.BiometricDataFlagSpecified = true;
        request.Header.BiometricDataFlag = false;


        TechnoLogica.RegiX.MVRBDSAdapterV2.PidAndDocIdType pd = new TechnoLogica.RegiX.MVRBDSAdapterV2.PidAndDocIdType();
        pd.DocID = command.IdentityDocumentNumber;
        pd.PID = command.EGN;
        request.RequestData.Item = pd;
        #endregion

        string response;
        try
        {
            var client = new NAIFImpl.IntSyncPortTypeClient();
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = false;
            doc.LoadXml(request.XmlSerialize());
            foreach (XmlNode node in doc)
            {
                if (node.NodeType == XmlNodeType.XmlDeclaration)
                {
                    doc.RemoveChild(node);
                    break;
                }
            }

            var callResponse = await client.CallAsync(new NAIFImpl.Call { Call1 = doc.OuterXml.Trim() });
            response = callResponse.Message;
            _logger.LogInformation("Call to NAIF {CommandName} succeeded.", nameof(MVRGetPersonalIdentityV2));
            _logger.LogDebug("NAIF {CommandName} Response {ResponseStr}", nameof(MVRGetPersonalIdentityV2), response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Call to NAIF {CommandName} failed.", nameof(MVRGetPersonalIdentityV2));
            TechnoLogica.RegiX.MVRBDSAdapterV2.PersonalIdentityInfoResponseType tempresult = new TechnoLogica.RegiX.MVRBDSAdapterV2.PersonalIdentityInfoResponseType();
            tempresult.ReturnInformations = new TechnoLogica.RegiX.MVRBDSAdapterV2.ReturnInformation();
            tempresult.ReturnInformations.ReturnCode = "1111";
            tempresult.ReturnInformations.Info = "Call unsuccessful.";

            return new ServiceResult<RegiXSearchResult>
            {
                StatusCode = System.Net.HttpStatusCode.BadGateway,
                Error = "NAIF connection failed.",
                Result = new FailedNAIFResult
                {
                    Response = new Dictionary<string, object> { { "PersonalIdentityInfoResponse", tempresult } }
                }
            };
        }


        #region Result

        TechnoLogica.RegiX.MVRBDSAdapterV2.PersonalIdentityInfoResponseType result = new TechnoLogica.RegiX.MVRBDSAdapterV2.PersonalIdentityInfoResponseType();

        try
        {
            XmlDocument resultXmlDoc = new XmlDocument();
            resultXmlDoc.LoadXml(response);
            XPathMapper<TechnoLogica.RegiX.MVRBDSAdapterV2.PersonalIdentityInfoResponseType> mapper = CreatePersonalIdentityInfoMapV2();
            mapper.Map(resultXmlDoc, result);

            _logger.LogInformation("NAIF {NAIFResponseType} mapping succeeded.", nameof(TechnoLogica.RegiX.MVRBDSAdapterV2.PersonalIdentityInfoResponseType));
            return new ServiceResult<RegiXSearchResult>
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Result = new NAIFResult
                {
                    Response = new Dictionary<string, object> { { "PersonalIdentityInfoResponse", result } }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during parsing {NAIFResponseType}.", nameof(TechnoLogica.RegiX.MVRBDSAdapterV2.PersonalIdentityInfoResponseType));
            TechnoLogica.RegiX.MVRBDSAdapterV2.PersonalIdentityInfoResponseType tempresult = new TechnoLogica.RegiX.MVRBDSAdapterV2.PersonalIdentityInfoResponseType();
            tempresult.ReturnInformations = new TechnoLogica.RegiX.MVRBDSAdapterV2.ReturnInformation();
            tempresult.ReturnInformations.ReturnCode = "1111";
            tempresult.ReturnInformations.Info = response;
            ObjectMapper<TechnoLogica.RegiX.MVRBDSAdapterV2.PersonalIdentityInfoResponseType, TechnoLogica.RegiX.MVRBDSAdapterV2.PersonalIdentityInfoResponseType> selfmapper = CreatePersonalIdentitySelfMapperV2();
            selfmapper.Map(tempresult, result);
            return new ServiceResult<RegiXSearchResult>
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Error = "Error during parsing.",
                Result = new FailedNAIFResult
                {
                    Response = new Dictionary<string, object> { { "PersonalIdentityInfoResponse", result } }
                }
            };
        }
        #endregion
    }

    public async Task<ServiceResult<CheckUidRestrictionsResult>> CheckUidRestrictionsAsync(CheckUidRestrictions command)
    {
        string url = Environment.GetEnvironmentVariable("EID_NAIF_EAU_ENDPOINT_ADDRESS") ?? "http://ws2016kapsht:9050/services/eau/bds.v2";
        string soapEnvelope = @$"<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:SOAP-ENC=""http://schemas.xmlsoap.org/soap/encoding/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
	                <SOAP-ENV:Body>
	                    <m:Call xmlns:m=""http://general.service.ict.mvr.bg/""><![CDATA[
	            <OtherServicesRequest xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://bg.mvr.ict/types"">
	                <Header>
	                    <MessageID xmlns=""http://bg.mvr.ict/common/types"">{command.CorrelationId}</MessageID>
	                    <DateTime xmlns=""http://bg.mvr.ict/common/types"">{DateTime.Now.ToString("o")}</DateTime>
	                    <SystemID xmlns=""http://bg.mvr.ict/common/types"">CSEI</SystemID>
	                    <Operation xmlns=""http://bg.mvr.ict/common/types"">000</Operation>
	                </Header>
	                <RequestData>
	                    <Citizen xmlns=""http://bg.mvr.ict/common/types"">
	                        <PID code=""{(int)command.UidType}"">{command.Uid}</PID>
	                        <HasPicture>false</HasPicture>
	                    </Citizen>
	                </RequestData>
	            </OtherServicesRequest>
	            ]]></m:Call>
	                </SOAP-ENV:Body>
	            </SOAP-ENV:Envelope>";

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml")
        };

        try
        {
            var hash = string.Empty;
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                byte[] hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes($"{command.UidType}{command.Uid}"));

                hash = BitConverter
                    .ToString(hashBytes)
                    .Replace("-", string.Empty);
            }
            var cacheKey = $"eID:PIVR:{nameof(CheckUidRestrictionsAsync)}:{hash}";
            var (responseStatusCode, responseContent) = await _cache.GetOrCreateAsync(cacheKey, async () =>
            {
                _logger.LogInformation("Request: {Request}", request.ToString());
                _logger.LogInformation("RequestBody: {Request}", request.Content.ReadAsStringAsync().Result);
                HttpResponseMessage res = await _httpClient.SendAsync(request);
                string responseContent = string.Empty;
                if (res.IsSuccessStatusCode)
                {
                    responseContent = await res.Content.ReadAsStringAsync();
                    await _cache.SetAsync(cacheKey, (res.StatusCode, responseContent), _cacheOptions.ExpireAfterInHours);
                }
                return (res.StatusCode, responseContent);
            });
            if (string.IsNullOrWhiteSpace(responseContent))
            {
                _logger.LogWarning("Getting check uid restrictions response failed with status code {StatusCode}", responseStatusCode);
                var errorMessage = "Failed getting check uid restrictions response.";
                return new ServiceResult<CheckUidRestrictionsResult>
                {
                    StatusCode = responseStatusCode,
                    Error = errorMessage,
                    Result = new CheckUidRestrictionsResultDTO { HasFailed = true, Error = errorMessage }
                };
            }

            _logger.LogDebug(responseContent);
            Envelope fullXml = null;
            try
            {
                fullXml = DeserializeFromString<Envelope>(responseContent);
                if (fullXml is null)
                {
                    _logger.LogError("Malformed envelope response.");
                    return new ServiceResult<CheckUidRestrictionsResult> { StatusCode = HttpStatusCode.BadGateway, Error = "Malformed envelope" };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Envelope deserialization failed.");
                return new ServiceResult<CheckUidRestrictionsResult> { StatusCode = HttpStatusCode.InternalServerError, Error = "Envelope deserialization failed" };
            }
            var rawOtherServicesResponse = fullXml.Body?.Message?.CData;
            if (string.IsNullOrWhiteSpace(rawOtherServicesResponse))
            {
                _logger.LogError("Malformed envelope body.");
                return new ServiceResult<CheckUidRestrictionsResult> { StatusCode = HttpStatusCode.BadGateway, Error = "Malformed envelope body" };
            }

            OtherServicesResponse otherServicesResponse = null;
            try
            {
                otherServicesResponse = DeserializeFromString<OtherServicesResponse>(rawOtherServicesResponse);
                if (otherServicesResponse is null)
                {
                    _logger.LogError("Malformed OtherServicesResponse response.");
                    return new ServiceResult<CheckUidRestrictionsResult> { StatusCode = HttpStatusCode.BadGateway, Error = "Malformed OtherServicesResponse response" };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OtherServicesResponse deserialization failed.");
                return new ServiceResult<CheckUidRestrictionsResult> { StatusCode = HttpStatusCode.InternalServerError, Error = "OtherServicesResponse deserialization failed" };
            }

            return new ServiceResult<CheckUidRestrictionsResult>
            {
                StatusCode = HttpStatusCode.OK,
                Result = new CheckUidRestrictionsResultDTO
                {
                    HasFailed = otherServicesResponse.ReturnInformation?.ReturnCode != "0",
                    Response = new CheckUidRestrictionsDataDTO
                    {
                        IsDead = otherServicesResponse.PersonIsDead(),
                        IsProhibited = otherServicesResponse.PersonIsProhibited(),
                        HasRevokedParentalRights = otherServicesResponse.PersonHasRevokedParentalRights()
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Getting check uid restrictions response failed.");
            return new ServiceResult<CheckUidRestrictionsResult> { StatusCode = HttpStatusCode.InternalServerError, Error = "Unhandled exception" };
        }
    }

    private static ObjectMapper<TechnoLogica.RegiX.MVRBDSAdapterV2.PersonalIdentityInfoResponseType, TechnoLogica.RegiX.MVRBDSAdapterV2.PersonalIdentityInfoResponseType> CreatePersonalIdentitySelfMapperV2()
    {
        var am = AccessMatrix.CreateForType(typeof(TechnoLogica.RegiX.MVRBDSAdapterV2.PersonalIdentityInfoResponseType));
        ObjectMapper<TechnoLogica.RegiX.MVRBDSAdapterV2.PersonalIdentityInfoResponseType, TechnoLogica.RegiX.MVRBDSAdapterV2.PersonalIdentityInfoResponseType> mapper = new ObjectMapper<TechnoLogica.RegiX.MVRBDSAdapterV2.PersonalIdentityInfoResponseType, TechnoLogica.RegiX.MVRBDSAdapterV2.PersonalIdentityInfoResponseType>(am);

        mapper.AddObjectMap((o) => o.ReturnInformations, (c) => c.ReturnInformations);
        mapper.AddPropertyMap((o) => o.ReturnInformations.ReturnCode, (c) => c.ReturnInformations.ReturnCode);
        mapper.AddPropertyMap((o) => o.ReturnInformations.Info, (c) => c.ReturnInformations.Info);

        return mapper;
    }
    private XPathMapper<TechnoLogica.RegiX.MVRBDSAdapterV2.PersonalIdentityInfoResponseType> CreatePersonalIdentityInfoMapV2()
    {
        var am = AccessMatrix.CreateForType(typeof(TechnoLogica.RegiX.MVRBDSAdapterV2.PersonalIdentityInfoResponseType));
        XPathMapper<TechnoLogica.RegiX.MVRBDSAdapterV2.PersonalIdentityInfoResponseType> mapper = new XPathMapper<TechnoLogica.RegiX.MVRBDSAdapterV2.PersonalIdentityInfoResponseType>(am);

        mapper.AddPropertyMap(d => d.ReturnInformations.ReturnCode, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ReturnInformation']/*[local-name()='ReturnCode']");
        mapper.AddPropertyMap(d => d.ReturnInformations.Info, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ReturnInformation']/*[local-name()='Info']");

        mapper.AddPropertyMap(d => d.EGN, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='DataBG']/*[local-name()='PIN']");
        mapper.AddPropertyMap(d => d.PersonNames.FirstName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='DataBG']/*[local-name()='Names']/*[local-name()='Cyrillic']/*[local-name()='First']");
        mapper.AddPropertyMap(d => d.PersonNames.Surname, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='DataBG']/*[local-name()='Names']/*[local-name()='Cyrillic']/*[local-name()='Surname']");
        mapper.AddPropertyMap(d => d.PersonNames.FamilyName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='DataBG']/*[local-name()='Names']/*[local-name()='Cyrillic']/*[local-name()='Family']");
        mapper.AddPropertyMap(d => d.PersonNames.FirstNameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='DataBG']/*[local-name()='Names']/*[local-name()='Latin']/*[local-name()='First']");
        mapper.AddPropertyMap(d => d.PersonNames.SurnameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='DataBG']/*[local-name()='Names']/*[local-name()='Latin']/*[local-name()='Surname']");
        mapper.AddPropertyMap(d => d.PersonNames.LastNameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='DataBG']/*[local-name()='Names']/*[local-name()='Latin']/*[local-name()='Family']");

        mapper.AddPropertyMap(d => d.DocumentType, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='DocumentType']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.DocumentTypeLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='DocumentType']/*[local-name()='Latin']");
        mapper.AddPropertyMap(d => d.IdentityDocumentNumber, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='IdentityNumber']");
        mapper.AddFunctionMap(p => p.IssueDate, node =>
        {
            XmlNode dateNode =
                   node.SelectSingleNode(
                       "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='IssueDate']");
            if (dateNode != null && !string.IsNullOrWhiteSpace(dateNode.InnerText))
            {
                return DateTime.ParseExact(dateNode.InnerText, "yyyy-MM-dd", CultureInfo.InvariantCulture.DateTimeFormat);
            }
            return null;
        });

        mapper.AddPropertyMap(d => d.IssuerPlace, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='IssuePlace']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.IssuerPlaceLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='IssuePlace']/*[local-name()='Latin']");
        mapper.AddPropertyMap(d => d.IssuerName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='IssuerName']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.IssuerNameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='IssuerName']/*[local-name()='Latin']");

        mapper.AddFunctionMap(d => d.ValidDate, node =>
        {
            XmlNode dateNode =
                   node.SelectSingleNode(
                       "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='ValidDate']");
            if (dateNode != null && !string.IsNullOrWhiteSpace(dateNode.InnerText))
            {
                return DateTime.ParseExact(dateNode.InnerText, "yyyy-MM-dd", CultureInfo.InvariantCulture.DateTimeFormat);
            }
            return null;
        });

        mapper.AddPropertyMap(d => d.DocumentActualStatus, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Status']/*[local-name()='Status']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.DocumentStatusReason, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Status']/*[local-name()='Reason']/*[local-name()='Cyrillic']");
        mapper.AddFunctionMap(d => d.ActualStatusDate, node =>
        {
            XmlNode dateNode =
                   node.SelectSingleNode(
                       "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='Status']/*[local-name()='StatusDate']");
            if (dateNode != null && !string.IsNullOrWhiteSpace(dateNode.InnerText))
            {
                return DateTime.ParseExact(dateNode.InnerText, "yyyy-MM-dd", CultureInfo.InvariantCulture.DateTimeFormat);
            }
            return null;
        });

        //DLCategories
        mapper.AddCollectionMap(d => d.DLCategоries, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='DLCategories']/*[local-name()='DLCategory']");
        mapper.AddPropertyMap(d => d.DLCategоries[0].Category, "./*[local-name()='Category']");
        mapper.AddFunctionMap(d => d.DLCategоries[0].Restrictions, node =>
        {
            if (node != null)
            {
                List<string> restrictionNode = node.ChildNodes.Cast<XmlNode>()
                .Where(x => x.LocalName == "Restrictions")
                .Select(x => x.InnerText)
                .ToList();
                return restrictionNode;
            }
            else
            {
                return null;
            }

        });
        mapper.AddFunctionMap(d => d.DLCategоries[0].DateCategory, node =>
        {
            XmlNode dateNode =
                   node.SelectSingleNode(
                       "./*[local-name()='DateCategory']");
            if (dateNode != null && !string.IsNullOrWhiteSpace(dateNode.InnerText))
            {
                return DateTime.ParseExact(dateNode.InnerText, "yyyy-MM-dd", CultureInfo.InvariantCulture.DateTimeFormat);
            }
            return null;
        });
        mapper.AddFunctionMap(d => d.DLCategоries[0].EndDateCategory, node =>
        {
            XmlNode dateNode =
                   node.SelectSingleNode(
                       "./*[local-name()='EndDateCategory']");
            if (dateNode != null && !string.IsNullOrWhiteSpace(dateNode.InnerText))
            {
                return DateTime.ParseExact(dateNode.InnerText, "yyyy-MM-dd", CultureInfo.InvariantCulture.DateTimeFormat);
            }
            return null;
        });


        //DLCommonRestrictions
        mapper.AddPropertyMap(d => d.DLCommonRestrictions, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='DLCommonRestrictions']");
        //DataForeignCitizen
        mapper.AddPropertyMap(d => d.DataForeignCitizen.PIN, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='DataForeignCitizen']/*[local-name()='PIN']");
        mapper.AddPropertyMap(d => d.DataForeignCitizen.PN, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='DataForeignCitizen']/*[local-name()='PN']");
        mapper.AddPropertyMap(d => d.DataForeignCitizen.Names.FirstName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='DataForeignCitizen']/*[local-name()='Names']/*[local-name()='Cyrillic']/*[local-name()='First']");
        mapper.AddPropertyMap(d => d.DataForeignCitizen.Names.FirstNameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='DataForeignCitizen']/*[local-name()='Names']/*[local-name()='Latin']/*[local-name()='First']");
        mapper.AddPropertyMap(d => d.DataForeignCitizen.Names.Surname, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='DataForeignCitizen']/*[local-name()='Names']/*[local-name()='Cyrillic']/*[local-name()='Surname']");
        mapper.AddPropertyMap(d => d.DataForeignCitizen.Names.SurnameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='DataForeignCitizen']/*[local-name()='Names']/*[local-name()='Latin']/*[local-name()='Surname']");
        mapper.AddPropertyMap(d => d.DataForeignCitizen.Names.FamilyName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='DataForeignCitizen']/*[local-name()='Names']/*[local-name()='Cyrillic']/*[local-name()='Family']");
        mapper.AddPropertyMap(d => d.DataForeignCitizen.Names.LastNameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='DataForeignCitizen']/*[local-name()='Names']/*[local-name()='Latin']/*[local-name()='Family']");
        mapper.AddCollectionMap(d => d.DataForeignCitizen.NationalityList, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='DataForeignCitizen']/*[local-name()='NationalityList']/*[local-name()='Nationality']");
        mapper.AddPropertyMap(d => d.DataForeignCitizen.NationalityList[0].NationalityNameLatin, "./*[local-name()='Latin']");
        mapper.AddPropertyMap(d => d.DataForeignCitizen.NationalityList[0].NationalityName, "./*[local-name()='Cyrillic']");
        mapper.AddFunctionMap(d => d.DataForeignCitizen.NationalityList[0].NationalityCode, node =>
        {
            XmlAttribute attr = node.Attributes["code"];
            if (attr != null)
            {
                return attr.Value;
            }
            return null;
        });

        mapper.AddPropertyMap(d => d.DataForeignCitizen.Gender.Cyrillic, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='DataForeignCitizen']/*[local-name()='Gender']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.DataForeignCitizen.Gender.Latin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='DataForeignCitizen']/*[local-name()='Gender']/*[local-name()='Latin']");
        mapper.AddFunctionMap(d => d.DataForeignCitizen.Gender.GenderCode, node =>
        {

            XmlNode n = node.SelectSingleNode("./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='DataForeignCitizen']/*[local-name()='Gender']");
            if (n == null || string.IsNullOrWhiteSpace(n.InnerText)) return null;
            XmlAttribute attr = n.Attributes["code"];
            if (attr != null)
            {
                return attr.Value;
            }
            return null;
        });

        mapper.AddFunctionMap(d => d.DataForeignCitizen.BirthDate, node =>
        {
            XmlNode dateNode =
                   node.SelectSingleNode(
                       "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='DataForeignCitizen']/*[local-name()='BirthDate']");
            if (dateNode != null && !string.IsNullOrWhiteSpace(dateNode.InnerText))
            {
                return DateTime.ParseExact(dateNode.InnerText, "yyyy-MM-dd", CultureInfo.InvariantCulture.DateTimeFormat);
            }
            return null;
        });
        //RPRemarks
        mapper.AddCollectionMap(d => d.RPRemarks, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='RPRemarks']/*[local-name()='RPRemark']");
        mapper.AddFunctionMap(d => d.RPRemarks, node =>
        {
            XmlNode remarks = node.SelectSingleNode(
                        "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='RPRemarks']");
            if (remarks != null)
            {
                List<string> restrictionNode = remarks.ChildNodes.Cast<XmlNode>()
                .Where(x => x.LocalName == "RPRemark")
                .Select(x => x.InnerText)
                .ToList();
                return restrictionNode;
            }
            else
            {
                return null;
            }
        });
        //RPTypeofPermit
        mapper.AddPropertyMap(d => d.RPTypeofPermit, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Document']/*[local-name()='RPTypeOfPermit']");

        mapper.AddFunctionMap(d => d.BirthDate, node =>
        {
            XmlNode dateNode =
                   node.SelectSingleNode(
                       "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='DataBG']/*[local-name()='BirthDate']");
            if (dateNode != null && !string.IsNullOrWhiteSpace(dateNode.InnerText))
            {
                return DateTime.ParseExact(dateNode.InnerText, "yyyy-MM-dd", CultureInfo.InvariantCulture.DateTimeFormat);
            }
            return null;
        });
        mapper.AddPropertyMap(d => d.BirthPlace.CountryName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='BirthPlace']/*[local-name()='Country']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.BirthPlace.CountryNameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='BirthPlace']/*[local-name()='Country']/*[local-name()='Latin']");
        mapper.AddFunctionMap(d => d.BirthPlace.CountryCode, node =>
        {

            XmlNode n = node.SelectSingleNode("./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='BirthPlace']/*[local-name()='Country']");
            if (n == null || string.IsNullOrWhiteSpace(n.InnerText)) return null;
            XmlAttribute attr = n.Attributes["code"];
            if (attr != null)
            {
                return attr.Value;
            }
            return null;
        });
        mapper.AddPropertyMap(d => d.BirthPlace.TerritorialUnitName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='BirthPlace']/*[local-name()='TerritorialUnitName']");
        mapper.AddPropertyMap(d => d.BirthPlace.DistrictName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='BirthPlace']/*[local-name()='DistrictName']");
        mapper.AddPropertyMap(d => d.BirthPlace.MunicipalityName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='BirthPlace']/*[local-name()='MunicipalityName']");

        mapper.AddPropertyMap(d => d.GenderName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='DataBG']/*[local-name()='Gender']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.GenderNameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='DataBG']/*[local-name()='Gender']/*[local-name()='Latin']");

        mapper.AddCollectionMap(d => d.NationalityList, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='NationalityList']/*[local-name()='Nationality']");
        mapper.AddFunctionMap(d => d.NationalityList[0].NationalityCode, node =>
        {
            XmlAttribute attr = node.Attributes["code"];
            if (attr != null)
            {
                return attr.Value;
            }
            return null;
        });

        mapper.AddPropertyMap(d => d.NationalityList[0].NationalityName, "./*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.NationalityList[0].NationalityNameLatin, "./*[local-name()='Latin']");

        mapper.AddPropertyMap(d => d.PermanentAddress.DistrictName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='District']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.PermanentAddress.DistrictNameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='District']/*[local-name()='Latin']");
        mapper.AddPropertyMap(d => d.PermanentAddress.MunicipalityName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Municipality']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.PermanentAddress.MunicipalityNameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Municipality']/*[local-name()='Latin']");
        mapper.AddPropertyMap(d => d.PermanentAddress.SettlementName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Settlement']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.PermanentAddress.SettlementNameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Settlement']/*[local-name()='Latin']");
        mapper.AddFunctionMap(d => d.PermanentAddress.SettlementCode, node =>
        {

            XmlNode n = node.SelectSingleNode("./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Settlement']");
            if (n == null || string.IsNullOrWhiteSpace(n.InnerText)) return null;
            XmlAttribute attr = n.Attributes["code"];
            if (attr != null)
            {
                return attr.Value;
            }
            return null;
        });

        mapper.AddPropertyMap(d => d.PermanentAddress.LocationName, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Location']/*[local-name()='Cyrillic']");
        mapper.AddPropertyMap(d => d.PermanentAddress.LocationNameLatin, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Location']/*[local-name()='Latin']");
        mapper.AddFunctionMap(d => d.PermanentAddress.LocationCode, node =>
        {

            XmlNode n = node.SelectSingleNode("./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Location']");
            if (n == null || string.IsNullOrWhiteSpace(n.InnerText)) return null;
            XmlAttribute attr = n.Attributes["code"];
            if (attr != null)
            {
                return attr.Value;
            }
            return null;
        });
        mapper.AddPropertyMap(d => d.PermanentAddress.BuildingNumber, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='BuildingNumber']");
        mapper.AddPropertyMap(d => d.PermanentAddress.Entrance, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Entrance']");
        mapper.AddPropertyMap(d => d.PermanentAddress.Floor, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Floor']");
        mapper.AddPropertyMap(d => d.PermanentAddress.Apartment, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='Address']/*[local-name()='Apartment']");

        mapper.AddFunctionMap(d => d.Height, node =>
        {
            XmlNode n = node.SelectSingleNode("./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='Height']");
            if (n != null && !string.IsNullOrWhiteSpace(n.InnerText))
            {
                return double.Parse(n.InnerText, CultureInfo.InvariantCulture.NumberFormat);
            }
            return null;
        });

        mapper.AddPropertyMap(d => d.EyesColor, "./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='EyesColor']");

        mapper.AddFunctionMap<byte[]>(d => d.Picture, node =>
        {
            XmlNode blobXML = node.SelectSingleNode("./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='Picture']");
            if (blobXML != null && !string.IsNullOrWhiteSpace(blobXML.InnerText))
            {
                return Convert.FromBase64String(blobXML.InnerText);
            }
            return null;
        });

        mapper.AddFunctionMap<byte[]>(d => d.IdentitySignature, node =>
        {
            XmlNode blobXML = node.SelectSingleNode("./*[local-name()='PersonalIdentityInfoResponse']/*[local-name()='ResponseData']/*[local-name()='PersonData']/*[local-name()='IdentitySignature']");
            if (blobXML != null && !string.IsNullOrWhiteSpace(blobXML.InnerText))
            {
                return Convert.FromBase64String(blobXML.InnerText);
            }
            return null;
        });

        return mapper;
    }

    public static T DeserializeFromString<T>(string xml)
    {
        var serializer = new XmlSerializer(typeof(T));
        using (var stringReader = new StringReader(xml))
        {
            return (T)serializer.Deserialize(stringReader);
        }
    }

}
internal class NAIFResult : RegiXSearchResult
{
    public IDictionary<string, object> Response { get; set; } = new Dictionary<string, object>();
    public virtual bool HasFailed { get; set; } = false;
    public virtual string? Error { get; set; }
}
internal class CheckUidRestrictionsResultDTO : CheckUidRestrictionsResult
{
    public CheckUidRestrictionsState Response { get; set; }
    public virtual bool HasFailed { get; set; } = false;
    public virtual string? Error { get; set; }
}
internal class CheckUidRestrictionsDataDTO : CheckUidRestrictionsState

{
    public bool IsProhibited { get; set; }
    public bool IsDead { get; set; }
    public bool HasRevokedParentalRights { get; set; }
}

internal class FailedNAIFResult : NAIFResult
{
    public override bool HasFailed { get; set; } = true;
    public override string? Error { get; set; } = "Unexpected error";
}

public interface INAIFService
{
    Task<ServiceResult<RegiXSearchResult>> GetForeignIdentityV2Async(MVRGetForeignIdentityV2 command);
    Task<ServiceResult<RegiXSearchResult>> GetPersonalIdentityV2Async(MVRGetPersonalIdentityV2 command);
    Task<ServiceResult<CheckUidRestrictionsResult>> CheckUidRestrictionsAsync(CheckUidRestrictions command);
}
