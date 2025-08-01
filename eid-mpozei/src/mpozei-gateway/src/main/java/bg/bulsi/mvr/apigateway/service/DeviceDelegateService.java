package bg.bulsi.mvr.apigateway.service;

import bg.bulsi.mvr.audit_logger.BaseAuditLogger;
import bg.bulsi.mvr.audit_logger.dto.AuditData;
import bg.bulsi.mvr.audit_logger.dto.EventPayload;
import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.audit_logger.model.MessageType;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.crypto.CertificateProcessor;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.mpozei.contract.dto.DeviceLogRequestInternal;
import bg.bulsi.mvr.mpozei.contract.dto.DeviceLogRequestInternal.EventTypeEnum;
import bg.bulsi.mvr.mpozei.contract.dto.DeviceLogRequestInternal.MessageTypeEnum;
import bg.bulsi.mvr.mpozei.contract.dto.DevicePukRequest;
import bg.bulsi.mvr.mpozei.gateway.api.v1.DeviceApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;

import java.security.NoSuchProviderException;
import java.security.cert.CertificateException;
import java.security.cert.X509Certificate;
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

	@Autowired
	private CertificateProcessor certificateProcessor;

	@Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_admin')")
	public Mono<String> getDevicePuk(DevicePukRequest request, ServerWebExchange exchange) {
		return eventSender.send(exchange, request, AuditEventType.GET_DEVICE_PUK, String.class);
	}

	@Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_admin')")
	public Mono<String> submitDeviceLog(DeviceLogRequestInternal deviceLogRequest, ServerWebExchange exchange) {
		// TODO: should validate some of the fields, for example RequestId, TargetUserId
		return UserContextHolder.getFromReactiveContext().flatMap(userContext -> {
			if (EventTypeEnum.CHANGE_PIN == deviceLogRequest.getEventType()
					&& MessageTypeEnum.SUCCESS == deviceLogRequest.getMessageType()) {

				try {
					X509Certificate parsedCertificate = this.certificateProcessor
							.extractCertificate(deviceLogRequest.getCertificate().getBytes());

					UUID eid = UUID.fromString(this.certificateProcessor.getSubjectEid(parsedCertificate));

				return eventSender.send(
							exchange, 
							eid, 
							AuditEventType.valueOf(deviceLogRequest.getEventType().getValue()),
							String.class);
				} catch (CertificateException | NoSuchProviderException e) {
					log.error(".submitDeviceLog() Unable to extract EID from X509Certificate", e);
				}
			} else {
				AuditData requestAuditEvent = AuditData.builder()
						.correlationId(userContext.getGlobalCorrelationId().toString())
						.eventType(AuditEventType.valueOf(deviceLogRequest.getEventType().getValue()))
						.messageType(MessageType.valueOf(deviceLogRequest.getMessageType().getValue()))
						.payload(new EventPayload((Map) deviceLogRequest.getEventPayload()))
						.requesterUserId(userContext.getRequesterUserId()).requesterSystemId(userContext.getSystemId())
						.requesterSystemName(userContext.getSystemName()).build();

				this.auditLogger.auditEvent(requestAuditEvent);
			}
			
			return Mono.just(HttpStatus.OK.getReasonPhrase());
		});
	}
}
