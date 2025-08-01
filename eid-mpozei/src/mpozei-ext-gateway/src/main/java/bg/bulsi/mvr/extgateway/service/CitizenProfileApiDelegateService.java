package bg.bulsi.mvr.extgateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.extgateway.util.PasswordUtil;
import bg.bulsi.mvr.mpozei.contract.dto.*;
import bg.bulsi.mvr.mpozei.extgateway.api.v1.CitizenProfileApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.util.UUID;

import static bg.bulsi.mvr.common.exception.ErrorCode.PASSWORDS_DONT_MATCH;
import static bg.bulsi.mvr.common.util.ValidationUtil.assertEquals;


@Component
@Slf4j
@RequiredArgsConstructor
public class CitizenProfileApiDelegateService implements CitizenProfileApiDelegate {
    private final EventSender eventSender;

    @Override
    public Mono<HttpResponse> createCitizenProfile(CitizenProfileRegistrationDTO dto, ServerWebExchange exchange) {
        return eventSender.send(
                exchange,
                dto,
                AuditEventType.CREATE_CITIZEN_PROFILE,
                HttpResponse.class);
    }

    @Override
    public Mono<HttpResponse> forgottenPassword(ForgottenPasswordDTO dto, ServerWebExchange exchange) {
        return eventSender.send(
                exchange,
                dto,
                AuditEventType.FORGOT_CITIZEN_PROFILE_PASSWORD,
                HttpResponse.class);
    }

//    @Override
//    public Mono<CitizenProfileResponse> getCitizenProfile(ServerWebExchange exchange) {
//        return eventSender.send(
//                exchange,
//                new HashMap<>(),
//                AuditEventType.GET_CITZEN_PROFILE,
//                CitizenProfileResponse.class);
//    }

    @Override
    public Mono<HttpResponse> resetPassword(String password,
                                      String confirmPassword,
                                      String token,
                                      ServerWebExchange exchange) {
        assertEquals(password, confirmPassword, PASSWORDS_DONT_MATCH);
        ResetPaswordDTO dto = new ResetPaswordDTO(password, token);
        return eventSender.send(
                exchange,
                dto,
                AuditEventType.POST_RESET_CITIZEN_PASSWORD,
                HttpResponse.class);
    }

    @Override
    public Mono<HttpResponse> updateCitizenProfile(CitizenProfileUpdateRequest request, ServerWebExchange exchange) {
        return eventSender.send(
                exchange,
                request,
                AuditEventType.UPDATE_CITIZEN_PROFILE,
                HttpResponse.class);
    }

    @Override
    public Mono<HttpResponse> updatePassword(String oldPassword, String newPassword, String confirmPassword, ServerWebExchange exchange) {
        assertEquals(newPassword, confirmPassword, PASSWORDS_DONT_MATCH);
        return eventSender.send(
                exchange,
                new UpdateProfilePasswordDTO(oldPassword, newPassword),
                AuditEventType.UPDATE_CITIZEN_PASSWORD,
                HttpResponse.class);
    }

    @Override
    public Mono<HttpResponse> verifyCitizenProfile(UUID token, ServerWebExchange exchange) {
        return eventSender.send(
                exchange,
                token,
                AuditEventType.VERIFY_CITIZEN_PROFILE,
                HttpResponse.class);
    }

    @Override
    public Mono<HttpResponse> updateEmail(String email, ServerWebExchange exchange) {
        return eventSender.send(
                exchange,
                email,
                AuditEventType.UPDATE_CITIZEN_PROFILE_EMAIL,
                HttpResponse.class);
    }

    @Override
    public Mono<HttpResponse> confirmUpdateEmail(UUID token, ServerWebExchange exchange) {
        return eventSender.send(
                exchange,
                token,
                AuditEventType.CONFIRM_UPDATE_CITIZEN_PROFILE_EMAIL,
                HttpResponse.class);
    }
}
