package bg.bulsi.mvr.mpozei.backend.service.impl;

import bg.bulsi.mvr.audit_logger.dto.EventPayload;
import bg.bulsi.mvr.common.exception.EntityNotFoundException;
import bg.bulsi.mvr.mpozei.backend.client.EjbcaClient;
import bg.bulsi.mvr.mpozei.backend.client.ReiClient;
import bg.bulsi.mvr.mpozei.backend.client.RueiClient;
import bg.bulsi.mvr.mpozei.backend.dto.CertificateStatusDTO;
import bg.bulsi.mvr.mpozei.backend.dto.CitizenCertificateDetailsDTO;
import bg.bulsi.mvr.mpozei.backend.dto.EidentityDTO;
import bg.bulsi.mvr.mpozei.backend.dto.ProfileStatus;
import bg.bulsi.mvr.mpozei.backend.mapper.CertificateMapper;
import bg.bulsi.mvr.mpozei.backend.service.CertificateService;
import bg.bulsi.mvr.mpozei.backend.service.EidentityService;
import bg.bulsi.mvr.mpozei.contract.dto.*;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;

import java.math.BigInteger;
import java.time.Clock;
import java.time.OffsetDateTime;
import java.time.format.DateTimeFormatter;
import java.util.List;
import java.util.function.Consumer;

import static bg.bulsi.mvr.mpozei.backend.dto.ejbca.EjbcaRevocationReason.CESSATION_OF_OPERATION;

@Slf4j
@Service
@RequiredArgsConstructor
public class EidentityServiceImpl implements EidentityService {
    private final ReiClient reiClient;
    private final RueiClient rueiClient;
    private final EjbcaClient ejbcaClient;
    private final CertificateService certificateService;
    private final CertificateMapper certificateMapper;

    @Override
    public NaifInvalidateEidResponse invalidateCitizenEidByNaif(InvalidateCitizenEidDTO dto) {
        EidentityDTO eidentity = reiClient.findEidentityByNumberAndType(dto.getPersonalId(), IdentifierType.valueOf(dto.getUiDType().name()));
        List<CitizenCertificateDetailsDTO> certificates = rueiClient.invalidateCertificatesByEidentityIds(List.of(eidentity.getId()))
                .stream()
                .toList();
        certificates.forEach(e -> {
            ejbcaClient.revokeCertificate(e.getIssuerDN(), new BigInteger(e.getSerialNumber()).toString(16), CESSATION_OF_OPERATION);
            certificateService.createCertificateHistory(certificateMapper.map(e));
        });
        reiClient.updateEidentityActive(eidentity.getId(), false);
        try {
            rueiClient.updateProfileStatusByEidentityId(ProfileStatus.DISABLED, eidentity.getId());
        } catch (Exception e ) {
            log.info(e.getMessage());
            log.info("Profile does not exist. Skipping...");
        }
        return certificateMapper.map(dto);
    }

	@Override
	public void updateCitizenIdentifier(PivrIdchangesResponseDto dto, Consumer<EventPayload> auditEventLogger) {
		try {
			EidentityDTO eidentity = this.reiClient.findEidentityByNumberAndType(dto.getOldPersonalId(),
					dto.getOldUidType());

			List<CitizenCertificateDetailsDTO> certificates = this.rueiClient
					.invalidateCertificatesByEidentityIds(List.of(eidentity.getId()));
			certificates.forEach(e -> ejbcaClient.revokeCertificate(e.getIssuerDN(),
					new BigInteger(e.getSerialNumber()).toString(16), CESSATION_OF_OPERATION));

			certificates.forEach(e -> this.certificateService.createCertificateHistory(certificateMapper.map(e)));

			// TODO: Update CitizenIdentifier in RUEI, when Endpoint becomes available

			this.reiClient.updateCitizenIdentifier(new ReiUpdateCitizenIdentifierDTO(
					dto.getOldPersonalId(), dto.getOldUidType(), dto.getNewPersonalId(), dto.getNewUidType()));

			EventPayload eventPayload = new EventPayload();
			eventPayload.setEidentityId(eidentity.getId().toString());
			eventPayload.setTargetName(eidentity.getFirstName(), eidentity.getSecondName(), eidentity.getLastName());
			eventPayload.setTargetUid(dto.getNewPersonalId());
			eventPayload.setTargetUidType(dto.getNewUidType().name());
			eventPayload.setRequestBody(dto);

			auditEventLogger.accept(eventPayload);
		} catch (Exception e) {
			log.error(".updateCitizenIdentifier() Could not get citizen by [citizenIdentifier={}] [citizenIdentifierType={}]",
					dto.getOldPersonalId(), dto.getOldUidType(), e);
		}
	}
}
