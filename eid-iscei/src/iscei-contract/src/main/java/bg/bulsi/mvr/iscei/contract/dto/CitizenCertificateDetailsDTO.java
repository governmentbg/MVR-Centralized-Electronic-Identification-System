package bg.bulsi.mvr.iscei.contract.dto;

import lombok.Data;
import org.springframework.format.annotation.DateTimeFormat;

import java.time.OffsetDateTime;
import java.util.List;
import java.util.UUID;

@Data
public class CitizenCertificateDetailsDTO {
    @DateTimeFormat(iso = DateTimeFormat.ISO.DATE_TIME)
    private OffsetDateTime validityFrom;

    @DateTimeFormat(iso = DateTimeFormat.ISO.DATE_TIME)
    private OffsetDateTime validityUntil;

    @DateTimeFormat(iso = DateTimeFormat.ISO.DATE_TIME)
    private OffsetDateTime createDate;

    @DateTimeFormat(iso = DateTimeFormat.ISO.DATE_TIME)
    private OffsetDateTime modifiedDate;

    private UUID id;
    private List<String> certificateCA;
    private String serialNumber;
    private String issuerDN;
    private CertificateStatusDTO status;
    private UUID citizenProfileId;
    private String certificate;
    private UUID eidentityId;
    private UUID lastModifiedApplicationId;
    private UUID eidAdministratorId;
    private LevelOfAssurance levelOfAssurance;
    private UUID deviceId;
}