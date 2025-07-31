package bg.bulsi.mvr.mpozei.backend.mapper;

import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.mpozei.backend.service.RaeiceiService;
import bg.bulsi.mvr.mpozei.contract.dto.*;
import bg.bulsi.mvr.mpozei.contract.dto.xml.EidApplicationXml;
import bg.bulsi.mvr.mpozei.model.application.*;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceDTO;
import org.mapstruct.*;
import org.springframework.beans.factory.annotation.Autowired;

import java.time.LocalDate;
import java.time.OffsetDateTime;
import java.time.format.DateTimeFormatter;
import java.util.Objects;
import java.util.UUID;

import static bg.bulsi.mvr.common.util.ValidationUtil.assertNotNull;

@Mapper(componentModel = "spring", nullValueCheckStrategy = NullValueCheckStrategy.ALWAYS)
public abstract class XmlMapper {
    @Autowired
    protected RaeiceiService raeiceiService;

    @Mapping(source = "personalIdentityDocument.identityType", target = "identityType")
    @Mapping(source = "personalIdentityDocument.identityNumber", target = "identityNumber")
    @Mapping(source = "personalIdentityDocument.identityIssueDate", target = "identityIssueDate", qualifiedByName = "mapLocalDateToString")
    @Mapping(source = "personalIdentityDocument.identityValidityToDate", target = "identityValidityToDate", qualifiedByName = "mapLocalDateToString")
    @Mapping(target = "citizenProfileId", ignore = true)
    @Mapping(target = "eidentityId", ignore = true)
    public abstract EidApplicationXml map(ApplicationXmlRequest request);

//    @Mapping(source = "personalIdentityDocument.identityType", target = "identityType")
//    @Mapping(source = "personalIdentityDocument.identityNumber", target = "identityNumber")
//    @Mapping(source = "personalIdentityDocument.identityIssueDate", target = "identityIssueDate", qualifiedByName = "mapLocalDateToString")
//    @Mapping(source = "personalIdentityDocument.identityIssuer", target = "identityIssuer")
//    @Mapping(source = "personalIdentityDocument.identityValidityToDate", target = "identityValidityToDate", qualifiedByName = "mapLocalDateToString")
//    EidSignatureXml map(OnlineApplicationRequest request);

    @BeanMapping(qualifiedByName = "initialAfterMapping")
    @Mapping(qualifiedByName = "mapLocalDateToString", target = "identityIssueDate")
    @Mapping(qualifiedByName = "mapLocalDateToString", target = "identityValidityToDate")
    @Mapping(target = "guardians", ignore = true)
    public abstract EidApplicationXml map(AbstractApplication application, PersonalIdentityDocument personalIdentityDocument);

    @Mapping(target = "guardians", source = "temporaryData.guardians")
    @Mapping(target = "email", ignore = true)
    @Mapping(target = "phoneNumber", ignore = true)
    public abstract EidApplicationXml map(EidApplicationXml eidApplicationXml, TemporaryData temporaryData);

    public abstract EidApplicationXml mapToParsedXml(String xml);

    @Named("mapLocalDateToString")
    public String mapLocalDateToString(LocalDate date) {
        return date.format(DateTimeFormatter.ISO_LOCAL_DATE);
    }

    @AfterMapping
    @Named("initialAfterMapping")
    public void initialAfterMapping(@MappingTarget EidApplicationXml xml, AbstractApplication application) {
        xml.setApplicationId(application.getId().toString());
        xml.setApplicationType(application.getApplicationType().name());
        DeviceDTO device = raeiceiService.getDeviceById(application.getDeviceId());
        xml.setDeviceId(device.getId());
        xml.setEidAdministratorId(application.getEidAdministratorId().toString());
        xml.setEidAdministratorOfficeId(application.getAdministratorFrontOfficeId().toString());
    }

    @Mapping(source = "applicationId", target = "id")
    @Mapping(source = "identityNumber", target = "temporaryData.personalIdentityDocument.identityNumber")
    @Mapping(source = "identityType", target = "temporaryData.personalIdentityDocument.identityType")
    @Mapping(source = "identityIssueDate", target = "temporaryData.personalIdentityDocument.identityIssueDate")
    @Mapping(source = "identityValidityToDate", target = "temporaryData.personalIdentityDocument.identityValidityToDate")
    @Mapping(target = "reasonText", ignore = true)
    @BeanMapping(qualifiedByName = "xmlToApplicationAfterMapping")
    public abstract AbstractApplication mapOnlineApplication(@MappingTarget AbstractApplication application, EidApplicationXml xml);

    public AbstractApplication mapOnlineApplication(EidApplicationXml xml) {
        return switch (ApplicationType.valueOf(xml.getApplicationType())) {
            case ISSUE_EID -> mapOnlineApplication(new IssueEidApplication(), xml);
            case RESUME_EID -> mapOnlineApplication(new ResumeEidApplication(), xml);
            case REVOKE_EID -> mapOnlineApplication(new RevokeEidApplication(), xml);
            case STOP_EID -> mapOnlineApplication(new StopEidApplication(), xml);
        };
    }

    @AfterMapping
    @Named("xmlToApplicationAfterMapping")
    public void xmlToApplicationAfterMapping(EidApplicationXml xml, @MappingTarget AbstractApplication application) {
        if (Objects.nonNull(xml.getEmail())) {
            application.getParams().setEmail(xml.getEmail());
        }
        if (Objects.nonNull(xml.getPhoneNumber())) {
            application.getParams().setPhoneNumber(xml.getPhoneNumber());
        }
        if (Objects.nonNull(xml.getDateOfBirth())) {
            application.getParams().setDateOfBirth(xml.getDateOfBirth());
        }
        if (Objects.nonNull(xml.getCitizenProfileId())) {
            application.getParams().setCitizenProfileId(UUID.fromString(xml.getCitizenProfileId()));
        }
        if (Objects.nonNull(xml.getCertificateId())) {
            application.getParams().setCertificateId(UUID.fromString(xml.getCertificateId()));
        }
        if (Objects.nonNull(xml.getLevelOfAssurance())) {
            application.getParams().setLevelOfAssurance(LevelOfAssurance.valueOf(xml.getLevelOfAssurance()));
        }
    }

    public LocalDate map(String value) {
        return LocalDate.parse(value, DateTimeFormatter.ISO_LOCAL_DATE);
    }

    public EidApplicationXml map(OnlineCertStatusApplicationRequest request) {
        EidApplicationXml xml = new EidApplicationXml();
        xml.setFirstName(request.getFirstName());
        xml.setFirstNameLatin(request.getFirstNameLatin());
        xml.setSecondName(request.getSecondName());
        xml.setSecondNameLatin(request.getSecondNameLatin());
        xml.setLastName(request.getLastName());
        xml.setLastNameLatin(request.getLastNameLatin());
        xml.setCitizenIdentifierNumber(request.getCitizenIdentifierNumber());
        xml.setCitizenIdentifierType(request.getCitizenIdentifierType().name());
        xml.setCitizenship(request.getCitizenship());
        xml.setIdentityNumber(request.getPersonalIdentityDocument().getIdentityNumber());
        xml.setIdentityType(request.getPersonalIdentityDocument().getIdentityType());
        xml.setIdentityIssueDate(request.getPersonalIdentityDocument().getIdentityIssueDate().format(DateTimeFormatter.ISO_LOCAL_DATE));
        xml.setIdentityValidityToDate(request.getPersonalIdentityDocument().getIdentityValidityToDate().format(DateTimeFormatter.ISO_LOCAL_DATE));
        xml.setApplicationType(request.getApplicationType().name());
        // TODO: 3/25/2024 add date of birth
//        xml.setDateOfBirth(request);
        String citizenProfileId = UserContextHolder.getFromServletContext().getCitizenProfileId();
        assertNotNull(citizenProfileId, ErrorCode.CITIZEN_PROFILE_ID_CANNOT_BE_NULL);
        xml.setCitizenProfileId(citizenProfileId);

        String eidentityId = UserContextHolder.getFromServletContext().getEidentityId();
        xml.setEidentityId(eidentityId);
        xml.setCitizenship(request.getCitizenship());

        if (request.getReasonId() != null ) {
        	 xml.setReasonId(request.getReasonId().toString());
        }
        xml.setReasonText(request.getReasonText());
        xml.setCertificateId(request.getCertificateId().toString());
        return xml;
    }
}
