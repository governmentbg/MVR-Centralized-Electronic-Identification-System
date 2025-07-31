package bg.bulsi.mvr.apigateway.service;

import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.UUID;

import bg.bulsi.mvr.raeicei.contract.dto.DocumentDTO;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.raeicei.contract.dto.EmployeeByUidResult;
import bg.bulsi.mvr.raeicei.contract.dto.EmployeeDTO;
import bg.bulsi.mvr.raeicei.contract.dto.IdentifierTypeDTO;
import bg.bulsi.mvr.raeicei.gateway.api.v1.EmployeeServiceApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import reactor.core.publisher.Mono;

@Component
@Slf4j
@RequiredArgsConstructor
public class EmployeeService implements EmployeeServiceApiDelegate {

	private final EventSender eventSender;

	@Override
	public Mono<EmployeeByUidResult> checkEmployee4AdministratorId(UUID id, String uid, IdentifierTypeDTO uidType,
			ServerWebExchange exchange) {
		return eventSender.send(exchange, buildPayload(id,uid,uidType), AuditEventType.CHEK_AIE_EMPLOYEE, EmployeeByUidResult.class);
	}

	@Override
	public Mono<EmployeeByUidResult> checkEmployee4CenterId(UUID id, String uid, IdentifierTypeDTO uidType,
			ServerWebExchange exchange) {
		return eventSender.send(exchange, buildPayload(id,uid,uidType), AuditEventType.CHEK_CEI_EMPLOYEE, EmployeeByUidResult.class);
	}

	@Override
	public Mono<Page<EmployeeDTO>> getAllEmployees(UUID systemId, ServerWebExchange exchange,Pageable pageable) {
		  Mono<Page> result = eventSender.send(
	                exchange,
	                Map.of("systemId", systemId, "pageable", pageable) ,
	                AuditEventType.GET_ALL_EMPLOYEE_BY_SYSTEM_ID,
	                Page.class);
	        return result.map(e -> (Page<EmployeeDTO>) e);
	}

	public HashMap<String, Object>buildPayload(UUID id, String uid, IdentifierTypeDTO uidType){
		HashMap<String, Object> payload= new HashMap<>(3);
		payload.put("id", id);
		payload.put("uid", uid);
		payload.put("uidType", uidType);
		return payload;
	}
}
