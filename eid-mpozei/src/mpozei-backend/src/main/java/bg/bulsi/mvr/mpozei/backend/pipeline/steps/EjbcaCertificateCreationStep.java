package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.crypto.CertificateProcessor;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.FaultMVRException;
import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.backend.client.EjbcaClient;
import bg.bulsi.mvr.mpozei.backend.dto.ejbca.EjbcaCertificateDTO;
import bg.bulsi.mvr.mpozei.backend.dto.ejbca.EjbcaCertificateEnrollRequest;
import bg.bulsi.mvr.mpozei.backend.dto.ejbca.EjbcaCertificateRequestRestRequest;
import bg.bulsi.mvr.mpozei.backend.mapper.ApplicationMapper;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;

import java.security.NoSuchProviderException;
import java.security.cert.CertificateException;
import java.security.cert.X509Certificate;

@Slf4j
@RequiredArgsConstructor
@Component
public class EjbcaCertificateCreationStep extends Step<AbstractApplication> {
    private final ApplicationMapper applicationMapper;
    private final EjbcaClient ejbcaClient;
    private final CertificateProcessor certificateProcessor;
    
	@Value("${ejbca.community-support:false}")
	private boolean communitySupport;
    
    @Override
    public AbstractApplication process(AbstractApplication application) {
        log.info("Application with id: {} entered EjbcaCertificateCreationStep", application.getId());

        EjbcaCertificateDTO ejbcaCertificate = enrollForEjbcaCertificate(application);
        application.getParams().setClientCertificate(ejbcaCertificate.getCertificate());
        application.getParams().setClientCertificateChain(ejbcaCertificate.getCertificateChain());
        X509Certificate parsedCertificate;
		try {
			parsedCertificate = certificateProcessor.extractCertificate(ejbcaCertificate.getCertificate().getBytes());
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
        return PipelineStatus.EJBCA_CERTIFICATE_CREATION;
    }

    private EjbcaCertificateDTO enrollForEjbcaCertificate(AbstractApplication application) {
        log.info("Sending request to ejbca for certificate enrollment for eidentityId: {}", application.getEidentityId());
        EjbcaCertificateDTO response = null;
        if(this.communitySupport) {
            EjbcaCertificateEnrollRequest request = applicationMapper.mapToEjbcaCertificateEnrollRequest(application);
            log.debug("EJBCA certificateRequest: {}", request.toString());
            response = ejbcaClient.enrollForCertificate(request);
        } else {
            EjbcaCertificateRequestRestRequest requestEndEntity = applicationMapper.mapToEjbcaCertificateRequest(application);
            log.debug("EJBCA certificateRequest: {}", requestEndEntity.toString());
            response = ejbcaClient.certificateRequest2Cert(requestEndEntity);
        }
        log.info("Created certificate in ejbca for eidentityId: {} with serialNumber: {}", application.getEidentityId(), response.getSerialNumber());
        return response;
    }
}
