package bg.bulsi.mvr.apigateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.mpozei.contract.dto.EidentityResponse;
import bg.bulsi.mvr.mpozei.contract.dto.IdentifierType;
import bg.bulsi.mvr.mpozei.gateway.api.v1.EidentityApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.util.HashMap;
import java.util.Map;
import java.util.UUID;

@Component
@Slf4j
@RequiredArgsConstructor
public class EidentityApiDelegateService implements EidentityApiDelegate {
    private final EventSender eventSender;

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_admin', 'ROLE_EXT_AEI_operator')")
    public Mono<EidentityResponse> getEidentityByField(String email, UUID citizenProfileId, String number, IdentifierType type, ServerWebExchange exchange) {
        Map<String, Object> payload = new HashMap<>();
        payload.put("email", email);
        payload.put("citizenIdentifierNumber", number);
        payload.put("citizenIdentifierType", type);
        payload.put("citizenProfileId", citizenProfileId);
        return eventSender.send(
                exchange,
                payload,
                AuditEventType.GET_EIDENTITIES_BY_FIELD,
                EidentityResponse.class);
    }

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_admin', 'ROLE_EXT_AEI_operator')")
    public Mono<EidentityResponse> getEidentityById(UUID id, ServerWebExchange exchange) {
        return eventSender.send(
                exchange,
                id,
                AuditEventType.GET_EIDENTITY_BY_ID,
                EidentityResponse.class);
    }
}
