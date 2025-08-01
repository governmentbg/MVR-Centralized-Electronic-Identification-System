package bg.bulsi.mvr.mpozei.backend.service.impl;

import bg.bulsi.mvr.common.config.security.UserContext;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.exception.EntityNotFoundException;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.FaultMVRException;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.common.pipeline.Pipeline;
import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.service.FileFormatService;
import bg.bulsi.mvr.common.util.MVRConstants;
import bg.bulsi.mvr.mpozei.backend.client.RueiClient;
import bg.bulsi.mvr.mpozei.backend.dto.*;
import bg.bulsi.mvr.mpozei.backend.mapper.ApplicationMapper;
import bg.bulsi.mvr.mpozei.backend.mapper.CertificateMapper;
import bg.bulsi.mvr.mpozei.backend.mapper.XmlMapper;
import bg.bulsi.mvr.mpozei.backend.pipeline.pipelines.application.external_administrators.issue_eid.EnrollForCertificateExtAdminPipeline;
import bg.bulsi.mvr.mpozei.backend.pipeline.pipelines.application.issue_eid.EnrollForCertificatePipeline;
import bg.bulsi.mvr.mpozei.backend.service.*;
import bg.bulsi.mvr.mpozei.contract.dto.*;
import bg.bulsi.mvr.mpozei.contract.dto.xml.EidApplicationXml;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import bg.bulsi.mvr.mpozei.model.certificate.CertificateHistory;
import bg.bulsi.mvr.mpozei.model.id_generator.id.ApplicationNumber;
import bg.bulsi.mvr.mpozei.model.nomenclature.ApplicationToReasonNomMapper;
import bg.bulsi.mvr.mpozei.model.nomenclature.PermittedUser;
import bg.bulsi.mvr.mpozei.model.nomenclature.ReasonNomenclature;
import bg.bulsi.mvr.mpozei.model.pan.EventRegistratorImpl;
import bg.bulsi.mvr.mpozei.model.repository.ApplicationRepository;
import bg.bulsi.mvr.mpozei.model.repository.view.ApplicationReportByOffice;
import bg.bulsi.mvr.mpozei.model.repository.view.ApplicationReportByOperators;
import bg.bulsi.mvr.mpozei.model.repository.view.ApplicationReportByRegion;
import bg.bulsi.mvr.mpozei.model.repository.view.ApplicationReportTotal;
import bg.bulsi.mvr.pdf_generator.PdfGenerator;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceType;
import bg.bulsi.mvr.raeicei.contract.dto.EidAdministratorDTO;
import bg.bulsi.mvr.raeicei.contract.dto.EidManagerFrontOfficeDTO;
import bg.bulsi.mvr.raeicei.model.entity.EidManagerFrontOffice;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.data.domain.Sort;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.validation.beanvalidation.LocalValidatorFactoryBean;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.OffsetDateTime;
import java.time.ZoneOffset;
import java.util.*;
import java.util.stream.Collectors;

import static bg.bulsi.mvr.common.exception.ErrorCode.*;
import static bg.bulsi.mvr.common.pipeline.PipelineStatus.CREATE_PAYMENT;
import static bg.bulsi.mvr.common.pipeline.PipelineStatus.ISSUE_EID_SIGNED;
import static bg.bulsi.mvr.common.util.ValidationUtil.*;
import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus.*;
import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType.*;
import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationType.*;

@Slf4j
@Service
@RequiredArgsConstructor
public class ApplicationServiceImpl implements ApplicationService {
    private final List<Pipeline<AbstractApplication>> pipelines;
    private final ApplicationRepository<AbstractApplication> abstractApplicationRepository;
    private final NomenclatureService nomenclatureService;
    private final NotificationService notificationService;
    private final RaeiceiService raeiceiService;
    private final ApplicationMapper applicationMapper;
    private final ApplicationToReasonNomMapper reasonNomMapper;
    private final FileFormatService fileFormatService;
    private final XmlMapper xmlMapper;
    private final LocalValidatorFactoryBean validator;
    private final QrCodeService qrCodeService;
    private final CertificateService certificateService;
    private final CertificateMapper certificateMapper;
    private final RueiClient rueiClient;
    private final PdfGenerator pdfGenerator;

    @Override
    @Transactional
    public AbstractApplication createDeskApplication(DeskApplicationRequest request) {
        AbstractApplication entity = applicationMapper.mapToApplication(request);
        entity.setSubmissionType(DESK);
        entity.getParams().setIsOnlineOffice(false);
        DeviceDTO device = raeiceiService.getDeviceById(request.getDeviceId());
        entity.setDeviceId(device.getId());
        entity.getParams().setLevelOfAssurance(findLevelOfAssurance());

        if (List.of(STOP_EID, RESUME_EID, REVOKE_EID).contains(entity.getApplicationType())) {
            addReason(entity, request.getReasonId(), request.getReasonText(), PermittedUser.ADMIN);
            entity.getParams().setCertificateId(request.getCertificateId());
        }
        UserContext context = UserContextHolder.getFromServletContext();
        addAdministratorInfo(device, entity, context.getEidAdministratorId(), context.getEidAdministratorFrontOfficeId());

        entity = abstractApplicationRepository.save(entity);
        return entity;
    }

    @Override
    @Transactional
    public AbstractApplication createOnlineCertStatusApplicationPlain(OnlineCertStatusApplicationRequest request) {
        AbstractApplication entity = applicationMapper.mapToApplication(request);
        
        if (UserContextHolder.getFromServletContext().isBaseProfileAuth() && STOP_EID.equals(entity.getApplicationType())) {
            entity.setSubmissionType(BASE_PROFILE);
        } else if (UserContextHolder.getFromServletContext().isEidAuth() && List.of(STOP_EID, RESUME_EID, REVOKE_EID).contains(entity.getApplicationType())){
            entity.setSubmissionType(EID);
        } else {
        	throw new ValidationMVRException(ErrorCode.APPLICATION_TYPE_WITH_PROVIDED_LOA_REQUIRES_XML_SIGNING);
        	//throw new ValidationMVRException(ErrorCode.ENDPOINT_IS_FOR_CHANGE_STATUS_APPLICATIONS);
        }
        
       // assertTrue(List.of(STOP_EID, RESUME_EID, REVOKE_EID).contains(entity.getApplicationType()), ErrorCode.ENDPOINT_IS_FOR_CHANGE_STATUS_APPLICATIONS);
        
        UUID certificateId = request.getCertificateId();
        UUID reasonId = request.getReasonId();
        String reasonText = request.getReasonText();
        
        this.commonCreateOnlineCertStatusApplication(entity, certificateId, reasonId, reasonText);
        
        String xml = createApplicationXml(request, entity);
        entity.setApplicationXml(xml);
        entity = abstractApplicationRepository.save(entity);
        return entity;
    }

    @Override
    @Transactional
    public AbstractApplication createOnlineCertStatusApplicationSigned(OnlineApplicationRequest request) {
        String originalXml = new String(Base64.getDecoder().decode(request.getXml()));

        EidApplicationXml xml = fileFormatService.createObjectFromXmlString(originalXml, EidApplicationXml.class);
        validator.validate(xml);
        AbstractApplication entity;
        try {
            entity = xmlMapper.mapOnlineApplication(xml);
        } catch (RuntimeException e) {
            throw new ValidationMVRException(e.getMessage(), ErrorCode.VALIDATION_ERROR);
        }
        
        assertFalse(abstractApplicationRepository.existsById(entity.getId()), ErrorCode.APPLICATION_WITH_THAT_ID_ALREADY_EXISTS);
        assertTrue(List.of(RESUME_EID, REVOKE_EID).contains(entity.getApplicationType()), ErrorCode.ENDPOINT_IS_FOR_RESUME_AND_REVOKE_APPLICATIONS);
        
        entity.setApplicationXml(originalXml);
        entity.setDetachedSignature(request.getSignature());
        entity.getTemporaryData().setSignatureProvider(request.getSignatureProvider());
        entity.setCitizenProfileId(UUID.fromString(UserContextHolder.getFromServletContext().getCitizenProfileId()));

        if (!UserContextHolder.getFromServletContext().isBaseProfileAuth()) {
            throw new ValidationMVRException(ErrorCode.BASE_PROFILE_AUTHENTICATION_REQUIRED);
        }
        
        entity.setSubmissionType(BASE_PROFILE);
        
        UUID certificateId = UUID.fromString(xml.getCertificateId());
        UUID reasonId =  xml.getReasonId() != null ? UUID.fromString(xml.getReasonId()) : null;
        String reasonText = xml.getReasonText();
        
        this.commonCreateOnlineCertStatusApplication(entity, certificateId, reasonId, reasonText);

        entity = abstractApplicationRepository.save(entity);
        return entity;
    }

	private void commonCreateOnlineCertStatusApplication(AbstractApplication entity, UUID certificateId, UUID reasonId,
			String reasonText) {
		CitizenCertificateSummaryResponse certificate = certificateService.getCertificateById(certificateId);
        entity.getParams().setIsOnlineOffice(true);
        DeviceDTO device = raeiceiService.getDeviceById(certificate.getDeviceId());
        entity.setDeviceId(device.getId());
        entity.getParams().setLevelOfAssurance(findLevelOfAssurance());

        addReason(entity, reasonId, reasonText, PermittedUser.PUBLIC);
        entity.getParams().setCertificateId(certificate.getId());
        EidManagerFrontOfficeDTO administratorFrontOffice = raeiceiService.getOfficeByName(EidManagerFrontOffice.ONLINE);

        addAdministratorInfo(device, entity, certificate.getEidAdministratorId().toString(), administratorFrontOffice.getId().toString());
	}
    
    @Override
    @Transactional
    public AbstractApplication createPersoCentreApplication(PersoCentreApplicationRequest request) {
        AbstractApplication entity = applicationMapper.mapToPersoCentreApplication(request);
        entity.setSubmissionType(PERSO_CENTRE);
        entity.getParams().setCertificateSigningRequest(request.getCertificateSigningRequest());
        entity.getParams().setCertificateCaName(request.getCertificateAuthorityName());
        entity.getParams().setIsOnlineOffice(false);
        DeviceDTO device = raeiceiService.getDeviceById(request.getDeviceId());
        entity.setDeviceId(device.getId());
        entity.getParams().setLevelOfAssurance(findLevelOfAssurance());

//        UserContext context = UserContextHolder.getFromServletContext();
//        addAdministratorInfo(device, entity, context.getEidAdministratorId(), context.getEidAdministratorFrontOfficeId());
        addAdministratorInfo(device, entity, request.getEidAdministratorId().toString(), request.getEidAdministratorOfficeId().toString());
        EidManagerFrontOfficeDTO office = raeiceiService.getOfficeById(entity.getAdministratorFrontOfficeId());
        assertFalse(office.getName().equals(EidManagerFrontOffice.ONLINE), ErrorCode.ONLINE_OFFICE_REQUIRES_EID_SUBMISSION);

        return abstractApplicationRepository.save(entity);
    }

    @Override
    @Transactional
    public AbstractApplication createOnlineApplication(OnlineApplicationRequest request) {
        String originalXml = new String(Base64.getDecoder().decode(request.getXml()));

        EidApplicationXml xml = fileFormatService.createObjectFromXmlString(originalXml, EidApplicationXml.class);
        validator.validate(xml);
        
        assertTrue(RegixService.isBulgarian(xml.getCitizenship()), APPLICATION_TYPE_REQUIRES_DESK_FOR_FOREIGNER);
        
        AbstractApplication entity;
        try {
            entity = xmlMapper.mapOnlineApplication(xml);
        } catch (RuntimeException e) {
            throw new ValidationMVRException(e.getMessage(), ErrorCode.VALIDATION_ERROR);
        }
        
        assertFalse(abstractApplicationRepository.existsById(entity.getId()), ErrorCode.APPLICATION_WITH_THAT_ID_ALREADY_EXISTS);
        assertTrue(entity.getApplicationType() == ISSUE_EID, ErrorCode.ENDPOINT_IS_ONLY_FOR_ISSUE_EID);
        entity.setApplicationXml(originalXml);
        entity.setDetachedSignature(request.getSignature());
        entity.setCitizenProfileId(UUID.fromString(xml.getCitizenProfileId()));
        DeviceDTO device = raeiceiService.getDeviceById(entity.getDeviceId());
        entity.getParams().setLevelOfAssurance(findLevelOfAssurance());

        addAdministratorInfo(device, entity, xml.getEidAdministratorId(), xml.getEidAdministratorOfficeId());
        boolean isOnlineOffice = false;
        EidManagerFrontOfficeDTO office = raeiceiService.getOfficeById(entity.getAdministratorFrontOfficeId());
        if (UserContextHolder.getFromServletContext().isBaseProfileAuth()) {
            entity.setSubmissionType(BASE_PROFILE);
            entity.getTemporaryData().setSignatureProvider(request.getSignatureProvider());
            //TODO: Uncomment later, this is temporary
            //assertFalse(office.getName().equals(EidManagerFrontOffice.ONLINE), ErrorCode.ONLINE_OFFICE_REQUIRES_EID_SUBMISSION);
        } else {
            assertFalse(office.getName().equals(EidManagerFrontOffice.ONLINE) && device.getType().equals(DeviceType.CHIP_CARD), ErrorCode.ONLINE_OFFICE_REQUIRES_MOBILE);
            entity.setSubmissionType(EID);
            if (office.getName().equals(EidManagerFrontOffice.ONLINE) && DeviceType.MOBILE.equals(device.getType())) {
                isOnlineOffice = true;
            }
        }
        entity.getParams().setIsOnlineOffice(isOnlineOffice);

//        if (List.of(STOP_EID).contains(entity.getApplicationType())) {
//            addReason(entity, UUID.fromString(xml.getReasonId()), xml.getReasonText());
//        }

        entity = abstractApplicationRepository.save(entity);
        return entity;
    }

    @Override
    @Transactional
    public AbstractApplication processApplication(AbstractApplication application) {
        findPipeline(application).process(application);
        return abstractApplicationRepository.save(application);
    }


	@Override
	@Transactional
	public <T extends Pipeline> AbstractApplication processApplication(
			AbstractApplication application,
			ErrorCode errorCode,
			Class<T> expectedPipelineClass) {

		Pipeline<AbstractApplication> currentPipeline = pipelines.stream().filter(p -> expectedPipelineClass.getCanonicalName().equals(p.getClass().getCanonicalName())).findFirst().get();

        if (!currentPipeline.canProcess(application)) {
            throw new ValidationMVRException(errorCode);
        }
		
        currentPipeline.process(application);
		
        return abstractApplicationRepository.save(application);
	}
    
    @Override
    @Transactional(readOnly = true)
    public AbstractApplication getAbstractApplicationById(UUID id) {
        String eidAdministratorId = UserContextHolder.getFromServletContext().getEidAdministratorId();

        if (Objects.nonNull(eidAdministratorId)) {
            return abstractApplicationRepository.findByIdAndEidAdministratorId(id, UUID.fromString(eidAdministratorId))
                    .orElseThrow(() -> new EntityNotFoundException(ErrorCode.APPLICATION_NOT_FOUND, id));
        }
        return abstractApplicationRepository.findById(id)
                .orElseThrow(() -> new EntityNotFoundException(ErrorCode.APPLICATION_NOT_FOUND, id));
    }

//    @Override
//    @Transactional(readOnly = true)
//    public AbstractApplication getAbstractApplicationById(UUID id, UUID eidentityId) {
//        return Objects.nonNull(eidentityId)
//                ?
//                abstractApplicationRepository.findByIdAndEidentityId(id, eidentityId)
//                        .orElseThrow(() -> new EntityNotFoundException(ErrorCode.APPLICATION_NOT_FOUND, id))
//                :
//                abstractApplicationRepository.findById(id)
//                        .orElseThrow(() -> new EntityNotFoundException(ErrorCode.APPLICATION_NOT_FOUND, id));
//    }
//
//	@Override
//    @Transactional
//	public AbstractApplication getAbstractApplicationByIdWithAdditionalData(UUID id) {
//        return abstractApplicationRepository.getAbstractApplicationByIdWithAdditionalData(id)
//                .orElseThrow(() -> new EntityNotFoundException(ErrorCode.APPLICATION_NOT_FOUND, id));
//	}
    
    @Override
    @Transactional
    public void updateApplication(AbstractApplication application) {
        abstractApplicationRepository.save(application);
    }

    @Override
    @Transactional
    public ApplicationStatus denyApplication(DenyApplicationDTO dto) {
        AbstractApplication application = getAbstractApplicationById(dto.getApplicationId());
        ApplicationSubmissionType submissionType = application.getSubmissionType();
        DeviceDTO device = raeiceiService.getDeviceById(application.getDeviceId());
        if (submissionType != BASE_PROFILE && !Objects.equals(device.getType(), DeviceType.MOBILE) && qrCodeService.qrCodeExistsByApplicationId(application.getId())) {
           assertFalse(List.of(DENIED, COMPLETED, GENERATED_CERTIFICATE).contains(application.getStatus()),
                    ErrorCode.APPLICATION_CANNOT_BE_DENIED, application.getStatus(), application.getId());
        }
        application.setStatus(DENIED);
        addReason(application, dto.getReasonId(), dto.getReasonText(), PermittedUser.ADMIN);

        CitizenCertificateUpdateDTO certificateStatusUpdate = applicationMapper.mapToCitizenCertificateUpdateDTO(application, CertificateStatusDTO.REVOKED);
        if(certificateStatusUpdate != null) {
	        this.rueiClient.updateCertificateStatus(certificateStatusUpdate);

	        UUID reasonId = application.getReason() != null ? application.getReason().getId() : null;
	        String reasonText = application.getReasonText();
	        CitizenCertificateSummaryDTO certificate = rueiClient.getCertificateById(application.getParams().getCertificateId());
	        CertificateHistory history = certificateMapper.map(certificate);
	        history.setApplicationId(application.getId());
	        history.setApplicationNumber(application.getApplicationNumber().getId());
	        history.setCreateDate(OffsetDateTime.of(application.getCreateDate(), ZoneOffset.UTC));
	        history.setModifiedDate(OffsetDateTime.of(application.getLastUpdate(), ZoneOffset.UTC));
	        history.setDeviceId(application.getDeviceId());
	        history.setReasonId(reasonId);
	        history.setReasonText(reasonText);

	        this.certificateService.createCertificateHistory(history);
        }
        
        notificationService.sendNotification(EventRegistratorImpl.MPOZEI_E_DENIED_DUE_TO_INVALID_CHECK_EID, application.getEidentityId());

        UserContextHolder.getFromServletContext().setTargetUserId(application.getEidentityId());
        return application.getStatus();
    }

    @Override
    @Transactional
    public ApplicationStatus invalidatePersoCentreApplication(PersoCentreConfirmApplicationRequest request) {
        AbstractApplication application = getAbstractApplicationById(request.getApplicationId());
        application.setStatus(DENIED);
        
        ReasonNomenclature reason = nomenclatureService.getReasonByName(ReasonNomenclature.STOPPED_REVOKED_BY_SYSTEM);
        application.setReason(reason);
        
        CitizenCertificateDetailsDTO certificateDetailsDTO = 
        		this.rueiClient.updateCertificateStatusByNaif(new RueiUpdateCertificateStatusNaifDTO(application.getParams().getCertificateId(), CertificateStatusDTO.REVOKED));
        
        CertificateHistory certificateHistory = certificateMapper.map(certificateDetailsDTO);
        certificateHistory.setApplicationId(application.getId());
        certificateHistory.setApplicationNumber(application.getApplicationNumber().getId());
        certificateHistory.setReasonId(reason.getId());
        certificateHistory.setReasonText(request.getReasonText());
        this.certificateService.createCertificateHistory(certificateHistory);
        
        return application.getStatus();
    }

    @Override
    @Transactional
    public ApplicationStatus updateApplicationStatus(UUID applicationId, ApplicationStatus applicationStatus) {
        AbstractApplication application = getAbstractApplicationById(applicationId);
        boolean isValid = false;
        switch (application.getStatus()) {
            case PENDING_PAYMENT -> isValid = applicationStatus == ApplicationStatus.PAID;
        }
        if (!isValid) {
            throw new ValidationMVRException(ErrorCode.STATUS_NOT_EXPECTED, applicationStatus, applicationId);
        }
        application.setStatus(applicationStatus);
        return abstractApplicationRepository.save(application).getStatus();
    }

    @Override
    @Transactional
    public Page<AbstractApplication> findApplications(ApplicationFilter filter, Pageable pageable) {
        if (Objects.nonNull(pageable) && pageable.getSort().isEmpty()) {
            Sort sort = Sort.by(Sort.Direction.DESC, "createDate");
            pageable = PageRequest.of(pageable.getPageNumber(), pageable.getPageSize(), sort);
        }

        if (Objects.nonNull(pageable) && Objects.nonNull(UserContextHolder.getFromServletContext().getEidAdministratorId())) {
            filter.setEidAdministratorId(UUID.fromString(UserContextHolder.getFromServletContext().getEidAdministratorId()));
        }
        return abstractApplicationRepository.findByFilter(this.applicationMapper.mapToDbApplicationFilter(filter), pageable);
    }

    @Override
    public Page<AbstractApplication> adminFindApplications(ApplicationFilter filter, Pageable pageable) {
        if (Objects.nonNull(pageable) && pageable.getSort().isEmpty()) {
            Sort sort = Sort.by(Sort.Direction.DESC, "createDate");
            pageable = PageRequest.of(pageable.getPageNumber(), pageable.getPageSize(), sort);
        }
        return abstractApplicationRepository.findByFilter(this.applicationMapper.mapToDbApplicationFilter(filter), pageable);
    }

    @Override
    @Transactional
    public AbstractApplication enrollForCertificate(EnrollCertificateDTO dto) {
        AbstractApplication application = getAbstractApplicationById(dto.getApplicationId());

        if (application.getStatus() == ApplicationStatus.GENERATED_CERTIFICATE) {
            throw new ValidationMVRException(ErrorCode.CERTIFICATE_IS_ALREADY_GENERATED, application.getId());
        }

        applicationMapper.map(application, dto);
        return processApplication(application, ErrorCode.NO_VALID_PIPELINES_EXIST , EnrollForCertificatePipeline.class);
    }

    @Override
    @Transactional
    public AbstractApplication enrollForExtAdminCertificate(CertificateExtAdminRequest dto) {
        AbstractApplication application = getAbstractApplicationById(dto.getApplicationId());

        if (application.getStatus() == ApplicationStatus.GENERATED_CERTIFICATE) {
            throw new ValidationMVRException(ErrorCode.CERTIFICATE_IS_ALREADY_GENERATED, application.getId());
        }

        applicationMapper.map(application, dto);
        return processApplication(application, ErrorCode.NO_VALID_PIPELINES_EXIST , EnrollForCertificateExtAdminPipeline.class);
    }

    @Override
    @Transactional
    public ApplicationStatus confirmCertificateStorage(CertificateConfirmDTO dto) {
        AbstractApplication application = getAbstractApplicationById(dto.getApplicationId());
        assertTrue(application.getStatus() == GENERATED_CERTIFICATE, ErrorCode.APPLICATION_CANNOT_BE_CONFIRMED);

        if (ApplicationConfirmationStatus.OK.equals(dto.getStatus())) {
            application.setStatus(CERTIFICATE_STORED);
        } else {
            rueiClient.updateCertificateStatus(applicationMapper.mapToCitizenCertificateUpdateDTO(application, CertificateStatusDTO.FAILED));
            ApplicationSubmissionType submissionType = application.getSubmissionType();
            Boolean isOnlineOffice = application.getParams().getIsOnlineOffice();
            application.setPipelineStatus(switch (submissionType){
                case DESK, BASE_PROFILE -> ISSUE_EID_SIGNED;
                case EID -> isOnlineOffice ? CREATE_PAYMENT : ISSUE_EID_SIGNED;
                case PERSO_CENTRE -> throw new ValidationMVRException(ErrorCode.CANNOT_CONFIRM_STORAGE);
            });
            application.getParams().setClientCertificate(null);
            application.getParams().setClientCertificateChain(null);
            application.getParams().setIssuerDn(null);
            application.getParams().setEndEntityProfileName(null);
            application.getParams().setCertificateSerialNumber(null);
            application.getParams().setCertificateId(null);
            application.setStatus(PAID);
        }
        return abstractApplicationRepository.save(application).getStatus();
    }

    @Override
    @Transactional
    public List<AbstractApplication> getDailyUnfinishedApplications() {
        return abstractApplicationRepository.getUnfinishedApplications(LocalDateTime.now().minusDays(3), LocalDateTime.now().minusDays(14));
    }

    @Override
    public void save(AbstractApplication application) {
        abstractApplicationRepository.save(application);
    }

    @Override
    public void saveAll(List<AbstractApplication> application) {
    	this.abstractApplicationRepository.saveAll(application);
    }
    
    @Override
    @Transactional
    public void attachCitizenProfileIdToApplications(UUID eidentityId, UUID citizenProfileId) {
        List<AbstractApplication> applications = abstractApplicationRepository.findAllByEidentityId(eidentityId, Pageable.unpaged())
                .stream()
                .filter(e -> Objects.isNull(e.getCitizenProfileId())).toList();
        applications.forEach(e -> e.setCitizenProfileId(citizenProfileId));
        abstractApplicationRepository.saveAll(applications);
    }

    @Override
    @Transactional
    public List<ApplicationReportByRegion> getApplicationReportByRegion(LocalDate from, LocalDate to) {
        return abstractApplicationRepository.getReportByRegion(from, to);
    }

    @Override
    @Transactional
    public List<ApplicationReportByOperators> getJsonApplicationReportByOperators(List<String> operators, LocalDate from, LocalDate to) {
        return abstractApplicationRepository.getReportByOperators(operators, from, to);
    }

    @Override
    @Transactional
    public List<ApplicationReportByOperators> getCsvApplicationReportByOperators(List<String> operators, LocalDate from, LocalDate to) {
        return abstractApplicationRepository.getReportByOperators(operators, from, to);
    }

    @Override
    @Transactional
    public List<ApplicationReportByOffice> getApplicationReportByOffices(UUID administratorId, LocalDate from, LocalDate to) {
        return abstractApplicationRepository.getReportByOffice(administratorId, from, to);
    }

    @Override
    @Transactional
    public List<ApplicationReportTotal> getApplicationReportTotal(LocalDate from, LocalDate to) {
        return abstractApplicationRepository.getReportTotal(from, to);
    }

    @Override
    public byte[] exportConfirmationApplication(AbstractApplication application) {
        DeviceDTO device = raeiceiService.getDeviceById(application.getDeviceId());
        boolean isValid = (DeviceType.MOBILE.equals(device.getType()) && PipelineStatus.EXPORT_APPLICATION == application.getPipelineStatus())
                || (application.getPipelineStatus().equals(ISSUE_EID_SIGNED) && List.of(SIGNED, PENDING_PAYMENT).contains(application.getStatus()));
        assertTrue(isValid, EXPORT_CONFIRMATION_IS_NOT_POSSIBLE);

        Map<String, Object> formParams = applicationMapper.mapToKeyValueMap(application);
        if (device.getType().equals(DeviceType.MOBILE)) {
            EidAdministratorDTO eidAdministrator = raeiceiService.getEidAdministratorById(application.getEidAdministratorId());
            formParams.put("qrCode", qrCodeService.generateQrCodeImage(application.getId(), eidAdministrator.getNameLatin()));
        }
        formParams.putAll(applicationMapper.mapToKeyValueMapFromFE());

        String templateName = "CONFIRM_EID";
//        if (!ExportApplicationStep.BULGARIA_COUNTRY_NAME.equalsIgnoreCase(application.getCitizenship().trim())) {
//            templateName = templateName + "_FOREIGNER";
//        }
        templateName = templateName + ".html";
        return pdfGenerator.generatePdf(templateName, formParams);
    }

    private Pipeline<AbstractApplication> findPipeline(AbstractApplication application) {
        List<Pipeline<AbstractApplication>> validPipelines = pipelines
                .stream()
                .filter(pipeline -> pipeline.canProcess(application) && !pipeline.isRepeatable())
                .toList();

        if (validPipelines.isEmpty()) {
            throw new ValidationMVRException(ErrorCode.OPERATION_NOT_SUPPORTED, application.getId());
        }
        if (validPipelines.size() > 1) {
            String availablePipelines = validPipelines.stream().map(p -> p.getClass().getSimpleName())
                    .collect(Collectors.joining(", "));
            log.info(".findPipeline() More than one valid pipelines are found ({}) that can process application with id: {}", availablePipelines, application.getId());

            throw new FaultMVRException(MORE_THAN_ONE_VALID_PIPELINES_EXIST, availablePipelines, application.getId());
        }

        String validPipelineName = validPipelines.get(0).getClass().getSimpleName();
        log.info(".findPipeline() PipelineName={}; ApplicationId={}", validPipelineName, application.getId());


        return validPipelines.get(0);
    }

    private void addReason(AbstractApplication application, UUID reasonId, String reasonText, PermittedUser permittedUser) {
    	String reasonNomTypeName = reasonNomMapper.getReasonNomTypeName(application);
    	if(reasonNomTypeName == null) {
    		return;
    	}
    	
        assertNotNull(reasonId, REASON_CANNOT_BE_NULL);
        ReasonNomenclature reason = nomenclatureService.getReasonById(reasonId);
		
        assertEquals(reason.getNomCode().getName(), reasonNomTypeName, REASON_NOT_SUPPORTED);
        assertEquals(permittedUser, reason.getPermittedUser(), REASON_NOT_ALLOWED);

        if (Boolean.TRUE.equals(reason.getTextRequired())) {
        	assertNotBlank(reasonText, REASON_TEXT_CANNOT_BE_BLANK);
            application.setReasonText(reasonText);
        } else {
            assertNull(reasonText, REASON_TEXT_REQUIRES_DIFFERENT_REASON);
        }
        application.setReason(reason);
    }

    private void addAdministratorInfo(DeviceDTO device, AbstractApplication entity, String eidAdministratorId, String eidAdministratorOfficeId) {
        assertNotNull(eidAdministratorId, ErrorCode.EID_ADMINISTRATOR_ID_CANNOT_BE_NULL);
        assertNotNull(eidAdministratorOfficeId, ErrorCode.ADMINISTRATOR_FRONT_OFFICE_ID_CANNOT_BE_NULL);

        EidAdministratorDTO eidAdministrator = raeiceiService.getEidAdministratorById(UUID.fromString(eidAdministratorId));
        entity.setEidAdministratorId(eidAdministrator.getId());

        if(eidAdministrator.getDeviceIds() != null) {
        	assertTrue(eidAdministrator.getDeviceIds().contains(device.getId()), DEVICE_NOT_SUPPORTED_BY_EID_ADMINISTRATOR);
        }

        EidManagerFrontOfficeDTO administratorFrontOffice = raeiceiService.getOfficeById(UUID.fromString(eidAdministratorOfficeId));
        entity.setAdministratorFrontOfficeId(administratorFrontOffice.getId());
        entity.getParams().setRegion(administratorFrontOffice.getRegion());
        log.info("Prepare new Application Number: {}-{}",eidAdministrator.getCode(),administratorFrontOffice.getCode());
        entity.setApplicationNumber(new ApplicationNumber(eidAdministrator.getCode(),administratorFrontOffice.getCode()));
        
    }

    private LevelOfAssurance findLevelOfAssurance() {
    //private LevelOfAssurance findLevelOfAssurance(String deviceType) {
    	return LevelOfAssurance.HIGH;
    	
//        return switch (deviceType) {
//            case Device.CHIP_CARD -> LevelOfAssurance.HIGH;
//            case Device.MOBILE, Device.OTHER -> LevelOfAssurance.SUBSTANTIAL;
//            default -> throw new ValidationMVRException(DEVICE_NOT_RECOGNIZED);
//        };
    }

    private String createApplicationXml(OnlineCertStatusApplicationRequest request, AbstractApplication application) {
        EidApplicationXml model = xmlMapper.map(request);
        model.setApplicationId(application.getId().toString());
        model.setLevelOfAssurance(application.getParams().getLevelOfAssurance().name());
        model.setEidAdministratorId(application.getEidAdministratorId().toString());
        model.setEidAdministratorOfficeId(application.getAdministratorFrontOfficeId().toString());
        DeviceDTO deviceType = raeiceiService.getDeviceById(application.getDeviceId());
        model.setDeviceId(deviceType.getId());
        return fileFormatService.createXmlStringFromObject(model);
    }

	@Override
	public boolean getActiveOnlineIssueApplicationsForChipCard(UUID citizenProfileId, UUID eidentityId, UUID currentApplicationId) {
		return this.abstractApplicationRepository.getActiveOnlineIssueApplicationsForChipCard(citizenProfileId, eidentityId, MVRConstants.DEVICE_CHIP_CARD_ID, currentApplicationId);
	}
}
