package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.common.service.FileFormatService;
import bg.bulsi.mvr.mpozei.backend.client.EjbcaClient;
import bg.bulsi.mvr.mpozei.backend.client.PunClient;
import bg.bulsi.mvr.mpozei.backend.client.RueiClient;
import bg.bulsi.mvr.mpozei.backend.dto.*;
import bg.bulsi.mvr.mpozei.backend.dto.ejbca.EjbcaRevocationResponse;
import bg.bulsi.mvr.mpozei.backend.mapper.CertificateMapper;
import bg.bulsi.mvr.mpozei.backend.service.CertificateService;
import bg.bulsi.mvr.mpozei.backend.service.NomenclatureService;
import bg.bulsi.mvr.mpozei.backend.service.RaeiceiService;
import bg.bulsi.mvr.mpozei.contract.dto.CertificateStatus;
import bg.bulsi.mvr.mpozei.contract.dto.xml.EidApplicationXml;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import bg.bulsi.mvr.mpozei.model.certificate.CertificateHistory;
import bg.bulsi.mvr.mpozei.model.nomenclature.ReasonNomenclature;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceDTO;
import bg.bulsi.mvr.raeicei.model.entity.Device;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;

import java.math.BigInteger;
import java.time.OffsetDateTime;
import java.time.ZoneOffset;
import java.util.Comparator;
import java.util.List;
import java.util.Objects;

import static bg.bulsi.mvr.common.exception.ErrorCode.DEVICE_TYPE_NOT_RECOGNIZED;
import static bg.bulsi.mvr.common.pipeline.PipelineStatus.EXISTING_CERTIFICATE_VERIFICATION;
import static bg.bulsi.mvr.mpozei.backend.dto.CertificateStatusDTO.REVOKED;
import static bg.bulsi.mvr.mpozei.backend.dto.ejbca.EjbcaRevocationReason.CESSATION_OF_OPERATION;

@Slf4j
@RequiredArgsConstructor
@Component
public class ExistingRueiCertificateVerificationStep extends Step<AbstractApplication> {
    private final RaeiceiService raeiceiService;
    private final FileFormatService fileFormatService;
    private final PunClient punClient;
    private final RueiClient rueiClient;
    private final CertificateMapper certificateMapper;
    private final NomenclatureService nomenclatureService;
    private final CertificateService certificateService;


    @Override
    public AbstractApplication process(AbstractApplication application) {
        log.info("Application with id: {} entered ExistingRueiCertificateVerificationStep", application.getId());
    	
        DeviceDTO device = raeiceiService.getDeviceById(application.getDeviceId());
        //TODO: is this correct? MOBILE is null
        // mobile and other being null means serialNumber doesnt play a role in the search filter
        // all devices by eidentityId are being returned
        String serialNumber = switch (device.getType()) {
            case MOBILE, OTHER -> null;
            case CHIP_CARD ->
                    fileFormatService.createObjectFromXmlString(application.getApplicationXml(), EidApplicationXml.class).getIdentityNumber();
            default -> throw new ValidationMVRException(DEVICE_TYPE_NOT_RECOGNIZED);
        };

        List<PunCarrierDTO> existingCarriers = punClient.getPunCarriersBySerialNumberAndEidentityId(serialNumber, application.getEidentityId())
                .stream()
                .filter(e -> Objects.equals(e.getDeviceType(), device.getId().toString()))
                .sorted(Comparator.comparing(PunCarrierDTO::getModifiedOn).reversed())
                .toList();

        if (existingCarriers.isEmpty()) {
            return application;
        }

        CitizenCertificateSummaryDTO certificate = null;
        try {
        	certificate = rueiClient.getCertificateById(existingCarriers.get(0).getCertificateId());
        	
            log.info("Success extracting certificate from RUEI - certificateId: {}; certificateStatus: {}", certificate.getId(), certificate.getStatus());
        } catch (Exception e){
            log.error("Could not extract certificate from RUEI - certificateId: {}", existingCarriers.get(0).getCertificateId());
            return application;
        }
        
        if (Objects.equals(certificate.getStatus().name(), REVOKED.name())) {
            return application;
        }

        CitizenCertificateUpdateDTO dto = new CitizenCertificateUpdateDTO(certificate.getSerialNumber(), certificate.getIssuerDN(), REVOKED, application.getEidentityId(), application.getId());
        log.info("Sending request to update status of certificate in RUEI for eidentityId: {}", application.getEidentityId());
        rueiClient.updateCertificateStatus(dto);

        CertificateHistory history = certificateMapper.map(certificate);
        history.setStatus(CertificateStatus.REVOKED);
//        history.setCreateDate(OffsetDateTime.now(ZoneOffset.UTC));
//        history.setModifiedDate(OffsetDateTime.now(ZoneOffset.UTC));
        history.setDeviceId(certificate.getDeviceId());
        ReasonNomenclature reason = nomenclatureService.getReasonByName(ReasonNomenclature.REASON_REPLACED);
        history.setReasonId(reason.getId());
        history.setApplicationId(application.getId());
        history.setApplicationNumber(application.getApplicationNumber().getId());


        certificateService.createCertificateHistory(history);
        application.getParams().setReplacedExistingCertificate(true);

        return application;
    }

    @Override
    public PipelineStatus getStatus() {
        return EXISTING_CERTIFICATE_VERIFICATION;
    }

}
