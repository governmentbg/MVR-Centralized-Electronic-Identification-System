package bg.bulsi.mvr.mpozei.backend.dto;

import bg.bulsi.mvr.mpozei.contract.dto.ApplicationType;
import bg.bulsi.mvr.mpozei.model.pan.EDeliveryStatus;
import lombok.Data;

import java.util.UUID;

@Data
public class SsevSendMessageDTO {
    private String citizenIdentifierNumber;
    private String firstName;
    private String secondName;
    private String lastName;
    private String email;
    private String phoneNumber;
    private String issuerDN;
    private String certificateSerialNumber;
    private UUID eidentityId;
    private ApplicationType applicationType;
    private String eDeliveryProfileId;
    private EDeliveryStatus eDeliveryStatus;
}
