package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.crypto.CertificateProcessor;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.FaultMVRException;
import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.backend.client.EjbcaClient;
import bg.bulsi.mvr.mpozei.backend.dto.ejbca.EjbcaRevocationResponse;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;

import java.security.NoSuchProviderException;
import java.security.cert.CertificateException;
import java.security.cert.X509Certificate;
import java.time.Clock;
import java.time.OffsetDateTime;
import java.time.format.DateTimeFormatter;
import static bg.bulsi.mvr.mpozei.backend.dto.ejbca.EjbcaRevocationReason.CESSATION_OF_OPERATION;

@Slf4j
@Component
@RequiredArgsConstructor
public class EjbcaCertificateRevocationStep extends Step<AbstractApplication> {
    private final EjbcaClient ejbcaClient;
    private final CertificateProcessor certificateProcessor;

    @Override
    public AbstractApplication process(AbstractApplication application) {
		log.info("Application with id: {} entered EjbcaCertificateRevocationStep", application.getId());

		X509Certificate parsedCertificate;
		try {
			parsedCertificate = certificateProcessor
					.extractCertificate(application.getParams().getClientCertificate().getBytes());
		} catch (CertificateException | NoSuchProviderException e) {
			log.info("Cannot parse input to X509 Certificate");
			log.error(e.toString());

			throw new FaultMVRException("Certificate could not be extracted", ErrorCode.INTERNAL_SERVER_ERROR);
		}
        revokeCertificateInEjbca(
                parsedCertificate.getIssuerX500Principal().toString(),
                parsedCertificate.getSerialNumber().toString(16)
        );
        return application;
    }

    @Override
    public PipelineStatus getStatus() {
        return PipelineStatus.EJBCA_CERTIFICATE_REVOCATION;
    }

    private EjbcaRevocationResponse revokeCertificateInEjbca(String issuerDn, String serialNumber) {
        log.info("Sending request to ejbca for certificate revocation for serialNumber: {}", serialNumber);
        EjbcaRevocationResponse response = ejbcaClient.revokeCertificate(
                issuerDn,
                serialNumber,
                CESSATION_OF_OPERATION);
        log.info("Received success response for certificate revocation with serialNumber: {}", serialNumber);
        return response;
    }
}
