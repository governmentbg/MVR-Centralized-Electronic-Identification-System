package bg.bulsi.mvr.mpozei.backend.dto;

import bg.bulsi.mvr.mpozei.contract.dto.CertificateStatus;
import bg.bulsi.mvr.mpozei.contract.dto.LevelOfAssurance;
import lombok.Data;
import org.springframework.format.annotation.DateTimeFormat;

import java.time.OffsetDateTime;
import java.util.UUID;

@Data
public class CitizenCertificateSummaryDTO {
    private UUID id;

    private CertificateStatus status;

    private UUID eidAdministratorId;

    private UUID eidAdministratorOfficeId;

    private UUID eidentityId;

    private UUID citizenProfileId;

    private UUID deviceId;
    
    private String commonName;

    @DateTimeFormat(iso = DateTimeFormat.ISO.DATE_TIME)
    private OffsetDateTime validityFrom;

    @DateTimeFormat(iso = DateTimeFormat.ISO.DATE_TIME)
    private OffsetDateTime validityUntil;

    @DateTimeFormat(iso = DateTimeFormat.ISO.DATE_TIME)
    private OffsetDateTime createDate;

    private UUID lastModifiedApplicationId;

    private String serialNumber;

    private String issuerDN;

    private LevelOfAssurance levelOfAssurance;
    
    private String alias;
}
