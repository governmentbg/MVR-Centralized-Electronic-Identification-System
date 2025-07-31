package bg.bulsi.mvr.audit_logger.model;

import com.fasterxml.jackson.annotation.JsonPropertyOrder;

import lombok.Data;


@JsonPropertyOrder(
		{ "eventId", "systemId", "eventDate" , "checksum" , "sourceFile" , 
		  "eventType" , "correlationId" , "message" , "requesterUserId" , "requesterSystemId" , 
		  "requesterSystemName" , "targetUserId" , "moduleId", "eventPayload"  })
@Data
public class AuditLogRecord {
    private String systemId;
    private String eventId;
    private String eventDate;
    private String checksum;
    private String sourceFile;
    private String eventType;
    private String correlationId;
    private String requesterUserId;
    private String requesterSystemId;
    private String requesterSystemName;
    private String targetUserId;
    private String moduleId;
    private String message;
    private Object eventPayload;
}
