package bg.bulsi.mvr.extgateway.service;

import bg.bulsi.mvr.audit_logger.BaseAuditLogger;
import bg.bulsi.mvr.audit_logger.dto.AuditData;
import bg.bulsi.mvr.audit_logger.dto.EventPayload;
import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.audit_logger.model.MessageType;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.mpozei.contract.dto.DeviceLogRequestExternal;
import bg.bulsi.mvr.mpozei.contract.dto.DeviceLogRequestExternal.EventTypeEnum;
import bg.bulsi.mvr.mpozei.extgateway.api.v1.DeviceApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;

import java.util.HashMap;
import java.util.Map;
import java.util.UUID;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

@Component
@Slf4j
@RequiredArgsConstructor
public class DeviceDelegateService implements DeviceApiDelegate {
	
	@Autowired
    private EventSender eventSender;
	
	@Autowired
    private BaseAuditLogger auditLogger;
    
    
    @Override
    public Mono<String> submitDeviceLog(DeviceLogRequestExternal deviceLogRequest, ServerWebExchange exchange) {
    	return UserContextHolder.getFromReactiveContext().flatMap(userContext -> {
        	if(EventTypeEnum.SUCCESSFUL_PIN_CHANGE == deviceLogRequest.getEventType()){
        	   EventPayload eventPayload = new EventPayload(new HashMap<>(deviceLogRequest.getEventPayload()));
        		
               return eventSender.send(
                        exchange,
                        UUID.fromString(userContext.getEidentityId()),
                        AuditEventType.valueOf(deviceLogRequest.getEventType().getValue()),
                        String.class,
                        eventPayload,
                        null);        	
            } else {
				AuditData requestAuditEvent = AuditData.builder()
						.correlationId(userContext.getGlobalCorrelationId().toString())
						.eventType(AuditEventType.valueOf(deviceLogRequest.getEventType().getValue()))
						.messageType(MessageType.REQUEST)
						.payload(new EventPayload((Map)deviceLogRequest.getEventPayload()))
						.requesterUserId(userContext.getRequesterUserId())
						.requesterSystemId(userContext.getSystemId())
						.requesterSystemName(userContext.getSystemName())
						.targetUserId(userContext.getRequesterUserId())
						.build();
			
				this.auditLogger.auditEvent(requestAuditEvent);
				
				return Mono.just(HttpStatus.OK.name());
            }
    	});
    }
}
