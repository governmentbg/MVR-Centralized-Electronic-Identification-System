package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.backend.client.RueiClient;
import bg.bulsi.mvr.mpozei.backend.dto.CitizenCertificateSummaryDTO;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Component;

import java.util.Objects;
import java.util.UUID;

import static bg.bulsi.mvr.common.exception.ErrorCode.*;
import static bg.bulsi.mvr.common.pipeline.PipelineStatus.RUEI_CERTIFICATE_RETRIEVAL;
import static bg.bulsi.mvr.common.util.ValidationUtil.assertEquals;
import static bg.bulsi.mvr.common.util.ValidationUtil.assertTrue;
import static bg.bulsi.mvr.mpozei.contract.dto.CertificateStatus.ACTIVE;
import static bg.bulsi.mvr.mpozei.contract.dto.CertificateStatus.STOPPED;

@Component
@RequiredArgsConstructor
public class RueiCertificateRetrievalStep extends Step<AbstractApplication> {
    private final RueiClient rueiClient;


    @Override
    public AbstractApplication process(AbstractApplication application) {
        CitizenCertificateSummaryDTO certificate = rueiClient.getCertificateById(application.getParams().getCertificateId());
        if (Objects.isNull(UserContextHolder.getFromServletContext().getEidAdministratorId())) {
            assertTrue(Objects.equals(application.getEidentityId(), certificate.getEidentityId()), REQUESTER_IS_NOT_OWNER);
        }
        switch (application.getApplicationType()) {
            case STOP_EID -> assertEquals(certificate.getStatus(), ACTIVE,
                    CERTIFICATE_CANNOT_BE_STOPPED, certificate.getId(), certificate.getStatus().name().toLowerCase());
            case REVOKE_EID -> assertTrue((certificate.getStatus() == ACTIVE || certificate.getStatus() == STOPPED),
                    CERTIFICATE_CANNOT_BE_REVOKED, HttpStatus.CONFLICT, certificate.getId(), certificate.getStatus().name().toLowerCase());
            case RESUME_EID -> assertTrue(certificate.getStatus() == STOPPED,
                    CERTIFICATE_CANNOT_BE_RESUMED, certificate.getId(), certificate.getStatus().name().toLowerCase());
        }

        application.getParams().setCertificateSerialNumber(certificate.getSerialNumber());
        return application;
    }

    @Override
    public PipelineStatus getStatus() {
        return RUEI_CERTIFICATE_RETRIEVAL;
    }
}
