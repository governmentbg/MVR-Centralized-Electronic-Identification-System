package bg.bulsi.mvr.mpozei.backend.dto;

import lombok.Data;

import java.util.UUID;

@Data
public class RueiVerifyProfileDTO {
    private UUID citizenProfileId;
    private String mobileApplicationInstanceId;
    private String firebaseId;
    private Boolean forceUpdate = false;
}
