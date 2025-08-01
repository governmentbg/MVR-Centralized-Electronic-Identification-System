package bg.bulsi.mvr.mpozei.model.application;

import bg.bulsi.mvr.mpozei.contract.dto.LevelOfAssurance;
import bg.bulsi.mvr.mpozei.model.pan.EDeliveryStatus;
import lombok.Data;

import java.time.LocalDateTime;
import java.util.List;
import java.util.UUID;

@Data
public class ApplicationParams {
    private String email;
    private String clientCertificate;
    private List<String> clientCertificateChain;
    private String issuerDn;
    private String certificateCaName;
    private String endEntityProfileName;
    //Must be in decimal format
    private String certificateSerialNumber;
    private String certificateSigningRequest;
    private UUID certificateId;
    private UUID citizenProfileId;
    private String phoneNumber;
    private String dateOfBirth;
    private LevelOfAssurance levelOfAssurance;
    //TODO: Check if this is used anywhere
    private Boolean requireGuardians = false;
    private Boolean isOnlineOffice;
    private String mobileApplicationInstanceId;
    private String region;
    private Boolean replacedExistingCertificate = false;
    private UUID misepPaymentId;
    private UUID mpozeiPaymentId;
    private String paymentAccessCode;
    private Double fee;
    private String feeCurrency;
    private Double secondaryFee;
    private String secondaryFeeCurrency;
    private LocalDateTime paymentDeadline;
    private String deviceSerialNumber;
    private String eDeliveryProfileId;
    private EDeliveryStatus eDeliveryStatus;
    private String numForm;
}
