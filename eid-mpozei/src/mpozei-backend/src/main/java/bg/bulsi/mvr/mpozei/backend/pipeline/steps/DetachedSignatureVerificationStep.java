package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.backend.client.PivrClient;
import bg.bulsi.mvr.mpozei.backend.dto.PivrSignatureValidateRequest;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;
import org.springframework.transaction.annotation.Transactional;

@Slf4j
@Component
@RequiredArgsConstructor
public class DetachedSignatureVerificationStep extends Step<AbstractApplication> {
    private final PivrClient pivrClient;
    @Override
    @Transactional
    public AbstractApplication process(AbstractApplication application) {
        log.info("Application with id: {} entered SignatureVerificationStep", application.getId());
        PivrSignatureValidateRequest request = new PivrSignatureValidateRequest();
        request.setDetachedSignature(application.getDetachedSignature());
        request.setSignatureProvider(application.getTemporaryData().getSignatureProvider());
        request.setOriginalFile(application.getApplicationXml());
        request.setCitizenIdentifierNumber(application.getCitizenIdentifierNumber());
        request.setCitizenIdentifierType(application.getCitizenIdentifierType().name());
        //TODO: Have to check if the responses are handled correctly
        pivrClient.validateSignedXml(request);
        return application;
    }

    @Override
    public PipelineStatus getStatus() {
        return PipelineStatus.SIGNATURE_VERIFICATION;
    }
}
