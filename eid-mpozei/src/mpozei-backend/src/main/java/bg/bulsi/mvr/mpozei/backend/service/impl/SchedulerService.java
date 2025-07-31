package bg.bulsi.mvr.mpozei.backend.service.impl;

import bg.bulsi.mvr.audit_logger.BaseAuditLogger;
import bg.bulsi.mvr.audit_logger.dto.AuditData;
import bg.bulsi.mvr.audit_logger.dto.EventPayload;
import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.audit_logger.model.MessageType;
import bg.bulsi.mvr.common.config.security.UserContext;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.mpozei.backend.client.EjbcaClient;
import bg.bulsi.mvr.mpozei.backend.client.PivrClient;
import bg.bulsi.mvr.mpozei.backend.client.ReiClient;
import bg.bulsi.mvr.mpozei.backend.client.RueiClient;
import bg.bulsi.mvr.mpozei.backend.dto.*;
import bg.bulsi.mvr.mpozei.backend.mapper.CertificateMapper;
import bg.bulsi.mvr.mpozei.backend.service.ApplicationPaymentsService;
import bg.bulsi.mvr.mpozei.backend.service.ApplicationService;
import bg.bulsi.mvr.mpozei.backend.service.CertificateService;
import bg.bulsi.mvr.mpozei.backend.service.EidentityService;
import bg.bulsi.mvr.mpozei.backend.service.NomenclatureService;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus;
import bg.bulsi.mvr.mpozei.contract.dto.PivrIdchangesResponseDto;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import bg.bulsi.mvr.mpozei.model.certificate.CertificateHistory;
import bg.bulsi.mvr.mpozei.model.nomenclature.NomLanguage;
import bg.bulsi.mvr.mpozei.model.nomenclature.ReasonNomenclature;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.apache.commons.collections4.ListUtils;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.retry.annotation.Backoff;
import org.springframework.retry.annotation.Retryable;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.math.BigInteger;
import java.time.LocalDate;
import java.time.LocalDateTime;
import java.util.List;
import java.util.concurrent.ExecutorService;
import java.util.function.Consumer;

import static bg.bulsi.mvr.mpozei.backend.dto.ejbca.EjbcaRevocationReason.CESSATION_OF_OPERATION;

@Slf4j
@Service
@RequiredArgsConstructor
public class SchedulerService {

    @Value("${executor.payments-thread-pool}")
    private Integer paymentsThreadPool;

    private final PivrClient pivrClient;
    private final RueiClient rueiClient;
    private final ReiClient reiClient;
    private final CertificateMapper certificateMapper;
    private final NomenclatureService nomenclatureService;
    private final EjbcaClient ejbcaClient;
    private final CertificateService certificateService;
    private final ApplicationService applicationService;
    private final ApplicationPaymentsService applicationPaymentsService;
    private final ExecutorService paymentsExecutorService;
    private final EidentityService eidentityService;
	private final BaseAuditLogger auditLogger;
    
    @Retryable(backoff = @Backoff(delay = 120000)) // Retry up to 3 times with a delay of 2 minutes
    @Scheduled(cron = "${cron-jobs.deceased.cron}") // Execute every day at 3 AM
    public void invalidateCertificatesForDeceasedCitizens() {
        log.info(".invalidateCertificatesForDeceasedCitizens()");

        UserContext userContext = initializeUserContext();
        LocalDateTime startDateTime = LocalDateTime.now().minusDays(1);
        LocalDateTime endDateTime = LocalDateTime.now().plusDays(1);

        log.info(".invalidateCertificatesForDeceasedCitizens() [startDateTime={}] [endDateTime={}]", startDateTime, endDateTime);

        List<DeceasedCitizenDTO> deceasedCitizens = pivrClient.getDeceasedByDateRange(startDateTime.toString(), endDateTime.toString());
        log.info("Deceased citizens in PIVR:{}", deceasedCitizens);
        List<EidentityRequestDTO> eidentityRequests = deceasedCitizens.stream().map(e -> new EidentityRequestDTO(e.getCitizenIdentifierNumber(), e.getCitizenIdentifierType())).toList();
        List<EidentityDTO> eidentities = reiClient.getEidentitiesByNumberAndType(new FindEidentitiesRequest(eidentityRequests));

        log.info(".invalidateCertificatesForDeceasedCitizens() [eidentities={}]", eidentities.size());

        if (!eidentities.isEmpty()) {
            List<CitizenCertificateDetailsDTO> certificates = rueiClient.invalidateCertificatesByEidentityIds(eidentities.stream().map(EidentityDTO::getId).toList())
                    .stream()
                    .toList();
            certificates.forEach(certificate -> {
                ejbcaClient.revokeCertificate(certificate.getIssuerDN(), new BigInteger(certificate.getSerialNumber()).toString(16), CESSATION_OF_OPERATION);
                CertificateHistory history = certificateMapper.map(certificate);
                ReasonNomenclature reason = nomenclatureService.getReasonByName(ReasonNomenclature.CITIZEN_PASSED_AWAY_REASON);
                history.setReasonId(reason.getId());
                this.certificateService.createCertificateHistory(history);
                
                //Log Audit Event
                EventPayload eventPayload = new EventPayload();
                eventPayload.setCertificateId(certificate.getId().toString());
                eventPayload.setEidentityId(certificate.getEidentityId().toString());
                
                this.logAudit(eventPayload, AuditEventType.INVALIDATE_CERTIFICATE_DECEASED_CITIZEN, MessageType.REQUEST, userContext);
            });
        }
        eidentities.forEach(eidentity -> {
            try {
                rueiClient.updateProfileStatusByEidentityId(ProfileStatus.DISABLED, eidentity.getId());
                
                //Log Audit Event
                EventPayload eventPayload = new EventPayload();
                //eventPayload.setProfileId(null);
                eventPayload.setEidentityId(eidentity.getId().toString());
                
                this.logAudit(eventPayload, AuditEventType.INVALIDATE_PROFILE_DECEASED_CITIZEN, MessageType.REQUEST, userContext);
            } catch (Exception e ) {
                log.info(e.getMessage());
                log.info("Profile does not exist. Skipping...");
            }
        });
        eidentities.forEach(e -> {
            reiClient.updateEidentityActive(e.getId(), false);
            
            //Log Audit Event
            EventPayload eventPayload = new EventPayload();
            eventPayload.setEidentityId(e.getId().toString());
            
            this.logAudit(eventPayload, AuditEventType.INVALIDATE_EIDENTITY_ID_DECEASED_CITIZEN, MessageType.REQUEST, userContext);
        });
    }

    @Retryable(backoff = @Backoff(delay = 120000)) // Retry up to 3 times with a delay of 2 minutes
    @Scheduled(cron = "${cron-jobs.sync-personal-id-changes.cron}") // Execute every day at 3 AM
	public void syncPersonalIdChange() {
		UserContext userContext = initializeUserContext();
		LocalDateTime startDateTime = LocalDateTime.now().minusDays(1);
		LocalDateTime endDateTime = LocalDateTime.now().plusDays(1);

		log.info(".syncPersonalIdChange() [startDateTime={}] [endDateTime={}]", startDateTime, endDateTime);

        Consumer<EventPayload> auditEventLogger = eventPayload -> 
    		this.logAudit(eventPayload, AuditEventType.UPDATE_CITIZEN_IDENTIFIER_BY_NAIF, MessageType.SUCCESS, userContext);    
		
		List<PivrIdchangesResponseDto> idChanges = pivrClient.getIdChanges(null, null, startDateTime.toString(), endDateTime.toString());
		for (PivrIdchangesResponseDto idChange : idChanges) {
			this.eidentityService.updateCitizenIdentifier(idChange, auditEventLogger);
		}
	}
    
    @Retryable(backoff = @Backoff(delay = 120000)) // Retry up to 3 times with a delay of 2 minutes
    @Scheduled(cron = "${cron-jobs.daily-expired.cron}") // Execute every day at 3.05 AM
    public void invalidateDailyExpiredCertificates() {
        log.info(".invalidateDailyExpiredCertificates()");

        UserContext userContext = initializeUserContext();
        List<CitizenCertificateDetailsDTO> certificates = rueiClient.invalidateExpiredCertificates(LocalDate.now());
        certificates.forEach(certificate -> {
        	//Create {@link CertificateHistory}
            CertificateHistory history = certificateMapper.map(certificate);
            ReasonNomenclature reason = nomenclatureService.getReasonByName(ReasonNomenclature.CERTIFICATE_EXPIRED_REASON);
            history.setReasonId(reason.getId());
            certificateService.createCertificateHistory(history);
            
            //Log Audit Event
            EventPayload eventPayload = new EventPayload();
            eventPayload.setCertificateId(certificate.getId().toString());
            eventPayload.setEidentityId(certificate.getEidentityId().toString());
            
            this.logAudit(eventPayload, AuditEventType.DAILY_EXPIRING_CERTIFICATE, MessageType.REQUEST, userContext);
        });
    }

    //TODO: rework this should be done in batches and should not extract all data for the applications
    @Transactional
    @Retryable(backoff = @Backoff(delay = 120000)) // Retry up to 3 times with a delay of 2 minutes
    @Scheduled(cron = "${cron-jobs.daily-unfinished.cron}") // Execute every day at 3.10 AM
    public void invalidateDailyUnfinishedApplications() {
        log.info(".invalidateDailyUnfinishedApplications()");

        UserContext userContext = initializeUserContext();
//      Unpaid are denied after 3 days
//      Unfinished are denied after 14 days
        List<AbstractApplication> applicationsToDeny = applicationService.getDailyUnfinishedApplications();
        log.info(".invalidateDailyUnfinishedApplications() [applicationsToDeny={}]", applicationsToDeny.size());

        applicationsToDeny.forEach(e -> {
            e.setStatus(ApplicationStatus.DENIED);
            ReasonNomenclature reason = this.nomenclatureService.getReasonByName(ReasonNomenclature.DENIED_TIMED_OUT);
            e.setReason(reason);
            
			EventPayload eventPayload = new EventPayload();
			eventPayload.setEidentityId(e.getEidentityId().toString());
			eventPayload.setApplicationStatus(e.getStatus().name());
			eventPayload.setApplicationId(e.getId().toString());
			eventPayload.setApplicationType(e.getApplicationType().name());
			eventPayload.setTargetName(e.getFirstName(), e.getSecondName(), e.getLastName());
			eventPayload.setTargetUid(e.getCitizenIdentifierNumber());
			eventPayload.setTargetUidType(e.getCitizenIdentifierType().name());
			
        	this.logAudit(eventPayload, AuditEventType.DAILY_UNFINISHED_APPLICATIONS, MessageType.SUCCESS, userContext);    
        });
        
        this.applicationService.saveAll(applicationsToDeny);
    }

    @Retryable(backoff = @Backoff(delay = 60000)) // Retry up to 3 times with a delay of 2 minutes
    @Scheduled(cron = "${cron-jobs.payments-check.cron}") // Execute every day at 3.10 AM
    @Transactional
    public void checkApplicationPayments() {
        log.info(".checkApplicationPayments()");
        List<AbstractApplication> applications = this.applicationPaymentsService.getPendingPaymentApplications();
        int partitionSize = Math.max(applications.size() / paymentsThreadPool, 1);

        log.info(".checkApplicationPayments() [applications.size()={}] [partitionSize={}]", applications.size(), partitionSize);

        UserContext userContext = initializeUserContext();
        Consumer<EventPayload> auditEventLogger = eventPayload -> 
        	this.logAudit(eventPayload, AuditEventType.UPDATE_APPLICATION_PAYMENT_STATUS, MessageType.SUCCESS, userContext);    
        
        List<List<AbstractApplication>> partitionedLists = ListUtils.partition(applications, partitionSize);

        for (List<AbstractApplication> sublist : partitionedLists) {
            paymentsExecutorService.submit(() -> this.applicationPaymentsService.checkApplicationsPaymentStatus(sublist, auditEventLogger));
        }
    }

    private void logAudit(EventPayload eventPayload, AuditEventType auditEventType, MessageType messageType, UserContext userContext) {
    	this.auditLogger.auditEvent(AuditData.builder()
				.correlationId(userContext.getGlobalCorrelationId().toString())
				.eventType(auditEventType)
				.messageType(messageType)
				.requesterUserId(userContext.getRequesterUserId())
				.requesterSystemId(userContext.getSystemId())
				.requesterSystemName(userContext.getSystemName())
				.targetUserId(userContext.getTargetUserId())
				.payload(eventPayload)
				.build());
    }
    
    private UserContext initializeUserContext() {
    	UserContext currentUserContext = UserContextHolder.emptyServletContext();
        UserContextHolder.setToServletContext(currentUserContext);
        
        return currentUserContext;
    }
}
