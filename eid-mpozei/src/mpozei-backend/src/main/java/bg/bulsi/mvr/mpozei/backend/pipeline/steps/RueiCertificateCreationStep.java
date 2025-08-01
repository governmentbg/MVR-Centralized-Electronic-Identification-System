package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.backend.client.RueiClient;
import bg.bulsi.mvr.mpozei.backend.dto.CitizenCertificateDTO;
import bg.bulsi.mvr.mpozei.backend.mapper.ApplicationMapper;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;

import java.util.UUID;

@Slf4j
@RequiredArgsConstructor
@Component(value="rueiCertificateCreationStep")
public class RueiCertificateCreationStep extends Step<AbstractApplication> {
    private final RueiClient rueiClient;
    private final ApplicationMapper applicationMapper;

    @Override
    public AbstractApplication process(AbstractApplication application) {
        log.info("Application with id: {} entered RueiCertificateCreationStep", application.getId());
        createCertificate(application);
        return application;
    }

    @Override
    public PipelineStatus getStatus() {
        return PipelineStatus.RUEI_CERTIFICATE_CREATION;
    }

    private void createCertificate(AbstractApplication application) {
        CitizenCertificateDTO citizenCertificate = applicationMapper.mapToCitizenCertificateDTO(application);
        log.info("Sending request to create certificate in RUEI with eidentityId: {}", application.getEidentityId());
        UUID certificateId = rueiClient.createCitizenCertificate(citizenCertificate);
        log.info("Received success response for certificate creation for eidentityId: {}", application.getEidentityId());
        application.getParams().setCertificateId(certificateId);
    }
}
