package bg.bulsi.mvr.mpozei.backend.dto;

import lombok.Data;

import java.util.UUID;

@Data
public class EnrollCertificateDTO {
    private UUID applicationId;
    private String certificateSigningRequest;
    private String certificateAuthorityName;
}
