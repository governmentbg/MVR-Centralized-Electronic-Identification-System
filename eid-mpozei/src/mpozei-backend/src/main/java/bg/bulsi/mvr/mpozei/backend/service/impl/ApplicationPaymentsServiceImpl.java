package bg.bulsi.mvr.mpozei.backend.service.impl;

import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus.PAID;
import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus.PAYMENT_EXPIRED;
import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus.PENDING_PAYMENT;

import java.util.List;
import java.util.function.Consumer;

import org.springframework.scheduling.annotation.Async;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Propagation;
import org.springframework.transaction.annotation.Transactional;
import bg.bulsi.mvr.audit_logger.dto.EventPayload;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.mpozei.backend.client.MisepClient;
import bg.bulsi.mvr.mpozei.backend.dto.misep.MisepStatusCode;
import bg.bulsi.mvr.mpozei.backend.service.ApplicationPaymentsService;
import bg.bulsi.mvr.mpozei.backend.service.ApplicationService;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationType;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import bg.bulsi.mvr.mpozei.model.repository.ApplicationRepository;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;

@Slf4j
@Service
@RequiredArgsConstructor
public class ApplicationPaymentsServiceImpl implements ApplicationPaymentsService {
	private final MisepClient misepClient;
	private final ApplicationRepository<AbstractApplication> abstractApplicationRepository;
	private final ApplicationService applicationService;

	@Override
	@Transactional
	public List<AbstractApplication> getPendingPaymentApplications() {
		return abstractApplicationRepository.findAllByStatusAndMisepPaymentIdNotNull(PENDING_PAYMENT);
	}

	@Override
	@Transactional(propagation = Propagation.REQUIRES_NEW)
	@Async
	public void checkApplicationsPaymentStatus(List<AbstractApplication> applications,
			Consumer<EventPayload> auditEventLogger) {
		UserContextHolder.setToServletContext(UserContextHolder.emptyServletContext());
		applications.forEach(application -> {
			MisepStatusCode response = misepClient.getPaymentStatus(application.getParams().getMisepPaymentId(),
					application.getCitizenProfileId());
			log.info("ApplicationId:{}, Status:{}", application.getId(), response);
			if (MisepStatusCode.Paid.equals(response)) {
				application.setStatus(PAID);
				log.info("Application with id:{} has been paid", application.getId());
				AbstractApplication currentApplication = abstractApplicationRepository.save(application);

				if (currentApplication.getApplicationType() != ApplicationType.ISSUE_EID
						&& List.of(ApplicationSubmissionType.EID, ApplicationSubmissionType.BASE_PROFILE)
								.contains(currentApplication.getSubmissionType())) {

					this.applicationService.processApplication(currentApplication);
				}

				// Log Audit Event
				EventPayload eventPayload = new EventPayload();
				eventPayload.setEidentityId(application.getEidentityId().toString());
				eventPayload.setApplicationStatus(application.getStatus().name());
				eventPayload.setApplicationId(application.getId().toString());

				auditEventLogger.accept(eventPayload);
			} else if (MisepStatusCode.TimedOut.equals(response)) {
				application.setStatus(PAYMENT_EXPIRED);
				log.info("Application with id:{} has expired payment", application.getId());
				abstractApplicationRepository.save(application);

				// Log Audit Event
				EventPayload eventPayload = new EventPayload();
				eventPayload.setEidentityId(application.getEidentityId().toString());
				eventPayload.setApplicationStatus(application.getStatus().name());
				eventPayload.setApplicationId(application.getId().toString());

				auditEventLogger.accept(eventPayload);
			}
		});
	}
}