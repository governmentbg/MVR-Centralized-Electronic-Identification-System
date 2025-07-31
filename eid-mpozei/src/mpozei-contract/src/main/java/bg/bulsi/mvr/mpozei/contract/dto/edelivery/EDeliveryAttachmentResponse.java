package bg.bulsi.mvr.mpozei.contract.dto.edelivery;

import lombok.Data;

@Data
public class EDeliveryAttachmentResponse {
    private String hash;
    private String hashAlgorithm;
    private Boolean hasFailed;
    private String error;
    private String name;
    private String size;
    private Long blobId;
    private String malwareScanStatus;
    private String signatureStatus;
    private String errorStatus;
}
