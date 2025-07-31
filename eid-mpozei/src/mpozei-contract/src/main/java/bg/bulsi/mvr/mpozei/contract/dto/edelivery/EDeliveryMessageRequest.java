package bg.bulsi.mvr.mpozei.contract.dto.edelivery;

import lombok.Data;

import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.UUID;

@Data
public class EDeliveryMessageRequest {
    public final static String MESSAGE_TEMPLATE_ID = "1";
    public final static UUID MESSAGE_CONTENT_ID = UUID.fromString("179ea4dc-7879-43ad-8073-72b263915656");
    public final static UUID BLOB_ID = UUID.fromString("e2135802-5e34-4c60-b36e-c86d910a571a");

    private List<Long> recipientProfileIds;
    private String subject;
    private String referencedOrn;
    private String additionalIdentifier;
    private String templateId = MESSAGE_TEMPLATE_ID;
    private Map<UUID, Object> fields = new HashMap<>();

    public void setContent(String content) {
        fields.put(MESSAGE_CONTENT_ID, content);
    }

    public void setBlobId(Long blobId) {
        fields.put(BLOB_ID, List.of(blobId));
    }
}
