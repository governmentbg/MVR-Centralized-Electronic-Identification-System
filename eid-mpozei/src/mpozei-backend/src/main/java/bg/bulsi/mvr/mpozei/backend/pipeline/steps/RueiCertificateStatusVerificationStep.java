package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.common.exception.WrongStatusException;
import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.backend.client.RueiClient;
import bg.bulsi.mvr.mpozei.backend.dto.CertificateStatusDTO;
import bg.bulsi.mvr.mpozei.backend.dto.CitizenCertificateValidateDTO;
import bg.bulsi.mvr.mpozei.backend.mapper.ApplicationMapper;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.stereotype.Component;

import static bg.bulsi.mvr.common.exception.ErrorCode.CERTIFICATE_IS_EXPIRED_OR_INVALID;
import static bg.bulsi.mvr.common.exception.ErrorCode.WRONG_CERTIFICATE_STATUS;

@Slf4j
@RequiredArgsConstructor
@Component(value="rueiCertificateStatusVerificationStep")
public class RueiCertificateStatusVerificationStep extends Step<AbstractApplication> {
    private final RueiClient rueiClient;
    private final ApplicationMapper applicationMapper;

    @Override
    public AbstractApplication process(AbstractApplication application) {
        log.info("Application with id: {} entered RueiCertificateStatusVerificationStep", application.getId());

        CertificateStatusDTO status = validateCertificateStatusInRuei(application);
        boolean isValid = switch (status) {
            case STOPPED -> true;
            case INVALID -> throw new ValidationMVRException(CERTIFICATE_IS_EXPIRED_OR_INVALID);
            default -> status == CertificateStatusDTO.ACTIVE;
        };

        if (!isValid) {
            throw new WrongStatusException(WRONG_CERTIFICATE_STATUS);
        }
        return application;
    }

    @Override
    public PipelineStatus getStatus() {
        return PipelineStatus.RUEI_CERTIFICATE_STATUS_VERIFICATION;
    }

    public CertificateStatusDTO validateCertificateStatusInRuei(AbstractApplication application) {
        CitizenCertificateValidateDTO dto = applicationMapper.mapToCitizenCertificateValidateDTO(application);
        log.info("Sending request to get status of certificate in RUEI for eidentityId: {}", application.getEidentityId());
        CertificateStatusDTO status = rueiClient.validateCitizenCertificate(dto);
        log.info("Received status: {} for certificate for eidentityId: {}", status, application.getEidentityId());
        return status;
    }
}
