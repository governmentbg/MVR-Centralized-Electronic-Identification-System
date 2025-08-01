package bg.bulsi.mvr.mpozei.backend.dto;

import bg.bulsi.mvr.mpozei.contract.dto.LevelOfAssurance;
import lombok.Data;

import java.util.List;
import java.util.UUID;

@Data
public class CitizenCertificateDTO {
    private String certificate;
    private List<String> certificateCA;
    private UUID eidentityId;
    private UUID lastModifiedApplicationId;
    private UUID eidAdministratorId;
    private UUID eidAdministratorOfficeId;
    private LevelOfAssurance levelOfAssurance;
    private UUID deviceId;
}