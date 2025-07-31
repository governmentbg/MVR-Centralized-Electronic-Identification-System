package bg.bulsi.mvr.audit_logger.dto;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.audit_logger.model.MessageType;
import lombok.Builder;
import lombok.Getter;

@Builder
@Getter
public class AuditData {
    private String correlationId;
    private AuditEventType eventType;
    //Opensearch reads the payload only, if in format key-pair values (Map, Object)
    private EventPayload payload;
    private MessageType messageType;
    private String requesterUserId;
    private String requesterSystemId;
    private String requesterSystemName;
    private String targetUserId;
}
