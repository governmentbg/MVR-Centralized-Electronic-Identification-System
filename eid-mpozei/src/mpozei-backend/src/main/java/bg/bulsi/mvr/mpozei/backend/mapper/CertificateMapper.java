package bg.bulsi.mvr.mpozei.backend.mapper;

import bg.bulsi.mvr.mpozei.backend.dto.*;
import bg.bulsi.mvr.mpozei.backend.service.NomenclatureService;
import bg.bulsi.mvr.mpozei.backend.service.RaeiceiService;
import bg.bulsi.mvr.mpozei.contract.dto.*;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import bg.bulsi.mvr.mpozei.model.certificate.CertificateHistory;
import bg.bulsi.mvr.raeicei.contract.dto.DeviceDTO;
import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.NullValueCheckStrategy;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

@Component
@Mapper(componentModel = "spring", nullValueCheckStrategy = NullValueCheckStrategy.ALWAYS)
public abstract class CertificateMapper {
    @Autowired
    private NomenclatureService nomenclatureService;

    @Autowired
    private RaeiceiService raeiceiService;

    public CertificateResponse mapToCertificateResponse(AbstractApplication application) {
        if (application == null) {
            return null;
        }
        CertificateResponse response = new CertificateResponse();
        response.setId(application.getParams().getCertificateId());
        response.setCertificate(application.getParams().getClientCertificate());
        List<String> certificateChain = application.getParams().getClientCertificateChain();
        response.setCertificateChain(certificateChain);
        response.setSerialNumber(application.getParams().getCertificateSerialNumber());
        response.setReplacedExistingCertificate(application.getParams().getReplacedExistingCertificate());
        return response;
    }

    public abstract CitizenCertificateSummaryResponse mapToCertificateSummaryResponse(CitizenCertificateSummaryDTO dto);

    @Mapping(target = "id", ignore = true)
    @Mapping(source = "id", target = "certificateId")
    public abstract CertificateHistory map(CitizenCertificateSummaryDTO certificate);

    public List<CertificateHistoryDTO> map(List<CertificateHistory> certificateHistories) {
        List<CertificateHistoryDTO> historyRecords = new ArrayList<>();
        certificateHistories.forEach(history -> {
            CertificateHistoryDTO item = new CertificateHistoryDTO();
            item.setId(history.getId());
            item.setApplicationNumber(history.getApplicationNumber());
            item.setCreatedDateTime(history.getCreateDate());
            item.setValidityUntil(history.getValidityUntil());
            item.setValidityFrom(history.getValidityFrom());
            item.setStatus(history.getStatus());
            item.setApplicationId(history.getApplicationId());
            item.setModifiedDateTime(history.getModifiedDate());
            item.setReasonId(history.getReasonId());
            item.setReasonText(history.getReasonText());
            historyRecords.add(item);
        });
        return historyRecords;
    }

    public abstract CertificateConfirmDTO map(CertificateBasicProfileConfirmationRequest request);
    public abstract CertificateConfirmDTO map(CertificateEidConfirmationRequest request);

    @Mapping(source = "id", target = "certificateId")
    public abstract CertificateHistory map(CitizenCertificateDetailsDTO dto);

    public NaifUpdateCertificateStatusResponse map(NaifUpdateCertificateStatusDTO dto) {
        if (dto == null) {
            return null;
        }
        NaifUpdateCertificateStatusResponse response = new NaifUpdateCertificateStatusResponse();
        response.setMessageID(UUID.randomUUID());
        response.setMessageRefID(dto.getMessageID());
        response.setUser(dto.getUser());
        response.setTimestamp(dto.getTimestamp());
        response.setResponseStatusCode(0);
        response.setResponseStatusDesc("OK");
        return response;
    }

    public NaifDeviceHistoryResponse map(NaifDeviceHistoryRequest request) {
        if (request == null) {
            return null;
        }
        NaifDeviceHistoryResponse response = new NaifDeviceHistoryResponse();
        response.setMessageID(UUID.randomUUID());
        response.setMessageRefID(request.getMessageID());
        response.setUser(request.getUser());
        response.setTimestamp(request.getTimestamp());
        response.setResponseStatusCode(0);
        response.setResponseStatusDesc("OK");
        response.setCarrierSerialNumber(request.getCarrierSerialNumber());
        return response;
    }

    public NaifDeviceCertificateDTO map(CitizenCertificateSummaryDTO certificate, List<CertificateHistory> histories) {
        NaifDeviceCertificateDTO result = new NaifDeviceCertificateDTO();
        List<NaifDeviceHistoryDTO> historyDTOs = mapAll(histories);
        result.setCreateDate(certificate.getCreateDate());
        result.setSerialNumber(certificate.getSerialNumber());
        result.setStatus(certificate.getStatus());
        result.setStatusData(historyDTOs);
        return result;
    }

    public List<NaifDeviceHistoryDTO> mapAll(List<CertificateHistory> histories) {
        return histories.stream().map(this::map).toList();
    }

    public NaifDeviceHistoryDTO map(CertificateHistory history) {
        if (history == null) {
            return null;
        }
        NaifDeviceHistoryDTO dto = new NaifDeviceHistoryDTO();
        dto.setEventDate(history.getCreateDate());
        dto.setReason(history.getReasonText());
        dto.setStatus(history.getStatus());
        return dto;
    }

	/*
	 * public NaifUpdateCitizenIdentifierResponse map(UpdateCitizenIdentifierDTO
	 * dto) { if (dto == null) { return null; } NaifUpdateCitizenIdentifierResponse
	 * response = new NaifUpdateCitizenIdentifierResponse();
	 * response.setMessageID(UUID.randomUUID());
	 * response.setMessageRefID(dto.getMessageID());
	 * response.setUser(dto.getUser()); response.setResponseStatusCode(0);
	 * response.setResponseStatusDesc("OK");
	 * response.setTimestamp(dto.getTimestamp()); return response; }
	 */

    public NaifInvalidateEidResponse map(InvalidateCitizenEidDTO dto) {
        if (dto == null) {
            return null;
        }
        NaifInvalidateEidResponse response = new NaifInvalidateEidResponse();
        response.setMessageID(UUID.randomUUID());
        response.setMessageRefID(dto.getMessageID());
        response.setUser(dto.getUser());
        response.setResponseStatusCode(0);
        response.setResponseStatusDesc("OK");
        response.setTimestamp(dto.getTimestamp());
        return response;
    }

    public NaifDeliveredCertificateResponse map(NaifDeliveredCertificateDTO dto) {
        if (dto == null) {
            return null;
        }
        NaifDeliveredCertificateResponse response = new NaifDeliveredCertificateResponse();
        response.setMessageID(UUID.randomUUID());
        response.setMessageRefID(dto.getMessageID());
        response.setUser(dto.getUser());
        response.setResponseStatusCode(0);
        response.setResponseStatusDesc("OK");
        response.setTimestamp(dto.getTimestamp());
        return response;
    }

    public PublicCertificateInfo map(RueiPublicCertificateInfo dto) {
        if (dto == null) {
            return null;
        }
        PublicCertificateInfo info = new PublicCertificateInfo();
        info.setStatus(CertificateStatusExternal.valueOf(dto.getStatus().name()));
        info.setCommonName(dto.getCommonName());
        info.setValidityFrom(dto.getValidityFrom());
        info.setValidityUntil(dto.getValidityUntil());
        info.setSerialNumber(dto.getSerialNumber());
        DeviceDTO device = raeiceiService.getDeviceById(dto.getDeviceId());
        info.setCarrierType(device.getName());
        return info;
    }

    public SsevSendMessageDTO mapToSseMessageRequest(CitizenCertificateDetailsDTO certificate, EidentityDTO eidentity, NaifDeliveredCertificateDTO dto) {
        if (certificate == null || eidentity == null || dto == null) {
            return null;
        }

        SsevSendMessageDTO message = new SsevSendMessageDTO();
        message.setApplicationType(ApplicationType.ISSUE_EID);
        message.setIssuerDN(certificate.getIssuerDN());
        message.setCitizenIdentifierNumber(eidentity.getCitizenIdentifierNumber());
        message.setCertificateSerialNumber(certificate.getSerialNumber());
        message.setFirstName(eidentity.getFirstName());
        message.setSecondName(eidentity.getSecondName());
        message.setLastName(eidentity.getLastName());
        message.setEmail(dto.getEmail());
        message.setPhoneNumber(dto.getPhoneNumber());
        message.setEidentityId(eidentity.getId());
        return message;
    }
}
