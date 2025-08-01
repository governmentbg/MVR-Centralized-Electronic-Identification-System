package bg.bulsi.mvr.apigateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.common.util.ValidationUtil;
import bg.bulsi.mvr.mpozei.contract.dto.*;
import bg.bulsi.mvr.mpozei.gateway.api.v1.CertificateApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.time.LocalDate;
import java.util.List;
import java.util.UUID;

@Component
@Slf4j
@RequiredArgsConstructor
public class CertificateApiDelegateService implements CertificateApiDelegate {
    public static final List<String> CERTIFICATE_SORT_FIELDS = List.of("id", "status", "validityFrom", "validityUntil", "deviceId", "serialNumber", "alias");

    private final EventSender eventSender;

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_admin')")
    public Mono<CertificateResponse> enrollPkcs10Certificate(CertificateRequest certificateRequest, ServerWebExchange exchange) {
        return eventSender.send(
                exchange,
                certificateRequest,
                AuditEventType.CREATE_CITIZEN_CERTIFICATE,
                CertificateResponse.class);
    }

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_admin', 'ROLE_EXT_AEI_operator')")
    public Mono<CitizenCertificateSummaryResponse> getCitizenCertificateById(UUID id, ServerWebExchange exchange) {
        return eventSender.send(
                exchange,
                id,
                AuditEventType.GET_CITIZEN_CERTIFICATE_BY_ID,
                CitizenCertificateSummaryResponse.class);
    }

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_admin', 'ROLE_EXT_AEI_operator')")
    public Mono<Page<FindCertificateResponse>> findCitizenCertificates(UUID eidentityId, String id, String serialNumber, List<CertificateStatus> statuses, LocalDate validityFrom, LocalDate validityUntil, List<UUID> deviceIds, String alias, ServerWebExchange exchange, Pageable pageable) {
        pageable = ValidationUtil.filterAllowedPageableSort(pageable, CERTIFICATE_SORT_FIELDS);
        CitizenCertificateFilter filter = new CitizenCertificateFilter(eidentityId, id, serialNumber, statuses, validityFrom, validityUntil, deviceIds, alias, false, pageable);
        Mono<Page> result = eventSender.send(
                exchange,
                filter,
                AuditEventType.FIND_CITIZEN_CERTIFICATES,
                Page.class);
        return result.map(e -> (Page<FindCertificateResponse>) e);
    }

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_admin', 'ROLE_EXT_AEI_operator')")
    public Mono<List<CertificateHistoryDTO>> getCitizenCertificateHistoryById(UUID id, ServerWebExchange exchange) {
            Mono<List> result = eventSender.send(
                exchange,
                id,
                AuditEventType.GET_CERTIFICATE_HISTORY,
                List.class);
            return result.map(e -> (List<CertificateHistoryDTO>) e);
    }

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_admin', 'ROLE_EXT_AEI_m2m')")
    public Mono<ApplicationStatus> confirmCertificateStoredOnDesk(CertificateEidConfirmationRequest request, ServerWebExchange exchange) {
        return eventSender.send(
                exchange,
                request,
                AuditEventType.CONFIRM_DESK_APPLICATION,
                ApplicationStatus.class);
    }
}
