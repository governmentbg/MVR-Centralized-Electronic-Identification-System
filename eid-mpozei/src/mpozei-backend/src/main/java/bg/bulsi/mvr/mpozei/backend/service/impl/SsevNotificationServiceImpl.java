package bg.bulsi.mvr.mpozei.backend.service.impl;

import bg.bulsi.mvr.common.crypto.CertificateProcessor;
import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.FaultMVRException;
import bg.bulsi.mvr.mpozei.backend.client.EDeliveryClient;
import bg.bulsi.mvr.mpozei.backend.client.RueiClient;
import bg.bulsi.mvr.mpozei.backend.dto.CitizenCertificateDetailsDTO;
import bg.bulsi.mvr.mpozei.backend.dto.SsevSendMessageDTO;
import bg.bulsi.mvr.mpozei.backend.mapper.ApplicationMapper;
import bg.bulsi.mvr.mpozei.backend.service.SsevNotificationService;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationType;
import bg.bulsi.mvr.mpozei.contract.dto.edelivery.*;
import bg.bulsi.mvr.mpozei.model.pan.EDeliveryStatus;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.apache.commons.io.FileUtils;
import org.springframework.stereotype.Service;

import java.io.File;
import java.util.List;
import java.util.Objects;

@Service
@RequiredArgsConstructor
@Slf4j
public class SsevNotificationServiceImpl implements SsevNotificationService {
    private final static String MESSAGE_TITLE = "Вашата електронна идентичност";
    private final static String MESSAGE_CONTENT = "Извършихте успешно %s на електронна идентичност.";
    private final EDeliveryClient eDeliveryClient;
    private final RueiClient rueiClient;
    private final ApplicationMapper applicationMapper;

    public static String createMessageContent(ApplicationType applicationType) {
        String action = switch (applicationType) {
            case ISSUE_EID -> "издаване";
            case STOP_EID -> "спиране";
            case RESUME_EID -> "възобновяване";
            case REVOKE_EID -> "прекратяване";
        };
        return String.format(MESSAGE_CONTENT, action);
    }

    public static String createMessageTitle() {
        return String.format(MESSAGE_TITLE);
    }

    @Override
    public SsevSendMessageDTO sendMessage(SsevSendMessageDTO dto) {
        if (Objects.isNull(dto.getEmail()) || Objects.isNull(dto.getPhoneNumber())) {
            dto.setEDeliveryStatus(EDeliveryStatus.PROFILE_DATA_NOT_PROVIDED);
            return dto;
        }
    	
        File file = null;
        try {
            createProfileIfNotExists(dto);

            EDeliveryMessageRequest message = applicationMapper.mapToEDeliveryMessageRequest(dto);
            CitizenCertificateDetailsDTO certificate = rueiClient.getCitizenCertificateByIssuerAndSN(dto.getIssuerDN(), dto.getCertificateSerialNumber());

            String tempDir = System.getProperty("java.io.tmpdir");
            file = new File(tempDir, "certificate.crt");
            FileUtils.write(file, getFullCertificate(certificate.getCertificate(), certificate.getCertificateCA()), "UTF-8");

            EDeliveryAttachmentResponse attachmentResponse = eDeliveryClient.uploadFile(file, dto.getEidentityId());
            if (attachmentResponse.getHasFailed()) {
                throw new FaultMVRException(attachmentResponse.getError(), ErrorCode.VALIDATION_ERROR);
            }
            message.setBlobId(attachmentResponse.getBlobId());
            eDeliveryClient.sendMessage(message, dto.getEidentityId());
        } catch (Exception e) {
            log.error("There was a problem during sending the notification \n {}", e);
            dto.setEDeliveryStatus(EDeliveryStatus.UNKNOWN_ERROR);
            return dto;
        } finally {
            if (file != null && file.exists()) {
                boolean deleted = file.delete();
                log.info("Temporary file deleted: {}", deleted);
            }
        }
        dto.setEDeliveryStatus(EDeliveryStatus.SUCCESS);
        return dto;
    }

    private void createProfileIfNotExists(SsevSendMessageDTO dto) {
        try {
            EDeliverySearchProfileDTO existingProfile = eDeliveryClient.searchProfile(dto.getEidentityId(), dto.getCitizenIdentifierNumber(), EDeliverySearchProfileDTO.PROFILE_TARGET_GROUP_ID);
            dto.setEDeliveryProfileId(existingProfile.getProfileId());
            return;
        } catch (Exception e) {
            log.error("No profile was found. Will continue creating profile. [Exception={}]", e);            
        }

        EDeliveryProfileRequest profileRequest = applicationMapper.mapToEDeliveryProfileRequest(dto);
        EDeliveryProfileResponse profile = eDeliveryClient.createPassiveIndividualProfile(profileRequest, dto.getEidentityId());
        dto.setEDeliveryProfileId(profile.getProfileId());
    }

    private String getFullCertificate(String certificate, List<String> caChain) {
        StringBuilder fullCert = new StringBuilder();
        fullCert.append(CertificateProcessor.convertDerBase64ToPEM(certificate));

        for (String chainCert : caChain) {
            fullCert.append(CertificateProcessor.convertDerBase64ToPEM(chainCert));
        }
        return fullCert.toString();
    }
}


