using eID.PIVR.Contracts.Commands;
using eID.PIVR.Contracts.Results;
using eID.PIVR.Service.RegiXResponses;
using eID.PIVR.Service.Validators;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace eID.PIVR.Service;

public class RegiXFakeService : BaseService, IRegiXService
{
    private const string _representativeField1Id = "00100";
    private const string _representativeField2Id = "00101";
    private const string _representativeField3Id = "00102";
    private const string _wayOfRepresentationFieldId = "00110";
    private const string _inactiveCompanyFieldId = "00260";

    private const string NotFoundReturnCode = "0100";
    private const string FoundReturnCode = "0000";

    private readonly ILogger<RegiXFakeService> _logger;

    public RegiXFakeService(ILogger<RegiXFakeService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResult<RegixSearchResultDTO>> SearchAsync(RegiXSearchCommand message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new RegiXSearchCommandValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<RegixSearchResultDTO>(validationResult.Errors);
        }

        var operation = JsonConvert.DeserializeObject<JObject>(message.Command)?["operation"]?.Value<string>() ?? string.Empty;
        var response = operation switch
        {
            "TechnoLogica.RegiX.AVTRAdapter.APIService.ITRAPI.GetActualStateV3" => GetTRActualStateV3Response(message),
            "TechnoLogica.RegiX.MVRERChAdapter.APIService.IMVRERChAPI.GetForeignIdentityV2" => GetMVRForeignIdentityV2Response(message),
            "TechnoLogica.RegiX.MVRBDSAdapter.APIService.IMVRBDSAPI.GetPersonalIdentityV2" => GetMVRPersonalIdentityV2Response(message),
            "TechnoLogica.RegiX.GraoNBDAdapter.APIService.INBDAPI.RelationsSearch" => GetGRAORelationsSearchResponse(message),
            "TechnoLogica.RegiX.AVBulstat2Adapter.APIService.IAVBulstat2API.GetStateOfPlay" => GetStateOfPlayResponse(message),
            "TechnoLogica.RegiX.AVBulstat2Adapter.APIService.IAVBulstat2API.FetchNomenclatures" => FetchNomenclaturesResponse(message),
            _ => throw new InvalidOperationException($"Not expected operation value: '{operation}'"),
        };

        if (response is null)
        {
            _logger.LogWarning("Regix caller returned null.");
            return new ServiceResult<RegixSearchResultDTO>
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Error = "Something went wrong"
            };
        }
        if (response.HasFailed)
        {
            return new ServiceResult<RegixSearchResultDTO>
            {
                StatusCode = System.Net.HttpStatusCode.BadGateway,
                Error = response.Error,
                Result = response
            };
        }
        return Ok(response);
    }

    private static RegixSearchResultDTO? GetMVRPersonalIdentityV2Response(RegiXSearchCommand message)
    {
        if (JObject.Parse(message.Command)?["argument"]?["parameters"] is not JArray parameters)
        {
            return new RegixSearchResultDTO
            {
                HasFailed = true,
                Response = new Dictionary<string, object?> { { "PersonalIdentityInfoResponse", new PersonalIdentityInfoResponseType() } }
            };
        }

        var identityDocumentNumber = GetJParmaterStringValue(parameters, "IdentityDocumentNumber");
        var egn = GetJParmaterStringValue(parameters, "EGN");
        var egn_identityDocumentNumber = $"{egn}_{identityDocumentNumber}";

        PersonalIdentityInfoResponseType? result = egn_identityDocumentNumber switch
        {
            // Internal service error
            "6502040207_0110301638" => throw new ArgumentException("IdentityDocumentNumber and EGN are throwing the exception"),
            // RegiX internal error
            "0243206060_0112309690" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = "1001",
                    Info = "Something went wrong"
                }
            },
            // NAIF kind of response 
            "0952076318_MB0032158" => new SortableDatesPersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(2009, 12, 07, 0, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BGR",
                    DistrictName = "София",
                    MunicipalityName = "СТОЛИЧНА",
                    TerritorialUnitName = "ГР.СОФИЯ Общ.СТОЛИЧНА Обл.СОФИЯ"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                DocumentActualStatus = "ВАЛИДЕН",
                IdentityDocumentNumber = "MB0032158",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР София/MoI BGR",
                IssuerNameLatin = "MoI BGR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "0952076318",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "BGR",
                            NationalityName = "България",
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "ДАРЕНА",
                    FirstNameLatin = "DARENA",
                    Surname = "ИВОВА",
                    SurnameLatin = "IVOVA",
                    FamilyName = "РАХОВА",
                    LastNameLatin = "RAHOVA"
                },
                DLCategоries = new DLCategory[] { },
                DataForeignCitizen = new ForeignCitizenType
                {
                    Names = new PersonNames { },
                    NationalityList = new Nationality[] { },
                    Gender = new GenderType { }
                },
                RPRemarks = new string[] { },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "СОФИЯ",
                    MunicipalityName = "СТОЛИЧНА",
                    SettlementCode = "68134",
                    SettlementName = "ГР.СОФИЯ",
                    LocationCode = "80217",
                    LocationName = "Ж.К.ДЪРВЕНИЦА",
                    BuildingNumber = "43",
                    Entrance = "B",
                    Apartment = "40",
                    Floor = "8"
                },
                Picture = "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",
                EyesColor = "ЗЕЛЕНИ",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2028, 1, 9, 0, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // ОК 
            "7009287574_MB0032158" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1970, 09, 28, 0, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BGR",
                    DistrictName = "София",
                    MunicipalityName = "СТОЛИЧНА",
                    TerritorialUnitName = "ГР.СОФИЯ Общ.СТОЛИЧНА Обл.СОФИЯ"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                DocumentActualStatus = "ВАЛИДЕН",
                IdentityDocumentNumber = "MB0032158",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "7009287574",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "BGR",
                            NationalityName = "България",
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Милена",
                    FirstNameLatin = "Milena",
                    Surname = "Петрова",
                    SurnameLatin = "Petrova",
                    FamilyName = "Георгиева",
                    LastNameLatin = "Georgieva"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "СОФИЯ",
                    MunicipalityName = "СТОЛИЧНА",
                    SettlementCode = "68134",
                    SettlementName = "ГР.СОФИЯ",
                    LocationCode = "80217",
                    LocationName = "Ж.К.ДЪРВЕНИЦА",
                    BuildingNumber = "43",
                    Entrance = "B",
                    Apartment = "40",
                    Floor = "8"
                },
                Picture = "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",
                EyesColor = "ЗЕЛЕНИ",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2028, 1, 9, 0, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // ОК 
            "9604270980_MB0032158" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1996, 04, 27, 0, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BGR",
                    DistrictName = "София",
                    MunicipalityName = "СТОЛИЧНА",
                    TerritorialUnitName = "ГР.СОФИЯ Общ.СТОЛИЧНА Обл.СОФИЯ"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                DocumentActualStatus = "ВАЛИДЕН",
                IdentityDocumentNumber = "MB0032158",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Мъж",
                GenderNameLatin = "Male",
                EGN = "9604270980",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "BGR",
                            NationalityName = "България",
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Станислав",
                    FirstNameLatin = "Stanislav",
                    Surname = "Недялков",
                    SurnameLatin = "Nedyalkov",
                    FamilyName = "Недялков",
                    LastNameLatin = "Nedyalkov"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "СОФИЯ",
                    MunicipalityName = "СТОЛИЧНА",
                    SettlementCode = "68134",
                    SettlementName = "ГР.СОФИЯ",
                    LocationCode = "80217",
                    LocationName = "Ж.К.ДЪРВЕНИЦА",
                    BuildingNumber = "43",
                    Entrance = "B",
                    Apartment = "40",
                    Floor = "8"
                },
                Picture = "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",
                EyesColor = "ЗЕЛЕНИ",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2028, 1, 9, 0, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // Управител на фирма от тестовия Булстат 
            "6908080808_MB0032158" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1969, 08, 08, 0, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BGR",
                    DistrictName = "София",
                    MunicipalityName = "СТОЛИЧНА",
                    TerritorialUnitName = "ГР.СОФИЯ Общ.СТОЛИЧНА Обл.СОФИЯ"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                DocumentActualStatus = "ВАЛИДЕН",
                IdentityDocumentNumber = "MB0032158",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "6908080808",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "BGR",
                            NationalityName = "България",
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Мария",
                    FirstNameLatin = "Mariya",
                    Surname = "Георгиева",
                    SurnameLatin = "Georgieva",
                    FamilyName = "Георгиева",
                    LastNameLatin = "Georgieva"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "СОФИЯ",
                    MunicipalityName = "СТОЛИЧНА",
                    SettlementCode = "68134",
                    SettlementName = "ГР.СОФИЯ",
                    LocationCode = "80217",
                    LocationName = "Ж.К.ДЪРВЕНИЦА",
                    BuildingNumber = "43",
                    Entrance = "B",
                    Apartment = "40",
                    Floor = "8"
                },
                Picture = "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",
                EyesColor = "ЗЕЛЕНИ",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2028, 1, 9, 0, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK
            "7608097458_MB0038479" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1976, 8, 9, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0038479",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "7608097458",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Камелия",
                    FirstNameLatin = "Kamelia",
                    Surname = "Димова",
                    SurnameLatin = "Dimova",
                    FamilyName = "Захариева",
                    LastNameLatin = "Zaharieva"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                Picture = "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",
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK
            "7111106259_MB0038738" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1971, 11, 10, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0038738",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "7111106259",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Юлия",
                    FirstNameLatin = "Julia",
                    Surname = "Руменова",
                    SurnameLatin = "Rumenova",
                    FamilyName = "Бозукова",
                    LastNameLatin = "Bozukova"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK
            "5810189203_MB0004538" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1958, 10, 01, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0004538",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "5810189203",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Мирела",
                    FirstNameLatin = "Mirela",
                    Surname = "Руменова",
                    SurnameLatin = "Rumenova",
                    FamilyName = "Янкова",
                    LastNameLatin = "Yankova"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK
            "9009206827_MB0032129" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1990, 09, 20, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0032129",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Мъж",
                GenderNameLatin = "Male",
                EGN = "9009206827",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Александър",
                    FirstNameLatin = "Aleksandar",
                    Surname = "Бойков",
                    SurnameLatin = "Boykov",
                    FamilyName = "Стоилов",
                    LastNameLatin = "Stoilov"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK
            "5703197876_MB0024169" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1957, 03, 19, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0024169",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "5703197876",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Бетина",
                    FirstNameLatin = "Betina",
                    Surname = "Иванова",
                    SurnameLatin = "Ivanova",
                    FamilyName = "Минкова",
                    LastNameLatin = "Minkova"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK
            "6910316772_MB0024169" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1969, 10, 31, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0024169",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "691031677",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Милена",
                    FirstNameLatin = "Milena",
                    Surname = "Кирилова",
                    SurnameLatin = "Kirilova",
                    FamilyName = "Черпанлиева",
                    LastNameLatin = "Cherpanlieva"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK
            "9002182960_MB0024172" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1990, 02, 18, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB002417",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "9002182960",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Калина",
                    FirstNameLatin = "Kalina",
                    Surname = "Руменова",
                    SurnameLatin = "Rumenova",
                    FamilyName = "Петрова",
                    LastNameLatin = "Petrova"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK
            "9209200960_MB0038767" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1992, 09, 20, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0038767",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Мъж",
                GenderNameLatin = "Male",
                EGN = "9209200960",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Стелиян",
                    FirstNameLatin = "Steliyan",
                    Surname = "Тонев",
                    SurnameLatin = "Tonev",
                    FamilyName = "Хаджиденев",
                    LastNameLatin = "Hadzhidenev"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK - за тестващите от МВР
            "9508010139_MB0032132" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1995, 08, 01, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB003213",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "9508010139",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Славина",
                    FirstNameLatin = "Slavina",
                    Surname = "Георгиева",
                    SurnameLatin = "Georgieva",
                    FamilyName = "Иванова",
                    LastNameLatin = "Ivanova"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK - за тестващите от МВР
            "8904139162_MB0038343" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1989, 04, 13, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0038343",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "890413916",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Ивайло",
                    FirstNameLatin = "Ivaylo",
                    Surname = "Митев",
                    SurnameLatin = "Mitev",
                    FamilyName = "Драгнев",
                    LastNameLatin = "Dragnev"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK - за тестващите от МВР
            "8904163370_MB0015841" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1989, 04, 16, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0015841",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "8904163370",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Михаела",
                    FirstNameLatin = "Mihaela",
                    Surname = "Иванова",
                    SurnameLatin = "Ivanova",
                    FamilyName = "Петрова",
                    LastNameLatin = "Petrova"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK - за тестващите от МВР
            "8904302921_MB0032103" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1989, 04, 30, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0032103",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "8904302921",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Христо",
                    FirstNameLatin = "Hristo",
                    Surname = "Колев",
                    SurnameLatin = "Kolev",
                    FamilyName = "Бонев",
                    LastNameLatin = "Bonev"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK - за тестващите от МВР
            "8910135309_MB0032093" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1989, 10, 13, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0032093",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Мъж",
                GenderNameLatin = "Male",
                EGN = "8910135309",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Милен",
                    FirstNameLatin = "Milen",
                    Surname = "Васев",
                    SurnameLatin = "Vassev",
                    FamilyName = "Канев",
                    LastNameLatin = "Kanev"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK - за тестващите от МВР
            "8912081958_MB0032080" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1989, 12, 08, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0032080",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "8912081958",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Пепа",
                    FirstNameLatin = "Pepa",
                    Surname = "Милева",
                    SurnameLatin = "Mileva",
                    FamilyName = "Радева",
                    LastNameLatin = "Radeva"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK - за тестващите от МВР
            "9503245357_MB0023131" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1995, 03, 24, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0023131",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "9503245357",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Кера",
                    FirstNameLatin = "Kera",
                    Surname = "Милова",
                    SurnameLatin = "Milova",
                    FamilyName = "Иванова",
                    LastNameLatin = "Ivanova"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK - Вальо
            "8304029040_MB0032145" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1983, 04, 02, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0032145",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Мъж",
                GenderNameLatin = "Male",
                EGN = "8304029040",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Валентин",
                    FirstNameLatin = "Valentin",
                    Surname = "Николаев",
                    SurnameLatin = "Nikolaev",
                    FamilyName = "Николов",
                    LastNameLatin = "Nikolov"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 04, 04, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK - за тестващите от МВР
            "9508010133_LT0000500" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1995, 08, 01, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "LT0000500",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "9508010133",
                NationalityList = new Nationality[]
                {
                    new Nationality
                    {
                        NationalityCode = "БЛГ",
                        NationalityCode2 = "БГ",
                        NationalityName = "България",
                        NationalityName2 = "България",
                        NationalityNameLatin = "Bulgaria"
                    }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Лиляна",
                    FirstNameLatin = "Lilyana",
                    Surname = "Томова",
                    SurnameLatin = "Tomova",
                    FamilyName = "Асенова",
                    LastNameLatin = "Asenova"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 04, 04, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK - за тестващите от МВР
            "9508010133_LT0000498" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1995, 08, 01, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "LT0000498",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "9508010133",
                NationalityList = new Nationality[]
                {
                    new Nationality
                    {
                        NationalityCode = "БЛГ",
                        NationalityCode2 = "БГ",
                        NationalityName = "България",
                        NationalityName2 = "България",
                        NationalityNameLatin = "Bulgaria"
                    }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Амелия",
                    FirstNameLatin = "Amelia",
                    Surname = "Първанова",
                    SurnameLatin = "Parvanova",
                    FamilyName = "Костова",
                    LastNameLatin = "Kostova"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 04, 04, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK - за тестващите от МВР
            "9508010133_LT0000479" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1995, 08, 01, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "LT0000479",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "9508010133",
                NationalityList = new Nationality[]
                {
                    new Nationality
                    {
                        NationalityCode = "БЛГ",
                        NationalityCode2 = "БГ",
                        NationalityName = "България",
                        NationalityName2 = "България",
                        NationalityNameLatin = "Bulgaria"
                    }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Кети",
                    FirstNameLatin = "Keti",
                    Surname = "Пимпирева",
                    SurnameLatin = "Pimpireva",
                    FamilyName = "Мунгова",
                    LastNameLatin = "Mungova"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 04, 04, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK - Иван
            "8611056461_MB0032080" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1986, 11, 05, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0032080",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Мъж",
                GenderNameLatin = "Male",
                EGN = "8611056461",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "Българин",
                            NationalityName2 = "Българин2",
                            NationalityNameLatin = "Bulgarian"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Иван",
                    FirstNameLatin = "Ivan",
                    Surname = "Тодоров",
                    SurnameLatin = "Todorov",
                    FamilyName = "Тодоров",
                    LastNameLatin = "Todorov"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "22",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK
            "8709299269_MB0038479" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1987, 09, 29, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0038479",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Мъж",
                GenderNameLatin = "Male",
                EGN = "8709299269",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Роман",
                    FirstNameLatin = "Roman",
                    Surname = "Анатолийович",
                    SurnameLatin = "Anatoliyovich",
                    FamilyName = "Криволапов",
                    LastNameLatin = "Kryvolapov"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK
            "7002096529_MB0038479" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1970, 02, 09, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0038479",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Мъж",
                GenderNameLatin = "Male",
                EGN = "7002096529",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Георги",
                    FirstNameLatin = "Georgi",
                    Surname = "Георгиев",
                    SurnameLatin = "Georgiev",
                    FamilyName = "Димитров",
                    LastNameLatin = "Dimitrov"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK
            "9204291016_MB0038770" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1992, 04, 29, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0038770",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "9204291016",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Теодора",
                    FirstNameLatin = "Teodora",
                    Surname = "Генадиева",
                    SurnameLatin = "Genadieva",
                    FamilyName = "Георгиева",
                    LastNameLatin = "Georgieva"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK
            "8912160954_MB0038479" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1989, 12, 16, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0038479",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "8912160954",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Ясена",
                    FirstNameLatin = "Yasena",
                    Surname = "Лъчезарова",
                    SurnameLatin = "Lachezarova",
                    FamilyName = "Дойчинова",
                    LastNameLatin = "Doychinova"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK
            "9801051029_MB0038479" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1998, 01, 05, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0038479",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Мъж",
                GenderNameLatin = "Male",
                EGN = "9801051029",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Красен",
                    FirstNameLatin = "Krasen",
                    Surname = "Ивелинов",
                    SurnameLatin = "Ivelinov",
                    FamilyName = "Филипов",
                    LastNameLatin = "Filipov"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK
            "9211185300_MB0038479" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1992, 11, 18, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0038479",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Мъж",
                GenderNameLatin = "Male",
                EGN = "9211185300",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Сунай",
                    FirstNameLatin = "Sunay",
                    Surname = "Салим",
                    SurnameLatin = "Salim",
                    FamilyName = "Гюлтекин",
                    LastNameLatin = "Gyultekin"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                Picture = "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",
                EyesColor = "сини",
                Height = 1.75,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK
            "9110036443_MB0038479" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1991, 10, 03, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0038479",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Мъж",
                GenderNameLatin = "Male",
                EGN = "9110036443",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Петър",
                    FirstNameLatin = "Petar",
                    Surname = "Чавдаров",
                    SurnameLatin = "Chavdarov",
                    FamilyName = "Алексиев",
                    LastNameLatin = "Aleksiev"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.75,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK
            "9108116358_MB0032129" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1991, 8, 11, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0032129",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "9108116358",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Лилиана",
                    FirstNameLatin = "Liliana",
                    Surname = "Георгиева",
                    SurnameLatin = "Georgieva",
                    FamilyName = "Крумова",
                    LastNameLatin = "Krumova"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK - wrong PUK MVR
            "7306224491_MB0035236" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1973, 6, 22, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0035236",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "7306224491",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Лилиана",
                    FirstNameLatin = "Liliana",
                    Surname = "Георгиева",
                    SurnameLatin = "Georgieva",
                    FamilyName = "Крумова",
                    LastNameLatin = "Krumova"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK - wrong PUK - MVR
            "5205014533_MB0035249" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1952, 5, 1, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0035249",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "5205014533",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Мими",
                    FirstNameLatin = "Mimi",
                    Surname = "Георгиева",
                    SurnameLatin = "Georgieva",
                    FamilyName = "Крумова",
                    LastNameLatin = "Krumova"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK - wrong PUK - MVR
            "6107110895_MB0035304" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1961, 7, 11, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0035304",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "6107110895",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Рени",
                    FirstNameLatin = "Reni",
                    Surname = "Георгиева",
                    SurnameLatin = "Georgieva",
                    FamilyName = "Крумова",
                    LastNameLatin = "Krumova"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK - wrong PUK MVR
            "5101014260_MB0035294" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1951, 1, 1, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0035294",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "5101014260",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Пепи",
                    FirstNameLatin = "Pepi",
                    Surname = "Георгиева",
                    SurnameLatin = "Georgieva",
                    FamilyName = "Крумова",
                    LastNameLatin = "Krumova"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK
            "5303140454_MB0032129" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1953, 3, 14, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0032129",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "5303140454",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Калина",
                    FirstNameLatin = "Kalina",
                    FamilyName = "Иванова",
                    LastNameLatin = "Ivanova"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK
            "8802214026_MB0038741" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1988, 02, 21, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0038741",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Мъж",
                GenderNameLatin = "Male",
                EGN = "8802214026",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Калоян",
                    FirstNameLatin = "Kaloyan",
                    Surname = "Младенов",
                    SurnameLatin = "Mladenov",
                    FamilyName = "Маринов",
                    LastNameLatin = "Marinov"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                Picture = "/9j/4QC8RXhpZgAASUkqAAgAAAAGABIBAwABAAAAAQAAABoBBQABAAAAVgAAABsBBQABAAAAXgAAACgBAwABAAAAAgAAABMCAwABAAAAAQAAAGmHBAABAAAAZgAAAAAAAABIAAAAAQAAAEgAAAABAAAABgAAkAcABAAAADAyMTABkQcABAAAAAECAwAAoAcABAAAADAxMDABoAMAAQAAAP//AAACoAQAAQAAAAABAAADoAQAAQAAAAABAAAAAAAA/9sAQwADAgICAgIDAgICAwMDAwQGBAQEBAQIBgYFBgkICgoJCAkJCgwPDAoLDgsJCQ0RDQ4PEBAREAoMEhMSEBMPEBAQ/9sAQwEDAwMEAwQIBAQIEAsJCxAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQ/8AAEQgBAAEAAwERAAIRAQMRAf/EAB0AAAAGAwEAAAAAAAAAAAAAAAABAgQFBgMHCAn/xABDEAABAwIFAQYDBQYFAwMFAAABAgMRAAQFBhIhMUEHEyJRYXEIgZEUMqGxwQkVI0Ji8DNSgpLRFnLhFyTxU3ODorL/xAAbAQACAwEBAQAAAAAAAAAAAAAAAQIDBAUGB//EACkRAAICAgICAQQDAQEBAQAAAAABAhEDIQQxEkEFExQiUSNhcTMyFYH/2gAMAwEAAhEDEQA/AOxDMVAmJA3oJAIoEJoBgjamIFIYKAC3pgKA60CDKfSkFhFPpTALYbnaPOgBliuPYDl+1Vf49jFlh9unYu3VwhpA/wBSiBTeh02ag7UPjG7D+zdhLRzG3j9+6nU3a4OtFztGxU4Fd2kf6p9KG0gr9nMXaD+0BzXm5hrDMj4Yzlhpq4Q89fuPd8stBQBBTokDeTpBO21K7Wg0jVGYfjG7T8yP3j99jT9mlNsq3bRhgQwnvDISs6kqVG6jsQeNxFKh3ZrO87aO1K8uHX73PeOOPuN92XHL5xZ0+5JgwTuN6lp9kdoqtzmDFn7cMuYo+4ylZOguqKQoiCdJ6x1otBsfoz5mxGErwVrHro2BbU0WFLlBSopUob+ZSnr0jiivYX6HzXaFn0WtnaKzFiTVtYI026W3FNlJIPBTBMhRG5O23FIDf/Zz8d/aplzE7ZrErg4pgrY0uWlwAXiIMkPaQeTImeIJ61F6JJp6O+ey/tjy72pWqrnC8NxCzKUhQ+125QlwQCShQkEDUPLkedSTTDZf0pSYKaKFZlCduaBCZSFRBJoHYewPFMQfHWkARFACYmigD00AGKAZkEg7UwCI9aiMSREUAHG1ABafWgQRoATQNAImgKD00xChtSAMqHSmAhagE6iQAPWmkBxr8aHxY492eYkx2fdneItW9240XMQxBqFuMqPDSeiTG5PO4iN6AODM79p+ec9KQrNebcVxYtqJYN3eOOhBJ3gKJiaGvQWyvFS0p7xbkqaIMSTyP/H40mFjc4ktITMFBB2iYn32p0IeBj7NbouXnEOIf1BtKhuoD+Y9QJ+tD2CGX2pwXikBIBSIEDrH5Uuhir9llnxNOIDqt1NgkhNOgsy2OF3Nw0p9QbtkggJLitOskTAHXaPrSEE3dXTaywtsqbKQCIlJ/wCP0oCx4zertFF3D1PhMAkLg8enpRQ7L1kD4jO03s3xxvFcAzJdd40TqafWXW1yRqCkqJBnSnfnaotfokpfs7D7GP2ieC4y8jCO1iwZwsqVCcStUrU3udgtsAlMDqNvSp20ti0+jsfAsy4FmewRiWXsWtcQtXW0uNv27gW2tKuCFDY9akqIskEbmSaVDFFPlUaAABFNAHSAAoAIg0h0AJMdKA0KQSDBoAVtMGgAiKYgA0iQDEUCoI+UUDEEUCsAG9AxVMiAxFCQFA7Ye1DC+y7Kd1jV5ctN3fduJsmnFQXnggqSkdT90mBvANTVIR5i5z+L3t4zk8u2vs93rNuXApVtZEWyCAfu/wAPSoj3JqG2rJWka0zhm3Fs0Xf72xxR+0CG+7ShLTYEQAhAACdo3HJMmSZpxIsg1sMMXrSnUIUgBDuiQRChImPQ8UMFsyvXLd1cOhNsCokuREeHYCmLsyXAYabtMQtbFAba/wAUHfUuZHP0+dAIiru8u767Lty5H8sEcBPQAcCKP6GjLhi3LW5eIAdLrZQCOUk9ZoAz/uW8Q8i5dKS1qBAg+LrEUqAwXwKghNm86YUS4VbSo+Q6UxDZb15a6SdQJlUHg7x+MUmNEthmIFTTRDDRKZUqU8CeDuAaEMwYuWLlRubW2S2BssNqmN4nTyNyPrTaEYVp0PBKwEpWAolO8dD+NRGbZ7CviTzv2HYsi5wh/wC2YY6oi4w99R0LB2JH+UjY+4pbXQ7T7PUPsP7ZMO7Z8tHMmHYLe4e0lYRpuFhUmJ2UnY/3xxVikpEWmjZtRYwgAeaQAjyoaAOOooABE0BYAKYAjxA0gDgA70iQCNtqBCRQMMjyoChJk0AFz1oAOKADMRzQREnwjWZIqS7A81P2hfaUjMHaNY5YwrGHX7fBbVX2m3S6FNN3C1EQUg7LCQAZ3gj1ke2HRyRZXiW3l3GtSVBBAOkEj2njefrTI+xvdMvKh9y5SQeSVb/LzNIYTTajpcU6FatvYdCafbAes/ZmLF0vrGtR8KgJVpgwAffkelAhg7cqum2mm3VgNiTJ5V7Uhhu2zKUNuMrUVD70/p9KGALfvAh15lQ1AbJ6x1NAMCb5SQl1C1JWDM/KndhQu4vg7ZNAHxJKtZA44ikAxcf71WtxRJiOZ26UMB/g98kW9xbLQCkp56jYn84oQDV57ShRQrVrWSZG4ihggk37gZQgAHSSZPX0pexj1dyw8mVpKXEBMRtM77/35UdsDrL4RPiuwzsxfw3I+bwtGE3N2UrvyRotkKQEp1DnSFBJJB2E7Uk3Fkqs9LMNxKyxawt8Sw25buLa5bS6062oKStChIII2IINTIjwCkACKQATJpAGU1KtAEDSBoB96Q9gJ33oGA0CSCiDNAwHigAgN6ABApkWCNopDCAk00I5Q+PXtrT2fZTwjLWDXL/77xN5dyypi4Lf2UNLTDiwn70ypISdpkmdMGUuqBaZ5lXWI3eJXz11eulxx5feOKUuSVEyfzoWhPZieRbNlKkLUUKA8JG0yZigQSmQpJIhQVwOINAxSi46FFsBIQkJAG2070dCMbN3bF8C4SspBgJQRSsdDp20tlr/AINupsGNhStdjSMgt7tKC2m0ASTBUUyT86XkhqD/AEYVYZdW6xcltYMzEbe1CmvQ/B+xs5ZuLW4otkFW8AU0xUxmu3uGtQM7fjQIxll0AE8UBQGy63qSg7rgUBRlUy4hKSrYqG80XY6FAIQdSk6Y6DiKBGYrU66XQUhUgR0I9qAHdypA/iG3W0Z2IMgiN/xoYHYPwD/E1iWW80W/Y5nTFNeBYsst4Q6+on7Jdn7rQUeG3OAOAuIjUaaB/wBnpMg6khQ4NDAMigAJPSkw9Bqmn6AKBFRGADzFAwiCTQHQcTxQACN6ACigAhsaADoIhEwKBsaYvithgOEXeN4tcsW1nZtLeeefcCG20JEkqJ2Aqa1sR5EfE52t3Ha72hX2OgWQwy0fetcOXatlOtnVIUvUAoqIg7gDmBSTtjapGkHkkp1oIOnjfrTIUZLGzvsSeQ1bMrcWOABNQlJRVsnGEpuolkbyfi6Wk97aqk8ACqlycb9mj7TKl0JOTMdHhbw9xe/+X9aHycf7F9rl/Q+s+yvHLh1DrtotuRqOoQKolzca6L4/H5Zdom2ezK+eUhDri9jGySKp+/XpGhfGyfbLRY9mq0pSypsSOpSSPoaolym9mmHCS0PLjs5QhseDcbiESPpUFyWTfCSAx2NtXCVXavArcpA3586l96+iP/zU9kTivYs54u7CQD15PtVkfkK7K5/GN9FVxPszvbVtQDYEbSdvzq+HNjIzT4E4lfdyLfonu0hZBgEdKu+6gUfZzGb+TsaCpdYVPQnrUlycf7Ivi5F6Ii+wy8slHv0GZ3FXRkpdGecXF0zE0hlaFOTCwNh0BqRAdtFesBKgUgE6eR6UAZu8Valu7tXFoW2QoKR4SlfIIPSP0pi2ezvw49oD/aZ2MZUzdfPh29u8PbbvFzJVcN/w3FH3Ukqj1pvYI2XzSGENjSYCjMUAEJG3NA7ACZoEETvE0hhigYRoAKgAEUAgx70AEaaEznH471Y6/wBiq8EwK1un3MUxK0t3G7dBUVI1lRkDeJQmem4pytoSdM8vcxMP25bs7plbV0244h1BBSfDtweOI+VEUDIO0w64xG8asraVLdPXp5zSlJQVscIubUUbtybk1jC2kIQ0CtQBUojc1wuRyHkZ6Li8RY1/Zf0YFbqQAppMj05rE5s6SxxJaxwK28JQ1BjyqtyZOOOJJnBEqQUhIAG/FRtlnijC1gTSXBqAAng9asTIVsklYSylIMgECJ4+VOyXivQtrDTpkNBUbGai7Y0kh41bJblLYCY2A6e9KhexreWYWlPeJ3Ud4HNFCZCX+BsvE6hIHnTjZGSTIR3ALVsk92FDkyOatTZS4pGC7wDD37dUNhKonak3JMl4xaNZZoyazcMvOtJBUJ2iuhg5DjSZyuVxVK2jT2J2Aw+4LawoEzsRuDXWjLyVnDnHxdDdXfWZ6qbMEj3EipEehx3rlw2kiVI5V4uB7GmB6r/s+MascU7AWrTDw4lrDMWurSHCCqSEOnj/AO7QuhHTgO1MAHikAAdooAAnzpADiglQUDpQIFAwTNABfKgAGgAUAwSBzTRE1H8UuZLvLnZFjN3hli3dXr9s7atBTRcU2FjSpwJA30gz0AIG9TuhVZ5HZhxTEsZx68v8bvXrq9fcWt514krWsncqmlFgya7L8HDuKOXzqAowQPICsHOyVGjpfHY/Kdm7cJa0vSobbAbVxJnoYKizW6UH1MxVLZoSsl8PQhBCdIEneR0qL2SSJjuGlICxJkE+kbUEhsm2ZcUVFRgc77zTTESDVom4SIBGk+XX+/zqQaHLWGoaRpjzVv09KaQmxIshqCYiR1qVELE3FgpKfANo2J86TVElshb23CQqTBJn3qF0DVkPdMqWSnSNuIq1Mrkhg/atlhQ0wY86lZVRUsVw0lh5UJIUOZpxdSITjcWaTzVhrffrcUAooJ0nz967nHlaPOcmKUitKYJZlSCoKE+RgGN/StJkMDlqpvUECPDMTx5H9KfoR6FfsvMadeyZnPASkhFrilvdJB83WlJP4MppoR3GKBgkeVIAAg7UgBEGmFgmKQ0JIM0DDA2oFYVAw6ACPpQAYE0xMSoGgRzv8buZcPwLsdvbN3GHLS9vilphhlaQq4GtKlJVO+iEydO8hPQ7v0CPKO5uHX7pRcUSSqSqdzQgaNsdlNmEWTt2UESYG1cfnztpHc+MhUWzaOFjS2FKA9K5cmdqCJuxgGCYqtlqJFt1TSwRBB2AmosmiwWzyV2qJMiYIHSmqY2BTbQSfESVGdj0o6F/Y7t7xLMAMkkGSAkmp+ZHxd9kgzcIcCiWlpUd/vbU00JoxodLj2lsJSSNj0NPyF4ocvKeS0pTqYgGDO3970nIl4/oq2IPjvTJAUOnp/ZquyVEQ6JMJgid6nErkhpfJASowRAmQOassraKziYAZcMniYqSVsqbpGkM1Bar5QA/mJ8wY4/Ou1x9RVHnuUrmyCcDDyFMpgDwySIKVdZPua2IwsiXVI73u1JUQoiFAQSB1/SmROyP2XuIXbWds5YWgNqt3sOZfd1KIWFId0ogcEQtU+W1NMVbPR2DSGEEq60AGBHrQAcUgCigkETvQAPWgAjFAAO1ArDABoGGBFAmwlSdopiPPr9ode4nb59w+3dUVWqsJSphCoCkkqWFlJ6pO0jmR5RTfaD0cPoaLjyVbwVQSRwaV0OrN4ZEYDWFJZTAlMn0rhcx3Oz0nAVQovFloa0pWRtxXPkdOJM2S0KJUlSZPE1BliH4W+ogFCRGwM9KhZNV2SdiApQCkgGZnVG1NOh9jy4KUpEAGOuoVMjdGW2XrSZAkAfzf8UlQ2OrZTzAP8RpQV5q3H4U9ehP+0ZHn1OJgpZWAQUGeI4O1OxVXoZ32JOqQEK0hKgSYUZJ286V2S0iFduO+SFaZUnkgjzpIi6Gy3VNLnSeOKkRYxvLhJA2I2Pzqasgyu42UmzeKdjokR0qyHZTkX4s0zjCAq7UrTJGqRHT2rsYukcDN2ym3zLltdu6FyknQSK2xejnyWxg4+e8CUkweVdTv/4pkTsj9mI+w12l5pZKlIcucFGgFwBKtL7ZI08k77HgDV500I9IwBNMAxQAIoAG1IAooARuTJpDDHrQMOgVhKE0AEkGgYsUyIY5poDzo/aPYv33aVg+FJ7nRaYUl093PealOrB1Dj+UR1+UUm7kkP0cguMIFwhtxwEpTKoEb+X5UpOyUVs3HkSE4WhXJVHTpXB5buZ6Tg/8y2CS5q38xWFnQJO11jTo55IqLJJkxa94rfu1KI6Co9liZPWIT3clK0lWxCtiKFolYTtqHkkgK1AQKmkRbMlmCz3iHAQBGkGd/Shqh3Y7aUmQNQAIkkzQtA2AsvaFlBBiYHFBGxg9YuOtkBuB5/5TT0N2NU2r7DagpQUAJHmT1pavQn0Rt0/3J3SQmd5pkbIy5eS8lW+xSRv50xOit4l3imVNIWdREQd5qyOnZRNNqkamxtS2sVcQtMK6T0Irs4q8E0cHOn5tMr+KtoZSAoJAMjSUkATvNaoMxTRX1oQXVKEAJB2AMyBtFWFTOwf2ZmHpve1DMOLd6hQsMHUgIMBQLryN4jcQkjnafWmuxHpSInemIPbyooAQKQBGm+gBxE0hiJk0CC6c0g6DChQSCUR7UCoAPlQMWkUyIsDemmB5+/tF8oWbGa8KzS2pary9sy24AkHShCgEifISr5qFEtNMEcQpW4/doaKpU6oD2FVydIsStm7sqtqtcObQZEJ2kVwc78pNnpOKvCCRZbXW8YE+9ZmqNidllw2y8KZJE/Sq2WIsNq0UrDaERtMn86raLYuiVZ0qJE/dgGTzSSJsfN2TSx/D5T5E1OxUhBwe5U5OmEfPf500xGVrBgt0PdzCkpgE0B/g6Xbpa8OqARvJpUOwOW7bif4aAB56p9KKC6Iu+tGkAwCSQRNCjQm0ys3tiCo7RHECnshorl62GiqUHarEVMgbjxK2PHFT9Fbeyr5mywi/WLpGywZVHUVow53D8TLyOMsn5Gt8zW67WWTsJHygRXV48vNHD5MPCRVXu9AQXU6ElZUFaY3jce29ajKd/fswsmqZwXN+fHbZGm5uWcNtnSZVCElbgA6CVt+8elNdi9Hdo85qQhVABfKo9gA8UwEj1pDE6Y4pDC0mgQQSZoGKI2igVhoFMGLI6xTEHQkBob4yezn/AK17I8SxS2ti9f4NbrcahGpQZUtBdj2SgK/0UPoFpnlVgWHl7MaLNxBSoOkaeoI/+Kz5nUGzRhV5EjetpYpt7PWshLaBvJ4iuG7lKkeji/CNsjBnZhp822GsKddSeRvPoAN60faUrkZXzt1EzN9rDTSggmFb+Can9imQ/wDotE3hvaphzy0d+/ufvECTPlFUT4TXRfj+QT7LhhGbre9QkhaVIUZ2PNZJ4HE6GLkxmWzD8dtw4lBMeh4+lU00abTJ9WOAIgFMEbDypWS8URD2aEsFalkHRsfanTbI2kVzGs+2NokPPKAE8jrzV0MMpspyciONEPe9sWF29uEs3DYUTMKI/wDmtUeIzDPnogLztiZcOsqQVcAEn8qujwyiXPIq77Ubq7YC2i2kRqSY59D5Gp/aRsrfOlWjLhmcrTEm1W166Q8DAKhsoHp6Gs+biuLuJow8yM1UuxTrepaltuJWiNiN4PlVDTWmaU09oStJUzKkzPNVrTLO0ac7TWii9QAkiRxXa4L/ABZ575FfkUh+2duHWrRsKWsqCEJAJJUT0FbjnOz2H+Efs2X2Z9g+WsEu7BVpiN4x+8b9CySovu+LxA8EJ0COkVJITNyRFSEHzS9AFBoAKkACJ9KKCxG4oCwEigACJpAGeKYBoHrSJMV1ipERQHtQBUO1ntDy/wBmuSrnG8wsIuG7hQs2rdY8L7jgICT6aQon0BqvNmWGPky/j4HyJ+KPIjDcJU12wX7KrbuEIvLhxDY3CWySUfLSRvWXPlUsHmvZqwYJQ5P05ejZeb1PM4abe1UUkqCTvHNc7BXlbOpyPLxpFAtcrfanVBsqQ4qYXW18hRWznLiyk9B3nZtj6x3rK0ajvMQTSXMgN8DIyDuMsZjw50LdbA09Z2n1qf3EJIh9pkgyyYFjt/ZFnvpCkH70kAf81myOMujXhU49l/t87s3RSkPfcgSDuTWGeL2jo483plrss0KLTSlpKknkzuKocaZsjO1ZF4xjpsy6sLUUqUVgE9PKrYJMpySaNf5gvnr7Q02taUaQFQr589K2QkonPyRcikXeE4lcvFFu4tQGwE7SfL0rVHPFGOXGnJkjhnZlmfEFpUh3QknclXHkZofMghR4E37LUz2S4xZ26lXOKJWtXAbBJPyNQfMi+kWr4+a7Y0dyVjtiULfbS42kymI2M+UbflT+4jJaI/azi9loyywUIdae1FYQZLnPG1Yssk2b8MWlsl0Wye7Jjg81Q+zTF0jUHapbRitupZAQttUHrP8AxXV+Pf4tHE+TX5pg7Kct3GL5tscQt7TV+7lpue8WkFKVpPhMHkyAflVvLz/RhZXwON9xlSa0dMWfbbmLLOdLbEbjMmI3WJBxKnlLeUpCgf5COII/lGw2riLmZlL6lnqX8fx8kPpqKR6AYXfoxTDLTEmwQm6YbeSPRSQf1r1MJecVL9nhckPCTj+hyI8zTZEB34pgF0qICTtQCEE70h0AGgdAmKBIBM7UA0KQSRQDFTvTQhQ3FMDQHxtWjL3Y0p99zSWcQYLcc6iFp2/0lVYee6xp/wBnT+Kj5Zmv6PPvLbTl92iuYldOBxbli2tJAiRp07/7aySkvt0l+zbGMly25fotuYbiVFGgEGSfPassDXkWykX+d0YS4bawS2FpEuOrPhR7Dkn0q6OCWTszz5EcXXY0xfOGd7PBf+pHGLpnDlLS2h1xCW+9UqY0JMmNjua1Q4UGtmKfyOVdERcZjxN+yZvbi7TcB22F459mu2lrYQXi0EOIKRDmqDomdKgqI3qx8OMVaK1z5ydSAm+xC2dUxeMKWE/elvQ6j1Un9RWaeKma8eXyVkthS1PXAWyqFCDHmKzz0tmqC8no2dgOF4jeWgWJI2mKxSas6eOL8TFmLD32my29q26EdacHRDJE1zi16thZQ0kqCOUp861wg2YsklEaYdd4viV61YWVu8X3lQ1b27feOrPudk/OtMcKZjnyJR/oy3WbMzYU/ilujS2rBlhNy27iLSHSrWlGlCfvLMqE6QYEngE1ojwoSM0ufkiSWHZ4zPcJubnRe91ZqCblYSi6ZbJ3BJRBj13qnJw4rovxfITl2SVn2luFSWL9ppTa90vMq1tn9U/OskuO47RtjyYy0ydZxAXSw7bwlDg6VBx1smpW9E/aSu2CVK8XpVLey9LWzVfa6yr7XYaUSdRHoZ4rqfHuos4vySuaNs9nWXbfLWXWLW2t0i7uUhVw6Buev0HFczl5Xmm36O98fx1x8SVbYwzHhtunMdppJ1OvtqE+UGfyFUdo6GPR6W5HVOS8CkeL93W8n/8AGmvWYf8AlH/EfPuR/wBp/wCsmiZ6VaUhGI2pAJFIAGgEYjuZpDACKAAugEEOOaBik8cUAZBNNEQ0mN4poDlf49r/ABNvKuXcNt5No/c3D7oHVbaEBP4LXXL+Suoo7vwleU3/AEccYLh6sPz13KphrCWdo4J3/MmssX5cb/8ATTmXjzdfosWJWSH2VL3JXPWIrNGVM1TjasoOI5Et13KX3k6yVagOAPWtkc7qjDLjx8vJlsvlN4/k9zJ+PNqcbTpXb3KfvMqT90naFDn61dj5Hi9lOXiKa/F0UPL/AGY2acTausSxJi4tmXO87pKFp72DwTBgfpVv10ULhu9l1zWW83P94jDsPt7ppGhh9m4KC1HGoaTqHoYqn6iqpGn6TTuJDYbhXdX7TTjTaHt+8DZlImOPSZNY8jtM2YlTRvnIWGpRYJT3ayYiQk7GsHbOr0iv59w7RfaltOTvE8fSpQdSIZFcdFC/crFxaLtWSwi6fXCnHlQGx1IgH8q2Ryb30YJwvrsm8iOs5EvHha2dm6m5ADo+1ErUUnUCTpHptEbVo+qvXRneC++yndoHZ5Z5mzZc5jwK9Ysvtyu+eZdBOl08lKkjqfTnrWhclVsxz4LbtFkyZlhzKWVbvALBYccxElV3dd2RPQJSFcAD57ztVGXkeTtGrBxFBbI1HZxhTKVrtXYdKtSlRKVH2qmXIfstXGjeiXwvAU2ejWdk7AJG2/vxVMsnl0XwxV2Tj7XcJStKSOkVUtlz0ay7RW3LjH8DYQlSu9u0JKeh8Q/Sa6fCVY5nG5+8sK/ZtHAM2JscSdwrELYNpQkATz8648k1s9LBpxQ5xqyRiGI4fiLEKTKoI8xxShtUWWk7PRjLtq5h2X8MsHIC7a0ZaUB5pQB+lewgvGKR88yPym3/AGSPXzpkQiaAE7UgCnpFA0YVEDYUgAmDQMP3oEKEcUB2KG1MQYPWhAxQIIpoDTHxY5QbzN2WvXmkF7CLlq6Sf6CdCx7Quf8ASKx8/H54r/R0fi8zxZ6/Zwliym2+0O9CUCRZtgQBMaUdeYrk49Ymv7O1nV51L+iWatTcJAJkmqHSZoStDi4wAX7YaCZITHH97U4yp2hShaohbnJ2ItlQYxQNf0EEx+NWqZS8T9GBns/uFmbnFHiOoSmNvczVkZplUsbRLjK9lhtmtthrVABKtyVD1JqOTKo6J48LkMMKwxtq/wDtLwCSSCB+VZ5zckaseNRdm8skIZNmmDEiZnafOsyezZ6IDO9sg4l3Wykq2PmR502t6DtbKBeYGG77QmQf5f6hV2OddmXJi9oyXmSbW7YTcoeU24eFoMKB9R+sVq9WjNVumQlxlXNNivTY4i06CZGsls/XcGi0+yPjJdGW3yrnB1QViV26pAO6W3U8fP8A4qD8eyaU+mTbOGLbSGV2xSU9VgEn5g1VMvhH9jj7IUp8W/QiagibG16lQYAKtzNSiV5CAZw22xDNuEPXTWsWy1rA8jp5rVDI8eOVGKeNZM0bJbtHw53DnVYtrTIcCQByUwCP796zL81bOq34uolj7KrR/M+OZawlTB03OItJMiYQVAKP0P50sUfLKor9keRk+nglN/pnowCPlXqjwoqDQAD1oAQZFIAiSDMGgZhUJ4pDCiOtABmZ2oDoWj1oAXTIsMGBQgYaeOBQgK72jYOrH8iY9hDTet26w59tof1lB0/jFQyx84OJbgl4ZYy/TPM/ELcP5lGMoSQV2gZdBO4cSoAj6JB+YrgrUaPS5NzTLLg6EOBKnelZcjpmzErRZ7W1b0gpjc8DqKimWNWJusvtXhlC4c/oMTVnlorcG2KRlFthPfXLriiB4gpXNReSXolHEu2Q+ZSi3ZTbshKQYBApLu2SlpUiCZt1PXjaEGUiJqTeiC2zdGTLcIwtJS2VmJMms7e9GpLWyBzd/Fvu9UZKdpI2pp2KSpFZvGUuAKKo0Hn0pog2SeDMN3Mo0pUNoVIk7dKtjNog4J7H1zlXvW1OW7ikKiBud+vyqTyfsj9L9GG3ySp1YXd3S4HASaPNCUGh2vLltZt7KWUpHP61W5E0qK9iDBQ8Eo+7wD5e9JMTRA4hp0KRqMpO9WwKcnRDYapIxhp51WkNyon2g/kDV81/GzNjl/MrHLdu/n26uL3ELpxnDWXJbExCRtPrWWWT6a8UdWEFLbN8fDPgljifaRYqsGf/AGmC2ztyCd9RjQD/ALlyP+2tnxmJyy+T9bOZ81nUcHhH2dkAzvXobPJhg7xQADQAjaKQAoAxTJNIfoNPMUAgyKBMSnY0EjICCKBMUIjzpiB6g0ABQCgQeIimwPP7tnyRaZVzhmBFmFJT9vW+hvolDkKAHyKfpXAzLxyuB6bE/qYI5CkYTehCkoWOkx1rHkRvxS0XHDrtnuUqWSATE1U3RfHZYrFSe6CwUhStx0p+RKgXjzSUKUtRgST6xUbCjV+YcSViGKJtMPbLpSolSp2AFWwjrykUzlb8YjnCWtN53e5U2mVHypTeh4//AFRuzJbYTahDjaT3nh+7VMHs0TWtFWzpbrt/tCinfUQgxFNK2KT0VEIu3LNTyEbwSE+YqTXplaT7QrLuYCbjQ6gNLHhhUgE+kdaGnEItSNkWDwdYSVFQM/Onf7LPEcOuNISdtIgpn1pWkKiv4m684FhAMCevXzpkGqKjfOqaUS4uTMzSoi3srV+4Qtayud6vxKzNmdEZYqcRelxDHfST4fMdZ9K0Tf4NGfEv5Ey2tsv37SbP7OzaWsDW00OR/wA1g0mdWLvR1B8KGUjhuD4tmZxlSDeuItWCRHgQCVEehKgP9Nd343HUHP8AZ5v5rKpZVjXo38mAK6dHFDBFDABoASTSAIe8UAYwBPNIl0BMA0AhSjvtFMVBdOKQMUmgYrigiGBTAMzTA5U+LHs3zKnEnM94NYOX2FPsJRiAa+/aKQmA4R1QQEyehBmBBrncvjOUvqxOrw+Wo4/oyOUUXXcPghckbe1cvLA6+GZa8MxFLzQSomT05H0rJJUboyLTZYsFtpRwmNt/L0pdltkdj2LvFnuG1fxHDpA61NRIuWiFwZpq3uB9pIS4rkfM1ZPqirG0m2TGHv2VvjKkuqQhLsAEmqp9aLIV5bNpWeZ8FwuybTbnvHEASBtNVKVF8o2Q2b8xYLjTLLSAG3SdKoVO9Wp3spf6GC3MHsMOS0txKlqBMyJ9qg3b0WR0qZr69ZuHMQfVZIIaV42yk7pV1/v1q9VKGzNJuOR0T2XsxXiNNrea0vTpkeVVyi6L4zT7LR+9UOJGp0EpO4Kp+tUu0WKmMrrFUkLT3iSR1jbepRZXJUVDHL5CZCDuRO3SroqyibK1d3EtEgzNaccd2YsstUO8rP2lvfF27J0qQQPQmo57rRbxUnLZtbs/yyM95gsss4ayoNOuBb7rY/wWhutXkNthPJIFVcbDLPkUC/m5o8bC53v0dt4PhFhgOFW2D4XbpYtbVsNtIHQDqfMnknzr1EIrHFRj0jxeScsknOXbHiR5mpIgGeaAAeKAC9aQBEcUDQgHfagP9CO6uaB2A80AHtEUCYYigYoEUCoOn6EKFNAR2ZcLRjWX8Rwl1IKL21dt1DzC0FJ/Ok1aocXTTPKm+JtblbLkhSFlPzBiuJOGz0GOekx3Y4kplUhW/E1jyQN+KZYrDFUlKElSgI9d6gol3mOUp71/7atwlKTIB6VJaHdkNmqxurm3LuHXyre4TKkrTv8AWrISS/8ASKssW1cWUGxzHnTDbsoxl4X7TRkEpCVR6EbfWtEsWKa/HRhWbLjl+TsujXadh5YT/GUjaDqSZHvWV8Z3RtXMj42VbHM6ZixBZcwFSmGwf8dSZV/pB2HzrVi48Ir89mLLysk3+GjDg73aHi9y2V4q+4iYPegR+AoyQwxWkPFPNN7ZuPA7O4sLJLb7veOKElRH3j1rC6XR0op1scOMp1KdQlOtNRTJMbLxZWotuCDwfWlOF7Q45KGb+KoQk6TE9KioClMgrq++0KPWTtB2q+MaM8pWN30nuUpAPUDzrRjZlyI358JfZ1lzO11mN7NOB22I29uxbttouEau7WtSjqSf5TCIkb71v4uOM78lZzuZllj8fB0zrXK2SMq5Mt1WuWcDtbBDhlZaR4lxxqUdz8zW6GOENQVHOyZsmX/3KywA7VNFQaaYBKgGmAQNQAFMAjG1ACADNIkDTvzQAFJFAACdoFAAgigQtA33oGxZipURCB3pdAGuSkj0qQjy+7WMvqy92hZjwdTcC2xK4Sgf0FZKT/tIri5l45Gjv8eSnjTKezKXONutZ8mzTj7LDYqTAKzKQJ3qqtmmxN7j7YeDbawfCCTwI61NY7KpZvHSGb+PakEoAV57EmOlTWJN7K5Z3WiPaQ7ihVpaUkkcir41AzTcsnZgxHKzVvhe/wDiFRUkgbx5U203YlFqNEvh2X7h/C2VO2/hS2STEE+9LzRJY3QuxdXh5QlsqXxxED1moSSl2OMpQ6LUjMKmmgFAakjU4Tzx9Os1S8f6NSztDW2zC69elpS0gI5MzIqEsNKyceTcqMl++HV9803M+ZqpL9lzd9EO+64okqG3FTSK2xmglTgZRBCt+eN+KnVFV2yQd2aEbngdN6cAno7B+CzC/s+RMYxVaTqusS7oHoUttp4+a1V2eHGoNnC50ryJHQ4TvNaqMQY2pAHq3FABK3G1MAkxNRH6DNSYgiI60hiCADzSAMQRQADvQMCdzxQApVMiGjekNitp3qQguDSYCuRTA4Z+M7KH7m7SGMyNslNvjtolSlAbF9qEK/8A17s/OudzIVLyR1OBkXj4s56baHeJkgzMAcVzpbR040mOblxRtSEEjT4TvvUYLZZOX4lZIcXibFop0SUaJmJ3rYmlGzn03OizMtZcslB29xJS1iB3bXi46c1Q/OXRtjDHHsl8MzTglm2U2uXrl5JM6lLEieo2oeGfdlsZ4+qJoP5Ix1nvVO3eHqQJUhadSdvIioqOSHZNwxT6Ml1nLAm7VGGYXgKXWGxoLjiykr9QBSWKTG5Y1oh14llF1SU3rNxY6R4SlQcTPrtNDjkj0QaxSIXGE4W00p7DccZuUwYSTuOashld1JGXLhilcWQ+A3neYgt51QVE+JJnfpz9K0ypxpGOFqZbhCkIUuQem/HvXOkqejqwdx2Mb1zSNCFAqUqPaiPdhJ6oFotlGpaRuOfWm0yKpCn3VuJgkHxCB1/verMa2VZHo9APhjwpOFdjOCAABV0X7hfuXVD8gK7fHVYkcDkvyys2mR61cygLekABAO9D0AaiI4qQCQPLrUfYB02ABQgMBkSaiMMGgYZnpQJikmBQMBJ60yIpChEGkNipqQgddqADHrTA1F8T/Zu72h9mN4nD2deJ4Of3haQJUrSD3iB/3ImB5hNVZoecaLcOT6c7PPJLndrGpIjiT51xJxaZ34STVgubhjSQptSgUzHrUEiyUlRAOZNuseXrcv3bRAJILX349DV8eQseqszPjSy7uiZwjsvQxJvscuVtKV/DfIBj0O1aI5IZFaRPHgcH4yZdcP7J8XfYUcCzGxcpIlKHGzqHzG1Rbvo6WPixrbEf+lnaCwF2Rs7dSlwvVqUCBPQRS7LVw9aaBY9mGd0LXqurZuJklkmPrHnToT4airckNrns5b74/vjNDrqimQhllIB6EceVNNGfJxkvZAY52P4ZcoS8Lu6ZQknfvClavaKg+RHHpIxz4n1PYxy9k68wd2FOlVuk+FazKvYmqp5lIhDjuGibRdIQVBvcA7kcev61Q15OzRF+Koa3D4ddCuo+lNRoTlYhp+EgFME8x50eNsTlSHDK++fQoDZKgZPoatrwVlabm6PSDsMuMPHZXlu2s71h9TeHtF0NuhRbcUNSkqg7EFREHyrtYV/FH/DgZ/8ArL/S/Ag1YVh0AJMzvQADxtS6AICigCI3o7AMUgMRE80h9BpQKB2GpIjamkRYEJ23pEm6FQPKnREIATQwsWNzxQgBMnimAr2pgEsBadJHNAjz1+KPshc7N88O4nhtuU4Hjyl3NtpTCGHZlxryEEgj+kjyrm8vDT8kdTh5vJeLNGF1QX3aiQPTpWOtWb/K3RYMMvG2GwlZBUUgA8zVLj7NMJ1ofMY0qz1JSqEODdJ3SfQg043HoG0TuAZgFk6XmLs2pUZKVKhJ9jV3m2auPlrVl6Zz2u7fbuf3wyFsthA/iDiZp/Udmvy1S6GON54W9a/ZG8WR4lFS1BwCSR9abyMrc40VVzNOG4elKbModfIhSzvHtVMpTl0Zp5YWMF4irEXg844dW8zVDtCUvIwYtd27jBtQ7ojy/WiNohkplUcdKNTYUVCdj6VpoyWYUKWRqV57E9KAB9oIUEQJJ2qUV7K5S9EzZshtOsiJ+7/z+NVzlbL8cPFGv8ZzvmzIOc7rE8v4reWRLxUly3eU2pPsUkEV2ONL+NHC5Uf5ZI7v+Ez4osRz/bWuXs6XZuH30pQxeLgLDnGhZ/mkpVB54mZ21pqRkacTrCJEg1GhhEUAEIoABFABUgC44NFAJHNIAcHagAzBHFCAAMDaigD53oAPpT9AGk0AA802AY34oAUBQBVO0/s3wLtQyndZXxxrwujWw8BKmHgDpcT7Tx1EjrUZRUl4scZODUkeYPaDlTFez7NuI5Ux1oJvMOeLSyN0rTylY/pUkgj0NcueLwk0deGXyipEYytRSh9o6o+6PWs8lTo1wd7MynLhzwKBMiRPNR0ie2RmINYohCjardRHUE1bCa9lOSEvRVX/AN+KdKWb65JG6tzWnyh7Rkay+pElh9ljqwla3n1TvJJO3nVcskPSLoY8j7ZbMKw5wAKuXFrUd96yzyX0a8eOtsnU3Hct920JEx61S1e2aPKtIaOaVFaY1vKG+/IqSTWyuTT0NF2gKSYUhRnpPyqTk7K1FDa6/gNBH3qcbkyM6iKwi1Vcum5WNKUmEyOTU8kvFUiOKDm/IsSbZzTJHsKyuRsUaNd9ptok3ToUhG+lQkeaR1967fE/LDFnn+avDPKJPfC7jqrPMl9g6nNILP2hqOQoLGqPnp+taG3FpmelKLR6h9lufLfNWCi0u30/vOxSlDwJguJI8Lg94M+RB6RVzplG0XZSSB50gEEb0ADnrQMKgAjQAnY8CoAGBtvTDoHWkAoxHFTAJIHWo0AZgcVIA0kb0kAZ3FMAJBoAUJmgBF1d21jbO3t6+hlhhBcccWoJSlIEkkngAULYHmp8RvaDlXtb7QcVzJk0qds7VSbDv1p0i4W0mCtI50mQBO8Cdq5nLk8eY63DismA1DYXH2d427ogeXMelU5IqS8kXY5+EvGRYGA0UmVDp71lNqZZMNtbNxgoeA+6d42o2S10ZFWGFsKUpLTB2lEAeL0NFth4xRJWlvh1wpAFohCYAI2kGo0yejNc5VsFt/aDcto6gp60/Jg4rsrmJWtrbKSO+1FyQmOFGpJWUydDG5ZatmlBSA2Tt4jNCtsi9ETcYglMpS4J8yKs8Cp5PQxYS5ilwluCE6t1A9PKpNrGrIxTyui4Ybh7SUpZSNkjYDisU5t7Z0McEtEuq0JKWgI1H6DrVKkX+Jqrtyw26sXLHG2Qs26SGnwk7Dfwn8x9K7Hxuak8bOJ8px/JrIjF8L3eX/ao2pqBosbhx3bYJ1Ij8YrozdnMgmj0H7KbZLuKYq4dSQcLUkiYBVqRCh5cGPepw2iqWmb2y9e3ItLW0xB0u960Ch1XJ6Qo+fEHr+c2miBMONlO/SnpioxT5UgCmgAiaBgPWKiAYiKEAXB2oYBmTTAAIihAHO1MACgAwDPFAChI3NACgUkAj5UAcv8Axm9qqsKyxc5Ewi5h19hT1+pCoKUQdKD7nc+gHnVuONLyZXN2/E4F7J3lXWXbxxZlRvlkzvyN/wAq4fP/AOi/w73xyvG/9JvErJLgLiYCkkkEbb1mxZHF76NWbEpq12MbbGyy4GLtBlPU7Ex1q+WK15RM8M/i/GRJDMD4SUpc2PmZ2qHhRb9YajFbsuQlUBR2AM0eKI+bslbXH7plRUp7To5kbVHxRYsjRndzs8pP2cJlKYlRP3Z2mmsdIi8zboSnGLZLn2q5bGpsQIMgH+5pSi3pDU0tsiMRzGq8XoaWkpBkmI29Kshi8eymeZz0iNZW5eOAtkgDYqAgfKpSkoIUIubLhgOHJbAQpAE7pHJUaw5Zts6GHGolrs7FwErMJjkzIArM5Wa4xJe2tkLQCkE8wYqtssK1n7CWcRwp+0uGkraWgpUDvzWnBk8ZWjNyMflFplf+E/KbuG5lzRjJTKbVhFiyrzUpWpQ+QSj6134vzimeayL6cmj0A7E8Dcvco3OMhotrxR/uGZkQlIgkTwNSj/trVijSMs2bcvrNFpZthtOzXgT7aSP1q32Q9ETiLObcLQ3ieXEoxGzebQ87hj6oWnUASWVn3+4T7dE1DxV6C2HgGcMEzJqatXlW960Sl6zuBoebUOQUmn4uheSbomSI5qJITO9AC+aiAfAgVIAt6QAO9MARJoAUGydhQBju7uxsGu9vbtplPmtQSPxo6CrGSM0YU8ru7AP3q/JhlSgP9XH406sRXMX7QbtkLbs8LQhSFhJD7njBJA3SkRO/+aiq7GtmgfiC7du0XIuZkYLg+ZU29q9Zpu0pZtEJUPvAp1K1E7oPBFaMWOM+zPknKD0c1ZvxjEc1YViGK4rdPXN5fMKcdecOoqUZ69KsyxqNIjCVy2an7I5awa7aO38cOEevWvN853M9L8eqgy9vMa5k6kq4rnt0dFIrmNYQm5I1yiNwpPStOHM4GXPx1kZXFnEcOJS6FOND7qkb/hWxTx5DBKGTH2BGNBJB8QP5Gn9OLF9SSM6ccbUgBx2COBzUfpIl9VjV3Gm5HiB8t4qSxog8rC/el7cyEAmekCD71F+MSS8pD+ww+9uCC6oJSeUjafc1VLIl0aIYm9stGH2IQhHcgJTwD5n0rLKRrhAtmB4Ss6Lq+UNWolCUk8evnWWct0jZjhrZZ7bS84oFpSWwB4iNiapbLkiWQEtjQ2iVHk1FEmVrN9w1Z4U666QNIJO9XYV5SpFOaSjG2XbsByLiH7is8IYY04ljdybh0H+UuRE/9qQAf+016vFi8IJM8bmzfUyNo72y5g1ngtlh+BWSIYw1gJ9yBEn1Jk1atEWPcab7xpu3TBJXO46R/wCaaYmOMRs1tMttW6tKmUJbA6EJAEUqAo+bezqzzsynEWX1YbjVuD3N43tqI4S5G5HkeR+FWRdMhKNqylYH2o41lTE1ZU7SGHEPMqDYugmTHRRj76T/AJhTlBMhGbjpm27RTN22h9haXG3EhSFpMhQPBBqlprRcnYoqqAwjv1pgAb9aOwFDYSKYFazX2i5QyWB+/sXabeI8Fs343VeXhHHuYFTjjlIhKaj2McMz9eZlwJeP4ZY/YrF0qTbqfgurAMFZA2SJkDnzocfElF+RQm7/ABHH8aYS5cKeuLhzQ0VmdCSdgB09aqUdlt6NoOFnLuEKZs0/4aCZ6rV5n3NTsrr2UbMVk4hVliCz4ny2p31VIM/35VJbA5V+MNu6RmrDrhLultdjBJA/+osRPzG1auMZ+R2jVlstrEMALSCopLJgJ69flzV2WNooTplD7PLf7E5eWahHdvrQRxsFGPwrynP1kPWfHbx2XpTRKhBkjcVzbOnWxvd2YeZUAkAjmKcZUxSjaKvc2gUSktnyO1aYujLKN6IG/wAAlJdZVqPMdavhlrTM88D7RFPYDdRrggGatWZFDwMy2mX7vvAHGySOgHP1pSyolHA72WvDsDQwEKUVJ8M9B+tZpZLNcMXiTdvapBSlDafWN5NVNlqjRPWVs24AHXAFIMlAFUykXxiWHD0SoFSE6EiNzVEmaIk1ZoWv7o0o5jyqDsmkh4oNtpMgz5+lIbRVXLJWasytYaUzaWig/c+Rg+FPzP4A12fjeP5y8n0jg/Lcn6cPBds7K+G7JKba1fzheNRpBt7MKH+9Y/8A5H+qu/Jnnca9m98KZ8DlwvYvKn5Dj9aj0i1i0tl/EAOUtxM/Uj6RR6Ex5ckKTqO5/wCaaEjHYsDUsRsrepB/RU+0ns2w7PmF9w8EsX9ukm1uoMoP+VUcpPl05qaZCUbNR4FmbOXYktGA53wZ65wdaz3Fy0oqSiTuEL4I66TB9uKk8SkrTK1kePUjd5G1YjSESBzTQFAzn23ZCyWh1q4xIX162YNraEOKSf6iNk/n6VohglLb0iqWWMTnzOvxIZ3zQ4u1we7/AHNZE7JtzDkeqyATPmkgbjY7xphhjEolllI1qvELm/uF3l3dOPOkalKWsqUeuo9T7+1OapBBW9nbtngrOGZIscMtxLbVo22k9T4OfnzWRuzZFUUzs7YQc3oLgH8JKykeR0mKr9kjY+PsqVZuBO5IEUkgYyzThYXh7A7v7iUEfIU4vZFbRyV8aOCxZ5exOQgLW9bqJB/oUnYc9a1cd7ZTyF0znvLtzKW20pUttbCVtqMJkxB2+VapbRmor9oycPzRcoX4U3JKh6KBgj8vrXmvlcfjPyPSfEZbh4suLICUAgiVjea4TO8kZ1pho6UBJHWJ1ULTGQuIWELLzQgbSKujIpnH2Mx3esIuUjVGx0cj3qd/ohV9ja5s7XSXErKZ6TtUlIi4oxNKtmfEhYBHQEQKdsVKzO2+lxSU+Eq6EzAqNfsdktYWzilBSnoIHEVVKdaLYw9k5ZICSfFqA3kCqZMvjEmbAJchGklPpIqt7LFolkPLSUttIlHnSJaMGMYiLK3UEeNxzwpSOSTsAKnig5ySRVmyKEW2W7s1yZcLctMMS33l/iT6S8ob+JRgD2H6V67jYVgxqJ4rlZnycrkdwWFhbZfwOzwKwSEtsNoZTAiTwSfcyate2JaLRbIDbKUAcAAUnsQmzbSVuPxOvj2PH4CkIyOKCueSKkhpDmxTCRPWpCZmeb2JHNSRFjO4wiyxKyuLLE7Rq4s3kEOtOpCkkexp9PQaemVbHMZwjLVk7f45iNtZW7KSVOPOBI28hyT6Desyi30WNqPZy12v/EKvNrisCyPfXFvg6BFxcBJbcuiZEDcFLf0J67bHdhwqG5GTLlc9RNGXl9cPmHFkdAVwoAxxueu45PPnNaShDZV8FGULUDtClcx+XI9vaotk1RL5fQu9xuxt0ltPeXCCA4kFJ8QO4A49OlQntE4dnoFdBAwhhstBsqaQUoJ3AgbT1rG0a0zWmXlfYc5wnYl5SfkZH61XWyZsXOLpw7AXMQK9ATpEx57U4q2Rk6Vk1mDD0qsktgfdSEzHkKK2COaPjTyuu57K2MVYaKjh1+ypZHRKkKTJ+emtGHTK824nEWAXiWX7EAEBDmgzt4FdevWNorYtoySZlzJazcOXduk62FJfmP5SN/w3+Vcz5DD9TG2jofG5/pZETeFPovWUEQFASR0ryU40exhKyZRaoDIAPrpJ2FU36La9jF9gL1kbyOtTTINDNzCWnhsjj+Xcf+RVikQcK2Ql1g6kud2HdyeP7ipKZBwMCMuz41yCOp3P0qX1CH0yQs8IZaAVvPmTzVbmWLGTNrbIHiG3UVU2XqOh7bo0qKg2UiJM1Al0TViFFIJ2A52pEjK7di2SVq2Snj1ppWKTpDPKrL+YcbViziCbOyJDaiNlLHJ9YmPefKu98bxK/kkec+U5nk/pRf8Ap1N8O+Xhc4g/ma5a/hWssMEjlwgaiPYbfM12JaOLBXs31bK+24shB4a8R9+lKqRMs76S3alSD4tMD3Ow/Oo0MVbBIt9SB4SZG3TgH8J+dOiIhIK1gDfepJEiTYZWI6QKdEWzP3aTzwOalRE1Z2z9pLeWrAYPg1yBdvjxrSZ0iYI96siqTkyvJOtI/9k=",
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            //OK
            "7002277867_MB0038479" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1970, 2, 27, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0038479",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Мъж",
                GenderNameLatin = "Male",
                EGN = "7002277867",
                NationalityList = new Nationality[]
                                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                                },
                PersonNames = new PersonNames
                {
                    FirstName = "Свилен",
                    FirstNameLatin = "Svilen",
                    Surname = "Динев",
                    SurnameLatin = "Dinev",
                    FamilyName = "Ванев",
                    LastNameLatin = "Vanev"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Младост 1",
                    SettlementNameLatin = "Mladost 1",
                    LocationCode = "1513",
                    LocationName = "Пирин",
                    LocationNameLatin = "Pirin",
                    BuildingNumber = "1",
                    Entrance = "Б",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "зелени",
                Height = 1.95,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            //OK
            "6010015061_MB0038479" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1960, 10, 1, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0038479",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Мъж",
                GenderNameLatin = "Male",
                EGN = "6010015061",
                NationalityList = new Nationality[]
                                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                                },
                PersonNames = new PersonNames
                {
                    FirstName = "Камен",
                    FirstNameLatin = "Kamen",
                    Surname = "Донев",
                    SurnameLatin = "Donev",
                    FamilyName = "Зидаров",
                    LastNameLatin = "Zidarov"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Люлин 1",
                    SettlementNameLatin = "Lyulin 1",
                    LocationCode = "1623",
                    LocationName = "Мащерка",
                    LocationNameLatin = "Mashterka",
                    BuildingNumber = "11",
                    Entrance = "Б",
                    Apartment = "19",
                    Floor = "6"
                },
                EyesColor = "пъстри",
                Height = 1.85,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // Под запрещение
            "7402182030_MB0038398" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1974, 2, 18, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0038398",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "7402182030",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Радост",
                    FirstNameLatin = "Radost",
                    Surname = "Иванова",
                    SurnameLatin = "Ivanova",
                    FamilyName = "Захариева",
                    LastNameLatin = "Zaharieva"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK - under 14
            "1250028580_MB0038479" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(2012, 10, 02, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0038479",
                IssueDate = new DateTime(2013, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "1250028580",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Роза",
                    FirstNameLatin = "Rosa",
                    Surname = "Петрова",
                    SurnameLatin = "Petrova",
                    FamilyName = "Динева",
                    LastNameLatin = "Dineva"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "8",
                    Entrance = "А",
                    Apartment = "17",
                    Floor = "7"
                },
                EyesColor = "сини",
                Height = 1.05,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK - under 18
            "0751114397_MB0038398" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(2007, 11, 11, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0038398",
                IssueDate = new DateTime(2013, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "0751114397",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Красимира",
                    FirstNameLatin = "Krasimira",
                    Surname = "Кирилова",
                    SurnameLatin = "Kirilova",
                    FamilyName = "Пеева",
                    LastNameLatin = "Peeva"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Dianabad",
                    LocationCode = "1103",
                    LocationName = "4-ти километър",
                    LocationNameLatin = "4th km",
                    BuildingNumber = "1",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // OK - foreigner with EGN & LNCh (LNCh = EGN)
            "9310166608_MB0038479" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1993, 10, 16, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "Канада",
                    CountryNameLatin = "Canada",
                    CountryCode = "CAN",
                    DistrictName = "Монреал-център",
                    MunicipalityName = "Монреал",
                    TerritorialUnitName = "Квебек"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0038479",
                IssueDate = new DateTime(2013, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                EGN = "9310166608",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Натали",
                    FirstNameLatin = "Natalie",
                    Surname = "Сенпиер",
                    SurnameLatin = "St-Pierre",
                    FamilyName = "Сенпиер",
                    LastNameLatin = "St-Pierre"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Редута",
                    SettlementNameLatin = "Reduta",
                    LocationCode = "1234",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Malina",
                    BuildingNumber = "7",
                    Entrance = "А",
                    Apartment = "10",
                    Floor = "4"
                },
                EyesColor = "сини",
                Height = 1.05,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // Починал
            "7112234758_MB0038398" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1971, 12, 23, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0038398",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Мъж",
                GenderNameLatin = "Male",
                EGN = "7112234758",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Петкан",
                    FirstNameLatin = "Petkan",
                    Surname = "Смъртников ",
                    SurnameLatin = "Smurtnikov",
                    FamilyName = "Смъртников ",
                    LastNameLatin = "Smurtnikov"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.65,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // Под частично запрещение
            "8703115620_MB0038398" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1987, 3, 11, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB0038398",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Мъж",
                GenderNameLatin = "Male",
                EGN = "8703115620",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Петър",
                    FirstNameLatin = "Petar",
                    Surname = "Спасов",
                    SurnameLatin = "Spasov",
                    FamilyName = "Кимчев",
                    LastNameLatin = "Kimchev"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.69,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            // Под пълно запрещение
            "7207312525_MB0032132" => new PersonalIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                ActualStatusDateSpecified = true,
                ActualStatusDate = DateTime.Now,
                BirthDateSpecified = true,
                BirthDate = new DateTime(1972, 7, 31, 6, 0, 0, 0, DateTimeKind.Local),
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                },
                DocumentType = "Лична карта",
                DocumentTypeLatin = "Identity card",
                IdentityDocumentNumber = "MB003213",
                IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                IssueDateSpecified = true,
                IssuerName = "МВР",
                IssuerNameLatin = "MVR",
                GenderName = "Мъж",
                GenderNameLatin = "Male",
                EGN = "7207312525",
                NationalityList = new Nationality[]
                {
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Константин",
                    FirstNameLatin = "Konstantin",
                    Surname = "Пиринов",
                    SurnameLatin = "Pirinov",
                    FamilyName = "Игнатов",
                    LastNameLatin = "Ignatov"
                },
                PermanentAddress = new PermanentAddress
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                Height = 1.70,
                HeightSpecified = true,
                ValidDate = new DateTime(2030, 4, 4, 6, 0, 0, 0, DateTimeKind.Local),
                ValidDateSpecified = true,
                IssuerPlace = "София",
                IssuerPlaceLatin = "Sofia"
            },
            _ => null
        };

        // Not found ("2710016153", "7701018609") or something else
        result ??= new PersonalIdentityInfoResponseType
        {
            ReturnInformations = new ReturnInformation
            {
                ReturnCode = NotFoundReturnCode,
                Info = $"IdentityDocumentNumber {identityDocumentNumber} or/and EGN {egn} is not found"
            }
        };

        return new RegixSearchResultDTO
        {
            HasFailed = false,
            Response = new Dictionary<string, object?> { { "PersonalIdentityInfoResponse", result } }
        };

    }

    private static RegixSearchResultDTO? GetGRAORelationsSearchResponse(RegiXSearchCommand message)
    {
        // Failed
        if (JObject.Parse(message.Command)?["argument"]?["parameters"] is not JArray parameters)
        {
            return new RegixSearchResultDTO
            {
                HasFailed = true,
                Response = new Dictionary<string, object?> { { "PersonRelation",new PersonRelationType() } }
            };
        }

        var identifierToken = parameters
            .FirstOrDefault(p => p is JObject parameterObject && parameterObject.ContainsKey("EGN"))?
            .SelectToken("EGN") as JObject;

        var identifier = identifierToken?["parameterStringValue"]?.Value<string>();

        var result = identifier switch
        {
            // Internal service error
            "7404300852" => throw new ArgumentException("This EGN is throwing the exception"),
            // RegiX internal error
            "5711196477" => new RelationsResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = "1001",
                    Info = "Something went wrong"
                }
            },
            // person - dead mother 2 nationalities, father, no sisters/brothers
            "3002094302" => new RelationsResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                PersonRelations = new PersonRelationType[]
                {
                    new PersonRelationType
                    {
                        RelationCode = RelationType.Mother,
                        RelationCodeSpecified = true,
                        EGN = "0611152455",
                        BirthDate = new DateTime(1906, 11, 15, 6, 0, 0, 0, DateTimeKind.Local),
                        BirthDateSpecified = true,
                        FirstName = "Мирела",
                        SurName = "Спасова",
                        FamilyName = "Чолакова",
                        Gender = new Gender
                        {
                            GenderName = GenderNameType.Female,
                            GenderNameSpecified = true
                        },
                        Nationality = new Nationality
                        {
                            NationalityName = "България",
                            NationalityCode = "BG",
                            NationalityName2 = "Русия",
                            NationalityCode2 = "RUS",
                        },
                        DeathDate = new DateTime(1980, 12, 06, 6, 0, 0, 0, DateTimeKind.Local),
                        DeathDateSpecified = true,
                    },
                    new PersonRelationType
                    {
                        RelationCode = RelationType.Father,
                        RelationCodeSpecified = true,
                        EGN = "0301257123",
                        BirthDate = new DateTime(1903, 01, 25, 6, 0, 0, 0, DateTimeKind.Local),
                        BirthDateSpecified = true,
                        FirstName = "Пейо",
                        SurName = "Трендафилов",
                        FamilyName = "Чолаков",
                        Gender = new Gender
                        {
                            GenderName = GenderNameType.Male,
                            GenderNameSpecified = true
                        },
                        Nationality = new Nationality
                        {
                            NationalityName = "България",
                            NationalityCode = "BG"
                        },
                        DeathDate = new DateTime(1978, 10, 09, 6, 0, 0, 0, DateTimeKind.Local),
                        DeathDateSpecified = true,
                    }
                }
            },
            // under 14years old - dead mother 2 nationalities, father, no sisters/brothers
            "1250028580" => new RelationsResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                PersonRelations = new PersonRelationType[]
                {
                    new PersonRelationType
                    {
                        RelationCode = RelationType.Mother,
                        RelationCodeSpecified = true,
                        EGN = "8012285394",
                        BirthDate = new DateTime(1980, 12, 28, 6, 0, 0, 0, DateTimeKind.Local),
                        BirthDateSpecified = true,
                        FirstName = "Милена",
                        SurName = "Петева",
                        FamilyName = "Чолакова",
                        Gender = new Gender
                        {
                            GenderName = GenderNameType.Female,
                            GenderNameSpecified = true
                        },
                        Nationality = new Nationality
                        {
                            NationalityName = "България",
                            NationalityCode = "BG",
                            NationalityName2 = "Русия",
                            NationalityCode2 = "RUS",
                        },
                        DeathDate = new DateTime(2020, 12, 06, 6, 0, 0, 0, DateTimeKind.Local),
                        DeathDateSpecified = true,
                    },
                    new PersonRelationType
                    {
                        RelationCode = RelationType.Father,
                        RelationCodeSpecified = true,
                        EGN = "77101853910",
                        BirthDate = new DateTime(1977, 10, 18, 6, 0, 0, 0, DateTimeKind.Local),
                        BirthDateSpecified = true,
                        FirstName = "Петър",
                        SurName = "Малинов",
                        FamilyName = "Бинев",
                        Gender = new Gender
                        {
                            GenderName = GenderNameType.Male,
                            GenderNameSpecified = true
                        },
                        Nationality = new Nationality
                        {
                            NationalityName = "България",
                            NationalityCode = "BG"
                        },
                        DeathDateSpecified = false
                    }
                }
            },
            // under 18years old - mother, father, no sisters/brothers
            "0751114397" => new RelationsResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                PersonRelations = new PersonRelationType[]
                {
                    new PersonRelationType
                    {
                        RelationCode = RelationType.Mother,
                        RelationCodeSpecified = true,
                        EGN = "850115219",
                        BirthDate = new DateTime(1985, 01, 15, 6, 0, 0, 0, DateTimeKind.Local),
                        BirthDateSpecified = true,
                        FirstName = "Керана",
                        SurName = "Ганева",
                        FamilyName = "Топова",
                        Gender = new Gender
                        {
                            GenderName = GenderNameType.Female,
                            GenderNameSpecified = true
                        },
                        Nationality = new Nationality
                        {
                            NationalityName = "България",
                            NationalityCode = "BG"
                        },
                        DeathDateSpecified = false
                    },
                    new PersonRelationType
                    {
                        RelationCode = RelationType.Father,
                        RelationCodeSpecified = true,
                        EGN = "8305258190",
                        BirthDate = new DateTime(1983, 05, 25, 6, 0, 0, 0, DateTimeKind.Local),
                        BirthDateSpecified = true,
                        FirstName = "Кирил",
                        SurName = "Стоев",
                        FamilyName = "Пеев",
                        Gender = new Gender
                        {
                            GenderName = GenderNameType.Male,
                            GenderNameSpecified = true
                        },
                        Nationality = new Nationality
                        {
                            NationalityName = "България",
                            NationalityCode = "BG"
                        },
                        DeathDateSpecified = false,
                    }
                }
            },
            // person - mother, father, one sister
            "9005059158" => new RelationsResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                PersonRelations = new PersonRelationType[]
                {
                    new PersonRelationType
                    {
                        RelationCode = RelationType.Mother,
                        RelationCodeSpecified = true,
                        EGN = "6811143174",
                        BirthDate = new DateTime(1968, 11, 14, 6, 0, 0, 0, DateTimeKind.Local),
                        BirthDateSpecified = true,
                        FirstName = "Пелагия",
                        SurName = "Петрова",
                        FamilyName = "Михнева",
                        Gender = new Gender
                        {
                            GenderName = GenderNameType.Female,
                            GenderNameSpecified = true
                        },
                        Nationality = new Nationality
                        {
                            NationalityName = "България",
                            NationalityCode = "BG"
                        },
                        DeathDateSpecified = false,
                    },
                    new PersonRelationType
                    {
                        RelationCode = RelationType.Father,
                        RelationCodeSpecified = true,
                        EGN = "6403225264",
                        BirthDate = new DateTime(1964, 03, 22, 6, 0, 0, 0, DateTimeKind.Local),
                        BirthDateSpecified = true,
                        FirstName = "Марин",
                        SurName = "Генадиев",
                        FamilyName = "Михнев",
                        Gender = new Gender
                        {
                            GenderName = GenderNameType.Male,
                            GenderNameSpecified = true
                        },
                        Nationality = new Nationality
                        {
                            NationalityName = "България",
                            NationalityCode = "BG"
                        },
                        DeathDateSpecified = false,
                    },
                    new PersonRelationType
                    {
                        RelationCode = RelationType.Sister,
                        RelationCodeSpecified = true,
                        EGN = "9507185390",
                        BirthDate = new DateTime(1995, 07, 18, 6, 0, 0, 0, DateTimeKind.Local),
                        BirthDateSpecified = true,
                        FirstName = "Венелина",
                        SurName = "Маринова",
                        FamilyName = "Михнева",
                        Gender = new Gender
                        {
                            GenderName = GenderNameType.Female,
                            GenderNameSpecified = true
                        },
                        Nationality = new Nationality
                        {
                            NationalityName = "България",
                            NationalityCode = "BG"
                        },
                        DeathDateSpecified = false,
                    }
                }
            },
            _ => null
        };

        // Not found
        result ??= new RelationsResponseType
        {
            ReturnInformations = new ReturnInformation
            {
                ReturnCode = NotFoundReturnCode,
                Info = $"Person with EGN {identifier} is not found"
            }
        };

        return new RegixSearchResultDTO
        {
            HasFailed = false,
            Response = new Dictionary<string, object?> { { "RelationsResponse",result } }
        };
    }

    private static RegixSearchResultDTO? GetStateOfPlayResponse(RegiXSearchCommand message)
    {
        // Failed
        if (JObject.Parse(message.Command)?["argument"]?["parameters"] is not JArray parameters)
        {
            return new RegixSearchResultDTO
            {
                HasFailed = true,
                Response = new Dictionary<string, object?> { { "StateOfPlayResponse",new StateOfPlayResponseType() } }
            };
        }

        var UICToken = parameters
            .FirstOrDefault(p => p is JObject parameterObject && parameterObject.ContainsKey("UIC"))?
            .SelectToken("UIC") as JObject;
        var caseCodeToken = parameters
            .FirstOrDefault(p => p is JObject parameterObject && parameterObject.ContainsKey("CaseCode"))?
            .SelectToken("CaseCode") as JObject;
        var caseYearToken = parameters
            .FirstOrDefault(p => p is JObject parameterObject && parameterObject.ContainsKey("CaseYear"))?
            .SelectToken("CaseYear") as JObject;
        var caseNumberToken = parameters
            .FirstOrDefault(p => p is JObject parameterObject && parameterObject.ContainsKey("CaseNumber"))?
            .SelectToken("CaseNumber") as JObject;

        var UIC = UICToken?["parameterStringValue"]?.Value<string>();
        var caseCourtCode = caseCodeToken?["parameterStringValue"]?.Value<string>() ?? default(string);
        var caseYear = caseYearToken?["parameterNumberValue"]?.Value<int>() ?? default(int);
        var caseNumber = caseNumberToken?["parameterStringValue"]?.Value<string>() ?? default(string);

        var caseAttributes = $"{caseCourtCode}_{caseYear}_{caseNumber}";
        StateOfPlayResponseType? result;

        if (UIC != null)
        {
            result = UIC switch
            {
                // Internal service error
                "895608380" => throw new ArgumentException("This UIC is throwing the exception"),
                // RegiX internal error
                "402891869" => new StateOfPlayResponseType
                {
                    ReturnInformations = new ReturnInformation
                    {
                        ReturnCode = NotFoundReturnCode,
                    }
                },
                //Компания с данни от тестовия Булстат
                //Managers: МАРИЯ ГЕОРГИЕВА ГЕОРГИЕВА; 
                "1212120908" => GetBulstatInfoForTestBulstatCompany(),
                //********* UIC - 9 digits (за отхвърляне по Вид събитие = 563)
                //Managers: Калата, Лили, Юлия; 
                "621181606" => GetBulstatInfoForQAsCompanyByUIC("621181606", "563"),
                // UIC - 9 digits (за отхвърляне по Вид събитие = 564)
                //Managers: Калата, Лили, Юлия; 
                "044418396" => GetBulstatInfoForQAsCompanyByUIC("044418396", "564"),
                // UIC - 9 digits (за отхвърляне по Вид събитие = 565)
                //Managers: Калата, Лили, Юлия; 
                "976359865" => GetBulstatInfoForQAsCompanyByUIC("976359865", "565"),
                // UIC - 13 digits (за отхвърляне по Вид събитие = 566)/
                //Managers: Калата, Лили, Юлия; 
                "1579860016297" => GetBulstatInfoForQAsCompanyByUIC("1579860016297", "566"),
                // UIC - 9 digits (за отхвърляне по Вид събитие = 568)
                //Managers: Калата, Лили, Юлия; 
                "690795006" => GetBulstatInfoForQAsCompanyByUIC("690795006", "568"),
                // UIC - 9 digits (за отхвърляне по Вид събитие = 569)
                //Managers: Калата, Лили, Юлия; 
                "384872922" => GetBulstatInfoForQAsCompanyByUIC("38487292", "569"),
                // UIC - 9 digits (за отхвърляне по Вид събитие = 570)
                //Managers: Калата, Лили, Юлия; 
                "381239712" => GetBulstatInfoForQAsCompanyByUIC("38123971", "570"),
                // UIC - 9 digits (за отхвърляне по Вид събитие = 1071)
                //Managers: Калата, Лили, Юлия; 
                "133151135" => GetBulstatInfoForQAsCompanyByUIC("133151135", "1071"),
                // UIC - 9 digits (ВАЛИДНА по Вид събитие = 550)
                //Managers: Калата, Лили, Юлия; 
                "653743918" => GetBulstatInfoForQAsCompanyByUIC("653743918", "550"),

                //********* UIC - 9 digits (за отхвърляне по състояние (SubjectPropState) = 1)
                //Managers: Калата, Лили, Юлия; 
                "202246846" => GetBulstatInfoForQAsCompanyByUIC("202246846", "550", subjPropState: "1"),
                // UIC - 9 digits (за отхвърляне по състояние (SubjectPropState) = 2)
                //Managers: Калата, Лили, Юлия; 
                "177286693" => GetBulstatInfoForQAsCompanyByUIC("177286693", "550", subjPropState: "2"),
                // UIC - 9 digits (за отхвърляне по състояние (SubjectPropState) = 572)
                //Managers: Калата, Лили, Юлия; 
                "206399526" => GetBulstatInfoForQAsCompanyByUIC("206399526", "550", subjPropState: "572"),
                // UIC - 9 digits (за отхвърляне по състояние (SubjectPropState) = 573)
                //Managers: Калата, Лили, Юлия; 
                "125500295" => GetBulstatInfoForQAsCompanyByUIC("125500295", "550", subjPropState: "573"),
                // UIC - 9 digits (за отхвърляне по състояние (SubjectPropState) = 574)
                //Managers: Калата, Лили, Юлия; 
                "130007884" => GetBulstatInfoForQAsCompanyByUIC("130007884", "550", subjPropState: "574"),
                // UIC - 9 digits (за отхвърляне по състояние (SubjectPropState) = 575)
                //Managers: Калата, Лили, Юлия; 
                "130007973" => GetBulstatInfoForQAsCompanyByUIC("130007973", "550", subjPropState: "575"),

                //********* UIC - 9 digits (за отхвърляне по Вид вписване (EntryType) = 758)
                //Managers: Калата, Лили, Юлия; 
                "131416460" => GetBulstatInfoForQAsCompanyByUIC("131416460", "550", entryType: "758"),

                //********* UIC - 9 digits (ОК! QA са сред управителите)
                //Managers: Калата, Лили, Юлия; 
                "131458121" => GetBulstatInfoQAsInPartnersByUIC("131458121", "550", entryType: "756"),

                //********* UIC - 9 digits (ОК! QA са сред партньорите)
                //Partners: Калата, Лили, Юлия; 
                "201095658" => GetBulstatInfoQAsInPartnersByUIC("201095658", "550", entryType: "756"),

                //********* UIC - 9 digits (ОК! QA са в колективния орган)
                //CollectiveBody: Калата, Лили, Юлия; 
                "201114182" => GetBulstatInfoQAsInCollectiveBodyByUIC("20111418", "550", entryType: "756"),

                //********* UIC - 9 digits (ОК! DEAU: Изпълнителна агенция по лекарствата, QA са сред управителите)
                //Managers: Калата, Лили, Юлия, Илия; 
                "121203554" => GetBulstatInfoQAsInPartnersByUIC("121203554", "550", entryType: "756", cyrillicFullName: "Изпълнителна агенция по лекарствата", cyrillicShortName: "Лекарствата"),

                //********* UIC - 9 digits (ОК! DEAU: Комисия за защита на личните данни, QA са сред управителите)
                //Managers: Калата, Лили, Юлия, Илия; 
                "130961721" => GetBulstatInfoQAsInPartnersByUIC("130961721", "550", entryType: "756", cyrillicFullName: "Комисия за защита на личните данни", cyrillicShortName: "Лични данни"),

                //********* UIC - 9 digits (ОК! DEAU: Министерство на правосъдието, QA са сред управителите)
                //Managers: Калата, Лили, Юлия, Илия; 
                "000695349" => GetBulstatInfoQAsInPartnersByUIC("000695349", "550", entryType: "756", cyrillicFullName: "Министерство на правосъдието", cyrillicShortName: "Правосъдието"),

                //********* UIC - 9 digits (ОК! DEAU: Лесозащитна станция - гр. Варна, QA са сред управителите)
                //Managers: Калата, Лили, Юлия, Илия; 
                "103618024" => GetBulstatInfoQAsInPartnersByUIC("103618024", "550", entryType: "756", cyrillicFullName: "Лесозащитна станция - гр. Варна", cyrillicShortName: "Лесото - гр. Варна"),

                //********* UIC - 9 digits (За отхвърляне! QA не присъстват във фирмата)
                //CollectiveBody: Калата, Лили, Юлия; 
                "201538609" => GetBulstatInfoQAsNotInCompanyByUIC("201538609", "550", entryType: "756"),

                _ => null
            };

            // Not found
            result ??= new StateOfPlayResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = NotFoundReturnCode,
                    Info = $"Subject with UIC {UIC} is not found"
                }
            };
        }
        //UIC is null => must use Case
        else
        {
            result = caseAttributes switch
            {
                // Internal service error
                "234_2020_4" => throw new ArgumentException("This caseAttributes is throwing the exception"),
                // RegiX internal error
                "234_2020_6" => new StateOfPlayResponseType
                {
                    ReturnInformations = new ReturnInformation
                    {
                        ReturnCode = NotFoundReturnCode,
                    }
                },
                //********* UIC - 9 digits (за отхвърляне по Вид събитие = 563)
                //Managers: Калата, Лили, Юлия; 
                "235_2023_16" => GetBulstatInfoForQAsCompanyByCase(caseCourtCode, caseYear, caseNumber, "563"),

                //********* UIC - 9 digits (ОК! по Вид събитие = 563)
                //Managers: Калата, Лили, Юлия; 
                "335_2024_16" => GetBulstatInfoForQAsCompanyByCase(caseCourtCode, caseYear, caseNumber, "550"),

                _ => null
            };

            // Not found
            result ??= new StateOfPlayResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = NotFoundReturnCode,
                    Info = $"Subject with caseAttributes {caseAttributes} is not found"
                }
            };
        }

        return new RegixSearchResultDTO
        {
            HasFailed = false,
            Response = new Dictionary<string, object?> { { "StateOfPlayResponse",result } }
        };
    }

    private static RegixSearchResultDTO? FetchNomenclaturesResponse(RegiXSearchCommand message)
    {
        // Failed
        if (JObject.Parse(message.Command)?["argument"]?["parameters"] is not JArray parameters)
        {
            return new RegixSearchResultDTO
            {
                HasFailed = true,
                Response = new Dictionary<string, object?> { { "FetchNomenclaturesResponse",new FetchNomenclaturesResponseType() } }
            };
        }

        FetchNomenclaturesResponseType? result = new FetchNomenclaturesResponseType
        {
            CountryNomElement = new CountryMultilangNomElement[]
            {
                new CountryMultilangNomElement()
                {
                    Code = "36",
                    NameBG = "АВСТРАЛИЯ",
                    Ordering = 1,
                    Active = true,
                    ISO2 = "AU",
                    ISO3 = "AUS"
                },
                new CountryMultilangNomElement()
                {
                    Code = "100",
                    NameBG = "БЪЛГАРИЯ",
                    Ordering = 1,
                    Active = true,
                    ISO2 = "BG",
                    ISO3 = "BGR"
                },
                new CountryMultilangNomElement()
                {
                    Code = "250",
                    NameBG = "ФРАНЦИЯ",
                    Ordering = 1,
                    Active = true,
                    ISO2 = "FR",
                    ISO3 = "FRA"
                },
            },
            SimpleNomenclature = new SimpleNomenclature[]
            {
                new SimpleNomenclature()
                {
                    Definition = new MetaDefinition()
                    {
                        Code = "28",
                        Name = "Вид документ за самоличност"
                    },
                    NomElement = new MultilangNomElement[]
                    {
                        new MultilangNomElement()
                        {
                            Code = "787",
                            NameBG = "граждански паспорт",
                            Ordering = 1,
                            Active = true,
                        },
                        new MultilangNomElement()
                        {
                            Code = "124",
                            NameBG = "карта на продължително пребиваващ в Република България чужденец",
                            Ordering = 2,
                            Active = true,
                        },
                        new MultilangNomElement()
                        {
                            Code = "788",
                            NameBG = "лична карта",
                            Ordering = 3,
                            Active = true,
                        },
                    }
                },
                new SimpleNomenclature()
                {
                    Definition = new MetaDefinition()
                    {
                        Code = "7",
                        Name = "Вид събитие"
                    },
                    NomElement = new MultilangNomElement[]
                        {
                            new MultilangNomElement()
                            {
                                Code = "550",
                                NameBG = "нов субект",
                                NameEN = "new subject",
                                Ordering = 1,
                                Active = true,
                            },
                            new MultilangNomElement()
                            {
                                Code = "551",
                                NameBG = "новообразуван чрез сливане",
                                Ordering = 2,
                                Active = true,
                            },
                            new MultilangNomElement()
                            {
                                Code = "55",
                                NameBG = "новообразуван чрез разделяне",
                                Ordering = 3,
                                Active = true,
                            },
                            new MultilangNomElement()
                            {
                                Code = "553",
                                NameBG = "новообразуван чрез отделяне",
                                Ordering = 4,
                                Active = true,
                            },
                            new MultilangNomElement()
                            {
                                Code = "555",
                                NameBG = "промяна на обстоятелства",
                                Ordering = 6,
                                Active = true,
                            },
                            new MultilangNomElement()
                            {
                                Code = "556",
                                NameBG = "преобразуване по ЗППДОбП",
                                Ordering = 7,
                                Active = false,
                            },
                            new MultilangNomElement()
                            {
                                Code = "557",
                                NameBG = "преобразуване на търговско дружество по ТЗ",
                                Ordering = 8,
                                Active = false,
                            },
                            new MultilangNomElement()
                            {
                                Code = "558",
                                NameBG = "промяна на юридически статут с нормативен/административен акт",
                                Ordering = 9,
                                Active = true,
                            },
                            new MultilangNomElement()
                            {
                                Code = "559",
                                NameBG = "промяна при сделка с предприятие",
                                Ordering = 10,
                                Active = false,
                            },
                            new MultilangNomElement()
                            {
                                Code = "560",
                                NameBG = "промяна чрез вливане на друг субект",
                                Ordering = 11,
                                Active = true,
                            },
                            new MultilangNomElement()
                            {
                                Code = "561",
                                NameBG = "възобновяване на прекратен субект",
                                Ordering = 12,
                                Active = true,
                            },
                            new MultilangNomElement()
                            {
                                Code = "56",
                                NameBG = "актуализация по данни от други регистри и информационни системи",
                                Ordering = 13,
                                Active = true,
                            },
                            new MultilangNomElement()
                            {
                                Code = "563",
                                NameBG = "закрит",
                                NameEN = "closed",
                                Ordering = 14,
                                Active = true,
                            },
                            new MultilangNomElement()
                            {
                                Code = "564",
                                NameBG = "прекратен чрез сливане",
                                Ordering = 15,
                                Active = true,
                            },
                            new MultilangNomElement()
                            {
                                Code = "566",
                                NameBG = "прекратен чрез вливане",
                                Ordering = 16,
                                Active = true,
                            },
                            new MultilangNomElement()
                            {
                                Code = "566",
                                NameBG = "прекратен чрез разделяне",
                                Ordering = 17,
                                Active = true,
                            },
                            new MultilangNomElement()
                            {
                                Code = "568",
                                NameBG = "заличен в съдебен регистър",
                                Ordering = 19,
                                Active = true,
                            },
                            new MultilangNomElement()
                            {
                                Code = "569",
                                NameBG = "заличен в регистъра на БТПП",
                                Ordering = 20,
                                Active = true,
                            },
                            new MultilangNomElement()
                            {
                                Code = "570",
                                NameBG = "анулирана регистрация в БУЛСТАТ",
                                Ordering = 21,
                                Active = true,
                            },
                            new MultilangNomElement()
                            {
                                Code = "1071",
                                NameBG = "прекратен при сделка с предприятие",
                                Ordering = 22,
                                Active = false,
                            },
                            new MultilangNomElement()
                            {
                                Code = "1227",
                                NameBG = "преобразуване по закон",
                                Ordering = 23,
                                Active = true,
                            },
                            new MultilangNomElement()
                            {
                                Code = "1524",
                                NameBG = "новоучредено търговско дружество при промяна на правната форма",
                                Ordering = 26,
                                Active = false,
                            },
                            new MultilangNomElement()
                            {
                                Code = "1526",
                                NameBG = "прекратено търговско дружество при промяна на правната форма",
                                Ordering = 27,
                                Active = false,
                            },
                        }
                },
           new SimpleNomenclature()
                {
                    Definition = new MetaDefinition()
                    {
                        Code = "8",
                        Name = "Състояние"
                    },
                    NomElement = new MultilangNomElement[]
                        {
                            new MultilangNomElement()
                            {
                                Code = "571",
                                NameBG = "развиващ дейност",
                                Ordering = 1,
                                Active = true,
                            },
                            new MultilangNomElement()
                            {
                                Code = "57",
                                NameBG = "в производство по несъстоятелност",
                                Ordering = 2,
                                Active = false,
                            },
                            new MultilangNomElement()
                            {
                                Code = "573",
                                NameBG = "в несъстоятелност",
                                Ordering = 3,
                                Active = false,
                            },
                            new MultilangNomElement()
                            {
                                Code = "574",
                                NameBG = "в ликвидация",
                                Ordering = 4,
                                Active = true,
                            },
                            new MultilangNomElement()
                            {
                                Code = "575",
                                NameBG = "неактивен",
                                Ordering = 5,
                                Active = true,
                            },
                            new MultilangNomElement()
                            {
                                Code = "1",
                                NameBG = "пререгистриран в ТР",
                                Ordering = 6,
                                Active = true,
                            },
                            new MultilangNomElement()
                            {
                                Code = "2",
                                NameBG = "архивиран",
                                NameEN = "archived",
                                Ordering = 7,
                                Active = true,
                            },
                         }
                },
           new SimpleNomenclature()
                {
                    Definition = new MetaDefinition()
                    {
                        Code = "25",
                        Name = "Вид вписване"
                    },
                    NomElement = new MultilangNomElement[]
                        {
                            new MultilangNomElement()
                            {
                                Code = "756",
                                NameBG = "първоначално вписване",
                                Ordering = 1,
                                Active = true,
                            },
                            new MultilangNomElement()
                            {
                                Code = "757",
                                NameBG = "промяна в обстоятелствата",
                                Ordering = 2,
                                Active = true,
                            },
                            new MultilangNomElement()
                            {
                                Code = "758",
                                NameBG = "заличаване",
                                Ordering = 3,
                                Active = true,
                            },
                            new MultilangNomElement()
                            {
                                Code = "574",
                                NameBG = "в ликвидация",
                                Ordering = 4,
                                Active = true,
                            },
                            new MultilangNomElement()
                            {
                                Code = "575",
                                NameBG = "неактивен",
                                Ordering = 5,
                                Active = true,
                            },
                            new MultilangNomElement()
                            {
                                Code = "1",
                                NameBG = "пререгистриран в ТР",
                                Ordering = 6,
                                Active = true,
                            },
                            new MultilangNomElement()
                            {
                                Code = "2",
                                NameBG = "архивиран",
                                Ordering = 7,
                                Active = true,
                            },
                         }
                }
            },
        };

        return new RegixSearchResultDTO
        {
            HasFailed = false,
            Response = new Dictionary<string, object?> { { "FetchNomenclaturesResponse",result } }
        };
    }

    private static RegixSearchResultDTO? GetMVRForeignIdentityV2Response(RegiXSearchCommand message)
    {
        // Failed
        if (JObject.Parse(message.Command)?["argument"]?["parameters"] is not JArray parameters)
        {
            return new RegixSearchResultDTO
            {
                HasFailed = true,
                Response = new Dictionary<string, object?> { { "ForeignIdentityInfoResponse", new ForeignIdentityInfoResponseType() } }
            };
        }

        var identifier = GetJParmaterStringValue(parameters, "Identifier");
        var identifierType = GetJParmaterStringValue(parameters, "IdentifierType");

        var result = identifier switch
        {
            // Internal service error
            "0009443411" => throw new ArgumentException("This LNCh is throwing the exception"),
            // RegiX internal error
            "5796321683" => new ForeignIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = "1001",
                    Info = "Something went wrong"
                }
            },
            // Dead person
            "0088207171" => new ForeignIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                BirthDate = "1920-01-02",
                DeathDate = new DateTime(2020, 10, 22, 6, 0, 0, 0, DateTimeKind.Local),
                DeathDateSpecified = true,
                LNCh = identifier,
                GenderName = "Мъж",
                GenderNameLatin = "Male",
                IdentityDocument = new IdentityDocument
                {
                    DocumentType = "Лична карта",
                    DocumentTypeLatin = "Identity card",
                    IdentityDocumentNumber = "MB0038398",
                    IssuerName = "МВР",
                    IssuerNameLatin = "MVR",
                    IssueDate = new DateTime(2002, 12, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    IssueDateSpecified = true,
                    IssuePlace = "Велико Търново",
                    IssuePlaceLatin = "Veliko Tarnovo",
                    StatusCyrillic = "Временно пребиваващ",
                    StatusDate = new DateTime(2010, 12, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDate = new DateTime(2026, 1, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDateSpecified = true
                },
                NationalityList = new Nationality[]
                {
                    new Nationality
                    {
                        NationalityCode = "ИТА",
                        NationalityCode2 = "ИТ",
                        NationalityName = "Италия",
                        NationalityName2 = "Италия",
                        NationalityNameLatin = "Italia"
                    },
                    new Nationality
                    {
                        NationalityCode = "БЛГ",
                        NationalityCode2 = "БГ",
                        NationalityName = "България",
                        NationalityName2 = "България",
                        NationalityNameLatin = "Bulgaria"
                    }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Мохамед",
                    FirstNameLatin = "Mohamed",
                    Surname = "Мохамед",
                    SurnameLatin = "Mohamed",
                    FamilyName = "Мохамедов",
                    LastNameLatin = "Mohamedov"
                }
            },
            // 10 years old 
            "1460721780" => new ForeignIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                BirthDate = DateTime.UtcNow.AddYears(-10).ToString("yyyy-MM-dd"),
                LNCh = identifier,
                GenderName = "Жена",
                GenderNameLatin = "Female",
                IdentityDocument = new IdentityDocument
                {
                    DocumentType = "Лична карта",
                    DocumentTypeLatin = "Identity card",
                    IdentityDocumentNumber = "MB0038398",
                    IssuerName = "МВР",
                    IssuerNameLatin = "MVR",
                    IssueDate = new DateTime(2002, 12, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    IssueDateSpecified = true,
                    IssuePlace = "Велико Търново",
                    IssuePlaceLatin = "Veliko Tarnovo",
                    StatusCyrillic = "Временно пребиваващ",
                    StatusDate = new DateTime(2010, 12, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDate = new DateTime(2026, 1, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDateSpecified = true
                },
                NationalityList = new Nationality[]
                {
                    new Nationality
                    {
                        NationalityCode = "ИТА",
                        NationalityCode2 = "ИТ",
                        NationalityName = "Италия",
                        NationalityName2 = "Италия",
                        NationalityNameLatin = "Italia"
                    },
                    new Nationality
                    {
                        NationalityCode = "БЛГ",
                        NationalityCode2 = "БГ",
                        NationalityName = "България",
                        NationalityName2 = "България",
                        NationalityNameLatin = "Bulgaria"
                    }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Ивана",
                    FirstNameLatin = "Ivana",
                    Surname = "Иванова",
                    SurnameLatin = "Ivanova",
                    FamilyName = "Иванова",
                    LastNameLatin = "Ivanova"
                },
                EyesColor = "сини",
                EyesColorLatin = "blue",
                Height = 1.65,
                HeightSpecified = true,
                BirthPlace = new BirthPlace
                {
                    CountryName = "ИТА",
                }
            },
            // 17 years old 
            "9585736702" => new ForeignIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                BirthDate = DateTime.UtcNow.AddYears(-17).ToString("yyyy-MM-dd"),
                LNCh = identifier,
                GenderName = "Жена",
                GenderNameLatin = "Female",
                IdentityDocument = new IdentityDocument
                {
                    DocumentType = "Лична карта",
                    DocumentTypeLatin = "Identity card",
                    IdentityDocumentNumber = "MB0038398",
                    IssuerName = "МВР",
                    IssuerNameLatin = "MVR",
                    IssueDate = new DateTime(2002, 12, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    IssueDateSpecified = true,
                    IssuePlace = "Велико Търново",
                    IssuePlaceLatin = "Veliko Tarnovo",
                    StatusCyrillic = "Временно пребиваващ",
                    StatusDate = new DateTime(2010, 12, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDate = new DateTime(2026, 1, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDateSpecified = true
                },
                NationalityList = new Nationality[]
                {
                    new Nationality
                    {
                        NationalityCode = "ИТА",
                        NationalityCode2 = "ИТ",
                        NationalityName = "Италия",
                        NationalityName2 = "Италия",
                        NationalityNameLatin = "Italia"
                    },
                    new Nationality
                    {
                        NationalityCode = "БЛГ",
                        NationalityCode2 = "БГ",
                        NationalityName = "България",
                        NationalityName2 = "България",
                        NationalityNameLatin = "Bulgaria"
                    }
                 },
                PersonNames = new PersonNames
                {
                    FirstName = "Марина",
                    FirstNameLatin = "Marina",
                    Surname = "Петкова",
                    SurnameLatin = "Petkova",
                    FamilyName = "Драганова",
                    LastNameLatin = "Draganova"
                },
                PermanentAddress = new AddressBG
                {
                    DistrictName = "Великотърновска",
                    DistrictNameLatin = "Veliko Tarnovo",
                    MunicipalityName = "Горнооряховска",
                    MunicipalityNameLatin = "Gorna Oriahovitza",
                    SettlementCode = "5100",
                    SettlementName = "Горна Оряховица",
                    SettlementNameLatin = "Gorna Oriahovitza",
                    LocationCode = "Божур",
                    LocationName = "улица Божур",
                    LocationNameLatin = "Bojur street",
                    BuildingNumber = "2",
                    Entrance = "B",
                    Apartment = "2",
                    Floor = "1"
                },
                EyesColor = "сини",
                EyesColorLatin = "blue",
                Height = 1.65,
                HeightSpecified = true,
                BirthPlace = new BirthPlace
                {
                    CountryName = "ИТА",
                }
            },
            // OK
            "8341361929" => new ForeignIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                BirthDate = "1999-08-30",
                LNCh = identifier,
                GenderName = "Жена",
                GenderNameLatin = "Female",
                IdentityDocument = new IdentityDocument
                {
                    DocumentType = "Лична карта",
                    DocumentTypeLatin = "Identity card",
                    IdentityDocumentNumber = "MB003213",
                    IssuerName = "МВР",
                    IssuerNameLatin = "MVR",
                    IssueDate = new DateTime(2002, 12, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    IssueDateSpecified = true,
                    IssuePlace = "Велико Търново",
                    IssuePlaceLatin = "Veliko Tarnovo",
                    StatusCyrillic = "Временно пребиваващ",
                    StatusDate = new DateTime(2010, 12, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDate = new DateTime(2026, 1, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDateSpecified = true
                },
                NationalityList = new Nationality[]
                    {
                        new Nationality
                        {
                            NationalityCode = "ИТА",
                            NationalityCode2 = "ИТ",
                            NationalityName = "Италия",
                            NationalityName2 = "Италия",
                            NationalityNameLatin = "Italia"
                        },
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                    },
                PersonNames = new PersonNames
                {
                    FirstName = "Лорета",
                    FirstNameLatin = "Loreta",
                    Surname = "Винче",
                    SurnameLatin = "Vince",
                    FamilyName = "Винче",
                    LastNameLatin = "Vince"
                },
                PermanentAddress = new AddressBG
                {
                    DistrictName = "Великотърновска",
                    DistrictNameLatin = "Veliko Tarnovo",
                    MunicipalityName = "Горнооряховска",
                    MunicipalityNameLatin = "Gorna Oriahovitza",
                    SettlementCode = "5100",
                    SettlementName = "Горна Оряховица",
                    SettlementNameLatin = "Gorna Oriahovitza",
                    LocationCode = "Божур",
                    LocationName = "улица Божур",
                    LocationNameLatin = "Bojur street",
                    BuildingNumber = "2",
                    Entrance = "B",
                    Apartment = "2",
                    Floor = "1"
                },
                //Converting a picture to Base64 via https://www.base64-image.de/
                Picture = "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",
                EyesColor = "сини",
                EyesColorLatin = "blue",
                Height = 1.65,
                HeightSpecified = true,
                BirthPlace = new BirthPlace
                {
                    CountryName = "Италия",
                    CountryNameLatin = "Italy",
                    CountryCode = "1293",
                    DistrictName = "Рим",
                    MunicipalityName = "Област Рим",
                    TerritorialUnitName = "Рим център"
                }
            },
            // OK, "ПОСТОЯННО ПРЕБИВАВАЩ В РБ"
            "5789302455" => new ForeignIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                BirthDate = "2002-09-30",
                LNCh = identifier,
                GenderName = "Жена",
                GenderNameLatin = "Female",
                IdentityDocument = new IdentityDocument
                {
                    DocumentType = "Лична карта",
                    DocumentTypeLatin = "Identity card",
                    IdentityDocumentNumber = "MB0038398",
                    IssuerName = "МВР",
                    IssuerNameLatin = "MVR",
                    IssueDate = new DateTime(2002, 12, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    IssueDateSpecified = true,
                    IssuePlace = "Велико Търново",
                    IssuePlaceLatin = "Veliko Tarnovo",
                    StatusDate = new DateTime(2010, 12, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDate = new DateTime(2026, 1, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDateSpecified = true,
                    RPTypeOfPermit = "ПОСТОЯННО ПРЕБИВАВАЩ В РБ"
                },
                NationalityList = new Nationality[]
                    {
                        new Nationality
                        {
                            NationalityCode = "ФРА",
                            NationalityCode2 = "ФР",
                            NationalityName = "Франция",
                            NationalityName2 = "Франция",
                            NationalityNameLatin = "France"
                        },
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                    },
                PersonNames = new PersonNames
                {
                    FirstName = "Валери",
                    FirstNameLatin = "Valery",
                    Surname = "Трамбле",
                    SurnameLatin = "Tremblay",
                    FamilyName = "Сенпиер",
                    LastNameLatin = "StPierre"
                },
                PermanentAddress = new AddressBG
                {
                    DistrictName = "Великотърновска",
                    DistrictNameLatin = "Veliko Tarnovo",
                    MunicipalityName = "Горнооряховска",
                    MunicipalityNameLatin = "Gorna Oriahovitza",
                    SettlementCode = "5100",
                    SettlementName = "Горна Оряховица",
                    SettlementNameLatin = "Gorna Oriahovitza",
                    LocationCode = "Мак",
                    LocationName = "улица Мак",
                    LocationNameLatin = "Mak street",
                    BuildingNumber = "4",
                    Entrance = "A",
                    Apartment = "2",
                    Floor = "1"
                },
                EyesColor = "сини",
                EyesColorLatin = "blue",
                Height = 1.65,
                HeightSpecified = true,
                BirthPlace = new BirthPlace
                {
                    CountryName = "Франция",
                    CountryNameLatin = "France",
                    CountryCode = "0093",
                    DistrictName = "Париж",
                    MunicipalityName = "Област Париж",
                    TerritorialUnitName = "Париж център"
                }
            },
            // OK, "ПОСТОЯННО ПРЕБИВАВАЩ В РБ"
            "9310166608" => new ForeignIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                BirthDate = "1993-10-16",
                LNCh = identifier,
                GenderName = "Жена",
                GenderNameLatin = "Female",
                IdentityDocument = new IdentityDocument
                {
                    DocumentType = "Паспорт",
                    DocumentTypeLatin = "Passport",
                    IdentityDocumentNumber = "38495330",
                    IssuerName = "МВР",
                    IssuerNameLatin = "MVR",
                    IssueDate = new DateTime(2013, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    IssueDateSpecified = true,
                    IssuePlace = "Велико Търново",
                    IssuePlaceLatin = "Veliko Tarnovo",
                    StatusDate = new DateTime(2010, 12, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDate = new DateTime(2026, 1, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDateSpecified = true,
                    RPTypeOfPermit = "ПОСТОЯННО ПРЕБИВАВАЩ В РБ"
                },
                NationalityList = new Nationality[]
                    {
                        new Nationality
                        {
                            NationalityCode = "КАН",
                            NationalityCode2 = "КН",
                            NationalityName = "Канада",
                            NationalityName2 = "Канада",
                            NationalityNameLatin = "Canada"
                        },
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                    },
                PersonNames = new PersonNames
                {
                    FirstName = "Натали",
                    FirstNameLatin = "Natalie",
                    Surname = "Сенпиер",
                    SurnameLatin = "St-Pierre",
                    FamilyName = "Сенпиер",
                    LastNameLatin = "St-Pierre"
                },
                PermanentAddress = new AddressBG
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Редута",
                    SettlementNameLatin = "Reduta",
                    LocationCode = "1234",
                    LocationName = "Малина",
                    LocationNameLatin = "Malina",
                    BuildingNumber = "7",
                    Entrance = "А",
                    Apartment = "10",
                    Floor = "4"
                },
                EyesColor = "сини",
                EyesColorLatin = "blue",
                Height = 1.65,
                HeightSpecified = true,
                BirthPlace = new BirthPlace
                {
                    CountryName = "Канада",
                    CountryNameLatin = "Canada",
                    CountryCode = "001",
                    DistrictName = "Монреал-център",
                    MunicipalityName = "Монреал",
                    TerritorialUnitName = "Квебек"
                }
            },
            // OK, "ПРЕДОСТАВЕНО УБЕЖИЩЕ"
            "9066356559" => new ForeignIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                BirthDate = "1983-09-16",
                LNCh = identifier,
                GenderName = "Жена",
                GenderNameLatin = "Female",
                IdentityDocument = new IdentityDocument
                {
                    DocumentType = "Паспорт",
                    DocumentTypeLatin = "Passport",
                    IdentityDocumentNumber = "38445760",
                    IssuerName = "МВР",
                    IssuerNameLatin = "MVR",
                    IssueDate = new DateTime(2013, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    IssueDateSpecified = true,
                    IssuePlace = "Велико Търново",
                    IssuePlaceLatin = "Veliko Tarnovo",
                    StatusDate = new DateTime(2010, 12, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDate = new DateTime(2026, 1, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDateSpecified = true,
                    RPTypeOfPermit = "ПРЕДОСТАВЕНО УБЕЖИЩЕ"
                },
                NationalityList = new Nationality[]
                    {
                        new Nationality
                        {
                            NationalityCode = "ИНД",
                            NationalityCode2 = "ИН",
                            NationalityName = "Индия",
                            NationalityName2 = "Индия",
                            NationalityNameLatin = "India"
                        }
                    },
                PersonNames = new PersonNames
                {
                    FirstName = "Индира",
                    FirstNameLatin = "Indira",
                    Surname = "Катапури",
                    SurnameLatin = "Katapuri",
                    FamilyName = "Мадури",
                    LastNameLatin = "Maduri"
                },
                PermanentAddress = new AddressBG
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Редута",
                    SettlementNameLatin = "Reduta",
                    LocationCode = "1234",
                    LocationName = "Малинака",
                    LocationNameLatin = "Malinaka",
                    BuildingNumber = "17",
                    Entrance = "А",
                    Apartment = "10",
                    Floor = "4"
                },
                EyesColor = "сини",
                EyesColorLatin = "blue",
                Height = 1.65,
                HeightSpecified = true,
                BirthPlace = new BirthPlace
                {
                    CountryName = "Индия",
                    CountryNameLatin = "India",
                    CountryCode = "91",
                    DistrictName = "Делхи-център",
                    MunicipalityName = "Делхи",
                    TerritorialUnitName = "-"
                }
            },
            // OK, "ЧУЖДЕНЕЦ С ХУМАНИТАРЕН СТАТУТ"
            "7704143022" => new ForeignIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                BirthDate = "1973-07-16",
                LNCh = identifier,
                GenderName = "Жена",
                GenderNameLatin = "Female",
                IdentityDocument = new IdentityDocument
                {
                    DocumentType = "Паспорт",
                    DocumentTypeLatin = "Passport",
                    IdentityDocumentNumber = "38495755",
                    IssuerName = "МВР",
                    IssuerNameLatin = "MVR",
                    IssueDate = new DateTime(2011, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    IssueDateSpecified = true,
                    IssuePlace = "Велико Търново",
                    IssuePlaceLatin = "Veliko Tarnovo",
                    StatusDate = new DateTime(2010, 12, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDate = new DateTime(2026, 1, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDateSpecified = true,
                    RPTypeOfPermit = "ЧУЖДЕНЕЦ С ХУМАНИТАРЕН СТАТУТ"
                },
                NationalityList = new Nationality[]
                    {
                        new Nationality
                        {
                            NationalityCode = "ИНД",
                            NationalityCode2 = "ИН",
                            NationalityName = "Индия",
                            NationalityName2 = "Индия",
                            NationalityNameLatin = "India"
                        }
                    },
                PersonNames = new PersonNames
                {
                    FirstName = "Субита",
                    FirstNameLatin = "Subitha",
                    Surname = "Патал",
                    SurnameLatin = "Patal",
                    FamilyName = "Ганди",
                    LastNameLatin = "Ghandi"
                },
                PermanentAddress = new AddressBG
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Редута",
                    SettlementNameLatin = "Reduta",
                    LocationCode = "1234",
                    LocationName = "Капинака",
                    LocationNameLatin = "Kapinaka",
                    BuildingNumber = "10",
                    Entrance = "А",
                    Apartment = "15",
                    Floor = "5"
                },
                EyesColor = "сини",
                EyesColorLatin = "blue",
                Height = 1.65,
                HeightSpecified = true,
                BirthPlace = new BirthPlace
                {
                    CountryName = "Индия",
                    CountryNameLatin = "India",
                    CountryCode = "91",
                    DistrictName = "Делхи-център",
                    MunicipalityName = "Делхи",
                    TerritorialUnitName = "-"
                }
            },
            // OK, "БЕЖАНЕЦ"
            "7674608015" => new ForeignIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                BirthDate = "1983-05-26",
                LNCh = identifier,
                GenderName = "Жена",
                GenderNameLatin = "Female",
                IdentityDocument = new IdentityDocument
                {
                    DocumentType = "Паспорт",
                    DocumentTypeLatin = "Passport",
                    IdentityDocumentNumber = "36695760",
                    IssuerName = "МВР",
                    IssuerNameLatin = "MVR",
                    IssueDate = new DateTime(2011, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    IssueDateSpecified = true,
                    IssuePlace = "Велико Търново",
                    IssuePlaceLatin = "Veliko Tarnovo",
                    StatusDate = new DateTime(2010, 12, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDate = new DateTime(2026, 1, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDateSpecified = true,
                    RPTypeOfPermit = "БЕЖАНЕЦ"
                },
                NationalityList = new Nationality[]
                    {
                        new Nationality
                        {
                            NationalityCode = "ФРА",
                            NationalityCode2 = "ФР",
                            NationalityName = "Франция",
                            NationalityName2 = "Франция",
                            NationalityNameLatin = "France"
                        }
                    },
                PersonNames = new PersonNames
                {
                    FirstName = "Едит",
                    FirstNameLatin = "Edith",
                    Surname = "Мари",
                    SurnameLatin = "Marie",
                    FamilyName = "Лаботе",
                    LastNameLatin = "Labeaute"
                },
                PermanentAddress = new AddressBG
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Редута",
                    SettlementNameLatin = "Reduta",
                    LocationCode = "1234",
                    LocationName = "Шипка",
                    LocationNameLatin = "Shipka",
                    BuildingNumber = "295",
                    Entrance = "А",
                    Apartment = "10",
                    Floor = "3"
                },
                EyesColor = "сини",
                EyesColorLatin = "blue",
                Height = 1.65,
                HeightSpecified = true,
                BirthPlace = new BirthPlace
                {
                    CountryName = "Франция",
                    CountryNameLatin = "France",
                    CountryCode = "33",
                    DistrictName = "Париж-център",
                    MunicipalityName = "Париж",
                    TerritorialUnitName = "-"
                }
            },
            // OK, "ДЪЛГОСРОЧНО ПРЕБИВАВАЩ В РБ"
            "2002843531" => new ForeignIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                BirthDate = "1989-08-15",
                LNCh = identifier,
                GenderName = "Жена",
                GenderNameLatin = "Female",
                IdentityDocument = new IdentityDocument
                {
                    DocumentType = "Паспорт",
                    DocumentTypeLatin = "Passport",
                    IdentityDocumentNumber = "38495777",
                    IssuerName = "МВР",
                    IssuerNameLatin = "MVR",
                    IssueDate = new DateTime(2011, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    IssueDateSpecified = true,
                    IssuePlace = "Велико Търново",
                    IssuePlaceLatin = "Veliko Tarnovo",
                    StatusDate = new DateTime(2010, 12, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDate = new DateTime(2026, 1, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDateSpecified = true,
                    RPTypeOfPermit = "ДЪЛГОСРОЧНО ПРЕБИВАВАЩ В РБ"
                },
                NationalityList = new Nationality[]
                    {
                        new Nationality
                        {
                            NationalityCode = "ФРА",
                            NationalityCode2 = "ФР",
                            NationalityName = "Франция",
                            NationalityName2 = "Франция",
                            NationalityNameLatin = "France"
                        }
                    },
                PersonNames = new PersonNames
                {
                    FirstName = "Етиен",
                    FirstNameLatin = "Etien",
                    Surname = "Любо",
                    SurnameLatin = "Lebeau",
                    FamilyName = "Люмарше",
                    LastNameLatin = "Lemarche"
                },
                PermanentAddress = new AddressBG
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Редута",
                    SettlementNameLatin = "Reduta",
                    LocationCode = "1234",
                    LocationName = "Шипка",
                    LocationNameLatin = "Shipka",
                    BuildingNumber = "85",
                    Entrance = "А",
                    Apartment = "10",
                    Floor = "3"
                },
                EyesColor = "сини",
                EyesColorLatin = "blue",
                Height = 1.65,
                HeightSpecified = true,
                BirthPlace = new BirthPlace
                {
                    CountryName = "Франция",
                    CountryNameLatin = "France",
                    CountryCode = "33",
                    DistrictName = "Париж-център",
                    MunicipalityName = "Париж",
                    TerritorialUnitName = "-"
                }
            },
            // OK, "КРАТКОСРОЧНО ПРЕБИВАВАЩ В РБ"
            "1262201095" => new ForeignIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                BirthDate = "1998-09-15",
                LNCh = identifier,
                GenderName = "Жена",
                GenderNameLatin = "Female",
                IdentityDocument = new IdentityDocument
                {
                    DocumentType = "Паспорт",
                    DocumentTypeLatin = "Passport",
                    IdentityDocumentNumber = "38498860",
                    IssuerName = "МВР",
                    IssuerNameLatin = "MVR",
                    IssueDate = new DateTime(2011, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    IssueDateSpecified = true,
                    IssuePlace = "Велико Търново",
                    IssuePlaceLatin = "Veliko Tarnovo",
                    StatusDate = new DateTime(2010, 12, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDate = new DateTime(2026, 1, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDateSpecified = true,
                    RPTypeOfPermit = "КРАТКОСРОЧНО ПРЕБИВАВАЩ В РБ"
                },
                NationalityList = new Nationality[]
                    {
                        new Nationality
                        {
                            NationalityCode = "ФРА",
                            NationalityCode2 = "ФР",
                            NationalityName = "Франция",
                            NationalityName2 = "Франция",
                            NationalityNameLatin = "France"
                        }
                    },
                PersonNames = new PersonNames
                {
                    FirstName = "Мари",
                    FirstNameLatin = "Marie",
                    Surname = "Луиз",
                    SurnameLatin = "Louise",
                    FamilyName = "Шармант",
                    LastNameLatin = "Charmante"
                },
                PermanentAddress = new AddressBG
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Редута",
                    SettlementNameLatin = "Reduta",
                    LocationCode = "1234",
                    LocationName = "Шипка",
                    LocationNameLatin = "Shipka",
                    BuildingNumber = "685",
                    Entrance = "Б",
                    Apartment = "10",
                    Floor = "3"
                },
                EyesColor = "сини",
                EyesColorLatin = "blue",
                Height = 1.65,
                HeightSpecified = true,
                BirthPlace = new BirthPlace
                {
                    CountryName = "Франция",
                    CountryNameLatin = "France",
                    CountryCode = "33",
                    DistrictName = "Париж-център",
                    MunicipalityName = "Париж",
                    TerritorialUnitName = "-"
                }
            },
            // OK, "ПРОДЪЛЖИТЕЛНО ПРЕБИВАВАЩ В РБ"
            "7241408774" => new ForeignIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                BirthDate = "2003-10-30",
                LNCh = identifier,
                GenderName = "Жена",
                GenderNameLatin = "Female",
                IdentityDocument = new IdentityDocument
                {
                    DocumentType = "Лична карта",
                    DocumentTypeLatin = "Identity card",
                    IdentityDocumentNumber = "MB0038398",
                    IssuerName = "МВР",
                    IssuerNameLatin = "MVR",
                    IssueDate = new DateTime(2002, 12, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    IssueDateSpecified = true,
                    IssuePlace = "Велико Търново",
                    IssuePlaceLatin = "Veliko Tarnovo",
                    StatusDate = new DateTime(2010, 12, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDate = new DateTime(2027, 1, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDateSpecified = true,
                    RPTypeOfPermit = "ПРОДЪЛЖИТЕЛНО ПРЕБИВАВАЩ В РБ"
                },
                NationalityList = new Nationality[]
                    {
                        new Nationality
                        {
                            NationalityCode = "РУС",
                            NationalityCode2 = "РУ",
                            NationalityName = "Русия",
                            NationalityName2 = "Русия",
                            NationalityNameLatin = "Russia"
                        },
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                    },
                PersonNames = new PersonNames
                {
                    FirstName = "Лина",
                    FirstNameLatin = "Lina",
                    Surname = "Алексиева",
                    SurnameLatin = "Aleksieva",
                    FamilyName = "Пушкина",
                    LastNameLatin = "Pushkina"
                },
                PermanentAddress = new AddressBG
                {
                    DistrictName = "Великотърновска",
                    DistrictNameLatin = "Veliko Tarnovo",
                    MunicipalityName = "Горнооряховска",
                    MunicipalityNameLatin = "Gorna Oriahovitza",
                    SettlementCode = "5100",
                    SettlementName = "Горна Оряховица",
                    SettlementNameLatin = "Gorna Oriahovitza",
                    LocationCode = "Карамфил",
                    LocationName = "улица Карамфил",
                    LocationNameLatin = "Karamfil street",
                    BuildingNumber = "1",
                    Entrance = "B",
                    Apartment = "1",
                    Floor = "5"
                },
                EyesColor = "сини",
                EyesColorLatin = "blue",
                Height = 1.65,
                HeightSpecified = true,
                BirthPlace = new BirthPlace
                {
                    CountryName = "Русия",
                    CountryNameLatin = "Russia",
                    CountryCode = "0093",
                    DistrictName = "Москва",
                    MunicipalityName = "Област Москва",
                    TerritorialUnitName = "Москва център"
                }
            },
            // OK, "ВРЕМЕННА ЗАКРИЛА"
            "9264955996" => new ForeignIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                BirthDate = "1988-12-30",
                LNCh = identifier,
                GenderName = "Жена",
                GenderNameLatin = "Female",
                IdentityDocument = new IdentityDocument
                {
                    DocumentType = "Паспорт",
                    DocumentTypeLatin = "Passport",
                    IdentityDocumentNumber = "384957699",
                    IssuerName = "МВР",
                    IssuerNameLatin = "MVR",
                    IssueDate = new DateTime(2002, 12, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    IssueDateSpecified = true,
                    IssuePlace = "Велико Търново",
                    IssuePlaceLatin = "Veliko Tarnovo",
                    StatusDate = new DateTime(2010, 12, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDate = new DateTime(2026, 1, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDateSpecified = true,
                    RPTypeOfPermit = "ВРЕМЕННА ЗАКРИЛА"
                },
                NationalityList = new Nationality[]
                    {
                        new Nationality
                        {
                            NationalityCode = "РУС",
                            NationalityCode2 = "РУ",
                            NationalityName = "Русия",
                            NationalityName2 = "Русия",
                            NationalityNameLatin = "Russia"
                        },
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                    },
                PersonNames = new PersonNames
                {
                    FirstName = "Наташа",
                    FirstNameLatin = "Natasha",
                    Surname = "Варевна",
                    SurnameLatin = "Varevna",
                    FamilyName = "Малинина",
                    LastNameLatin = "Malinina"
                },
                PermanentAddress = new AddressBG
                {
                    DistrictName = "Великотърновска",
                    DistrictNameLatin = "Veliko Tarnovo",
                    MunicipalityName = "Горнооряховска",
                    MunicipalityNameLatin = "Gorna Oriahovitza",
                    SettlementCode = "5100",
                    SettlementName = "Горна Оряховица",
                    SettlementNameLatin = "Gorna Oriahovitza",
                    LocationCode = "Карамфил",
                    LocationName = "улица Карамфил",
                    LocationNameLatin = "Karamfil street",
                    BuildingNumber = "1",
                    Entrance = "B",
                    Apartment = "1",
                    Floor = "5"
                },
                EyesColor = "сини",
                EyesColorLatin = "blue",
                Height = 1.65,
                HeightSpecified = true,
                BirthPlace = new BirthPlace
                {
                    CountryName = "Русия",
                    CountryNameLatin = "Russia",
                    CountryCode = "0093",
                    DistrictName = "Москва",
                    MunicipalityName = "Област Москва",
                    TerritorialUnitName = "Москва център"
                }
            },
            // OK, "Не указан статус"
            "0631947415" => new ForeignIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                BirthDate = "2003-11-30",
                LNCh = identifier,
                GenderName = "Жена",
                GenderNameLatin = "Female",
                IdentityDocument = new IdentityDocument
                {
                    DocumentType = "Лична карта",
                    DocumentTypeLatin = "Identity card",
                    IdentityDocumentNumber = "MB0038398",
                    IssuerName = "МВР",
                    IssuerNameLatin = "MVR",
                    IssueDate = new DateTime(2002, 12, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    IssueDateSpecified = true,
                    IssuePlace = "Велико Търново",
                    IssuePlaceLatin = "Veliko Tarnovo",
                    StatusDate = new DateTime(2010, 12, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDate = new DateTime(2030, 1, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDateSpecified = true,
                    RPTypeOfPermit = "Не указан статус"
                },
                NationalityList = new Nationality[]
                    {
                        new Nationality
                        {
                            NationalityCode = "АФГ",
                            NationalityCode2 = "АГ",
                            NationalityName = "Афганистан",
                            NationalityName2 = "Афганистан",
                            NationalityNameLatin = "Afghanistan"
                        },
                        new Nationality
                        {
                            NationalityCode = "БЛГ",
                            NationalityCode2 = "БГ",
                            NationalityName = "България",
                            NationalityName2 = "България",
                            NationalityNameLatin = "Bulgaria"
                        }
                    },
                PersonNames = new PersonNames
                {
                    FirstName = "Дабия",
                    FirstNameLatin = "Dahbia",
                    Surname = "Шалам",
                    SurnameLatin = "Shalam",
                    FamilyName = "Шалом",
                    LastNameLatin = "Shalom"
                },
                PermanentAddress = new AddressBG
                {
                    DistrictName = "Великотърновска",
                    DistrictNameLatin = "Veliko Tarnovo",
                    MunicipalityName = "Горнооряховска",
                    MunicipalityNameLatin = "Gorna Oriahovitza",
                    SettlementCode = "5100",
                    SettlementName = "Горна Оряховица",
                    SettlementNameLatin = "Gorna Oriahovitza",
                    LocationCode = "Роза",
                    LocationName = "улица Роза",
                    LocationNameLatin = "Rosa street",
                    BuildingNumber = "20",
                    Entrance = "B",
                    Apartment = "5",
                    Floor = "3"
                },
                EyesColor = "сини",
                EyesColorLatin = "blue",
                Height = 1.65,
                HeightSpecified = true,
                BirthPlace = new BirthPlace
                {
                    CountryName = "Афганистан",
                    CountryNameLatin = "Afghanistan",
                    CountryCode = "0093",
                    DistrictName = "Кабул",
                    MunicipalityName = "Кабул област",
                    TerritorialUnitName = "Кабул център"
                }
            },
            // OK, "ДЪЛГОСРОЧНО ПРЕБИВАВАЩ В РБ"
            "9910256468" => new ForeignIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                BirthDate = "1999-10-25",
                LNCh = "1003262608",
                GenderName = "Мъж",
                GenderNameLatin = "Male",
                IdentityDocument = new IdentityDocument
                {
                    DocumentType = "Разрешение за пребиваване",
                    DocumentTypeLatin = "Residence Permit",
                    IdentityDocumentNumber = "MB0038343",
                    IssuerName = "МВР",
                    IssuerNameLatin = "MVR",
                    IssueDate = new DateTime(2011, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    IssueDateSpecified = true,
                    IssuePlace = "Велико Търново",
                    IssuePlaceLatin = "Veliko Tarnovo",
                    StatusDate = new DateTime(2010, 12, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDate = new DateTime(2026, 1, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDateSpecified = true,
                    RPTypeOfPermit = "ДЪЛГОСРОЧНО ПРЕБИВАВАЩ В РБ"
                },
                NationalityList = new Nationality[]
                    {
                        new Nationality
                        {
                            NationalityCode = "ФРА",
                            NationalityCode2 = "ФР",
                            NationalityName = "Франция",
                            NationalityName2 = "Франция",
                            NationalityNameLatin = "France"
                        }
                    },
                PersonNames = new PersonNames
                {
                    FirstName = "Кашиф",
                    FirstNameLatin = "Kashif",
                    Surname = "Мухамад",
                    SurnameLatin = "Muhammad",
                    FamilyName = "Фаиз",
                    LastNameLatin = "Faaiz"
                },
                PermanentAddress = new AddressBG
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Редута",
                    SettlementNameLatin = "Reduta",
                    LocationCode = "1234",
                    LocationName = "Шипка",
                    LocationNameLatin = "Shipka",
                    BuildingNumber = "85",
                    Entrance = "А",
                    Apartment = "10",
                    Floor = "3"
                },
                EyesColor = "сини",
                EyesColorLatin = "blue",
                Height = 1.66,
                HeightSpecified = true,
                BirthPlace = new BirthPlace
                {
                    CountryName = "Франция",
                    CountryNameLatin = "France",
                    CountryCode = "33",
                    DistrictName = "Париж-център",
                    MunicipalityName = "Париж",
                    TerritorialUnitName = "-"
                }
            },
            // OK, "ДЪЛГОСРОЧНО ПРЕБИВАВАЩ В РБ"
            "9508010133" => new ForeignIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                BirthDate = "1995-08-01",
                LNCh = "9508010133",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                IdentityDocument = new IdentityDocument
                {
                    DocumentType = "Лична карта",
                    DocumentTypeLatin = "Identity card",
                    IdentityDocumentNumber = "LT0000500",
                    IssuerName = "МВР",
                    IssuerNameLatin = "MVR",
                    IssueDate = new DateTime(2003, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    IssueDateSpecified = true,
                    IssuePlace = "София",
                    IssuePlaceLatin = "Sofia",
                    StatusDate = DateTime.Now,
                    ValidDate = new DateTime(2030, 04, 04, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDateSpecified = true,
                    RPTypeOfPermit = "ДЪЛГОСРОЧНО ПРЕБИВАВАЩ В РБ"
                },
                NationalityList = new Nationality[]
                {
                    new Nationality
                    {
                        NationalityCode = "БЛГ",
                        NationalityCode2 = "БГ",
                        NationalityName = "България",
                        NationalityName2 = "България",
                        NationalityNameLatin = "Bulgaria"
                    }
                },
                PersonNames = new PersonNames
                {
                    FirstName = "Лиляна",
                    FirstNameLatin = "Lilyana",
                    Surname = "Томова",
                    SurnameLatin = "Tomova",
                    FamilyName = "Асенова",
                    LastNameLatin = "Asenova"
                },
                PermanentAddress = new AddressBG
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Изток",
                    SettlementNameLatin = "Iztok",
                    LocationCode = "1113",
                    LocationName = "Тинтява",
                    LocationNameLatin = "Tintyava",
                    BuildingNumber = "2",
                    Entrance = "А",
                    Apartment = "7",
                    Floor = "3"
                },
                EyesColor = "сини",
                EyesColorLatin = "blue",
                Height = 1.65,
                HeightSpecified = true,
                BirthPlace = new BirthPlace
                {
                    CountryName = "България",
                    CountryNameLatin = "Bulgaria",
                    CountryCode = "BG",
                    DistrictName = "София",
                    MunicipalityName = "Софийска област",
                    TerritorialUnitName = "-"
                }
            },
            // OK, "ДЪЛГОСРОЧНО ПРЕБИВАВАЩ В РБ", Частично запрещение
            "1006173440" => new ForeignIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                BirthDate = "1999-10-25",
                LNCh = "1006173440",
                GenderName = "Мъж",
                GenderNameLatin = "Male",
                IdentityDocument = new IdentityDocument
                {
                    DocumentType = "Разрешение за пребиваване",
                    DocumentTypeLatin = "Residence Permit",
                    IdentityDocumentNumber = "MB0015841",
                    IssuerName = "МВР",
                    IssuerNameLatin = "MVR",
                    IssueDate = new DateTime(2011, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    IssueDateSpecified = true,
                    IssuePlace = "Велико Търново",
                    IssuePlaceLatin = "Veliko Tarnovo",
                    StatusDate = new DateTime(2010, 12, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDate = new DateTime(2026, 1, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDateSpecified = true,
                    RPTypeOfPermit = "ДЪЛГОСРОЧНО ПРЕБИВАВАЩ В РБ"
                },
                NationalityList = new Nationality[]
                    {
                        new Nationality
                        {
                            NationalityCode = "ФРА",
                            NationalityCode2 = "ФР",
                            NationalityName = "Франция",
                            NationalityName2 = "Франция",
                            NationalityNameLatin = "France"
                        }
                    },
                PersonNames = new PersonNames
                {
                    FirstName = "Никита",
                    FirstNameLatin = "NIKITA",
                    Surname = "",
                    SurnameLatin = "",
                    FamilyName = "Ермолчик",
                    LastNameLatin = "Ermolchik"
                },
                PermanentAddress = new AddressBG
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Редута",
                    SettlementNameLatin = "Reduta",
                    LocationCode = "1234",
                    LocationName = "Шипка",
                    LocationNameLatin = "Shipka",
                    BuildingNumber = "85",
                    Entrance = "А",
                    Apartment = "10",
                    Floor = "3"
                },
                EyesColor = "сини",
                EyesColorLatin = "blue",
                Height = 1.66,
                HeightSpecified = true,
                BirthPlace = new BirthPlace
                {
                    CountryName = "Франция",
                    CountryNameLatin = "France",
                    CountryCode = "33",
                    DistrictName = "Париж-център",
                    MunicipalityName = "Париж",
                    TerritorialUnitName = "-"
                }
            },
            // OK, "ДЪЛГОСРОЧНО ПРЕБИВАВАЩ В РБ", Пълно запрещение
            "4207055674" => new ForeignIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                BirthDate = "1942-07-05",
                LNCh = "35448",
                GenderName = "Жена",
                GenderNameLatin = "Female",
                IdentityDocument = new IdentityDocument
                {
                    DocumentType = "Разрешение за пребиваване",
                    DocumentTypeLatin = "Residence Permit",
                    IdentityDocumentNumber = "MB0032103",
                    IssuerName = "МВР",
                    IssuerNameLatin = "MVR",
                    IssueDate = new DateTime(2011, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    IssueDateSpecified = true,
                    IssuePlace = "Велико Търново",
                    IssuePlaceLatin = "Veliko Tarnovo",
                    StatusDate = new DateTime(2010, 12, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDate = new DateTime(2026, 1, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDateSpecified = true,
                    RPTypeOfPermit = "ДЪЛГОСРОЧНО ПРЕБИВАВАЩ В РБ"
                },
                NationalityList = new Nationality[]
                    {
                        new Nationality
                        {
                            NationalityCode = "ФРА",
                            NationalityCode2 = "ФР",
                            NationalityName = "Франция",
                            NationalityName2 = "Франция",
                            NationalityNameLatin = "France"
                        }
                    },
                PersonNames = new PersonNames
                {
                    FirstName = " Раиса",
                    FirstNameLatin = "Raisa",
                    Surname = "",
                    SurnameLatin = "",
                    FamilyName = "Димитрова",
                    LastNameLatin = "Dimitrova"
                },
                PermanentAddress = new AddressBG
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Редута",
                    SettlementNameLatin = "Reduta",
                    LocationCode = "1234",
                    LocationName = "Шипка",
                    LocationNameLatin = "Shipka",
                    BuildingNumber = "85",
                    Entrance = "А",
                    Apartment = "10",
                    Floor = "3"
                },
                EyesColor = "сини",
                EyesColorLatin = "blue",
                Height = 1.60,
                HeightSpecified = true,
                BirthPlace = new BirthPlace
                {
                    CountryName = "Франция",
                    CountryNameLatin = "France",
                    CountryCode = "33",
                    DistrictName = "Париж-център",
                    MunicipalityName = "Париж",
                    TerritorialUnitName = "-"
                }
            },
            // OK, "ДЪЛГОСРОЧНО ПРЕБИВАВАЩ В РБ", Непълнолетен
            "0752130540" => new ForeignIdentityInfoResponseType
            {
                ReturnInformations = new ReturnInformation
                {
                    ReturnCode = FoundReturnCode,
                },
                BirthDate = "2007-12-13",
                LNCh = "1003157603",
                GenderName = "Мъж",
                GenderNameLatin = "Male",
                IdentityDocument = new IdentityDocument
                {
                    DocumentType = "Разрешение за пребиваване",
                    DocumentTypeLatin = "Residence Permit",
                    IdentityDocumentNumber = "MB0032093",
                    IssuerName = "МВР",
                    IssuerNameLatin = "MVR",
                    IssueDate = new DateTime(2011, 11, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    IssueDateSpecified = true,
                    IssuePlace = "Велико Търново",
                    IssuePlaceLatin = "Veliko Tarnovo",
                    StatusDate = new DateTime(2010, 12, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDate = new DateTime(2026, 1, 1, 6, 0, 0, 0, DateTimeKind.Local),
                    ValidDateSpecified = true,
                    RPTypeOfPermit = "ДЪЛГОСРОЧНО ПРЕБИВАВАЩ В РБ"
                },
                NationalityList = new Nationality[]
                    {
                        new Nationality
                        {
                            NationalityCode = "ФРА",
                            NationalityCode2 = "ФР",
                            NationalityName = "Франция",
                            NationalityName2 = "Франция",
                            NationalityNameLatin = "France"
                        }
                    },
                PersonNames = new PersonNames
                {
                    FirstName = "Александр",
                    FirstNameLatin = "Aleksandr",
                    Surname = "",
                    SurnameLatin = "",
                    FamilyName = "Анохин",
                    LastNameLatin = "Anokhin"
                },
                PermanentAddress = new AddressBG
                {
                    DistrictName = "Софийска област",
                    DistrictNameLatin = "Sofia district",
                    MunicipalityName = "София",
                    MunicipalityNameLatin = "Sofia",
                    SettlementCode = "1000",
                    SettlementName = "Редута",
                    SettlementNameLatin = "Reduta",
                    LocationCode = "1234",
                    LocationName = "Шипка",
                    LocationNameLatin = "Shipka",
                    BuildingNumber = "85",
                    Entrance = "А",
                    Apartment = "10",
                    Floor = "3"
                },
                EyesColor = "сини",
                EyesColorLatin = "blue",
                Height = 1.55,
                HeightSpecified = true,
                BirthPlace = new BirthPlace
                {
                    CountryName = "Франция",
                    CountryNameLatin = "France",
                    CountryCode = "33",
                    DistrictName = "Париж-център",
                    MunicipalityName = "Париж",
                    TerritorialUnitName = "-"
                }
            },
            _ => null
        };

        // Not found
        result ??= new ForeignIdentityInfoResponseType
        {
            ReturnInformations = new ReturnInformation
            {
                ReturnCode = NotFoundReturnCode,
                Info = $"Person with {identifierType} {identifier} is not found"
            }
        };

        return new RegixSearchResultDTO
        {
            HasFailed = false,
            Response = new Dictionary<string, object?> { { "ForeignIdentityInfoResponse", result } }
        };
    }

    private static string GetJParmaterStringValue(JArray jArray, string parameterName)
    {
        var token = jArray
            .FirstOrDefault(p => p is JObject parameterObject && parameterObject.ContainsKey(parameterName))?
            .SelectToken(parameterName) as JObject;

        return token?["parameterStringValue"]?.Value<string>() ?? string.Empty;
    }

    private static RegixSearchResultDTO? GetTRActualStateV3Response(RegiXSearchCommand message)
    {
        // Failed
        if (JObject.Parse(message.Command)?["argument"]?["parameters"] is not JArray parameters)
        {
            return PrepareTestDataFailed();
        }

        var searchUIC = GetJParmaterStringValue(parameters, "UIC");

        var result = searchUIC switch
        {
            "111111113" => default,
            "222222226" => PrepareTestDataFailed(),
            "333333339" => PrepareTestDataNotFound(),
            "147119101" => PrepareTestData1("147119101"),
            "207014830" => PrepareTestData2("207014830"),
            "104043152" => PrepareTestData3("104043152"),
            "762640804" => PrepareTestData4("762640804"),
            "288465035" => PrepareTestData5("288465035"),
            "812823088" => PrepareTestData6("812823088"),
            "774660519" => PrepareTestData7("774660519"),
            "167558732" => PrepareTestData8("167558732"),
            "459602623" => PrepareTestData9("459602623"),
            "6053805090797" => PrepareTestData10("6053805090797"),
            "361094227" => PrepareTestData11("361094227"),
            "694201230" => PrepareTestData12("694201230"),
            "826814836" => PrepareTestData13("826814836"),
            "938122202" => PrepareTestData14("938122202"),
            "769840394" => PrepareTestData15("769840394"),
            "177208082" => PrepareTestData16("177208082"),
            _ => PrepareTestDataNotFound()
        };

        return result;
    }

    private static RegixSearchResultDTO PrepareTestDataFailed()
    {
        return new RegixSearchResultDTO
        {
            HasFailed = true,
            Response = new Dictionary<string, object?> { { "ActualStateResponseV3", new ActualStateResponseV3() } }
        };
    }

    private static RegixSearchResultDTO PrepareTestDataNotFound()
    {
        return new RegixSearchResultDTO
        {
            HasFailed = false,
            Response = new Dictionary<string, object?> {
                {
                    "ActualStateResponseV3",
                    new ActualStateResponseV3 {
                        DataFound = "false"
                    }
                }
            }
        };
    }

    private static RegixSearchResultDTO PrepareTestData1(string uIC)
    {
        var actualState = new ActualStateResponseV3
        {
            DataFound = "true",
            DataValidForDate = DateTime.Today.AddYears(-1),
            Deed = new Deed
            {
                UIC = uIC,
                CompanyName = "СТИЛО ЕООД",
                LegalForm = "ЕООД",
                CaseNo = 423,
                CaseYear = 2007,
                Subdeeds = new Subdeeds
                {
                    Subdeed = new Subdeed[]
                    {
                        new Subdeed
                        {
                            Records = new Records
                            {
                                Record = new RecordItem[] {
                                    new RecordItem<ManagerData>
                                    {
                                        RecordId = "123123123",
                                        IncomingId = "222333444",
                                        FieldActionDate = DateTime.Today.AddYears(-1),
                                        FieldIdent = _representativeField1Id,
                                        MainField = PrepareMainField(_representativeField1Id),
                                        RecordData = PrepareManagerPerson("Админ Админов", "8802184852")
                                    },
                                    new RecordItem<ManagerData>
                                    {
                                        RecordId = "123123124",
                                        IncomingId = "222333445",
                                        FieldActionDate = DateTime.Today.AddYears(-1),
                                        FieldIdent = _representativeField2Id,
                                        MainField = PrepareMainField(_representativeField2Id),
                                        RecordData = PrepareManagerPerson("Иван Георгиев", "2804115607")
                                    },
                                    new RecordItem<ManagerData>
                                    {
                                        RecordId = "123123125",
                                        IncomingId = "222333446",
                                        FieldActionDate = DateTime.Today.AddYears(-1),
                                        FieldIdent = _representativeField1Id,
                                        MainField = PrepareMainField(_representativeField3Id),
                                        RecordData = PrepareManagerPerson("Йордан Георгиев", "2105251412")
                                    },
                                    new RecordItem<WayOfRepresentationData>
                                    {
                                        RecordId = "123412341",
                                        IncomingId = "223345",
                                        FieldActionDate = DateTime.Today.AddYears(-1),
                                        FieldIdent = _wayOfRepresentationFieldId,
                                        MainField = PrepareMainField(_wayOfRepresentationFieldId),
                                        RecordData = PrepareWayOfRepresentationData("САМО ЗАЕДНО")
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
        return new RegixSearchResultDTO
        {
            HasFailed = false,
            Response = new Dictionary<string, object?> { { "ActualStateResponseV3", actualState } }
        };
    }

    private static RegixSearchResultDTO PrepareTestData2(string uIC)
    {
        var actualState = new ActualStateResponseV3
        {
            DataFound = "true",
            DataValidForDate = DateTime.Today.AddYears(-1),
            Deed = new Deed
            {
                UIC = uIC,
                CompanyName = "НОЛИМИТС 10",
                LegalForm = "ООД",
                CaseNo = 573,
                CaseYear = 2009,
                Subdeeds = new Subdeeds
                {
                    Subdeed = new Subdeed[]
                    {
                        new Subdeed
                        {
                            Records = new Records
                            {
                                Record = new RecordItem[] {
                                new RecordItem<ManagerData>
                                    {
                                        RecordId = "1231234545",
                                        IncomingId = "222789444",
                                        FieldActionDate = DateTime.Today.AddYears(-1),
                                        FieldIdent = _representativeField1Id,
                                        MainField = PrepareMainField(_representativeField1Id),
                                        RecordData = PrepareManagerPerson("Админ Админов", "8802184852")
                                    },
                                    new RecordItem<ManagerData>
                                    {
                                        IncomingId = "222789555",
                                        FieldActionDate = DateTime.Today.AddYears(-1),
                                        FieldIdent = _representativeField2Id,
                                        MainField = PrepareMainField(_representativeField2Id),
                                        RecordData = PrepareManagerPerson("Петкан Смъртников", "7112234758")
                                    },
                                    new RecordItem<WayOfRepresentationData>
                                    {
                                        RecordId = "1231234549",
                                        IncomingId = "222789777",
                                        FieldActionDate = DateTime.Today.AddYears(-1),
                                        FieldIdent = _wayOfRepresentationFieldId,
                                        MainField = PrepareMainField(_wayOfRepresentationFieldId),
                                        RecordData = PrepareWayOfRepresentationData("ЗАЕДНО И ПООТДЕЛНО")
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
        return new RegixSearchResultDTO
        {
            HasFailed = false,
            Response = new Dictionary<string, object?> { { "ActualStateResponseV3", actualState } }
        };
    }

    private static RegixSearchResultDTO PrepareTestData3(string uIC)
    {
        var actualState = new ActualStateResponseV3
        {
            DataFound = "true",
            DataValidForDate = DateTime.Today.AddYears(-1),
            Deed = new Deed
            {
                UIC = uIC,
                CompanyName = "ВАЛЕРИ СТОЯНОВ",
                LegalForm = "ЕТ",
                CaseNo = 793,
                CaseYear = 2010,
                Subdeeds = new Subdeeds
                {
                    Subdeed = new Subdeed[]
                    {
                        new Subdeed
                        {
                            Records = new Records
                            {
                                Record = new RecordItem[] {
                                new RecordItem<ManagerData>
                                    {
                                        IncomingId = "422789544",
                                        FieldActionDate = DateTime.Today.AddYears(-1),
                                        FieldIdent = _representativeField1Id,
                                        MainField = PrepareMainField(_representativeField1Id),
                                        RecordData = PrepareManagerPerson("Админ Админов", "8802184852")
                                    },
                                    new RecordItem<WayOfRepresentationData>
                                    {
                                        RecordId = "7231234549",
                                        IncomingId = "333789777",
                                        FieldActionDate = DateTime.Today.AddYears(-1),
                                        FieldIdent = _wayOfRepresentationFieldId,
                                        MainField = PrepareMainField(_wayOfRepresentationFieldId),
                                        RecordData = PrepareWayOfRepresentationData("ЗАЕДНО И ПООТДЕЛНО")
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
        return new RegixSearchResultDTO
        {
            HasFailed = false,
            Response = new Dictionary<string, object?> { { "ActualStateResponseV3", actualState } }
        };
    }

    private static RegixSearchResultDTO PrepareTestData4(string uIC)
    {
        var actualState = new ActualStateResponseV3
        {
            DataFound = "true",
            DataValidForDate = DateTime.Today.AddYears(-1),
            Deed = new Deed
            {
                UIC = uIC,
                CompanyName = "СТИЛ ЕТ",
                LegalForm = "ЕТ",
                CaseNo = 8123,
                CaseYear = 2020,
                Subdeeds = new Subdeeds
                {
                    Subdeed = new Subdeed[]
                    {
                        new Subdeed
                        {
                            Records = new Records
                            {
                                Record = new RecordItem[] {
                                new RecordItem<ManagerData>
                                    {
                                        IncomingId = "532789564",
                                        FieldActionDate = DateTime.Today.AddYears(-1),
                                        FieldIdent = _representativeField1Id,
                                        MainField = PrepareMainField(_representativeField1Id),
                                        RecordData = PrepareManagerPerson("Йордан Георгиев", "2105251412")
                                    },
                                new RecordItem<ManagerData>
                                    {
                                        RecordId = "123123120",
                                        IncomingId = "222333440",
                                        FieldActionDate = DateTime.Today.AddYears(-1),
                                        FieldIdent = _representativeField1Id,
                                        MainField = PrepareMainField(_representativeField1Id),
                                        RecordData = PrepareManagerPerson("Юлия Руменова Бозукова", "7111106259")
                                    },
                                    new RecordItem<WayOfRepresentationData>
                                    {
                                        RecordId = "6241234549",
                                        IncomingId = "453789877",
                                        FieldActionDate = DateTime.Today.AddYears(-1),
                                        FieldIdent = _wayOfRepresentationFieldId,
                                        MainField = PrepareMainField(_wayOfRepresentationFieldId),
                                        RecordData = PrepareWayOfRepresentationData("")
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
        return new RegixSearchResultDTO
        {
            HasFailed = false,
            Response = new Dictionary<string, object?> { { "ActualStateResponseV3", actualState } }
        };
    }

    private static RegixSearchResultDTO PrepareTestData5(string uIC)
    {
        var actualState = new ActualStateResponseV3
        {
            DataFound = "true",
            DataValidForDate = DateTime.Today.AddYears(-1),
            Deed = new Deed
            {
                UIC = uIC,
                CompanyName = "КОРАЛ ЕТ",
                LegalForm = "ЕТ",
                CaseNo = 653,
                CaseYear = 2022,
                Subdeeds = new Subdeeds
                {
                    Subdeed = new Subdeed[]
                    {
                        new Subdeed
                        {
                            Records = new Records
                            {
                                Record = new RecordItem[] {
                                new RecordItem<ManagerData>
                                    {
                                        IncomingId = "172199564",
                                        FieldActionDate = DateTime.Today.AddYears(-1),
                                        FieldIdent = _representativeField1Id,
                                        MainField = PrepareMainField(_representativeField1Id),
                                        RecordData = PrepareManagerPerson("Кирил Иванов", "9612121028")
                                    },
                                    new RecordItem<WayOfRepresentationData>
                                    {
                                        RecordId = "6551624549",
                                        IncomingId = "173789877",
                                        FieldActionDate = DateTime.Today.AddYears(-1),
                                        FieldIdent = _wayOfRepresentationFieldId,
                                        MainField = PrepareMainField(_wayOfRepresentationFieldId),
                                        RecordData = PrepareWayOfRepresentationData("ЗАЕДНО И ПООТДЕЛНО")
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
        return new RegixSearchResultDTO
        {
            HasFailed = false,
            Response = new Dictionary<string, object?> { { "ActualStateResponseV3", actualState } }
        };
    }

    private static RegixSearchResultDTO PrepareTestData6(string uIC)
    {
        var actualState = new ActualStateResponseV3
        {
            DataFound = "true",
            DataValidForDate = DateTime.Today.AddYears(-1),
            Deed = new Deed
            {
                UIC = uIC,
                CompanyName = "ПОЛЕТ ЕТ",
                LegalForm = "ЕТ",
                CaseNo = 1453,
                CaseYear = 2019,
                Subdeeds = new Subdeeds
                {
                    Subdeed = new Subdeed[]
                    {
                        new Subdeed
                        {
                            Records = new Records
                            {
                                Record = new RecordItem[] {
                                new RecordItem<ManagerData>
                                    {
                                        IncomingId = "272419564",
                                        FieldActionDate = DateTime.Today.AddYears(-1),
                                        FieldIdent = _representativeField1Id,
                                        MainField = PrepareMainField(_representativeField1Id),
                                        RecordData = PrepareManagerPerson("Петрана Василева", "6002016774")
                                    },
                                    new RecordItem<WayOfRepresentationData>
                                    {
                                        RecordId = "6853624129",
                                        IncomingId = "243772579",
                                        FieldActionDate = DateTime.Today.AddYears(-1),
                                        FieldIdent = _wayOfRepresentationFieldId,
                                        MainField = PrepareMainField(_wayOfRepresentationFieldId),
                                        RecordData = PrepareWayOfRepresentationData("ЗАЕДНО И ПООТДЕЛНО")
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
        return new RegixSearchResultDTO
        {
            HasFailed = false,
            Response = new Dictionary<string, object?> { { "ActualStateResponseV3", actualState } }
        };
    }

    private static RegixSearchResultDTO PrepareTestData7(string uIC)
    {
        var actualState = new ActualStateResponseV3
        {
            DataFound = "true",
            DataValidForDate = DateTime.Today.AddYears(-1),
            Deed = new Deed
            {
                UIC = uIC,
                CompanyName = "ПЕКИР ЕООД",
                LegalForm = "ЕООД",
                CaseNo = 2323,
                CaseYear = 2009,
                Subdeeds = new Subdeeds
                {
                    Subdeed = new Subdeed[]
                    {
                        new Subdeed
                        {
                            Records = new Records
                            {
                                Record = new RecordItem[] {
                                    new RecordItem<ManagerData>
                                    {
                                        RecordId = "145124523",
                                        IncomingId = "245334544",
                                        FieldActionDate = DateTime.Today.AddYears(-1),
                                        FieldIdent = _representativeField1Id,
                                        MainField = PrepareMainField(_representativeField1Id),
                                        RecordData = PrepareManagerPerson("Кирил Иванов", "9612121028")
                                    },
                                    new RecordItem<ManagerData>
                                    {
                                        RecordId = "663123124",
                                        IncomingId = "2452333445",
                                        FieldActionDate = DateTime.Today.AddYears(-1),
                                        FieldIdent = _representativeField2Id,
                                        MainField = PrepareMainField(_representativeField2Id),
                                        RecordData = PrepareManagerPerson("Петрана Василева", "6002016774")
                                    },
                                    new RecordItem<WayOfRepresentationData>
                                    {
                                        RecordId = "722412341",
                                        IncomingId = "327345",
                                        FieldActionDate = DateTime.Today.AddYears(-1),
                                        FieldIdent = _wayOfRepresentationFieldId,
                                        MainField = PrepareMainField(_wayOfRepresentationFieldId),
                                        RecordData = PrepareWayOfRepresentationData("САМО ЗАЕДНО")
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
        return new RegixSearchResultDTO
        {
            HasFailed = false,
            Response = new Dictionary<string, object?> { { "ActualStateResponseV3", actualState } }
        };
    }

    private static RegixSearchResultDTO PrepareTestData8(string uIC)
    {
        var actualState = new ActualStateResponseV3
        {
            DataFound = "true",
            DataValidForDate = DateTime.Today.AddYears(-1),
            Deed = new Deed
            {
                UIC = uIC,
                CompanyName = "ДАКИ ЕООД",
                LegalForm = "ЕООД",
                CaseNo = 2363,
                CaseYear = 2011,
                Subdeeds = new Subdeeds
                {
                    Subdeed = new Subdeed[]
                    {
                        new Subdeed
                        {
                            Records = new Records
                            {
                                Record = new RecordItem[] {
                                    new RecordItem<ManagerData>
                                    {
                                        RecordId = "345127773",
                                        IncomingId = "641334774",
                                        FieldActionDate = DateTime.Today.AddYears(-1),
                                        FieldIdent = _representativeField1Id,
                                        MainField = PrepareMainField(_representativeField1Id),
                                        RecordData = PrepareManagerPerson("Йордан Георгиев", "2105251412")
                                    },
                                    new RecordItem<ManagerData>
                                    {
                                        RecordId = "655123124",
                                        IncomingId = "2666333445",
                                        FieldActionDate = DateTime.Today.AddYears(-1),
                                        FieldIdent = _representativeField2Id,
                                        MainField = PrepareMainField(_representativeField2Id),
                                        RecordData = PrepareManagerPerson("Кирил Иванов", "9612121028")
                                    },
                                    new RecordItem<WayOfRepresentationData>
                                    {
                                        RecordId = "926432341",
                                        IncomingId = "547395",
                                        FieldActionDate = DateTime.Today.AddYears(-1),
                                        FieldIdent = _wayOfRepresentationFieldId,
                                        MainField = PrepareMainField(_wayOfRepresentationFieldId),
                                        RecordData = PrepareWayOfRepresentationData("ЗАЕДНО И ПООТДЕЛНО")
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
        return new RegixSearchResultDTO
        {
            HasFailed = false,
            Response = new Dictionary<string, object?> { { "ActualStateResponseV3", actualState } }
        };
    }

    private static RegixSearchResultDTO PrepareTestData9(string uIC)
    {
        var actualState = new ActualStateResponseV3
        {
            DataFound = "true",
            DataValidForDate = DateTime.Today.AddYears(-1),
            Deed = new Deed
            {
                UIC = uIC,
                CompanyName = "МИЛЕ ЕТ",
                LegalForm = "ЕТ",
                CaseNo = 7153,
                CaseYear = 2005,
                Subdeeds = new Subdeeds
                {
                    Subdeed = new Subdeed[]
                    {
                        new Subdeed
                        {
                            Records = new Records
                            {
                                Record = new RecordItem[] {
                                new RecordItem<ManagerData>
                                    {
                                        IncomingId = "544766564",
                                        FieldActionDate = DateTime.Today.AddYears(-1),
                                        FieldIdent = _representativeField1Id,
                                        MainField = PrepareMainField(_representativeField1Id),
                                        RecordData = PrepareManagerPerson("Милен Коларов", "8512223960")
                                    },
                                    new RecordItem<WayOfRepresentationData>
                                    {
                                        RecordId = "5271234549",
                                        IncomingId = "734789877",
                                        FieldActionDate = DateTime.Today.AddYears(-1),
                                        FieldIdent = _wayOfRepresentationFieldId,
                                        MainField = PrepareMainField(_wayOfRepresentationFieldId),
                                        RecordData = PrepareWayOfRepresentationData("ЗАЕДНО И ПООТДЕЛНО")
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
        return new RegixSearchResultDTO
        {
            HasFailed = false,
            Response = new Dictionary<string, object?> { { "ActualStateResponseV3", actualState } }
        };
    }

    private static RegixSearchResultDTO PrepareTestData10(string uIC)
    {

        var actualState = new ActualStateResponseV3
        {
            DataFound = "true",
            DataValidForDate = DateTime.Today.AddYears(-1),
            Deed = new Deed
            {
                UIC = uIC,
                CompanyName = "Тестери ЕООД",
                LegalForm = "ЕООД",
                CaseNo = 420,
                CaseYear = 2023,
                Subdeeds = new Subdeeds
                {
                    Subdeed = new Subdeed[]
                    {
                            new Subdeed
                            {
                                Records = new Records
                                {
                                    Record = new RecordItem[] {
                                        new RecordItem<ManagerData>
                                        {
                                            RecordId = "123123121",
                                            IncomingId = "222333441",
                                            FieldActionDate = DateTime.Today.AddYears(-1),
                                            FieldIdent = _representativeField1Id,
                                            MainField = PrepareMainField(_representativeField1Id),
                                            RecordData = PrepareManagerPerson("Юлия Руменова Бозукова", "7111106259")
                                        },
                                        new RecordItem<ManagerData>
                                        {
                                            RecordId = "123123126",
                                            IncomingId = "222333443",
                                            FieldActionDate = DateTime.Today.AddYears(-1),
                                            FieldIdent = _representativeField1Id,
                                            MainField = PrepareMainField(_representativeField3Id),
                                            RecordData = PrepareManagerPerson("Калоян Младенов Маринов", "8802214026")
                                        },
                                        new RecordItem<WayOfRepresentationData>
                                        {
                                            RecordId = "1234123414",
                                            IncomingId = "2233455",
                                            FieldActionDate = DateTime.Today.AddYears(-1),
                                            FieldIdent = _wayOfRepresentationFieldId,
                                            MainField = PrepareMainField(_wayOfRepresentationFieldId),
                                            RecordData = PrepareWayOfRepresentationData("САМО ЗАЕДНО")
                                        }
                                    }
                                }
                            }
                    }
                }
            }
        };
        return new RegixSearchResultDTO
        {
            HasFailed = false,
            Response = new Dictionary<string, object?> { { "ActualStateResponseV3", actualState } }
        };
    }

    private static RegixSearchResultDTO PrepareTestData11(string uIC)
    {
        var actualState = new ActualStateResponseV3
        {
            DataFound = "true",
            DataValidForDate = DateTime.Today.AddYears(-1),
            Deed = new Deed
            {
                UIC = uIC,
                CompanyName = "Аналист Консулт ЕТ",
                LegalForm = "ЕТ",
                CaseNo = 5713,
                CaseYear = 2023,
                Subdeeds = new Subdeeds
                {
                    Subdeed = new Subdeed[]
                    {
                            new Subdeed
                            {
                                Records = new Records
                                {
                                    Record = new RecordItem[] {
                                        new RecordItem<ManagerData>
                                        {
                                            RecordId = "123123120",
                                            IncomingId = "222333440",
                                            FieldActionDate = DateTime.Today.AddYears(-1),
                                            FieldIdent = _representativeField1Id,
                                            MainField = PrepareMainField(_representativeField1Id),
                                            RecordData = PrepareManagerPerson("Юлия Руменова Бозукова", "7111106259")
                                        },
                                        new RecordItem<ManagerData>
                                        {
                                            RecordId = "123123000",
                                            IncomingId = "222333000",
                                            FieldActionDate = DateTime.Today.AddYears(-1),
                                            FieldIdent = _representativeField1Id,
                                            MainField = PrepareMainField(_representativeField3Id),
                                            RecordData = PrepareManagerPerson("Валентин Николаев Николов", "8304029040")
                                        },
                                        new RecordItem<ManagerData>
                                        {
                                            RecordId = "123123123",
                                            IncomingId = "222333123",
                                            FieldActionDate = DateTime.Today.AddYears(-1),
                                            FieldIdent = _representativeField1Id,
                                            MainField = PrepareMainField(_representativeField3Id),
                                            RecordData = PrepareManagerPerson("Калоян Младенов Маринов", "8802214026")
                                        },
                                        new RecordItem<WayOfRepresentationData>
                                        {
                                            RecordId = "1231230000",
                                            IncomingId = "222780000",
                                            FieldActionDate = DateTime.Today.AddYears(-1),
                                            FieldIdent = _wayOfRepresentationFieldId,
                                            MainField = PrepareMainField(_wayOfRepresentationFieldId),
                                            RecordData = PrepareWayOfRepresentationData("ЗАЕДНО И ПООТДЕЛНО")
                                        }
                                    }
                                }
                            }
                    }
                }
            }
        };
        return new RegixSearchResultDTO
        {
            HasFailed = false,
            Response = new Dictionary<string, object?> { { "ActualStateResponseV3", actualState } }
        };
    }

    private static RegixSearchResultDTO PrepareTestData12(string uIC)
    {

        var actualState = new ActualStateResponseV3
        {
            DataFound = "true",
            DataValidForDate = DateTime.Today.AddYears(-1),
            Deed = new Deed
            {
                UIC = uIC,
                CompanyName = "Алекс и Юлето ООД",
                LegalForm = "ООД",
                CaseNo = 421,
                CaseYear = 2022,
                Subdeeds = new Subdeeds
                {
                    Subdeed = new Subdeed[]
                    {
                            new Subdeed
                            {
                                Records = new Records
                                {
                                    Record = new RecordItem[] {
                                        new RecordItem<ManagerData>
                                        {
                                            RecordId = "123123121",
                                            IncomingId = "222333441",
                                            FieldActionDate = DateTime.Today.AddYears(-1),
                                            FieldIdent = _representativeField1Id,
                                            MainField = PrepareMainField(_representativeField1Id),
                                            RecordData = PrepareManagerPerson("Юлия Руменова Бозукова", "7111106259")
                                        },
                                        new RecordItem<ManagerData>
                                        {
                                            RecordId = "12212312",
                                            IncomingId = "22333344",
                                            FieldActionDate = DateTime.Today.AddYears(-1),
                                            FieldIdent = _representativeField2Id,
                                            MainField = PrepareMainField(_representativeField2Id),
                                            RecordData = PrepareManagerPerson("Александър Бойков Стоилов", "9009206827")
                                        },
                                        new RecordItem<WayOfRepresentationData>
                                        {
                                            RecordId = "1224123414",
                                            IncomingId = "2223455",
                                            FieldActionDate = DateTime.Today.AddYears(-1),
                                            FieldIdent = _wayOfRepresentationFieldId,
                                            MainField = PrepareMainField(_wayOfRepresentationFieldId),
                                            RecordData = PrepareWayOfRepresentationData("САМО ЗАЕДНО")
                                        }
                                    }
                                }
                            }
                    }
                }
            }
        };
        return new RegixSearchResultDTO
        {
            HasFailed = false,
            Response = new Dictionary<string, object?> { { "ActualStateResponseV3", actualState } }
        };
    }

    private static RegixSearchResultDTO PrepareTestData13(string uIC)
    {

        var actualState = new ActualStateResponseV3
        {
            DataFound = "true",
            DataValidForDate = DateTime.Today.AddYears(-1),
            Deed = new Deed
            {
                UIC = uIC,
                CompanyName = "Чуждоземци в БГ ООД",
                LegalForm = "ООД",
                CaseNo = 421,
                CaseYear = 2022,
                Subdeeds = new Subdeeds
                {
                    Subdeed = new Subdeed[]
                    {
                            new Subdeed
                            {
                                Records = new Records
                                {
                                    Record = new RecordItem[] {
                                        new RecordItem<ManagerData>
                                        {
                                            RecordId = "123123123",
                                            IncomingId = "222333443",
                                            FieldActionDate = DateTime.Today.AddYears(-1),
                                            FieldIdent = _representativeField1Id,
                                            MainField = PrepareMainField(_representativeField1Id),
                                            RecordData = PrepareManagerPerson("Мари Шармант", "1262201095")
                                        },
                                        new RecordItem<ManagerData>
                                        {
                                            RecordId = "122123124",
                                            IncomingId = "223333444",
                                            FieldActionDate = DateTime.Today.AddYears(-1),
                                            FieldIdent = _representativeField2Id,
                                            MainField = PrepareMainField(_representativeField2Id),
                                            RecordData = PrepareManagerPerson("Етиен Лемарше", "2002843531")
                                        },
                                        new RecordItem<ManagerData>
                                        {
                                            RecordId = "122123125",
                                            IncomingId = "223333445",
                                            FieldActionDate = DateTime.Today.AddYears(-1),
                                            FieldIdent = _representativeField2Id,
                                            MainField = PrepareMainField(_representativeField2Id),
                                            RecordData = PrepareManagerPerson("Едит Лаботе", "7674608015")
                                        },
                                        new RecordItem<ManagerData>
                                        {
                                            RecordId = "122123126",
                                            IncomingId = "223333446",
                                            FieldActionDate = DateTime.Today.AddYears(-1),
                                            FieldIdent = _representativeField2Id,
                                            MainField = PrepareMainField(_representativeField2Id),
                                            RecordData = PrepareManagerPerson("Субита Ганди", "7704143022")
                                        },
                                        new RecordItem<ManagerData>
                                        {
                                            RecordId = "122123127",
                                            IncomingId = "223333447",
                                            FieldActionDate = DateTime.Today.AddYears(-1),
                                            FieldIdent = _representativeField2Id,
                                            MainField = PrepareMainField(_representativeField2Id),
                                            RecordData = PrepareManagerPerson("Индира Мадури", "9066356559")
                                        },
                                        new RecordItem<ManagerData>
                                        {
                                            RecordId = "122123128",
                                            IncomingId = "223333448",
                                            FieldActionDate = DateTime.Today.AddYears(-1),
                                            FieldIdent = _representativeField2Id,
                                            MainField = PrepareMainField(_representativeField2Id),
                                            RecordData = PrepareManagerPerson("Наташа Малинина", "9264955996")
                                        },
                                        new RecordItem<ManagerData>
                                        {
                                            RecordId = "122123129",
                                            IncomingId = "223333449",
                                            FieldActionDate = DateTime.Today.AddYears(-1),
                                            FieldIdent = _representativeField2Id,
                                            MainField = PrepareMainField(_representativeField2Id),
                                            RecordData = PrepareManagerPerson("Натали Сенпиер", "9310166608")
                                        },
                                        new RecordItem<WayOfRepresentationData>
                                        {
                                            RecordId = "1224123415",
                                            IncomingId = "2223456",
                                            FieldActionDate = DateTime.Today.AddYears(-1),
                                            FieldIdent = _wayOfRepresentationFieldId,
                                            MainField = PrepareMainField(_wayOfRepresentationFieldId),
                                            RecordData = PrepareWayOfRepresentationData("ЗАЕДНО И ПООТДЕЛНО")
                                        }
                                    }
                                }
                            }
                    }
                }
            }
        };
        return new RegixSearchResultDTO
        {
            HasFailed = false,
            Response = new Dictionary<string, object?> { { "ActualStateResponseV3", actualState } }
        };
    }

    private static RegixSearchResultDTO PrepareTestData14(string uIC)
    {
        var actualState = new ActualStateResponseV3
        {
            DataFound = "true",
            DataValidForDate = DateTime.Today.AddYears(-1),
            Deed = new Deed
            {
                UIC = uIC,
                CompanyName = "Друг начин ЕТ",
                LegalForm = "ЕТ",
                CaseNo = 5713,
                CaseYear = 2023,
                Subdeeds = new Subdeeds
                {
                    Subdeed = new Subdeed[]
                    {
                            new Subdeed
                            {
                                Records = new Records
                                {
                                    Record = new RecordItem[] {
                                        new RecordItem<ManagerData>
                                        {
                                            RecordId = "123123120",
                                            IncomingId = "222333440",
                                            FieldActionDate = DateTime.Today.AddYears(-1),
                                            FieldIdent = _representativeField1Id,
                                            MainField = PrepareMainField(_representativeField1Id),
                                            RecordData = PrepareManagerPerson("Юлия Руменова Бозукова", "7111106259")
                                        },
                                        new RecordItem<ManagerData>
                                        {
                                            RecordId = "123123120",
                                            IncomingId = "222333440",
                                            FieldActionDate = DateTime.Today.AddYears(-1),
                                            FieldIdent = _representativeField1Id,
                                            MainField = PrepareMainField(_representativeField1Id),
                                            RecordData = PrepareManagerPerson("Валентин Николаев Николов", "8304029040")
                                        },
                                        new RecordItem<WayOfRepresentationData>
                                        {
                                            RecordId = "1231230000",
                                            IncomingId = "222780000",
                                            FieldActionDate = DateTime.Today.AddYears(-1),
                                            FieldIdent = _wayOfRepresentationFieldId,
                                            MainField = PrepareMainField(_wayOfRepresentationFieldId),
                                            RecordData = PrepareWayOfRepresentationData("ДРУГО")
                                        }
                                    }
                                }
                            }
                    }
                }
            }
        };
        return new RegixSearchResultDTO
        {
            HasFailed = false,
            Response = new Dictionary<string, object?> { { "ActualStateResponseV3", actualState } }
        };
    }

    private static RegixSearchResultDTO PrepareTestData15(string uIC)
    {
        var actualState = new ActualStateResponseV3
        {
            DataFound = "true",
            DataValidForDate = DateTime.Today.AddYears(-1),
            Deed = new Deed
            {
                UIC = uIC,
                CompanyName = "Прекратена ЕТ",
                LegalForm = "ЕТ",
                DeedStatus = "C",
                CaseNo = 5713,
                CaseYear = 2023,
                Subdeeds = new Subdeeds
                {
                    Subdeed = new Subdeed[]
                    {
                            new Subdeed
                            {
                                Records = new Records
                                {
                                    Record = new RecordItem[] {
                                        new RecordItem<ManagerData>
                                        {
                                            RecordId = "123123120",
                                            IncomingId = "222333440",
                                            FieldActionDate = DateTime.Today.AddYears(-1),
                                            FieldIdent = _representativeField1Id,
                                            MainField = PrepareMainField(_representativeField1Id),
                                            RecordData = PrepareManagerPerson("Юлия Руменова Бозукова", "7111106259")
                                        },
                                        new RecordItem<WayOfRepresentationData>
                                        {
                                            RecordId = "1231230000",
                                            IncomingId = "222780000",
                                            FieldActionDate = DateTime.Today.AddYears(-1),
                                            FieldIdent = _wayOfRepresentationFieldId,
                                            MainField = PrepareMainField(_wayOfRepresentationFieldId),
                                            RecordData = PrepareWayOfRepresentationData("ПООТДЕЛНО")
                                        }
                                    }
                                }
                            }
                    }
                }
            }
        };
        return new RegixSearchResultDTO
        {
            HasFailed = false,
            Response = new Dictionary<string, object?> { { "ActualStateResponseV3", actualState } }
        };
    }

    private static RegixSearchResultDTO PrepareTestData16(string uIC)
    {

        var actualState = new ActualStateResponseV3
        {
            DataFound = "true",
            DataValidForDate = DateTime.Today.AddYears(-1),
            Deed = new Deed
            {
                UIC = uIC,
                CompanyName = "ЮВАЛ ЕООД",
                LegalForm = "ЕООД",
                CaseNo = 420,
                CaseYear = 2023,
                Subdeeds = new Subdeeds
                {
                    Subdeed = new Subdeed[]
                    {
                            new Subdeed
                            {
                                Records = new Records
                                {
                                    Record = new RecordItem[] {
                                        new RecordItem<ManagerData>
                                        {
                                            RecordId = "123101121",
                                            IncomingId = "222388441",
                                            FieldActionDate = DateTime.Today.AddYears(-1),
                                            FieldIdent = _representativeField1Id,
                                            MainField = PrepareMainField(_representativeField1Id),
                                            RecordData = PrepareManagerPerson("Юлия Руменова Бозукова", "7111106259")
                                        },
                                        new RecordItem<ManagerData>
                                        {
                                            RecordId = "123101126",
                                            IncomingId = "222388443",
                                            FieldActionDate = DateTime.Today.AddYears(-1),
                                            FieldIdent = _representativeField1Id,
                                            MainField = PrepareMainField(_representativeField3Id),
                                            RecordData = PrepareManagerPerson("Валентин Николаев Николов", "8304029040")
                                        },
                                        new RecordItem<WayOfRepresentationData>
                                        {
                                            RecordId = "1234188414",
                                            IncomingId = "2288455",
                                            FieldActionDate = DateTime.Today.AddYears(-1),
                                            FieldIdent = _wayOfRepresentationFieldId,
                                            MainField = PrepareMainField(_wayOfRepresentationFieldId),
                                            RecordData = PrepareWayOfRepresentationData("САМО ЗАЕДНО")
                                        }
                                    }
                                }
                            }
                    }
                }
            }
        };
        return new RegixSearchResultDTO
        {
            HasFailed = false,
            Response = new Dictionary<string, object?> { { "ActualStateResponseV3", actualState } }
        };
    }


    private static MainField PrepareMainField(string fieldId)
    {
        return new MainField
        {
            MainFieldIdent = fieldId,
            GroupId = "415",
            GroupName = "Основни обстоятелства",
            SectionId = "1",
            SectionName = "Общ статус"
        };
    }

    private static ManagerData PrepareManagerPerson(string name, string egn)
    {
        return new ManagerData
        {
            Manager = new Manager
            {
                Person = new Person
                {
                    Name = name,
                    Indent = egn,
                    IndentType = "EGN",
                    CountryID = "1",
                    CountryName = "БЪЛГАРИЯ"
                }
            }
        };
    }

    private static WayOfRepresentationData PrepareWayOfRepresentationData(string value)
    {
        return new WayOfRepresentationData { WayOfRepresentation = value };
    }
    //Default subjPropState = 571 (OK! - развиващ дейност)
    //Default entryType = "756"(ОК! - първоначално вписване)
    private static StateOfPlayResponseType GetBulstatInfoForQAsCompanyByUIC
        (string uic, string eventType, string subjPropState = "571", string entryType = "756")
    {
        return new StateOfPlayResponseType
        {
            Subject = new Subject()
            {
                UIC = new UIC
                {
                    UIC1 = uic,
                    UICType = new NomenclatureEntry
                    {
                        Code = "1"
                    }
                },
                SubjectType = new NomenclatureEntry
                {
                    Code = "897"    //Физическо лице
                },
                LegalEntitySubject = new LegalEntity
                {
                    Country = new NomenclatureEntry
                    {
                        Code = "100"
                    },
                    LegalForm = new NomenclatureEntry
                    {
                        Code = "486"    //Сдружение
                    },
                    LegalStatute = new NomenclatureEntry
                    {
                        Code = "436"    //неюридическо лице
                    },
                    SubjectGroup = new NomenclatureEntry
                    {
                        Code = "437"    //търговец по ТЗ
                    },
                    CyrillicFullName = "Латерна 28",
                    CyrillicShortName = "Лат 28",
                    LatinFullName = "Laterna 28",
                    SubordinateLevel = new NomenclatureEntry
                    {
                        Code = "616"    //първо ниво на подчиненост
                    },
                    TRStatus = "OK?"
                }
            },
            Managers = new SubjectRelManager[]
                    {
                        new SubjectRelManager()
                        {
                            Position = new NomenclatureEntry
                            {
                                Code = "640"    //Длъжност в управлението - директор
                            },
                            RelatedSubject = new Subject
                            {
                                NaturalPersonSubject = new NaturalPerson
                                {
                                    Country = new NomenclatureEntry
                                    {
                                        Code = "100" //БГ
                                    },
                                    EGN = "9108116358",
                                    CyrillicName = "Лилиана Крумова",
                                    LatinName = "Liliana Krumova"
                                }

                            },

                            RepresentedSubjects = new Subject[]{}
                        },
                        new SubjectRelManager()
                        {
                            Position = new NomenclatureEntry
                            {
                                Code = "639"    //Длъжност в управлението - главен директор
                            },
                            RelatedSubject = new Subject
                            {
                                NaturalPersonSubject = new NaturalPerson
                                {
                                    Country = new NomenclatureEntry
                                    {
                                        Code = "100" //БГ
                                    },
                                    EGN = "7111106259",
                                    CyrillicName = "Юлия Бозукова",
                                    LatinName = "Julia Bozukova"
                                }

                            }
                        },
                        new SubjectRelManager()
                        {
                            Position = new NomenclatureEntry
                            {
                                Code = "639"    //Длъжност в управлението - главен директор
                            },
                            RelatedSubject = new Subject
                            {
                                NaturalPersonSubject = new NaturalPerson
                                {
                                    Country = new NomenclatureEntry
                                    {
                                        Code = "100" //БГ
                                    },
                                    EGN = "8802214026",
                                    CyrillicName = "Калоян Младенов Маринов",
                                    LatinName = "Kaloyan Mladenov Marinov"
                                }

                            }
                        }
                    },
            State = new SubjectPropState
            {
                State = new NomenclatureEntry
                {
                    Code = subjPropState
                },
            },
            Event = new Event
            {
                EventType = new NomenclatureEntry
                {
                    Code = eventType
                },
                EntryType = new NomenclatureEntry
                {
                    Code = entryType
                }

            },
            ReturnInformations = new ReturnInformation
            {
                ReturnCode = FoundReturnCode,
            }
        };
    }

    private static StateOfPlayResponseType GetBulstatInfoForTestBulstatCompany()
    {
        return new StateOfPlayResponseType
        {
            Subject = new Subject()
            {
                EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                UIC = new UIC
                {
                    EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                    UIC1 = "1212120908",
                    UICType = new NomenclatureEntry
                    {
                        Code = "1"
                    }
                },
                Communications = new Communication[]
                {
                    new Communication
                    {
                        EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                        Type = new NomenclatureEntry
                        {
                            Code = "721"
                        },
                        Value = "02/8090776"
                    },
                    new Communication
                    {
                        EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                        Type = new NomenclatureEntry
                        {
                            Code = "723"
                        },
                        Value = "info@amperel.net"
                    },
                    new Communication
                    {
                        EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                        Type = new NomenclatureEntry
                        {
                            Code = "724"
                        },
                        Value = "www.amperel.net"
                    }
                },
                Addresses = new Address[]
                {
                    new Address
                    {
                        EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                        AddressType = new NomenclatureEntry
                        {
                            Code = "718"
                        },
                        Country = new NomenclatureEntry
                        {
                            Code = "100" //БГ
                        },
                        PostalCode = "8800",
                        Location = new NomenclatureEntry
                        {
                            Code = "67338"
                        },
                        Street = "ж.к. Дружба",
                        Building = "29",
                        Entrance = "В",
                        Apartment = "25"
                    },
                    new Address
                    {
                        EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                        AddressType = new NomenclatureEntry
                        {
                            Code = "719"
                        },
                        Country = new NomenclatureEntry
                        {
                            Code = "100" //БГ
                        },
                        PostalCode = "8800",
                        Location = new NomenclatureEntry
                        {
                            Code = "67338"
                        },
                        Street = "ул. ДРАГАН ЦАНКОВ",
                        Building = "71",
                        Entrance = "Б",
                        Floor = "2",
                        Apartment = "14"
                    }
                },
                Remark = "T25:Remark",
                SubjectType = new NomenclatureEntry
                {
                    Code = "897"    //Физическо лице
                },
                LegalEntitySubject = new LegalEntity
                {
                    EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                    Country = new NomenclatureEntry
                    {
                        Code = "100"
                    },
                    LegalForm = new NomenclatureEntry
                    {
                        Code = "486"    //Сдружение
                    },
                    LegalStatute = new NomenclatureEntry
                    {
                        Code = "436"    //неюридическо лице
                    },
                    SubjectGroup = new NomenclatureEntry
                    {
                        Code = "437"    //търговец по ТЗ
                    },
                    CyrillicFullName = "АМПЕРЕЛ ООД",
                    CyrillicShortName = "АМПЕРЕЛ",
                    LatinFullName = "Amperel Ltd",
                    SubordinateLevel = new NomenclatureEntry
                    {
                        Code = "616"    //първо ниво на подчиненост
                    }
                }
            },
            MainActivity2008 = new SubjectPropActivityKID2008
            {
                EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                KID2008 = new NomenclatureEntry
                {
                    Code = "8412"
                }
            },
            MainActivity2003 = new SubjectPropActivityKID2003
            {
                EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                NKID2003 = new NomenclatureEntry
                {
                    Code = "818"
                }
            },
            Installments = new SubjectPropInstallments[]
            {
                new SubjectPropInstallments
                {
                    EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                    Count = 1,
                    Amount = 100
                }
            },
            LifeTime = new SubjectPropLifeTime
            {
                EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                Date = "2001-01-01",
                Description = "Description"
            },
            AccountingRecordForm = new SubjectPropAccountingRecordForm
            {
                EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                Form = new NomenclatureEntry
                {
                    Code = "964"    //Длъжност в управлението - директор
                },
            },
            OwnershipForms = new SubjectPropOwnershipForm[]
            {
                new SubjectPropOwnershipForm
                {
                    EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                    Form = new NomenclatureEntry
                    {
                        Code = "576"
                    },
                    Percent = 100,
                }
            },
            FundingSources = new SubjectPropFundingSource[]
            {
                new SubjectPropFundingSource
                {
                    EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                    Source = new NomenclatureEntry
                    {
                        Code = "589"
                    },
                    Percent = 100,
                }
            },
            State = new SubjectPropState
            {
                EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                State = new NomenclatureEntry
                {
                    Code = "571"
                }
            },
            Managers = new SubjectRelManager[]
            {
                new SubjectRelManager()
                {
                    EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                    RelatedSubject = new Subject
                    {
                        EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                        UIC = new UIC
                        {
                            EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                            UIC1 = "1110150562011",
                            UICType = new NomenclatureEntry
                            {
                                Code = "1"
                            }
                        },
                        SubjectType = new NomenclatureEntry
                        {
                            Code = "897"    //Физическо лице
                        },
                        NaturalPersonSubject = new NaturalPerson
                        {
                            EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                            Country = new NomenclatureEntry
                            {
                                Code = "100" //БГ
                            },
                            EGN = "6908080808",
                            CyrillicName = "МАРИЯ ГЕОРГИЕВА ГЕОРГИЕВА",
                            LatinName = "MARIYA GEORGIEVA GEORGIEVA",
                            BirthDate = "1969-08-08T00:00:00"
                        }
                    },
                    Position = new NomenclatureEntry
                    {
                        Code = "640"
                    },
                    RepresentedSubjects = new Subject[]{}
                }
            },
            Partners = new SubjectRelPartner[]
            {
                new SubjectRelPartner()
                {
                    EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                    OperationType = SubscriptionElementOperationType.erase,
                    RelatedSubject = new Subject
                    {
                        EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                        UIC = new UIC
                        {
                            EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                            UIC1 = "1110150562011",
                            UICType = new NomenclatureEntry
                            {
                                Code = "1"
                            }
                        },
                        SubjectType = new NomenclatureEntry
                        {
                            Code = "897"    //Физическо лице
                        },
                        NaturalPersonSubject = new NaturalPerson
                        {
                            EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                            Country = new NomenclatureEntry
                            {
                                Code = "100" //БГ
                            },
                            EGN = "7008080808",
                            CyrillicName = "ГЕРГАНА ГЕОРГИЕВА ГЕОРГИЕВА",
                            LatinName = "GERGANA GEORGIEVA GEORVIEVA",
                            BirthDate = "1970-08-08T00:00:00"
                        }
                    },
                    Percent = 1.123m,
                    Amount = 1.12m,
                }
            },
            Assignee = new SubjectRelAssignee()
            {
                EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                OperationType = SubscriptionElementOperationType.register,
                RelatedSubjects = new Subject[]
                {
                    new Subject
                    {
                        EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                        UIC = new UIC
                        {
                            EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                            UIC1 = "1110150562011",
                            UICType = new NomenclatureEntry
                            {
                                Code = "1"
                            }
                        },
                        SubjectType = new NomenclatureEntry
                        {
                            Code = "896"
                        },
                        LegalEntitySubject = new LegalEntity
                        {
                            EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                            Country = new NomenclatureEntry
                            {
                                Code = "100" //БГ
                            },
                            LegalForm = new NomenclatureEntry
                            {
                                Code = "530"
                            },
                            LegalStatute = new NomenclatureEntry
                            {
                                Code = "436"
                            },
                            SubjectGroup = new NomenclatureEntry
                            {
                                Code = "452"
                            },
                            CyrillicFullName = "ДИРЕКЦИЯ \"СОЦИАЛНО ПОДПОМАГАНЕ\" - СОФИЯ",
                            CyrillicShortName = "ДИРЕКЦИЯ \" СОЦИАЛНО ПОДПОМАГАНЕ\" - СОФИЯ",
                            LatinFullName = "DIREKTSIYA \" SOTSIALNO PODPOMAGANE\" - SOFIA",
                            SubordinateLevel = new NomenclatureEntry
                            {
                                Code = "617"
                            },
                        },
                        Communications = new Communication[]
                        {
                            new Communication
                            {
                                EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                                Type = new NomenclatureEntry
                                {
                                    Code = "721"
                                },
                                Value = "(02)624192"
                            },
                            new Communication
                            {
                                EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                                Type = new NomenclatureEntry
                                {
                                    Code = "722"
                                },
                                Value = "(02)660349"
                            }
                        },
                        Addresses = new Address[]
                        {
                            new Address
                            {
                                EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                                AddressType = new NomenclatureEntry
                                {
                                    Code = "718"
                                },
                                Country = new NomenclatureEntry
                                {
                                    Code = "100" //БГ
                                },
                                PostalCode = "6300",
                                Location = new NomenclatureEntry
                                {
                                    Code = "77195"
                                },
                                Street = "ул.ПАТРИАРХ ЕВТИМИЙ",
                                StreetNumber = "2"
                            }
                        },
                        Remark = "Тест"
                    }
                }
            },
            Belonging = new SubjectRelBelonging()
            {
                EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                RelatedSubject = new Subject()
                {
                    EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                    UIC = new UIC
                    {
                        EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                        UIC1 = "121015056",
                        UICType = new NomenclatureEntry
                        {
                            Code = "1"
                        }
                    },
                    SubjectType = new NomenclatureEntry
                    {
                        Code = "896"
                    },
                    LegalEntitySubject = new LegalEntity
                    {
                        EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                        Country = new NomenclatureEntry
                        {
                            Code = "100" //БГ
                        },
                        LegalForm = new NomenclatureEntry
                        {
                            Code = "533"
                        },
                        LegalStatute = new NomenclatureEntry
                        {
                            Code = "435"
                        },
                        SubjectGroup = new NomenclatureEntry
                        {
                            Code = "453"
                        },
                        CyrillicFullName = "АГЕНЦИЯ ЗА СОЦИАЛНО ПОДПОМАГАНЕ",
                        CyrillicShortName = "АГЕНЦИЯ ЗА СОЦИАЛНО ПОДПОМАГАНЕ",
                        LatinFullName = "AGENTSIYA ZA SOTSIALNO PODPOMAGANE",
                        SubordinateLevel = new NomenclatureEntry
                        {
                            Code = "616"
                        }
                    }
                },
                Type = new NomenclatureEntry
                {
                    Code = "710"
                }
            },
            CollectiveBodies = new SubjectPropCollectiveBody[]
            {
                new SubjectPropCollectiveBody()
                {
                    EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                    Members = new SubjectRelCollectiveBodyMember[]
                    {
                        new SubjectRelCollectiveBodyMember()
                        {
                            EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                            OperationType = SubscriptionElementOperationType.register,
                            RelatedSubject = new Subject()
                            {
                                EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                                UIC = new UIC
                                {
                                    EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                                    UIC1 = "9311111110",
                                    UICType = new NomenclatureEntry
                                    {
                                        Code = "2"
                                    }
                                },
                                SubjectType = new NomenclatureEntry
                                {
                                    Code = "897"
                                },
                                NaturalPersonSubject = new NaturalPerson
                                {
                                    EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                                    Country = new NomenclatureEntry
                                    {
                                        Code = "100" //БГ
                                    },
                                    EGN = "9311111111",
                                    CyrillicName = "ДИАНА НИКОЛОВА НИКОЛОВА",
                                    LatinName = "DIANA NIKOLOVA NIKOLOVA",
                                    BirthDate = "2013-01-01T00:00:00",
                                    IdentificationDoc = new IdentificationDoc()
                                    {
                                        EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                                        IDType = new NomenclatureEntry
                                        {
                                            Code = "788"
                                        },
                                        IDNumber = "643333333",
                                        Country = new NomenclatureEntry
                                        {
                                            Code = "100"
                                        },
                                        IssueDate = "2012-02-07T00:00:00"
                                    }
                                },
                                CountrySubject = new NomenclatureEntry
                                {
                                    Code = "100"
                                },
                                Communications = new Communication[]
                                {
                                    new Communication
                                    {
                                        EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                                        Type = new NomenclatureEntry
                                        {
                                            Code = "721"
                                        },
                                        Value = "089123456"
                                    },
                                    new Communication
                                    {
                                        EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                                        Type = new NomenclatureEntry
                                        {
                                            Code = "723"
                                        },
                                        Value = "best_test@abv.bg"
                                    }
                                },
                                Addresses = new Address[]
                                {
                                    new Address
                                    {
                                        EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                                        AddressType = new NomenclatureEntry
                                        {
                                            Code = "2"
                                        },
                                        Country = new NomenclatureEntry
                                        {
                                            Code = "100" //БГ
                                        },
                                        PostalCode = "617",
                                        Location = new NomenclatureEntry
                                        {
                                            Code = "22767"
                                        },
                                        Street = "ул. ВЕРЕЯ",
                                        StreetNumber = "5"
                                    },
                                    new Address
                                    {
                                        EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                                        AddressType = new NomenclatureEntry
                                        {
                                            Code = "720"
                                        },
                                        Country = new NomenclatureEntry
                                        {
                                            Code = "100" //БГ
                                        },
                                        PostalCode = "617",
                                        Location = new NomenclatureEntry
                                        {
                                            Code = "22767"
                                        }
                                    }
                                },
                            Remark = "регистр. карта изд. на 01.01.2011 г."
                            },
                            Position = new NomenclatureEntry
                            {
                                Code = "640"
                            },
                            RepresentedSubjects = new Subject[]
                            {
                                new Subject()
                                {
                                    EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                                    UIC = new UIC
                                    {
                                        EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                                        UIC1 = "1210150561111",
                                        UICType = new NomenclatureEntry
                                        {
                                            Code = "1"
                                        }
                                    },
                                    SubjectType = new NomenclatureEntry
                                    {
                                        Code = "896"
                                    },
                                    LegalEntitySubject = new LegalEntity()
                                    {
                                        Country = new NomenclatureEntry
                                        {
                                            Code = "100" //БГ
                                        },
                                        LegalForm = new NomenclatureEntry
                                        {
                                            Code = "530"
                                        },
                                        LegalStatute = new NomenclatureEntry
                                        {
                                            Code = "436"
                                        },
                                        SubjectGroup = new NomenclatureEntry
                                        {
                                            Code = "452"
                                        },
                                        CyrillicFullName = "ДИРЕКЦИЯ \"СОЦИАЛНО ПОДПОМАГАНЕ\" - ПЛОВДИВ",
                                        CyrillicShortName = "ДИРЕКЦИЯ \"СОЦИАЛНО ПОДПОМАГАНЕ\" - ПЛОВДИВ",
                                        LatinFullName = "DIREKTSIYA \" SOTSIALNO PODPOMAGANE\" - PLOVDIV",
                                        SubordinateLevel = new NomenclatureEntry
                                        {
                                            Code = "617"
                                        }
                                    },
                                    Communications = new Communication[]
                                    {
                                        new Communication
                                        {
                                            EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                                            Type = new NomenclatureEntry
                                            {
                                                Code = "721"
                                            },
                                            Value = "0887123456"
                                        },
                                        new Communication
                                        {
                                            EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                                            Type = new NomenclatureEntry
                                            {
                                                Code = "723"
                                            },
                                            Value = "good.test@abv.bg"
                                        }
                                    },
                                    Addresses = new Address[]
                                    {
                                        new Address
                                        {
                                            EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                                            AddressType = new NomenclatureEntry
                                            {
                                                Code = "718"
                                            },
                                            Country = new NomenclatureEntry
                                            {
                                                Code = "100" //БГ
                                            },
                                            PostalCode = "8800",
                                            Location = new NomenclatureEntry
                                            {
                                                Code = "67338"
                                            },
                                            Street = "ул. ДРАГАН ЦАНКОВ",
                                            Building = "71",
                                            Entrance = "Б",
                                            Floor = "1",
                                            Apartment = "114"
                                        },
                                        new Address
                                        {
                                            EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                                            AddressType = new NomenclatureEntry
                                            {
                                                Code = "720"
                                            },
                                            Country = new NomenclatureEntry
                                            {
                                                Code = "100" //БГ
                                            },
                                            PostalCode = "8800",
                                            Location = new NomenclatureEntry
                                            {
                                                Code = "67338"
                                            },
                                            Street = "ул. ВЕЛИКОКНЯЖЕВСКА",
                                            StreetNumber = "16",
                                            Floor = "4",
                                            Apartment = "14"
                                        }
                                    },
                                Remark = "Тест тест тест"
                                }
                            }
                        }
                    }
                }
            },
            ActivityDate = new SubjectPropActivityDate()
            {
                EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                Type = new NomenclatureEntry
                {
                    Code = "1"
                },
                Date = "2016-08-16T00:00:00"
            },
            AdditionalActivities2008 = new SubjectPropActivityKID2008[]
            {
                new SubjectPropActivityKID2008()
                {
                    EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                    KID2008 = new NomenclatureEntry
                    {
                        Code = "3"
                    }
                }
            },
            Professions = new SubjectPropProfession[]
            {
                new SubjectPropProfession()
                {
                    EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                    Profession = new NomenclatureEntry
                    {
                        Code = "22"
                    }
                },
                new SubjectPropProfession()
                {
                    EntryTime = new DateTime(2001, 12, 17, 09, 30, 47, 0, DateTimeKind.Local),
                    Profession = new NomenclatureEntry
                    {
                        Code = "2"
                    }
                }
            },
            ReturnInformations = new ReturnInformation
            {
                ReturnCode = FoundReturnCode,
            }
        };
    }

    private static StateOfPlayResponseType GetBulstatInfoQAsInPartnersByUIC
        (string uic, string eventType, string subjPropState = "571", string entryType = "756", string cyrillicFullName = "Латерна 28", string cyrillicShortName = "Лат 28", string latinFullName = "Laterna 28")
    {
        return new StateOfPlayResponseType
        {
            Subject = new Subject()
            {
                UIC = new UIC
                {
                    UIC1 = uic,
                    UICType = new NomenclatureEntry
                    {
                        Code = "1"
                    }
                },
                SubjectType = new NomenclatureEntry
                {
                    Code = "897"    //Физическо лице
                },
                LegalEntitySubject = new LegalEntity
                {
                    Country = new NomenclatureEntry
                    {
                        Code = "100"
                    },
                    LegalForm = new NomenclatureEntry
                    {
                        Code = "486"    //Сдружение
                    },
                    LegalStatute = new NomenclatureEntry
                    {
                        Code = "436"    //неюридическо лице
                    },
                    SubjectGroup = new NomenclatureEntry
                    {
                        Code = "437"    //търговец по ТЗ
                    },
                    CyrillicFullName = cyrillicFullName,
                    CyrillicShortName = cyrillicShortName,
                    LatinFullName = latinFullName,
                    SubordinateLevel = new NomenclatureEntry
                    {
                        Code = "616"    //първо ниво на подчиненост
                    },
                    TRStatus = "OK?"
                }
            },
            Managers = new SubjectRelManager[]
                    {
                        new SubjectRelManager()
                        {
                            Position = new NomenclatureEntry
                            {
                                Code = "640"    //Длъжност в управлението - директор
                            },
                            RelatedSubject = new Subject
                            {
                                NaturalPersonSubject = new NaturalPerson
                                {
                                    Country = new NomenclatureEntry
                                    {
                                        Code = "100" //БГ
                                    },
                                    EGN = "6408081154",
                                    CyrillicName = "Марин Стоев",
                                    LatinName = "Marin Stoev"
                                }

                            },

                            RepresentedSubjects = new Subject[]{}
                        }
                    },
            Partners = new SubjectRelPartner[]
            {
                new SubjectRelPartner()
                {
                    RelatedSubject = new Subject()
                    {
                        NaturalPersonSubject = new NaturalPerson
                        {
                            Country = new NomenclatureEntry
                            {
                                Code = "100" //БГ
                            },
                            EGN = "7111106259",
                            CyrillicName = "Юлия Руменова Бозукова",
                            LatinName = "Julia Rumenova Bozukova"
                        }
                    }
                },
                new SubjectRelPartner()
                {
                    RelatedSubject = new Subject()
                    {
                        NaturalPersonSubject = new NaturalPerson
                        {
                            Country = new NomenclatureEntry
                            {
                                Code = "100" //БГ
                            },
                            EGN = "8802214026",
                            CyrillicName = "Калоян Младенов Маринов",
                            LatinName = "Kaloyan Mladenov Marinov"
                        }
                    }
                },
                new SubjectRelPartner()
                {
                    RelatedSubject = new Subject()
                    {
                        NaturalPersonSubject = new NaturalPerson
                        {
                            Country = new NomenclatureEntry
                            {
                                Code = "100" //БГ
                            },
                            EGN = "9108116358",
                            CyrillicName = "Лилиана Крумова",
                            LatinName = "Liliana Krumova"
                        }
                    }
                },
            new SubjectRelPartner()
            {
                RelatedSubject = new Subject()
                {
                    NaturalPersonSubject = new NaturalPerson
                    {
                        Country = new NomenclatureEntry
                        {
                            Code = "100" //БГ
                        },
                        EGN = "900521100",
                        CyrillicName = "Илиян Петров",
                        LatinName = "Ilian Petrov"
                    }
                }
            }
        },
            State = new SubjectPropState
            {
                State = new NomenclatureEntry
                {
                    Code = subjPropState
                },
            },
            Event = new Event
            {
                EventType = new NomenclatureEntry
                {
                    Code = eventType
                },
                EntryType = new NomenclatureEntry
                {
                    Code = entryType
                }

            },
            ReturnInformations = new ReturnInformation
            {
                ReturnCode = FoundReturnCode,
            }
        };
    }

    private static StateOfPlayResponseType GetBulstatInfoQAsInCollectiveBodyByUIC
        (string uic, string eventType, string subjPropState = "571", string entryType = "756")
    {
        return new StateOfPlayResponseType
        {
            Subject = new Subject()
            {
                UIC = new UIC
                {
                    UIC1 = uic,
                    UICType = new NomenclatureEntry
                    {
                        Code = "1"
                    }
                },
                SubjectType = new NomenclatureEntry
                {
                    Code = "897"    //Физическо лице
                },
                LegalEntitySubject = new LegalEntity
                {
                    Country = new NomenclatureEntry
                    {
                        Code = "100"
                    },
                    LegalForm = new NomenclatureEntry
                    {
                        Code = "486"    //Сдружение
                    },
                    LegalStatute = new NomenclatureEntry
                    {
                        Code = "436"    //неюридическо лице
                    },
                    SubjectGroup = new NomenclatureEntry
                    {
                        Code = "437"    //търговец по ТЗ
                    },
                    CyrillicFullName = "Латерна 28",
                    CyrillicShortName = "Лат 28",
                    LatinFullName = "Laterna 28",
                    SubordinateLevel = new NomenclatureEntry
                    {
                        Code = "616"    //първо ниво на подчиненост
                    },
                    TRStatus = "OK?"
                }
            },
            Managers = new SubjectRelManager[]
                    {
                        new SubjectRelManager()
                        {
                            Position = new NomenclatureEntry
                            {
                                Code = "640"    //Длъжност в управлението - директор
                            },
                            RelatedSubject = new Subject
                            {
                                NaturalPersonSubject = new NaturalPerson
                                {
                                    Country = new NomenclatureEntry
                                    {
                                        Code = "100" //БГ
                                    },
                                    EGN = "6408081154",
                                    CyrillicName = "Марин Стоев",
                                    LatinName = "Marin Stoev"
                                }

                            },

                            RepresentedSubjects = new Subject[]{}
                        }
                    },
            CollectiveBodies = new SubjectPropCollectiveBody[]
            {
                new SubjectPropCollectiveBody()
                {
                    Members = new SubjectRelCollectiveBodyMember[]
                    {
                        new SubjectRelCollectiveBodyMember
                        {
                            RelatedSubject = new Subject
                            {
                                NaturalPersonSubject = new NaturalPerson
                                {
                                    Country = new NomenclatureEntry
                                    {
                                        Code = "100" //БГ
                                    },
                                    EGN = "7111106259",
                                    CyrillicName = "Юлия Бозукова",
                                    LatinName = "Julia Bozukova"
                                }
                            }
                        },
                        new SubjectRelCollectiveBodyMember
                        {
                            RelatedSubject = new Subject
                            {
                                NaturalPersonSubject = new NaturalPerson
                                {
                                    Country = new NomenclatureEntry
                                    {
                                        Code = "100" //БГ
                                    },
                                    EGN = "9108116358",
                                    CyrillicName = "Лилиана Крумова",
                                    LatinName = "Liliana Krumova"
                                }
                            }
                        },
                        new SubjectRelCollectiveBodyMember
                        {
                            RelatedSubject = new Subject
                            {
                                NaturalPersonSubject = new NaturalPerson
                                {
                                    Country = new NomenclatureEntry
                                    {
                                        Code = "100" //БГ
                                    },
                                    EGN = "8802214026",
                                    CyrillicName = "Калоян Младенов Маринов",
                                    LatinName = "Kaloyan Mladenov Marinov"
                                }
                            }
                        },
                    }
                },
            },
            State = new SubjectPropState
            {
                State = new NomenclatureEntry
                {
                    Code = subjPropState
                },
            },
            Event = new Event
            {
                EventType = new NomenclatureEntry
                {
                    Code = eventType
                },
                EntryType = new NomenclatureEntry
                {
                    Code = entryType
                }

            },
            ReturnInformations = new ReturnInformation
            {
                ReturnCode = FoundReturnCode,
            }
        };
    }

    private static StateOfPlayResponseType GetBulstatInfoQAsNotInCompanyByUIC
        (string uic, string eventType, string subjPropState = "571", string entryType = "756")
    {
        return new StateOfPlayResponseType
        {
            Subject = new Subject()
            {
                UIC = new UIC
                {
                    UIC1 = uic,
                    UICType = new NomenclatureEntry
                    {
                        Code = "1"
                    }
                },
                SubjectType = new NomenclatureEntry
                {
                    Code = "897"    //Физическо лице
                },
                LegalEntitySubject = new LegalEntity
                {
                    Country = new NomenclatureEntry
                    {
                        Code = "100"
                    },
                    LegalForm = new NomenclatureEntry
                    {
                        Code = "486"    //Сдружение
                    },
                    LegalStatute = new NomenclatureEntry
                    {
                        Code = "436"    //неюридическо лице
                    },
                    SubjectGroup = new NomenclatureEntry
                    {
                        Code = "437"    //търговец по ТЗ
                    },
                    CyrillicFullName = "Латерна 28",
                    CyrillicShortName = "Лат 28",
                    LatinFullName = "Laterna 28",
                    SubordinateLevel = new NomenclatureEntry
                    {
                        Code = "616"    //първо ниво на подчиненост
                    },
                    TRStatus = "OK?"
                }
            },
            Managers = new SubjectRelManager[]
                    {
                        new SubjectRelManager()
                        {
                            Position = new NomenclatureEntry
                            {
                                Code = "640"    //Длъжност в управлението - директор
                            },
                            RelatedSubject = new Subject
                            {
                                NaturalPersonSubject = new NaturalPerson
                                {
                                    Country = new NomenclatureEntry
                                    {
                                        Code = "100" //БГ
                                    },
                                    EGN = "6408081154",
                                    CyrillicName = "Марин Стоев",
                                    LatinName = "Marin Stoev"
                                }

                            },

                            RepresentedSubjects = new Subject[]{}
                        }
                    },
            CollectiveBodies = new SubjectPropCollectiveBody[]
            {
                new SubjectPropCollectiveBody()
                {
                    Members = new SubjectRelCollectiveBodyMember[]
                    {
                        new SubjectRelCollectiveBodyMember
                        {
                            RelatedSubject = new Subject
                            {
                                NaturalPersonSubject = new NaturalPerson
                                {
                                    Country = new NomenclatureEntry
                                    {
                                        Code = "100" //БГ
                                    },
                                    EGN = "6507157290",
                                    CyrillicName = "Марио Лалов",
                                    LatinName = "Mario Lalov"
                                }
                            }
                        },
                        new SubjectRelCollectiveBodyMember
                        {
                            RelatedSubject = new Subject
                            {
                                NaturalPersonSubject = new NaturalPerson
                                {
                                    Country = new NomenclatureEntry
                                    {
                                        Code = "100" //БГ
                                    },
                                    EGN = "6408081154",
                                    CyrillicName = "Иван Иванов",
                                    LatinName = "Ivan Ivanov"
                                }
                            }
                        },
                    }
                },
            },
            Partners = new SubjectRelPartner[]
            {
                new SubjectRelPartner()
                {
                    RelatedSubject = new Subject()
                    {
                        NaturalPersonSubject = new NaturalPerson
                        {
                            Country = new NomenclatureEntry
                            {
                                Code = "100" //БГ
                            },
                            EGN = "9001146336",
                            CyrillicName = "Пепа Колева",
                            LatinName = "Pepa Koleva"
                        }
                    }
                },
                new SubjectRelPartner()
                {
                    RelatedSubject = new Subject()
                    {
                        NaturalPersonSubject = new NaturalPerson
                        {
                            Country = new NomenclatureEntry
                            {
                                Code = "100" //БГ
                            },
                            EGN = "430810503",
                            CyrillicName = "Мила Лалова",
                            LatinName = "Mila Lalova"
                        }
                    }
                },
            },
            State = new SubjectPropState
            {
                State = new NomenclatureEntry
                {
                    Code = subjPropState
                },
            },
            Event = new Event
            {
                EventType = new NomenclatureEntry
                {
                    Code = eventType
                },
                EntryType = new NomenclatureEntry
                {
                    Code = entryType
                }

            },
            ReturnInformations = new ReturnInformation
            {
                ReturnCode = FoundReturnCode,
            }
        };
    }

    //Default subjPropState = 571 (OK! - развиващ дейност)
    //Default entryType = "756"(ОК! - първоначално вписване)
    private static StateOfPlayResponseType GetBulstatInfoForQAsCompanyByCase
        (string caseCourtCode, int caseYear, string caseNumber, string eventType, string subjPropState = "571", string entryType = "756")
    {
        var uic = "204536296";

        return new StateOfPlayResponseType
        {
            Subject = new Subject()
            {
                UIC = new UIC
                {
                    UIC1 = uic,
                    UICType = new NomenclatureEntry
                    {
                        Code = "1"
                    }
                },
                SubjectType = new NomenclatureEntry
                {
                    Code = "897"    //Физическо лице
                },
                LegalEntitySubject = new LegalEntity
                {
                    Country = new NomenclatureEntry
                    {
                        Code = "100"
                    },
                    LegalForm = new NomenclatureEntry
                    {
                        Code = "486"    //Сдружение
                    },
                    LegalStatute = new NomenclatureEntry
                    {
                        Code = "436"    //неюридическо лице
                    },
                    SubjectGroup = new NomenclatureEntry
                    {
                        Code = "437"    //търговец по ТЗ
                    },
                    CyrillicFullName = "Латерна 28",
                    CyrillicShortName = "Лат 28",
                    LatinFullName = "Laterna 28",
                    SubordinateLevel = new NomenclatureEntry
                    {
                        Code = "616"    //първо ниво на подчиненост
                    },
                    TRStatus = "OK?"
                }
            },
            Managers = new SubjectRelManager[]
                    {
                        new SubjectRelManager()
                        {
                            Position = new NomenclatureEntry
                            {
                                Code = "640"    //Длъжност в управлението - директор
                            },
                            RelatedSubject = new Subject
                            {
                                NaturalPersonSubject = new NaturalPerson
                                {
                                    Country = new NomenclatureEntry
                                    {
                                        Code = "100" //БГ
                                    },
                                    EGN = "9108116358",
                                    CyrillicName = "Лилиана Крумова",
                                    LatinName = "Liliana Krumova"
                                }

                            },

                            RepresentedSubjects = new Subject[]{}
                        },
                        new SubjectRelManager()
                        {
                            Position = new NomenclatureEntry
                            {
                                Code = "639"    //Длъжност в управлението - главен директор
                            },
                            RelatedSubject = new Subject
                            {
                                NaturalPersonSubject = new NaturalPerson
                                {
                                    Country = new NomenclatureEntry
                                    {
                                        Code = "100" //БГ
                                    },
                                    EGN = "7111106259",
                                    CyrillicName = "Юлия Бозукова",
                                    LatinName = "Julia Bozukova"
                                }

                            }
                        },
                        new SubjectRelManager()
                        {
                            Position = new NomenclatureEntry
                            {
                                Code = "639"    //Длъжност в управлението - главен директор
                            },
                            RelatedSubject = new Subject
                            {
                                NaturalPersonSubject = new NaturalPerson
                                {
                                    Country = new NomenclatureEntry
                                    {
                                        Code = "100" //БГ
                                    },
                                    EGN = "8802214026",
                                    CyrillicName = "Калоян Младенов Маринов",
                                    LatinName = "Kaloyan Mladenov Marinov"
                                }

                            }
                        }
                    },
            State = new SubjectPropState
            {
                State = new NomenclatureEntry
                {
                    Code = subjPropState
                },
            },
            Event = new Event
            {
                EventType = new NomenclatureEntry
                {
                    Code = eventType
                },
                EntryType = new NomenclatureEntry
                {
                    Code = entryType
                },
                Case = new Case()
                {
                    Court = new NomenclatureEntry
                    {
                        Code = caseCourtCode
                    },
                    Year = caseYear,
                    Number = caseNumber
                }

            },
            ReturnInformations = new ReturnInformation
            {
                ReturnCode = FoundReturnCode,
            }
        };
    }
}
