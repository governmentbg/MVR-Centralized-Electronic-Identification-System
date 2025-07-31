package bg.bulsi.mvr.mpozei.backend.dto;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.util.UUID;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class CitizenCertificateUpdateDTO {
    private String serialNumber;
    private String issuerDN;
    private CertificateStatusDTO status;
    private UUID eidentityId;
    private UUID lastModifiedApplicationId;
}
