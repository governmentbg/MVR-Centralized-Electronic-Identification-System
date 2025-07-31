package bg.bulsi.mvr.mpozei.backend.dto;

import lombok.AllArgsConstructor;
import lombok.Data;

import java.util.UUID;

@Data
@AllArgsConstructor
public class RueiUpdateCertificateStatusNaifDTO {
    private UUID certificateId;
    private CertificateStatusDTO status;
}
