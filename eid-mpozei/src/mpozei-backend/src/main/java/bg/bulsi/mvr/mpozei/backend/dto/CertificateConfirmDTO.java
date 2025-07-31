package bg.bulsi.mvr.mpozei.backend.dto;

import bg.bulsi.mvr.mpozei.contract.dto.ApplicationConfirmationStatus;
import lombok.Data;

import java.util.UUID;

@Data
public class CertificateConfirmDTO {
    private UUID applicationId;
    private ApplicationConfirmationStatus status;
    private String reason;
    private String reasonText;
}
