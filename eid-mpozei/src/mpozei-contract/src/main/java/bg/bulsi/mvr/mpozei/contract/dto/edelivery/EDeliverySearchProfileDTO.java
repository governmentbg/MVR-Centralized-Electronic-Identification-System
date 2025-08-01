package bg.bulsi.mvr.mpozei.contract.dto.edelivery;

import lombok.Data;

@Data
public class EDeliverySearchProfileDTO {
    public final static String PROFILE_TARGET_GROUP_ID = "1";

    private String profileId;
    private String identifier;
    private String name;
    private String email;
    private String phone;
}
