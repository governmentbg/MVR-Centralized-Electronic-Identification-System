package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.backend.client.RueiClient;
import bg.bulsi.mvr.mpozei.backend.dto.CertificateStatusDTO;
import bg.bulsi.mvr.mpozei.backend.dto.CitizenCertificateUpdateDTO;
import bg.bulsi.mvr.mpozei.backend.mapper.ApplicationMapper;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;

import java.util.UUID;

@Slf4j
@RequiredArgsConstructor
@Component(value="rueiCertificateStatusUpdateStep")
public class RueiCertificateStatusUpdateStep extends Step<AbstractApplication> {
    private final RueiClient rueiClient;
    private final ApplicationMapper applicationMapper;

    @Override
    public AbstractApplication process(AbstractApplication application) {
        log.info("Application with id: {} entered RueiCertificateStatusUpdateStep", application.getId());
        updateCertificateStatusInRuei(application);
        return application;
    }

    @Override
    public PipelineStatus getStatus() {
        return PipelineStatus.RUEI_CHANGE_EID_STATUS;
    }

    public void updateCertificateStatusInRuei(AbstractApplication application) {
        CertificateStatusDTO status = switch (application.getApplicationType()) {
            case ISSUE_EID, RESUME_EID -> CertificateStatusDTO.ACTIVE;
            case REVOKE_EID -> CertificateStatusDTO.REVOKED;
            case STOP_EID -> CertificateStatusDTO.STOPPED;
        };
        CitizenCertificateUpdateDTO dto = applicationMapper.mapToCitizenCertificateUpdateDTO(application, status);
        log.info("Sending request to update status of certificate in RUEI for eidentityId: {}", application.getEidentityId());
        rueiClient.updateCertificateStatus(dto);
        log.info("Received success response for certificate status update for eidentityId: {}", application.getEidentityId());
    }
}
