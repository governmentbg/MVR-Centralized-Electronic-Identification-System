package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.backend.client.RueiClient;
import bg.bulsi.mvr.mpozei.backend.dto.CitizenCertificateSummaryDTO;
import bg.bulsi.mvr.mpozei.backend.mapper.CertificateMapper;
import bg.bulsi.mvr.mpozei.backend.service.CertificateService;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import bg.bulsi.mvr.mpozei.model.certificate.CertificateHistory;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;

import java.time.OffsetDateTime;
import java.time.ZoneOffset;
import java.util.UUID;

@Slf4j
@Component
@RequiredArgsConstructor
public class CertificateHistoryCreationStep extends Step<AbstractApplication> {
    private final RueiClient rueiClient;
    private final CertificateMapper certificateMapper;
    private final CertificateService certificateService;

    @Override
    public AbstractApplication process(AbstractApplication application) {
        log.info("Application with id: {} entered CertificateHistoryCreationStep", application.getId());

        UUID reasonId = application.getReason() != null ? application.getReason().getId() : null;
        String reasonText = application.getReasonText();
        CitizenCertificateSummaryDTO certificate = rueiClient.getCertificateById(application.getParams().getCertificateId());
        CertificateHistory history = certificateMapper.map(certificate);
        history.setApplicationId(application.getId());
        history.setApplicationNumber(application.getApplicationNumber().getId());
//        history.setCreateDate(OffsetDateTime.now(ZoneOffset.UTC));
//        history.setModifiedDate(OffsetDateTime.now(ZoneOffset.UTC));
        history.setDeviceId(application.getDeviceId());
        history.setReasonId(reasonId);
        history.setReasonText(reasonText);

        certificateService.createCertificateHistory(history);

        return application;
    }

    @Override
    public PipelineStatus getStatus() {
        return PipelineStatus.CERTIFICATE_HISTORY_CREATION;
    }
}
