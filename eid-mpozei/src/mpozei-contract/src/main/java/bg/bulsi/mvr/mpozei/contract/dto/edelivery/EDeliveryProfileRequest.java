package bg.bulsi.mvr.mpozei.contract.dto.edelivery;

import lombok.Data;

@Data
public class EDeliveryProfileRequest {
    private String identifier;
    private String firstName;
    private String middleName;
    private String lastName;
    private String email;
    private String phone;
    private ProfileAddress address = new ProfileAddress();

    @Data
    public static class ProfileAddress {
        private String residence;
        private String city;
        private String state;
        private String countryIso;
    }
}
