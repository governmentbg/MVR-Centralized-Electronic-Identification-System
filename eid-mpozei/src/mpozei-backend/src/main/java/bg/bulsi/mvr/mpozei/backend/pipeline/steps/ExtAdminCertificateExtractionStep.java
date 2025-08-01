package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.crypto.CertificateProcessor;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.FaultMVRException;
import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;

import java.security.NoSuchProviderException;
import java.security.cert.CertificateException;
import java.security.cert.X509Certificate;
import java.util.List;

@Slf4j
@RequiredArgsConstructor
@Component
public class ExtAdminCertificateExtractionStep extends Step<AbstractApplication> {
    private final CertificateProcessor certificateProcessor;
    
    @Override
    public AbstractApplication process(AbstractApplication application) {
        log.info("Application with id: {} entered ExtAdminCertificateCreationStep", application.getId());

        String certificate = application.getParams().getClientCertificate();
        List<String> certificateAuthorities = application.getParams().getClientCertificateChain();
        application.getParams().setClientCertificateChain(certificateAuthorities);
        X509Certificate parsedCertificate;
		try {
			parsedCertificate = certificateProcessor.extractCertificate(certificate.getBytes());
		} catch (CertificateException | NoSuchProviderException e) {
			log.info("Cannot parse input to X509 Certificate");
            log.error(e.toString());
            
          throw new FaultMVRException("Certificate could not be extracted", ErrorCode.INTERNAL_SERVER_ERROR);
		}

        application.getParams().setIssuerDn(parsedCertificate.getIssuerX500Principal().getName());
        application.getParams().setCertificateSerialNumber(parsedCertificate.getSerialNumber().toString());
        return application;
    }

    @Override
    public PipelineStatus getStatus() {
        return PipelineStatus.EXT_ADMIN_CERTIFICATE_CREATION;
    }
}
