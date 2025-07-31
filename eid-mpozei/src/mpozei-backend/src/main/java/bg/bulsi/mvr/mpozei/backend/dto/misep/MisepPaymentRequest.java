package bg.bulsi.mvr.mpozei.backend.dto.misep;

import bg.bulsi.mvr.mpozei.contract.dto.IdentifierType;
import lombok.Data;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

@Data
public class MisepPaymentRequest {
    private UUID citizenProfileId;
    private Request request = new Request();

    @Data
    public static class Actor {
        private static final String participantType = "APPLICANT";
        private static final String type = "PERSON";
        private Uid uid = new Uid();
        private String name;
        private Info info = new Info();
    }

    @Data
    public static class Address {
        private String country;
        private String city;
        private String state;
        private String zip;
        private String address;
    }

    @Data
    public static class Contact {
        private String phone;
        private String email;
        private Address address = new Address();
    }

    @Data
    public static class Info {
        private Contact contacts = new Contact();
        private MisepBankAccount bankAccount = new MisepBankAccount();
    }

    @Data
    public static class PaymentData {
        private String paymentId;
        private String currency;
        private Double amount;
        private MisepPaymentStatusType status = MisepPaymentStatusType.Pending;
        private Integer typeCode = 0;
        private String referenceNumber;
        private String referenceType;
        private String referenceDate;
        private String expirationDate;
        private String createDate;
        private String reason;
        private String additionalInformation;
        private String administrativeServiceUri;
        private String administrativeServiceSupplierUri;
        private String administrativeServiceNotificationURL;
    }

    @Data
    public static class Request {
        private ArrayList<Actor> actors = new ArrayList<>();
        private PaymentData paymentData = new PaymentData();
    }

    @Data
    public static class Uid {
        private IdentifierType type;
        private String value;
    }
}
