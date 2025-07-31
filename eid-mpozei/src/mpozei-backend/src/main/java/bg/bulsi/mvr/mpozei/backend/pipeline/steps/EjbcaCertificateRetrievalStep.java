package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.exception.EntityNotFoundException;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.backend.client.EjbcaClient;
import bg.bulsi.mvr.mpozei.backend.client.RueiClient;
import bg.bulsi.mvr.mpozei.backend.dto.*;
import bg.bulsi.mvr.mpozei.backend.dto.ejbca.EjbcaSearchCertificateDTO;
import bg.bulsi.mvr.mpozei.backend.dto.ejbca.EjbcaSearchCertificateRequest;
import bg.bulsi.mvr.mpozei.backend.dto.ejbca.EjbcaSearchCertificateResponse;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;

import java.math.BigInteger;
import java.util.Base64;
import java.util.UUID;

import static bg.bulsi.mvr.common.exception.ErrorCode.NO_CERTIFICATES_FOUND_FOR_EIDENTITY_ID;
import static bg.bulsi.mvr.common.util.ValidationUtil.assertFalse;

@Slf4j
@Component
@RequiredArgsConstructor
public class EjbcaCertificateRetrievalStep extends Step<AbstractApplication> {
    public static final String QUERY = "QUERY";

    private final EjbcaClient ejbcaClient;
    private final RueiClient rueiClient;

    @Override
    public AbstractApplication process(AbstractApplication application) {
        log.info("Application with id: {} entered EjbcaCertificateCreationStep", application.getId());

        CitizenCertificateSummaryDTO rueiCertificate = rueiClient.getCertificateById(application.getParams().getCertificateId());
        EjbcaSearchCertificateResponse certificates = searchEjbcaCertificatesByEidentityId(application.getEidentityId(), rueiCertificate);

        assertFalse(certificates.getCertificates().isEmpty(), NO_CERTIFICATES_FOUND_FOR_EIDENTITY_ID, application.getEidentityId());
        EjbcaSearchCertificateDTO certificate = certificates.getCertificates()
                .stream()
                .findFirst()
                .orElseThrow(() -> new EntityNotFoundException(ErrorCode.CERTIFICATE_NOT_FOUND_IN_EJBCA, rueiCertificate.getId()));
        //base64 encoded certificate
        String certToValidate = new String(Base64.getDecoder().decode(certificate.getCertificate())).replaceAll("\\s","");
        application.getParams().setClientCertificate(certToValidate);
        application.getParams().setCertificateSerialNumber(new BigInteger(certificate.getSerialNumber(), 16).toString());
        return application;
    }

    @Override
    public PipelineStatus getStatus() {
        return PipelineStatus.EJBCA_CERTIFICATE_RETRIEVAL;
    }

    private EjbcaSearchCertificateResponse searchEjbcaCertificatesByEidentityId(UUID eidentityId, CitizenCertificateSummaryDTO certificate) {
        EjbcaSearchCertificateRequest request = new EjbcaSearchCertificateRequest();
        //we don't know it
//        request.addEqualsCriteria(END_ENTITY_PROFILE_NAME_FIELD, eidentityId.toString());
//        request.addEqualsCriteria(ISSUER_DN_FIELD, certificate.getIssuerDN());
        String hexSerialNumber = new BigInteger(certificate.getSerialNumber()).toString(16).toUpperCase();
        request.addLikeCriteria(QUERY, hexSerialNumber);
//        request.addLikeCriteria(QUERY, certificate.getIssuerDN());
//        request.addLikeCriteria(QUERY, certificate.getIssuerDN());
        //TODO: do we have to search by issuer also
        log.info("Sending request to ejbca for certificate search for eidentityId: {}", eidentityId);
        EjbcaSearchCertificateResponse response = ejbcaClient.searchCertificates(request);
        log.info("Received certificates with serialNumbers: {}", response.getCertificates().stream().map(EjbcaSearchCertificateDTO::getSerialNumber).toList());
        return response;
    }
}
