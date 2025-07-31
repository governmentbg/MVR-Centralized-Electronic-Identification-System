package bg.bulsi.mvr.mpozei.backend.service.impl;

import bg.bulsi.mvr.common.config.security.UserContext;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.exception.EntityNotFoundException;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.mpozei.backend.client.EjbcaClient;
import bg.bulsi.mvr.mpozei.backend.client.PunClient;
import bg.bulsi.mvr.mpozei.backend.client.ReiClient;
import bg.bulsi.mvr.mpozei.backend.client.RueiClient;
import bg.bulsi.mvr.mpozei.backend.dto.*;
import bg.bulsi.mvr.mpozei.backend.mapper.CertificateMapper;
import bg.bulsi.mvr.mpozei.backend.service.CertificateService;
import bg.bulsi.mvr.mpozei.backend.service.NomenclatureService;
import bg.bulsi.mvr.mpozei.backend.service.RaeiceiService;
import bg.bulsi.mvr.mpozei.backend.service.SsevNotificationService;
import bg.bulsi.mvr.mpozei.contract.dto.*;
import bg.bulsi.mvr.mpozei.model.certificate.CertificateHistory;
import bg.bulsi.mvr.mpozei.model.repository.CertificateHistoryRepository;
import bg.bulsi.mvr.raeicei.contract.dto.EidAdministratorDTO;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.data.domain.Page;
import org.springframework.stereotype.Service;

import java.math.BigInteger;
import java.time.OffsetDateTime;
import java.time.ZoneOffset;
import java.time.temporal.ChronoUnit;
import java.util.List;
import java.util.Objects;
import java.util.UUID;

import static bg.bulsi.mvr.common.exception.ErrorCode.*;
import static bg.bulsi.mvr.common.util.ValidationUtil.*;
import static bg.bulsi.mvr.mpozei.backend.dto.ejbca.EjbcaRevocationReason.CESSATION_OF_OPERATION;
import static bg.bulsi.mvr.mpozei.contract.dto.CertificateStatus.ACTIVE;
import static bg.bulsi.mvr.mpozei.contract.dto.CertificateStatus.STOPPED;

@Slf4j
@Service
@RequiredArgsConstructor
public class CertificateServiceImpl implements CertificateService {
    private final RueiClient rueiClient;
    private final ReiClient reiClient;
    private final PunClient punClient;
    private final EjbcaClient ejbcaClient;
    private final CertificateMapper certificateMapper;
    private final CertificateHistoryRepository certificateHistoryRepository;
    private final RaeiceiService raeiceiService;
    private final NomenclatureService nomenclatureService;
    private final SsevNotificationService ssevNotificationService;

    @Value("${mvr.certificate.days-before-is-expiring:30}")
    private long daysBeforeIsExpiring;

    @Override
    public CitizenCertificateSummaryResponse getCertificateById(UUID id) {
        CitizenCertificateSummaryDTO dto = rueiClient.getCertificateById(id);
        // TODO: 3/19/2024 refactor when new keycloak knows what is the authentication
        if (Objects.isNull(UserContextHolder.getFromServletContext().getEidAdministratorId())) {
            UserContext context = UserContextHolder.getFromServletContext();
            assertFalse(context.getEidentityId() == null && context.getCitizenProfileId() == null, MISSING_USER_IDS);

            if ( (Objects.nonNull(context.getEidentityId()) && !Objects.equals(UUID.fromString(context.getEidentityId()), dto.getEidentityId())) ||
                    (Objects.nonNull(context.getCitizenProfileId()) && !Objects.equals(UUID.fromString(context.getCitizenProfileId()), dto.getCitizenProfileId())) ) {
                log.error("Requester doesnt match with certificate owner");
                throw new EntityNotFoundException(ErrorCode.CITIZEN_CERTIFICATE_NOT_FOUND_ID);
            }
        }
        CitizenCertificateSummaryResponse response = certificateMapper.mapToCertificateSummaryResponse(dto);
        EidAdministratorDTO eidAdministrator = raeiceiService.getEidAdministratorById(response.getEidAdministratorId());
        response.setEidAdministratorName(eidAdministrator.getName());

        OffsetDateTime currentDateTime = OffsetDateTime.now();
        response.setIsExpiring(this.isCertificateExpiring(currentDateTime, dto.getValidityUntil()));

//        List<PunCarrierDTO> punCarriers = punClient.getPunCarrierByEidentityIdAndCertificateId(dto.getEidentityId(), dto.getId());
////        if (punCarriers.size() > 1) {
////            throw new FaultMVRException("More than one certificate matches the query in PUN");
////        }
//        if (!punCarriers.isEmpty()) {
//            // TODO: 1/19/2024 remove pundevicetype and use regular since its now string in pun
//
//            response.setDeviceId(punCarriers.get(0).getPunDeviceId());
//            DeviceDTO device = raeiceiService.getDeviceById(punCarriers.get(0).getPunDeviceId());
//            response.setLevelOfAssurance(this.findLevelOfAssurance(device.getType()));
//        }
        return response;
    }

    @Override
    public Page<FindCertificateResponse> findCitizenCertificates(CitizenCertificateFilter filter) {
//        List<PunCarrierDTO> punCarriers = Objects.nonNull(filter.getEidentityId()) ? punClient.getPunCarriersByEidentityId(filter.getEidentityId()) : null;

        Page<FindCertificateResponse> certificateResponses = rueiClient.findCitizenCertificates(
        				filter.getEidentityId(),
        				filter.getCitizenProfileId(),
        				filter.getId(),
        				filter.getSerialNumber(),
        				filter.getStatuses(),
        				filter.getValidityFrom(),
        				filter.getValidityUntil(),
        				filter.getDeviceIds(),
        				filter.getAlias(),
        				filter.isPublicApi(),
        				filter.getPageable());
        
        OffsetDateTime currentDateTime = OffsetDateTime.now();
        certificateResponses.forEach(c -> {
        	c.setIsExpiring(this.isCertificateExpiring(currentDateTime, c.getValidityUntil()));
        });

//        if (Objects.nonNull(punCarriers)) {
//            certificateResponses.forEach(certificate -> {
//            	PunCarrierDTO punCarrier = punCarriers
//                        .stream()
//                        .filter(carrier -> carrier.getCertificateId().equals(certificate.getId()))
//                        .findFirst()
//                        .orElse(null);
//            	
////                UUID punDeviceId = (punCarrier != null) ? punCarrier.getPunDeviceId() : null;
//                
//                certificate.setDeviceId(punCarrier.de);
//                DeviceDTO device = raeiceiService.getDeviceById(punDeviceId);
//                certificate.setLevelOfAssurance(this.findLevelOfAssurance(device.getType()));
//            });
//        }
        return certificateResponses;
    }

    @Override
    public void createCertificateHistory(CertificateHistory history) {
        history.setCreateDate(OffsetDateTime.now(ZoneOffset.UTC));
        history.setModifiedDate(OffsetDateTime.now(ZoneOffset.UTC));
    	
        certificateHistoryRepository.save(history);
    }

    @Override
    public List<CertificateHistory> getCertificateHistoryByCertificateId(UUID certificateId) {
        Boolean isAdmin = Objects.nonNull(UserContextHolder.getFromServletContext().getEidAdministratorId());
        return certificateHistoryRepository.findAllByCertificateId(certificateId, isAdmin);
    }

    @Override
    public NaifUpdateCertificateStatusResponse updateCertificateStatusByNaif(NaifUpdateCertificateStatusDTO dto) {
        ValidationService.validateCitizenIdentifierNumber(dto.getPersonalId(), IdentifierType.valueOf(dto.getUiDType().name()));
        EidentityDTO eidentity  = reiClient.findEidentityByNumberAndType(dto.getPersonalId(), IdentifierType.valueOf(dto.getUiDType().name()));

        List<PunCarrierDTO> punCarriers = punClient.getPunCarriersBySerialNumberAndEidentityId(dto.getCarrierSerialNumber(), eidentity.getId());
        punCarriers.sort((o1, o2) -> o2.getModifiedOn().compareTo(o1.getModifiedOn()));

        CitizenCertificateSummaryDTO certificate =  rueiClient.getCertificateById(punCarriers.get(0).getCertificateId());
        switch (dto.getAction()) {
            case REVOKED -> assertTrue(List.of(ACTIVE,STOPPED).contains(certificate.getStatus()), WRONG_CERTIFICATE_STATUS);
            case STOPPED -> assertTrue(Objects.equals(ACTIVE, certificate.getStatus()), WRONG_CERTIFICATE_STATUS);
            case ACTIVE -> assertTrue(Objects.equals(STOPPED, certificate.getStatus()), WRONG_CERTIFICATE_STATUS);
        }
        CitizenCertificateDetailsDTO rueiCertificate = rueiClient.updateCertificateStatusByNaif(new RueiUpdateCertificateStatusNaifDTO(certificate.getId(), CertificateStatusDTO.valueOf(dto.getAction().name())));

        if (NaifCertificateStatus.REVOKED.equals(dto.getAction())) {
            ejbcaClient.revokeCertificate(rueiCertificate.getIssuerDN(),
                    new BigInteger(rueiCertificate.getSerialNumber()).toString(16),
                    CESSATION_OF_OPERATION);
        }
        CertificateHistory certificateHistory = certificateMapper.map(rueiCertificate);

        if (Objects.nonNull(dto.getReasonText())) {
            String reasonCode = switch (dto.getAction()) {
                case STOPPED -> "STOPPED_BY_NAIF";
                case REVOKED -> "REVOKED_BY_NAIF";
                case ACTIVE -> "RESUMED_BY_NAIF";
            };
            certificateHistory.setReasonId(nomenclatureService.getReasonByName(reasonCode).getId());
            certificateHistory.setReasonText(dto.getReasonText());
        }

        createCertificateHistory(certificateHistory);
        return certificateMapper.map(dto);
    }

    @Override
    public NaifDeliveredCertificateResponse activateCertificateByNaif(NaifDeliveredCertificateDTO dto) {
        List<PunCarrierDTO> punCarriers = punClient.getPunCarriersBySerialNumber(dto.getCarrierSerialNumber());
        punCarriers.sort((o1, o2) -> o2.getModifiedOn().compareTo(o1.getModifiedOn()));
        PunCarrierDTO punCertificate = punCarriers.get(0);

        EidentityDTO eidentity = reiClient.getEidentityById(punCertificate.getEidentityId());
        assertEquals(eidentity.getCitizenIdentifierNumber(), dto.getPersonalId(), PERSONAL_DETAILS_ARE_INCORRECT);
        assertEquals(eidentity.getCitizenIdentifierType(), IdentifierType.valueOf(dto.getUiDType().name()), PERSONAL_DETAILS_ARE_INCORRECT);

        CitizenCertificateDetailsDTO rueiCertificate = rueiClient.updateCertificateStatusByNaif(new RueiUpdateCertificateStatusNaifDTO(punCertificate.getCertificateId(), CertificateStatusDTO.ACTIVE));

        SsevSendMessageDTO messageRequest = certificateMapper.mapToSseMessageRequest(rueiCertificate, eidentity, dto);
        ssevNotificationService.sendMessage(messageRequest);

        CertificateHistory certificateHistory = certificateMapper.map(rueiCertificate);
        createCertificateHistory(certificateHistory);
        return certificateMapper.map(dto);
    }

    @Override
    public NaifDeviceHistoryResponse getDeviceHistoryByNaif(NaifDeviceHistoryRequest request) {
        List<PunCarrierDTO> punCarriers = punClient.getPunCarriersBySerialNumber(request.getCarrierSerialNumber());
        punCarriers.sort((o1, o2) -> o2.getModifiedOn().compareTo(o1.getModifiedOn()));

        List<UUID> certificateIds = punCarriers.stream().map(PunCarrierDTO::getCertificateId).toList();
        List<CitizenCertificateSummaryDTO> certificates = certificateIds.stream().map(e -> rueiClient.getCertificateById(e)).toList();
        NaifDeviceHistoryResponse response = certificateMapper.map(request);

        certificates.forEach(certificate -> {
            List<CertificateHistory> histories = getCertificateHistoryByCertificateId(certificate.getId());
            response.addCertificateDataItem(certificateMapper.map(certificate, histories));
        });
        return response;
    }


    private boolean isCertificateExpiring(OffsetDateTime currentDateTime, OffsetDateTime validityUntil) {
    	long daysDiff = ChronoUnit.DAYS.between(currentDateTime, validityUntil);

    	return  validityUntil.isAfter(currentDateTime) &&  daysDiff <= daysBeforeIsExpiring;
    }

//    /**
//     * FIXME: this is mock, copy of {@link bg.bulsi.mvr.mpozei.backend.service.impl.ApplicationServiceImpl.findLevelOfAssurance(String)}
//     */
//    private LevelOfAssurance findLevelOfAssurance(String deviceType) {
//    	if(deviceType == null) {
//    		return null;
//    	}
//    	
//        return switch (deviceType) {
//            case Device.IDENTITY_CARD -> LevelOfAssurance.HIGH;
//            case Device.MOBILE -> LevelOfAssurance.SUBSTANTIAL;
//            default -> LevelOfAssurance.SUBSTANTIAL;
//        };
//    }
}
