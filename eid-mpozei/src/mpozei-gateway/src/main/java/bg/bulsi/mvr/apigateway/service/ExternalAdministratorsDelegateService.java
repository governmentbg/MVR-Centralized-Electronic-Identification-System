package bg.bulsi.mvr.apigateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationResponse;
import bg.bulsi.mvr.mpozei.contract.dto.CertificateExtAdminRequest;
import bg.bulsi.mvr.mpozei.contract.dto.CertificateResponse;
import bg.bulsi.mvr.mpozei.contract.dto.DeskApplicationRequest;
import bg.bulsi.mvr.mpozei.gateway.api.v1.ExternalAdministratorsApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

@Component
@Slf4j
@RequiredArgsConstructor
public class ExternalAdministratorsDelegateService implements ExternalAdministratorsApiDelegate {
    private final ApplicationApiDelegateService applicationApiDelegateService;
    private final EventSender eventSender;

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_EXT_AEI_m2m')")
    public Mono<ApplicationResponse> createDeskApplication(DeskApplicationRequest request, ServerWebExchange exchange) {
        return applicationApiDelegateService.createDeskApplication(request, true, exchange);
    }

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_EXT_AEI_m2m')")
    public Mono<CertificateResponse> enrollPkcs10Certificate(CertificateExtAdminRequest request, ServerWebExchange exchange) {
        return eventSender.send(
                exchange,
                request,
                AuditEventType.CREATE_CITIZEN_CERTIFICATE,
                CertificateResponse.class);
    }
}
