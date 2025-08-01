package bg.bulsi.mvr.mpozei.backend.rabbitmq;

import bg.bulsi.mvr.audit_logger.BaseAuditLogger;
import bg.bulsi.mvr.audit_logger.dto.AuditData;
import bg.bulsi.mvr.audit_logger.dto.EventPayload;
import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.audit_logger.model.MessageType;
import bg.bulsi.mvr.common.config.security.UserContext;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.crypto.CertificateProcessor;
import bg.bulsi.mvr.common.exception.BaseMVRException;
import bg.bulsi.mvr.common.exception.EntityNotFoundException;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.common.pipeline.Pipeline;
import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.rabbitmq.consumer.DefaultRabbitListener;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.common.service.FileFormatService;
import bg.bulsi.mvr.common.util.MVRConstants;
import bg.bulsi.mvr.mpozei.backend.client.MisepClient;
import bg.bulsi.mvr.mpozei.backend.client.PunClient;
import bg.bulsi.mvr.mpozei.backend.client.ReiClient;
import bg.bulsi.mvr.mpozei.backend.client.RueiClient;
import bg.bulsi.mvr.mpozei.backend.dto.*;
import bg.bulsi.mvr.mpozei.backend.mapper.*;
import bg.bulsi.mvr.mpozei.backend.pipeline.pipelines.application.ExportPipeline;
import bg.bulsi.mvr.mpozei.backend.pipeline.pipelines.application.issue_eid.SignIssueEidDeskPipeline;
import bg.bulsi.mvr.mpozei.backend.pipeline.pipelines.application.issue_eid.SignIssueEidOnlinePipeline;
import bg.bulsi.mvr.mpozei.backend.service.*;
import bg.bulsi.mvr.mpozei.backend.service.impl.ValidationService;
import bg.bulsi.mvr.mpozei.contract.dto.*;
import bg.bulsi.mvr.mpozei.contract.dto.report.ReportByOfficesRequest;
import bg.bulsi.mvr.mpozei.contract.dto.report.ReportByOperatorsRequest;
import bg.bulsi.mvr.mpozei.contract.dto.report.ReportByRegionRequest;
import bg.bulsi.mvr.mpozei.contract.dto.report.ReportTotalRequest;
import bg.bulsi.mvr.mpozei.contract.dto.xml.EidApplicationXml;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import bg.bulsi.mvr.mpozei.model.certificate.CertificateHistory;
import bg.bulsi.mvr.mpozei.model.nomenclature.NomenclatureType;
import bg.bulsi.mvr.mpozei.model.nomenclature.ReasonNomenclature;
import bg.bulsi.mvr.mpozei.model.opensearch.HtmlHelpPage;
import bg.bulsi.mvr.mpozei.model.pan.EventRegistratorImpl;
import bg.bulsi.mvr.mpozei.model.repository.view.ApplicationReportByOperators;
import bg.bulsi.mvr.pan_client.EventRegistrator;
import bg.bulsi.mvr.pan_client.EventRegistrator.Event;
import bg.bulsi.mvr.pan_client.NotificationSender;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceType;
import com.opencsv.CSVWriter;
import com.opencsv.ICSVWriter;

import jakarta.annotation.PostConstruct;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.amqp.support.converter.RemoteInvocationResult;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.http.HttpStatus;
import org.springframework.messaging.handler.annotation.Header;
import org.springframework.messaging.handler.annotation.Payload;
import org.springframework.stereotype.Component;
import org.springframework.transaction.annotation.Propagation;
import org.springframework.transaction.annotation.Transactional;

import java.io.IOException;
import java.io.StringWriter;
import java.security.NoSuchProviderException;
import java.security.cert.CertificateException;
import java.security.cert.X509Certificate;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.*;

import static bg.bulsi.mvr.common.exception.ErrorCode.*;
import static bg.bulsi.mvr.common.util.ValidationUtil.*;
import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType.*;

@Slf4j
@Component
@RequiredArgsConstructor
public class ListenerDispatcher {
    private static final String MVR_DEV_CA = "MVR DEV CA";
	private final ApplicationService applicationService;
    private final ApplicationMapper applicationMapper;
    private final CertificateMapper certificateMapper;
    private final NomenclatureMapper nomenclatureMapper;
    private final EidentityMapper eidentityMapper;
    private final ReportMapper reportMapper;
    private final ReiClient reiClient;
    private final RueiClient rueiClient;
    private final PunClient punClient;
    private final NomenclatureService nomenclatureService;
    private final CertificateService certificateService;
    private final FileFormatService fileFormatService;
    private final XmlMapper xmlMapper;
    private final ValidationService validationService;
    private final QrCodeService qrCodeService;
    private final EidentityService eidentityService;
    private final RaeiceiService raeiceiService;
    private final ReportService reportService;
    private final SoapService soapService;
    private final OpenSearchService openSearchService;
    private final HelpPageMapper helpPageMapper;
    private final MisepClient misepClient;
	private final BaseAuditLogger auditLogger;
	private final EventRegistrator eventRegistrator;
	private final NotificationSender notificationSender;
	private final CertificateProcessor certificateProcessor;
	private Event successfullPinChangeEvent;
	
    @DefaultRabbitListener(queues = {"mpozei.rpcqueue.POST.mpozei.api.v1.applications",
                                     "mpozei.rpcqueue.POST.mpozei.api.v1.external-administrators.applications"})
    @Transactional
    public RemoteInvocationResult createDeskApplication(@Valid @Payload DeskApplicationRequest request, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        AbstractApplication application = applicationService.createDeskApplication(request);
        
        this.logAudit(application, request, auditEventType, UserContextHolder.getFromServletContext());
        
        application = applicationService.processApplication(application);
        RemoteInvocationResult result = new RemoteInvocationResult(applicationMapper.mapToApplicationResponse(application));

        log.info(".createDeskApplication() [result={}]", result);
        return result;
    }

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.external.api.v1.applications.generate-xml")
    public RemoteInvocationResult generateApplicationXml(@Valid @Payload ApplicationXmlRequest request, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        EidApplicationXml model = xmlMapper.map(request);
        model.setApplicationId(UUID.randomUUID().toString());
        UserContext context = UserContextHolder.getFromServletContext();
        
        this.logAudit(request, request, auditEventType, UserContextHolder.getFromServletContext());

        this.validationService.validateGenerateXml(request);
        
        model.setCitizenProfileId(context.getCitizenProfileId());
        model.setEidentityId(context.getEidentityId());
        model.setCreateDate(LocalDateTime.now().format(DateTimeFormatter.ISO_DATE_TIME));
        
        String xml = fileFormatService.createXmlStringFromObject(model);
        return new RemoteInvocationResult(new ApplicationXmlResponse(xml));
    }

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.external.api.v1.applications.sign")
    public RemoteInvocationResult signOnlineApplication(@Valid @Payload SignOnlineApplicationRequest request, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        UUID applicationId = qrCodeService.validateOtpCode(request.getOtpCode());
        AbstractApplication application = applicationService.getAbstractApplicationById(applicationId);

        this.logAudit(application, null, auditEventType, UserContextHolder.getFromServletContext());
        
        ApplicationSubmissionType submissionType = application.getSubmissionType();
        assertTrue(List.of(BASE_PROFILE, DESK).contains(submissionType), SIGNS_DESK_AND_BASE_PROFILE_APPLICATIONS);
        assertTrue(!application.getParams().getIsOnlineOffice(), SIGN_IS_NOT_FOR_ONLINE_OFFICE);
        DeviceDTO device = raeiceiService.getDeviceById(application.getDeviceId());
        assertEquals(device.getType(), DeviceType.MOBILE, SIGNS_MOBILE_APPLICATIONS);
        if (application.getStatus() == ApplicationStatus.SIGNED) {
            throw new ValidationMVRException(ErrorCode.APPLICATION_IS_ALREADY_SIGNED, application.getId());
        }

        RueiVerifyProfileDTO dto = applicationMapper.map(request);
        dto.setCitizenProfileId(UUID.fromString(UserContextHolder.getFromServletContext().getCitizenProfileId()));
        rueiClient.validateCitizenProfile(dto);

        // mobileInstanceId, it is set when we create the application,and here again
        application.getParams().setMobileApplicationInstanceId(dto.getMobileApplicationInstanceId());
        
        Class<? extends Pipeline> pipeline = submissionType == DESK ? SignIssueEidDeskPipeline.class : SignIssueEidOnlinePipeline.class;
        applicationService.processApplication(application, ErrorCode.SIGN_APPLICATION_REJECTED, pipeline);
        return new RemoteInvocationResult(application.getStatus());
    }

//    @Transactional
//    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.external.api.v1.applications.eid-sign")
//    public RemoteInvocationResult signOnlineApplicationWithEid(@Valid @Payload SignOnlineApplicationEidRequest request) {
//        AbstractApplication application = applicationService.getAbstractApplicationById(request.getApplicationId());
//
//        applicationService.processApplication(application);
//        return new RemoteInvocationResult();
//    }

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.external.api.v1.applications")
    public RemoteInvocationResult createOnlineApplication(@Valid @Payload OnlineApplicationRequest request, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        AbstractApplication application = applicationService.createOnlineApplication(request);
        
        this.logAudit(application, null, auditEventType, UserContextHolder.getFromServletContext());
        
        application = applicationService.processApplication(application);
        RemoteInvocationResult result = new RemoteInvocationResult(applicationMapper.mapToOnlineApplicationResponse(application));

        log.info(".createOnlineApplication() [result={}]", result);
        return result;
    }

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.api.v1.applications.applicationid.complete")
    public RemoteInvocationResult completeIssueEidApplication(@Valid @Payload UUID id, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        AbstractApplication application = applicationService.getAbstractApplicationById(id);
        
        this.logAudit(application, null, auditEventType, UserContextHolder.getFromServletContext());
        
        applicationService.processApplication(application);
        return new RemoteInvocationResult(application.getStatus());
    }

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.external.api.v1.applications.applicationid.complete")
    public RemoteInvocationResult completeOnlineAplicationCertificateStatus(@Valid @Payload UUID id, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        AbstractApplication application = applicationService.getAbstractApplicationById(id);
        
        this.logAudit(application, null, auditEventType, UserContextHolder.getFromServletContext());
        
        applicationService.processApplication(application);
        return new RemoteInvocationResult(application.getStatus());
    }
    
//    @Transactional
//    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.external.api.v1.mobile.applications.applicationid.complete")
//    public RemoteInvocationResult completeIssueEidExternalApplication(@Valid @Payload UUID id) {
//        AbstractApplication application = applicationService.getAbstractApplicationById(id);
//        assertEquals(ApplicationSubmissionType.valueOf(application.getParam(APPLICATION_SUBMISSION_TYPE)), EID, "Application was not submitted using EID");
//        applicationService.processApplication(application);
//        return new RemoteInvocationResult(application.getStatus());
//    }

//    @Transactional
//    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.external.api.v1.mobile.applications.applicationid.complete")
//    public RemoteInvocationResult completeIssueEidExternalApplication(@Valid @Payload UUID id) {
//
//    }

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.api.v1.perso-centre.applications")
    public RemoteInvocationResult createPersoCentreApplication(@Valid @Payload PersoCentreApplicationRequest request, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        AbstractApplication application = applicationService.createPersoCentreApplication(request);
        
        this.logAudit(application, null, auditEventType, UserContextHolder.getFromServletContext());
        
        applicationService.processApplication(application);
        PersoCentreApplicationResponse response = applicationMapper.map(application);
        return new RemoteInvocationResult(response);
    }

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.api.v1.perso-centre.applications.confirm")
    public RemoteInvocationResult confirmPersoCentreApplication(@Valid @Payload PersoCentreConfirmApplicationRequest request, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        if (ApplicationConfirmationStatus.ERROR.equals(request.getStatus())) {
            ApplicationStatus status = applicationService.invalidatePersoCentreApplication(request);
            return new RemoteInvocationResult(HttpResponse.builder().statusCode(400).message(status.name()).build());
        }
        AbstractApplication application = applicationService.getAbstractApplicationById(request.getApplicationId());
        
        this.logAudit(application, null, auditEventType, UserContextHolder.getFromServletContext());
        
        applicationService.processApplication(application);
        return new RemoteInvocationResult(HttpResponse.builder().statusCode(200).message(application.getStatus().name()).build());
    }

//    @Transactional
//    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.api.v1.naif.certificates.update-status")
//    public RemoteInvocationResult updateCertificateStatusFromNaif(@Valid @Payload NaifCertificateStatusUpdateDTO dto) {
//        rueiClient.updateCertificateStatus()
//    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.api.v1.certificates.enroll")
    @Transactional
    public RemoteInvocationResult enrollForDeskCertificate(@Valid @Payload CertificateRequest request, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        AbstractApplication application = applicationService.getAbstractApplicationById(request.getApplicationId());
        
        Boolean isOnlineOffice = application.getParams().getIsOnlineOffice();
        assertTrue(!isOnlineOffice, ENROLLS_DESK_APPLICATIONS);
        DeviceDTO device = raeiceiService.getDeviceById(application.getDeviceId());
        assertEquals(device.getType(), DeviceType.CHIP_CARD, ENROLLS_CHIP_CARD_CERTIFICATES);
        application = applicationService.enrollForCertificate(applicationMapper.map(request));
        RemoteInvocationResult result = new RemoteInvocationResult(certificateMapper.mapToCertificateResponse(application));

        this.logAudit(application, request, auditEventType, UserContextHolder.getFromServletContext());
        
        log.info(".enrollForDeskCertificate() [result={}]", result);
        return result;
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.external.api.v1.mobile.certificates.enroll.base-profile")
    @Transactional
    public RemoteInvocationResult enrollForBaseProfileCertificate(@Valid @Payload MobileCertificateBasicProfileRequest request, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        EnrollCertificateDTO dto = applicationMapper.map(request);
        dto.setApplicationId(qrCodeService.validateOtpCode(request.getOtpCode()));
        dto.setCertificateAuthorityName(MVR_DEV_CA);
        
        AbstractApplication application = applicationService.getAbstractApplicationById(dto.getApplicationId());
        
        this.logAudit(application, null, auditEventType, UserContextHolder.getFromServletContext());
        
        assertTrue(List.of(DESK, BASE_PROFILE).contains(application.getSubmissionType()), ENROLLS_DESK_AND_BASE_PROFILE_APPLICATIONS);
        DeviceDTO device = raeiceiService.getDeviceById(application.getDeviceId());
        assertEquals(device.getType(), DeviceType.MOBILE, ENROLLS_MOBILE_CERTIFICATES);
        assertTrue(!application.getParams().getIsOnlineOffice(), CONFIRM_IS_NOT_FOR_ONLINE_OFFICE);

        application = applicationService.enrollForCertificate(dto);
        RemoteInvocationResult result = new RemoteInvocationResult(certificateMapper.mapToCertificateResponse(application));
        UserContextHolder.getFromServletContext().setTargetUserId(application.getEidentityId());
        log.info(".enrollForBaseProfileCertificate() [result={}]", result);
        return result;
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.external.api.v1.mobile.certificates.enroll.eid")
    @Transactional
    public RemoteInvocationResult enrollForEidCertificate(@Valid @Payload MobileCertificateEidRequest request, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        AbstractApplication application = applicationService.getAbstractApplicationById(request.getApplicationId());

        this.logAudit(application, null, auditEventType, UserContextHolder.getFromServletContext());
        
        assertTrue(application.getSubmissionType() == EID, ENROLLS_EID_APPLICATIONS);
        assertTrue(application.getParams().getIsOnlineOffice(), ENROLLS_ONLY_ONLINE_OFFICE);
        DeviceDTO device = raeiceiService.getDeviceById(application.getDeviceId());
        assertEquals(device.getType(), DeviceType.MOBILE, ENROLLS_MOBILE_CERTIFICATES);
        application = applicationService.enrollForCertificate(applicationMapper.map(request));
        RemoteInvocationResult result = new RemoteInvocationResult(certificateMapper.mapToCertificateResponse(application));
        UserContextHolder.getFromServletContext().setTargetUserId(application.getEidentityId());
        log.info(".enrollForEidCertificate() [result={}]", result);
        return result;
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.api.v1.external-administrators.certificates.enroll")
    @Transactional
    public RemoteInvocationResult enrollForExternalAdministratorsCertificate(@Valid @Payload CertificateExtAdminRequest request, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) throws CertificateException, NoSuchProviderException {
        AbstractApplication application = applicationService.getAbstractApplicationById(request.getApplicationId());

        Boolean isOnlineOffice = application.getParams().getIsOnlineOffice();
        assertTrue(!isOnlineOffice, ENROLLS_DESK_APPLICATIONS);
        
        DeviceDTO device = raeiceiService.getDeviceById(application.getDeviceId());
        assertNotEquals(device.getType(), DeviceType.CHIP_CARD, ENROLLS_NON_CHIP_CARD_CERTIFICATES);
        assertNotEquals(application.getEidAdministratorId(), MVRConstants.MVR_ADMINISTRATOR_ID, EID_ADMINISTRATOR_CANNOT_BE_MVR);
        
		X509Certificate parsedCertificate = this.certificateProcessor
				.extractCertificate(request.getClientCertificate().getBytes());
        
		boolean certificateExists = false;
		try {
			this.rueiClient.getCitizenCertificateByIssuerAndSN(parsedCertificate.getIssuerX500Principal().getName(), parsedCertificate.getSerialNumber().toString());
			certificateExists = true;
			//TODO: check how to catch specific error code
		} catch (BaseMVRException e) {
			certificateExists = false;
		}
		
		if(certificateExists) {
			throw new ValidationMVRException(ErrorCode.CERTIFICATE_IS_ALREADY_GENERATED, application.getId());
		}
		
        application = applicationService.enrollForExtAdminCertificate(request);
        RemoteInvocationResult result = new RemoteInvocationResult(certificateMapper.mapToCertificateResponse(application));

        this.logAudit(application, request, auditEventType, UserContextHolder.getFromServletContext());

        log.info(".enrollForDeskCertificate() [result={}]", result);
        return result;
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.GET.mpozei.api.v1.applications.applicationid.export")
    @Transactional
    public RemoteInvocationResult exportApplication(@Payload UUID applicationId, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        AbstractApplication application = applicationService.getAbstractApplicationById(applicationId);
        
        this.logAudit(application, null, auditEventType, UserContextHolder.getFromServletContext());
        
        if (ApplicationStatus.PENDING_PAYMENT.equals(application.getStatus())) {
            throw new ValidationMVRException(APPLICATION_NOT_PAID);
        }
        applicationService.processApplication(application, ErrorCode.NO_VALID_PIPELINES_EXIST ,ExportPipeline.class);

        return new RemoteInvocationResult(application.getTemporaryData().getApplicationExportResponse());
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.GET.mpozei.api.v1.applications.applicationid.export-confirmation")
    @Transactional
    public RemoteInvocationResult exportConfirmationApplication(@Payload UUID applicationId) {
        AbstractApplication application = applicationService.getAbstractApplicationById(applicationId);
        byte[] result = applicationService.exportConfirmationApplication(application);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.api.v1.applications.applicationid.status")
    @Transactional
    public RemoteInvocationResult updateApplicationStatus(@Valid @Payload Map<String, String> payload, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        UUID id = UUID.fromString(payload.get("id"));
        ApplicationStatus status = ApplicationStatus.valueOf(payload.get("status"));
        //asdfasdf
        ApplicationStatus response = applicationService.updateApplicationStatus(id, status);
        return new RemoteInvocationResult(response);
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.api.v1.applications.applicationid.sign")
    @Transactional
    public RemoteInvocationResult signApplication(@Valid @Payload Map<String, Object> payload, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        UUID id = (UUID) payload.get("applicationId");
        ApplicationSignRequest applicationSignRequest = (ApplicationSignRequest) payload.get("applicationSignRequest");

        AbstractApplication application = applicationService.getAbstractApplicationById(id);
        
        this.logAudit(application, applicationSignRequest, auditEventType, UserContextHolder.getFromServletContext());
        
        assertTrue(!application.getParams().getIsOnlineOffice(), SIGN_IS_NOT_FOR_ONLINE_OFFICE);
        if (application.getApplicationType() == ApplicationType.ISSUE_EID) {
            DeviceDTO device = raeiceiService.getDeviceById(application.getDeviceId());
            assertEquals(device.getType(), DeviceType.CHIP_CARD, SIGNS_CHIP_CARD_APPLICATIONS);
        }
        if (application.getApplicationType() != ApplicationType.ISSUE_EID && application.getStatus() == ApplicationStatus.SIGNED) {
            throw new ValidationMVRException(ErrorCode.APPLICATION_IS_ALREADY_SIGNED, application.getId());
        }

        applicationMapper.map(application, applicationSignRequest);
        application = application.getApplicationType() == ApplicationType.ISSUE_EID ? applicationService.processApplication(application, ErrorCode.NO_VALID_PIPELINES_EXIST , SignIssueEidDeskPipeline.class)
                : applicationService.processApplication(application);

        UserContextHolder.getFromServletContext().setTargetUserId(application.getEidentityId());

        return new RemoteInvocationResult(application.getStatus());
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.api.v1.applications.applicationid.deny")
    @Transactional
    public RemoteInvocationResult denyApplication(@Valid @Payload DenyApplicationDTO dto) {
        ApplicationStatus result = applicationService.denyApplication(dto);
        
        //asd
        
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.GET.mpozei.api.v1.applications.find")
    @Transactional(readOnly = true, propagation = Propagation.REQUIRES_NEW)
    public RemoteInvocationResult findApplications(@Valid @Payload ApplicationFilter filter) {
        log.info(".findApplications() [filter={}]", filter);

        Page<ApplicationDTO> result = applicationMapper.mapToApplicationDTOPage(applicationService.findApplications(filter, filter.getPageable()));

        result.get().findFirst().ifPresent(e -> {
            UserContextHolder.getFromServletContext().setTargetUserId(e.getEidentityId());
        });

        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.GET.mpozei.api.v1.applications.admin-find")
    @Transactional(readOnly = true, propagation = Propagation.REQUIRES_NEW)
    public RemoteInvocationResult adminFindApplications(@Valid @Payload ApplicationFilter filter) {
        log.info(".adminFindApplications() [filter={}]", filter);

        Page<ApplicationDTO> result = applicationMapper.mapToApplicationDTOPage(applicationService.findApplications(filter, filter.getPageable()));

        result.get().findFirst().ifPresent(e -> {
            UserContextHolder.getFromServletContext().setTargetUserId(e.getEidentityId());
        });

        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.GET.mpozei.api.v1.eidentities.find")
    @Transactional
    public RemoteInvocationResult findEidentityFieldInternal(@Valid @Payload Map<String, Object> payload) {
        UUID citizenProfileId = (UUID) payload.get("citizenProfileId");
        String email = (String) payload.get("email");
        String citizenIdentifierNumber = (String) payload.get("citizenIdentifierNumber");
        IdentifierType citizenIdentifierType = (IdentifierType) payload.get("citizenIdentifierType");
        
        EidentityDTO eidentity = null;
        CitizenProfileDTO citizenProfile = null;
        //Find by citizenIdentifierNumber and citizenIdentifierType
    	if(citizenIdentifierNumber != null && citizenIdentifierType != null) {
	        this.validationService.validateCitizenIdentifierAndType(citizenIdentifierNumber, citizenIdentifierType.name());
	        
	        eidentity = reiClient.findEidentityByNumberAndTypeInternal(citizenIdentifierNumber, citizenIdentifierType);
	        citizenProfile = null;
	        try {
	            citizenProfile = rueiClient.getCitizenProfileByEidentityId(eidentity.getId());
	        } catch (BaseMVRException ex) {
	            log.info(ex.getMessage());
	        }
	    //Find by citizenProfileId
    	} else if (citizenProfileId != null) {
    		citizenProfile = rueiClient.getCitizenProfileById(citizenProfileId);
    		if(citizenProfile.getEidentityId() != null) {
    			eidentity = reiClient.getEidentityById(citizenProfile.getEidentityId());
    		}
    	} else {
    	//Find by email
    		citizenProfile = rueiClient.getCitizenProfileByEmail(email);
    		if(citizenProfile.getEidentityId() != null) {
    			eidentity = reiClient.getEidentityById(citizenProfile.getEidentityId());
    		}
    	}
    	
        EidentityResponse response = eidentityMapper.map(eidentity, citizenProfile);

        UserContextHolder.getFromServletContext().setTargetUserId(eidentity != null ? eidentity.getId() : null);

        return new RemoteInvocationResult(response);
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.GET.mpozei.api.v1.eidentities.id")
    @Transactional
    public RemoteInvocationResult findEidentityById(@Valid @Payload UUID id) {
        EidentityDTO eidentity = reiClient.getEidentityByIdInternal(id);
        CitizenProfileDTO citizenProfile = null;
        try {
            citizenProfile = rueiClient.getCitizenProfileByEidentityId(eidentity.getId());
        } catch (Exception ignored) {
            log.info("Citizen Profile not found by eidentityId");
        }
        EidentityResponse response = eidentityMapper.map(eidentity, citizenProfile);

        UserContextHolder.getFromServletContext().setTargetUserId(id);

        return new RemoteInvocationResult(response);
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.GET.mpozei.external.api.v1.eidentities")
    @Transactional
    public RemoteInvocationResult findEidentityByIdExternal(UUID citizenProfileId) {
    	CitizenProfileDTO citizenProfile = rueiClient.getCitizenProfileById(citizenProfileId);

        EidentityDTO eidentity = null;
        if (Objects.nonNull(citizenProfile.getEidentityId())) {
            eidentity = reiClient.getEidentityById(citizenProfile.getEidentityId());
        }
        EidentityExternalResponse response = eidentityMapper.mapToEidentityExternalResponse(eidentity, citizenProfile);

        return new RemoteInvocationResult(response);
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.GET.mpozei.api.v1.certificates.id")
    @Transactional
    public RemoteInvocationResult getCertificateById(@Valid @Payload UUID certificateId) {
        CitizenCertificateSummaryResponse result = certificateService.getCertificateById(certificateId);
        List<CertificateHistory> histories = certificateService.getCertificateHistoryByCertificateId(certificateId);
        if (!histories.isEmpty()) {
            CertificateHistory last = histories.get(0);
            result.setDeviceId(last.getDeviceId());
            result.setApplicationNumber(last.getApplicationNumber());
            result.setReasonId(last.getReasonId());
            result.setReasonText(last.getReasonText());
        }
        
        List<PunCarrierDTO> punCarriers = punClient.getPunCarrierByEidentityIdAndCertificateId(result.getEidentityId(), certificateId);
        punCarriers.sort((o1, o2) -> o2.getModifiedOn().compareTo(o1.getModifiedOn()));
        
        if(!punCarriers.isEmpty()) {
        	result.setCarrierSerialNumber(punCarriers.get(0).getSerialNumber());
        }
        
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.GET.mpozei.api.v1.certificates.find")
    @Transactional
    public RemoteInvocationResult findCitizenCertificates(CitizenCertificateFilter filter) {
        log.info(".findCitizenCertificates() [filter={}]", filter);

        Page<FindCertificateResponse> result = this.certificateService.findCitizenCertificates(filter);

        result.get().findFirst().ifPresent(e -> {
            UserContextHolder.getFromServletContext().setTargetUserId(e.getEidentityId());
        });

        log.info(".findCitizenCertificates() [result={}]", result);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.GET.mpozei.api.v1.nomenclatures.reasons")
    @Transactional
    public RemoteInvocationResult getAllReasons() {
        List<ReasonNomenclature> reasons = nomenclatureService.getAllReasons();
        //Get distinct Nomenclature Ids from reasons
        List<UUID> reasonsNomTypeIds = reasons.stream().map(r -> r.getNomCode().getId()).distinct().toList();
        List<NomenclatureType> nomenclatures = nomenclatureService.getAllNomenclatureTypesByIds(reasonsNomTypeIds);
        List<NomenclatureTypeDTO> result = nomenclatureMapper.mapToNomenclatures(nomenclatures, reasons);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.GET.mpozei.api.v1.applications.id")
    @Transactional
    public RemoteInvocationResult getApplicationById(@Valid @Payload UUID id, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        AbstractApplication application = applicationService.getAbstractApplicationById(id);
        
        this.logAudit(application, null, auditEventType, UserContextHolder.getFromServletContext());
        
        ApplicationDetailsResponse result = applicationMapper.mapToDetailsResponse(application);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.GET.mpozei.external.api.v1.applications.id")
    @Transactional
    public RemoteInvocationResult getApplicationByIdExternal(@Valid @Payload UUID id, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        assertNotNull(UserContextHolder.getFromServletContext().getCitizenProfileId(), CITIZEN_PROFILE_ID_CANNOT_BE_NULL);
        AbstractApplication application = applicationService.getAbstractApplicationById(id);
        
        this.logAudit(application, null, auditEventType, UserContextHolder.getFromServletContext());
        
        UUID citizenProfileId = UUID.fromString(UserContextHolder.getFromServletContext().getCitizenProfileId());
        if (!Objects.equals(citizenProfileId, application.getCitizenProfileId())) {
            log.error("Requester doesnt match with application owner");
            throw new EntityNotFoundException(ErrorCode.APPLICATION_NOT_FOUND, id);
        }
        ApplicationDetailsExternalResponse result = applicationMapper.mapToExtDetailsResponse(application);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.GET.mpozei.api.v1.certificates.id.history")
    @Transactional
    public RemoteInvocationResult getCertificateHistoryByCertificateId(@Valid @Payload UUID certificateId) {
        List<CertificateHistory> certificateHistories = certificateService.getCertificateHistoryByCertificateId(certificateId);
        List<CertificateHistoryDTO> result = certificateMapper.map(certificateHistories);
        return new RemoteInvocationResult(result);
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.external.api.v1.mobile.certificates.confirm")
    @Transactional
    public RemoteInvocationResult confirmMobileCertificateStorage(@Valid @Payload CertificateBasicProfileConfirmationRequest request, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        UUID applicationId = qrCodeService.validateOtpCode(request.getOtpCode());
        CertificateConfirmDTO dto = certificateMapper.map(request);
        dto.setApplicationId(applicationId);
        AbstractApplication application = applicationService.getAbstractApplicationById(applicationId);
        
        this.logAudit(application, request, auditEventType, UserContextHolder.getFromServletContext());
        
        DeviceDTO device = raeiceiService.getDeviceById(application.getDeviceId());
        assertTrue(DeviceType.MOBILE.equals(device.getType()), CONFIRMS_MOBILE_STORAGE);
        assertTrue(List.of(BASE_PROFILE, DESK).contains(application.getSubmissionType()), CONFIRMS_DESK_AND_BASE_PROFILE_APPLICATIONS);
        assertTrue(!application.getParams().getIsOnlineOffice(), CONFIRM_IS_NOT_FOR_ONLINE_OFFICE);

        ApplicationStatus status = applicationService.confirmCertificateStorage(dto);
        return new RemoteInvocationResult(status);
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.external.api.v1.mobile.certificates.confirm.eid")
    @Transactional
    public RemoteInvocationResult confirmMobileCertificateStorageWithEid(@Valid @Payload CertificateEidConfirmationRequest request, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        CertificateConfirmDTO dto = certificateMapper.map(request);
        AbstractApplication application = applicationService.getAbstractApplicationById(request.getApplicationId());
        
        this.logAudit(application, request, auditEventType, UserContextHolder.getFromServletContext());
        
        DeviceDTO device = raeiceiService.getDeviceById(application.getDeviceId());
        assertTrue(DeviceType.MOBILE.equals(device.getType()), CONFIRMS_MOBILE_STORAGE);
        assertTrue(application.getSubmissionType() == EID, CONFIRMS_EID_APPLICATIONS);
        assertTrue(application.getParams().getIsOnlineOffice(), CONFIRMS_ONLY_ONLINE_OFFICE);

        applicationService.confirmCertificateStorage(dto);
        application = applicationService.processApplication(application);
        return new RemoteInvocationResult(application.getStatus());
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.api.v1.certificates.confirm")
    @Transactional
    public RemoteInvocationResult confirmDeskCertificateStorage(@Valid @Payload CertificateEidConfirmationRequest request, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        CertificateConfirmDTO dto = certificateMapper.map(request);
        AbstractApplication application = applicationService.getAbstractApplicationById(request.getApplicationId());
        
        this.logAudit(application, request, auditEventType, UserContextHolder.getFromServletContext());
        
        assertTrue(!application.getParams().getIsOnlineOffice(), CONFIRM_IS_NOT_FOR_ONLINE_OFFICE);

        ApplicationStatus status = applicationService.confirmCertificateStorage(dto);
        return new RemoteInvocationResult(status);
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.external.api.v1.eidentities.attach-to-profile.citizenprofileid")
    @Transactional
    public RemoteInvocationResult attachEidentityToProfile(@Valid @Payload UUID citizenProfileId) {
        assertNotNull(UserContextHolder.getFromServletContext().getEidentityId(), USER_ID_CANNOT_BE_NULL);
        EidentityDTO eidentity = reiClient.getEidentityById(UUID.fromString(UserContextHolder.getFromServletContext().getEidentityId()));
        CitizenProfileAttachDTO citizenProfileAttachDTO = eidentityMapper.map(eidentity);
        citizenProfileAttachDTO.setCitizenProfileId(citizenProfileId);
        rueiClient.attachCitizenProfile(citizenProfileAttachDTO);
        applicationService.attachCitizenProfileIdToApplications(eidentity.getId(), citizenProfileId);
        return new RemoteInvocationResult(HttpResponse.builder().message("OK").statusCode(200).build());
    }

//    @DefaultRabbitListener(queues = "mpozei.rpcqueue.GET.mpozei.external.api.v1.citizens")
//    @Transactional
//    public RemoteInvocationResult getCitizenProfileById() {
//        CitizenProfileResponse dto = rueiClient.getCitizenProfileById();
//        return new RemoteInvocationResult(dto);
//    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.PUT.mpozei.external.api.v1.citizens")
    @Transactional
    public RemoteInvocationResult updateCitizenProfile(@Valid @Payload CitizenProfileUpdateRequest request) {
        String citizenProfileId = UserContextHolder.getFromServletContext().getCitizenProfileId();
        assertNotNull(citizenProfileId, CITIZEN_PROFILE_ID_CANNOT_BE_NULL);
        String message = rueiClient.updateCitizenProfile(UUID.fromString(citizenProfileId), request);
        return new RemoteInvocationResult(HttpResponse.builder().message(message).statusCode(200).build());
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.external.api.v1.citizens.register")
    @Transactional
    public RemoteInvocationResult registerCitizenProfile(@Valid @Payload CitizenProfileRegistrationDTO dto) {
        String message = rueiClient.registerCitizenProfile(dto);
        return new RemoteInvocationResult(HttpResponse.builder().message(message).statusCode(200).build());
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.PUT.mpozei.external.api.v1.citizens.register")
    @Transactional
    public RemoteInvocationResult verifyCitizenProfileRegistration(@Valid @Payload UUID token) {
        String message = rueiClient.verifyCitizenProfile(token);
        return new RemoteInvocationResult(HttpResponse.builder().message(message).statusCode(200).build());
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.external.api.v1.citizens.update-password")
    @Transactional
    public RemoteInvocationResult updateCitizenProfilePassword(@Valid @Payload UpdateProfilePasswordDTO dto) {
        String message = rueiClient.updateCitizenProfilePassword(dto.getOldPassword(), dto.getNewPassword());
        return new RemoteInvocationResult(HttpResponse.builder().message(message).statusCode(200).build());
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.external.api.v1.citizens.forgotten-password")
    @Transactional
    public RemoteInvocationResult forgottenProfilePassword(@Valid @Payload ForgottenPasswordDTO dto) {
        String message = rueiClient.forgottenPassword(dto);
        return new RemoteInvocationResult(HttpResponse.builder().message(message).statusCode(200).build());
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.external.api.v1.citizens.update-email")
    @Transactional
    public RemoteInvocationResult updateCitizenProfileEmail(@Valid @Payload String email) {
        return new RemoteInvocationResult(rueiClient.updateCitizenProfileEmail(email));
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.external.api.v1.citizens.update-email.confirm")
    @Transactional
    public RemoteInvocationResult confirmUpdateCitizenProfileEmail(@Valid @Payload UUID token) {
        return new RemoteInvocationResult(rueiClient.confirmUpdateCitizenProfileEmail(token));
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.external.api.v1.citizens.reset-password")
    @Transactional
    public RemoteInvocationResult resetProfilePassword(@Valid @Payload ResetPaswordDTO dto) {
        String message = rueiClient.resetPassword(dto.getToken(), dto.getPassword());
        return new RemoteInvocationResult(HttpResponse.builder().message(message).statusCode(200).build());
    }

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.api.v1.applications.applicationid.add-guardians")
    public RemoteInvocationResult addGuardiansToMobileApplication(@Valid @Payload HashMap<String, Object> body, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        ApplicationAddGuardiansRequest request = (ApplicationAddGuardiansRequest) body.get("request");
        UUID applicationId = UUID.fromString(body.get("applicationId").toString());
        AbstractApplication application = applicationService.getAbstractApplicationById(applicationId);
        
        this.logAudit(application, request, auditEventType, UserContextHolder.getFromServletContext());
        
        assertTrue(application.getPipelineStatus() == PipelineStatus.EXPORT_APPLICATION, APPLICATION_NOT_IN_REQUIRED_PIPELINE_STATUS);
//        DeviceDTO device = raeiceiService.getDeviceById(application.getDeviceId());
//        assertTrue(device.getType().equals(Device.MOBILE), ONLY_MOBILE_APPLICATION_SETS_GUARDIANS);
//        assertTrue(application.getParams().getRequireGuardians(), APPLICATION_DOES_NOT_REQUIRE_GUARDIANS);

        application.getTemporaryData().setGuardians(request.getGuardians());
        this.validationService.validateGuardiansOnlineApplication(application);

        EidApplicationXml eidApplicationXml = fileFormatService.createObjectFromXmlString(application.getApplicationXml(), EidApplicationXml.class);
        eidApplicationXml.setGuardians(applicationMapper.mapToGuardianDetailsXml(request.getGuardians()));
        String xmlWithGuardians = this.fileFormatService.createXmlStringFromObject(eidApplicationXml);
        application.setApplicationXml(xmlWithGuardians);
        applicationService.save(application);
        return new RemoteInvocationResult(HttpStatus.OK.name());
    }

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.external.api.v1.applications.certificate-status-change.plain")
    public RemoteInvocationResult createOnlineCertStatusChangeApplicationPlain(@Valid @Payload OnlineCertStatusApplicationRequest request, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        AbstractApplication application = applicationService.createOnlineCertStatusApplicationPlain(request);
        
        this.logAudit(application, request, auditEventType, UserContextHolder.getFromServletContext());

        application = applicationService.processApplication(application);
        RemoteInvocationResult result = new RemoteInvocationResult(applicationMapper.mapToOnlineApplicationResponse(application));

        log.info(".createOnlineCertStatusChangeApplication() [result={}]", result);
        return result;
    }

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.external.api.v1.applications.certificate-status-change.signed")
    public RemoteInvocationResult createOnlineCertStatusChangeApplicationSigned(@Valid @Payload OnlineApplicationRequest request, @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType) {
        AbstractApplication application = applicationService.createOnlineCertStatusApplicationSigned(request);
        
        this.logAudit(application, request, auditEventType, UserContextHolder.getFromServletContext());
        
        application = applicationService.processApplication(application);
        RemoteInvocationResult result = new RemoteInvocationResult(applicationMapper.mapToOnlineApplicationResponse(application));

        log.info(".createOnlineCertStatusChangeApplication() [result={}]", result);
        return result;
    }

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.external.api.v1.mobile.verify-login")
    public RemoteInvocationResult verifyProfileLogin(@Valid @Payload ProfileVerifyLoginRequest request) {
    	//TODO: Save also in application.params
        RueiVerifyProfileDTO dto = applicationMapper.map(request);
        dto.setCitizenProfileId(UUID.fromString(UserContextHolder.getFromServletContext().getCitizenProfileId()));
        rueiClient.validateCitizenProfile(dto);
        return new RemoteInvocationResult(HttpResponse.builder().statusCode(200).message("OK").build());
    }

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.api.v1.naif.certificates.update-status")
    public RemoteInvocationResult updateCertificateStatusByNaif(@Valid @Payload NaifUpdateCertificateStatusDTO dto) {
        NaifUpdateCertificateStatusResponse result = certificateService.updateCertificateStatusByNaif(dto);
        return new RemoteInvocationResult(result);
    }

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.api.v1.naif.certificates.history")
    public RemoteInvocationResult getDeviceHistoryByNaif(@Valid @Payload NaifDeviceHistoryRequest request) {
        NaifDeviceHistoryResponse result = certificateService.getDeviceHistoryByNaif(request);
        return new RemoteInvocationResult(result);
    }

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.api.v1.naif.certificates.activate")
    public RemoteInvocationResult activateDeliveredCertificateByNaif(@Valid @Payload NaifDeliveredCertificateDTO dto) {
        NaifDeliveredCertificateResponse response = certificateService.activateCertificateByNaif(dto);
        return new RemoteInvocationResult(response);
    }

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.api.v1.naif.eidentities.invalidate-eid")
    public RemoteInvocationResult invalidateCitizenEidByNaif(@Valid @Payload InvalidateCitizenEidDTO dto) {
        NaifInvalidateEidResponse response = eidentityService.invalidateCitizenEidByNaif(dto);
        return new RemoteInvocationResult(response);
    }

	/*
	 * @Transactional
	 * 
	 * @DefaultRabbitListener(queues =
	 * "mpozei.rpcqueue.POST.mpozei.api.v1.naif.citizens.update-citizen-identifier")
	 * public RemoteInvocationResult updateCitizenIdentifierByNaif(@Valid @Payload
	 * UpdateCitizenIdentifierDTO dto) { NaifUpdateCitizenIdentifierResponse
	 * response = eidentityService.updateCitizenIdentifier(dto); return new
	 * RemoteInvocationResult(response); }
	 */

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.GET.mpozei.api.v1.applications.reports.by-administrator")
    public RemoteInvocationResult getApplicationsReportByOffices(@Valid @Payload ReportByOfficesRequest request) {
        List<List<String>> result = reportService.getApplicationsReportByOffices(request);
        return new RemoteInvocationResult(result);
    }

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.GET.mpozei.api.v1.applications.reports.by-region")
    public RemoteInvocationResult getApplicationsReportByRegion(@Valid @Payload ReportByRegionRequest request) {
        List<List<String>> result = reportService.getApplicationsReportByRegion(request);
        return new RemoteInvocationResult(result);
    }

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.GET.mpozei.api.v1.applications.reports.total")
    public RemoteInvocationResult getApplicationsReportTotal(@Valid @Payload ReportTotalRequest request) {
        List<List<String>> result = reportService.getApplicationsReportTotal(request);
        return new RemoteInvocationResult(result);
    }

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.GET.mpozei.api.v1.applications.reports.ui.by-operators")
    public RemoteInvocationResult getJsonApplicationsReportByOperators(@Valid @Payload ReportByOperatorsRequest request) {
        List<ApplicationReportByOperators> list = reportService.getJsonApplicationsReportByOperators(request);
        List<ApplicationReportByOperatorsDTO> result = reportMapper.mapToDtoList(list);
        return new RemoteInvocationResult(result);
    }

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.GET.mpozei.api.v1.applications.reports.csv.by-operators")
    public RemoteInvocationResult getCsvApplicationsReportByOperators(@Valid @Payload ReportByOperatorsRequest request) throws IOException {
        List<String[]> data = reportService.getCsvApplicationsReportByOperators(request);
        String result = generateCsv(data);
        return new RemoteInvocationResult(result);
    }

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.GET.mpozei.api.v1.applications.reports.csv.find-applications")
    public RemoteInvocationResult getCsvFindApplications(@Valid @Payload ApplicationFilter filter) throws IOException {
        List<String[]> data = reportService.getCsvFindApplications(filter);
        String result = this.generateCsv(data);
        return new RemoteInvocationResult(result);
    }
    
    
    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.api.v1.device.puk")
    public RemoteInvocationResult getDevicePuk(@Valid @Payload DevicePukRequest request) {
        byte[] result = soapService.prepareDataAsByteArray(request.getChipSerialNumber(), request.getHolderId());
        return new RemoteInvocationResult(Base64.getEncoder().encodeToString(result));
    }

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.POST.mpozei.api.v1.help-pages")
    public RemoteInvocationResult createHelpPage(@Valid @Payload HelpPageDTO dto) {
        HtmlHelpPage entity = helpPageMapper.map(dto);
        entity = openSearchService.createHelpPage(entity);
        HelpPageDTO response = helpPageMapper.map(entity);
        return new RemoteInvocationResult(response);
    }

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.PUT.mpozei.api.v1.help-pages")
    public RemoteInvocationResult updateHelpPage(@Valid @Payload HelpPageDTO dto) {
        HtmlHelpPage entity = openSearchService.updateHelpPage(dto);
        HelpPageDTO response = helpPageMapper.map(entity);
        return new RemoteInvocationResult(response);
    }

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.GET.mpozei.api.v1.help-pages.find")
    public RemoteInvocationResult findHelpPages(@Valid @Payload FindHelpPagesFilter filter) {
        Page<HtmlHelpPage> entities = openSearchService.findHelpPagesByFilter(filter);
        Page<HelpPageDTO> response = helpPageMapper.mapAll(entities);
        return new RemoteInvocationResult(response);
    }

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.GET.mpozei.api.v1.help-pages.find-all")
    public RemoteInvocationResult getAllHelpPages() {
        List<HtmlHelpPage> entities = openSearchService.getAllHelpPages();
        List<HelpPageDTO> response = helpPageMapper.mapAll(entities);
        return new RemoteInvocationResult(response);
    }

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.GET.mpozei.api.v1.help-pages.id")
    public RemoteInvocationResult getHelpPageById(@Valid @Payload String id) {
        HtmlHelpPage entity = openSearchService.getHelpPageById(id);
        HelpPageDTO response = helpPageMapper.map(entity);
        return new RemoteInvocationResult(response);
    }

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.GET.mpozei.external.api.v1.help-pages.name.pagename")
    public RemoteInvocationResult getHelpPageByPageName(@Valid @Payload String pageName) {
        List<HtmlHelpPage> entities = openSearchService.getHelpPagesByPageName(pageName);
        List<HelpPageDTO> response = entities.stream().map(helpPageMapper::map).toList();
        return new RemoteInvocationResult(response);
    }

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.DELETE.mpozei.api.v1.help-pages.id.delete")
    public RemoteInvocationResult deleteHelpPageById(@Valid @Payload String id) {
        openSearchService.deleteHelpPageById(id);
        return new RemoteInvocationResult(HttpResponse.builder().message("SUCCESS").statusCode(200).build());
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.PUT.mpozei.external.api.v1.certificates.alias")
    public RemoteInvocationResult updateCertificateAlias(@Payload Map<String, String> payload) {
        UUID certificateId = UUID.fromString(payload.get("certificateId"));
        String alias = payload.get("alias");
        rueiClient.updateCitizenCertificateAlias(certificateId, alias);
        return new RemoteInvocationResult(HttpResponse.builder().statusCode(200).message("OK").build());
    }

    @DefaultRabbitListener(queues = "mpozei.rpcqueue.GET.mpozei.external.api.v1.certificates.public-find")
    public RemoteInvocationResult publicFindCertificatesInfo(@Payload Map<String, Object> payload) {
        String keyword = payload.get("keyword").toString();
        Pageable pageable = (Pageable) payload.get("pageable");
        Page<RueiPublicCertificateInfo> rueiCertificateInfos = rueiClient.getPublicCertificateInfos(keyword, pageable);
        Page<PublicCertificateInfo> certificateInfos = rueiCertificateInfos.map(certificateMapper::map);
        return new RemoteInvocationResult(certificateInfos);
    }

    @Transactional
    @DefaultRabbitListener(queues = "mpozei.rpcqueue.GET.mpozei.external.api.v1.payments")
    public RemoteInvocationResult getAllPayments() {
        List<MisepPaymentDTO> payments = misepClient.getAllPaymentRequests(UserContextHolder.getFromServletContext().getCitizenProfileId());
        return new RemoteInvocationResult(payments);
    }

    @Transactional
    @DefaultRabbitListener(queues = {"mpozei.rpcqueue.POST.mpozei.api.v1.device.log",
    								 "mpozei.rpcqueue.POST.mpozei.external.api.v1.device.log"})
    public RemoteInvocationResult sumbitDeviceLog(UUID eid) {
    	this.notificationSender.send(successfullPinChangeEvent.code(), eid);
    	
    	return new RemoteInvocationResult(HttpStatus.OK.getReasonPhrase());
    }
    
	/*
	 * @Transactional
	 * 
	 * @DefaultRabbitListener(queues =
	 * "mpozei.rpcqueue.GET.mpozei.external.api.v1.payments.paymentrequestid.status")
	 * public RemoteInvocationResult getPaymentById(@Valid @Payload Map<String,
	 * Object> payload) { String citizenProfileId =
	 * payload.get("citizenProfileId").toString(); String paymentRequestId =
	 * payload.get("paymentRequestId").toString(); String status =
	 * misepClient.getPaymentStatusByPaymentId(paymentRequestId, citizenProfileId);
	 * return new RemoteInvocationResult(new MisepPaymentStatusDTO(status)); }
	 */

    public String generateCsv(List<String[]> data) throws IOException {
        StringWriter stringWriter = new StringWriter();
        try (CSVWriter writer = new CSVWriter(stringWriter,
                ';',
                ICSVWriter.NO_QUOTE_CHARACTER,
                ICSVWriter.DEFAULT_ESCAPE_CHARACTER,
                ICSVWriter.DEFAULT_LINE_END)) {
            writer.writeAll(data);
        }

        return stringWriter.toString();
    }
    
    private void logAudit(BaseApplicationRequest application, Object request, AuditEventType auditEventType, UserContext userContext) {
    	EventPayload eventPayloadReq = new EventPayload();
		eventPayloadReq.setRequesterUid(userContext.getCitizenIdentifier());
		eventPayloadReq.setRequesterUidType(userContext.getCitizenIdentifierType());
		eventPayloadReq.setRequesterName(userContext.getName());
		eventPayloadReq.setTargetUid(application.getCitizenIdentifierNumber());
		eventPayloadReq.setTargetUidType(application.getCitizenIdentifierType().name());
		eventPayloadReq.setTargetName(application.getFirstName(), application.getSecondName(), application.getLastName());
		eventPayloadReq.setRequestBody(request);
		
		this.auditLogger.auditEvent(AuditData.builder()
				.correlationId(userContext.getGlobalCorrelationId().toString())
				.eventType(auditEventType)
				.messageType(MessageType.REQUEST)
				.requesterUserId(userContext.getRequesterUserId())
				.requesterSystemId(userContext.getSystemId())
				.requesterSystemName(userContext.getSystemName())
//				.targetUserId(application.getEidentityId() != null ? application.getEidentityId().toString() : null)
				.payload(eventPayloadReq)
				.build());
    }
    
    // @Header("AUDIT_EVENT_TYPE") AuditEventType auditEventType
    private void logAudit(AbstractApplication application, Object request, AuditEventType auditEventType, UserContext userContext) {
    	EventPayload eventPayloadReq = new EventPayload();
		eventPayloadReq.setRequesterUid(userContext.getCitizenIdentifier());
		eventPayloadReq.setRequesterUidType(userContext.getCitizenIdentifierType());
		eventPayloadReq.setRequesterName(userContext.getName());
		eventPayloadReq.setTargetUid(application.getCitizenIdentifierNumber());
		eventPayloadReq.setTargetUidType(application.getCitizenIdentifierType().name());
		eventPayloadReq.setTargetName(application.getFirstName(), application.getSecondName(), application.getLastName());
		eventPayloadReq.setApplicationId(application.getId().toString());
		eventPayloadReq.setApplicationStatus(application.getStatus().name());
		eventPayloadReq.setRequestBody(request);
		
		this.auditLogger.auditEvent(AuditData.builder()
				.correlationId(userContext.getGlobalCorrelationId().toString())
				.eventType(auditEventType)
				.messageType(MessageType.REQUEST)
				.requesterUserId(userContext.getRequesterUserId())
				.requesterSystemId(userContext.getSystemId())
				.requesterSystemName(userContext.getSystemName())
				.targetUserId(application.getEidentityId() != null ? application.getEidentityId().toString() : null)
				.payload(eventPayloadReq)
				.build());
    }
    
    @PostConstruct
    private void init() {
    	this.successfullPinChangeEvent = eventRegistrator.getEvent(EventRegistratorImpl.MPOZEI_E_SUCCESSFUL_PIN_CHANGE);
    }
}


