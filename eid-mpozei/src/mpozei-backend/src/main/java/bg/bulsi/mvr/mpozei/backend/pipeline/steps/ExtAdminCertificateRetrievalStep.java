package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.backend.client.RueiClient;
import bg.bulsi.mvr.mpozei.backend.dto.CitizenCertificateDetailsDTO;
import bg.bulsi.mvr.mpozei.backend.dto.CitizenCertificateSummaryDTO;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;

import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Component;

import java.util.Objects;
import static bg.bulsi.mvr.common.exception.ErrorCode.CERTIFICATE_CANNOT_BE_RESUMED;
import static bg.bulsi.mvr.common.exception.ErrorCode.CERTIFICATE_CANNOT_BE_REVOKED;
import static bg.bulsi.mvr.common.exception.ErrorCode.CERTIFICATE_CANNOT_BE_STOPPED;
import static bg.bulsi.mvr.common.exception.ErrorCode.REQUESTER_IS_NOT_OWNER;
import static bg.bulsi.mvr.common.util.ValidationUtil.assertEquals;
import static bg.bulsi.mvr.common.util.ValidationUtil.assertTrue;
import static bg.bulsi.mvr.mpozei.contract.dto.CertificateStatus.ACTIVE;
import static bg.bulsi.mvr.mpozei.contract.dto.CertificateStatus.STOPPED;

@Slf4j
@Component
@RequiredArgsConstructor
public class ExtAdminCertificateRetrievalStep extends Step<AbstractApplication> {
    private final RueiClient rueiClient;

    @Override
    public AbstractApplication process(AbstractApplication application) {
        log.info("Application with id: {} entered ExtAdminCertificateRetrievalStep", application.getId());

        CitizenCertificateSummaryDTO rueiCertificate = rueiClient.getCertificateById(application.getParams().getCertificateId());
        CitizenCertificateDetailsDTO certificate = rueiClient.getCitizenCertificateByIssuerAndSN(rueiCertificate.getIssuerDN(), rueiCertificate.getSerialNumber());

        if (Objects.isNull(UserContextHolder.getFromServletContext().getEidAdministratorId())) {
            assertTrue(Objects.equals(application.getEidentityId(), certificate.getEidentityId()), REQUESTER_IS_NOT_OWNER);
        }
        switch (application.getApplicationType()) {
	        case STOP_EID -> assertEquals(certificate.getStatus().name(), ACTIVE.name(),
	                CERTIFICATE_CANNOT_BE_STOPPED, certificate.getId(), certificate.getStatus().name().toLowerCase());
	        case REVOKE_EID ->
	                assertTrue((certificate.getStatus().name().equals(ACTIVE.name()) || certificate.getStatus().name().equals(STOPPED.name())),
	                        CERTIFICATE_CANNOT_BE_REVOKED, HttpStatus.CONFLICT, certificate.getId(), certificate.getStatus().name().toLowerCase());
	        case RESUME_EID -> assertTrue(certificate.getStatus().name().equals(STOPPED.name()),
	                CERTIFICATE_CANNOT_BE_RESUMED, certificate.getId(), certificate.getStatus().name().toLowerCase());
        }
        
	    application.getParams().setClientCertificate(certificate.getCertificate());
	    application.getParams().setCertificateSerialNumber(certificate.getSerialNumber());
	    return application;
    }

    @Override
    public PipelineStatus getStatus() {
        return PipelineStatus.EXT_ADMIN_CERTIFICATE_RETRIEVAL;
    }
}
