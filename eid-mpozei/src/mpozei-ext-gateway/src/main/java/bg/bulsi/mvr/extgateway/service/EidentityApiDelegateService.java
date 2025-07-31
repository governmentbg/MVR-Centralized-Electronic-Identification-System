package bg.bulsi.mvr.extgateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.mpozei.contract.dto.EidentityExternalResponse;
import bg.bulsi.mvr.mpozei.contract.dto.EidentityResponse;
import bg.bulsi.mvr.mpozei.contract.dto.HttpResponse;
import bg.bulsi.mvr.mpozei.contract.dto.ProfileVerifyLoginRequest;
import bg.bulsi.mvr.mpozei.extgateway.api.v1.EidentityApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.util.HashMap;
import java.util.UUID;


import  bg.bulsi.mvr.common.exception.ErrorCode;
import  bg.bulsi.mvr.common.util.ValidationUtil;

@Component
@Slf4j
@RequiredArgsConstructor
public class EidentityApiDelegateService implements EidentityApiDelegate {
    private final EventSender eventSender;

    @Override
    public Mono<HttpResponse> verifyCitizenProfileLogin(ProfileVerifyLoginRequest request, ServerWebExchange exchange) {
        return eventSender.send(
                exchange,
                request,
                AuditEventType.VERIFY_CITIZEN_PROFILE,
                HttpResponse.class);
    }

    @Override
    public Mono<EidentityExternalResponse> getEidentityById(ServerWebExchange exchange) {
		return UserContextHolder.getFromReactiveContext().flatMap(context -> {
			ValidationUtil.assertNotNull(context.getCitizenProfileId(), ErrorCode.CITIZEN_PROFILE_ID_CANNOT_BE_NULL);
			UUID citizenProfileId = UUID.fromString(context.getCitizenProfileId());

			log.info(".getEidentityById() [RequestID={}, citizenProfileId={}]",exchange.getRequest().getId(), citizenProfileId);
			return eventSender.send(
					exchange,
					citizenProfileId,
					AuditEventType.GET_EIDENTITY_BY_ID,
					EidentityExternalResponse.class);
		});
    }

    @Override
    public Mono<HttpResponse> attachEidentity(UUID citizenProfileId, ServerWebExchange exchange) {
        return eventSender.send(
                exchange,
                citizenProfileId,
                AuditEventType.ASSOCIATE_PROFILES,
                HttpResponse.class);
    }
}
