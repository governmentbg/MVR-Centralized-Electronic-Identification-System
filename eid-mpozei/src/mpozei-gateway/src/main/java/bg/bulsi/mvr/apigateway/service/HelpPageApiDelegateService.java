package bg.bulsi.mvr.apigateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.mpozei.contract.dto.FindHelpPagesFilter;
import bg.bulsi.mvr.mpozei.contract.dto.HelpPageDTO;
import bg.bulsi.mvr.mpozei.contract.dto.HttpResponse;
import bg.bulsi.mvr.mpozei.gateway.api.v1.HelpPageApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

@Component
@Slf4j
@RequiredArgsConstructor
public class HelpPageApiDelegateService implements HelpPageApiDelegate {
    private final EventSender eventSender;

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_admin')")
    public Mono<HelpPageDTO> createHelpPage(HelpPageDTO dto, ServerWebExchange exchange) {
        return eventSender.send(
                exchange,
                dto,
                AuditEventType.CREATE_HELP_PAGE,
                HelpPageDTO.class);
    }

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_admin')")
    public Mono<Page<HelpPageDTO>> findHelpPages(String keyword, String language, ServerWebExchange exchange, Pageable pageable) {
        return eventSender.send(
                exchange,
                new FindHelpPagesFilter(keyword, language, pageable),
                AuditEventType.FIND_HELP_PAGES,
                Page.class).map(e -> (Page<HelpPageDTO>) e);
    }

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_admin')")
    public Mono<HelpPageDTO> getHelpPageById(String id, ServerWebExchange exchange) {
        return eventSender.send(
                exchange,
                id,
                AuditEventType.GET_HELP_PAGE_BY_ID,
                HelpPageDTO.class);
    }

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_admin')")
    public Mono<HttpResponse> deleteHelpPageById(String id, ServerWebExchange exchange) {
        return eventSender.send(
                exchange,
                id,
                AuditEventType.DELETE_HELP_PAGE,
                HttpResponse.class);
    }

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_admin')")
    public Mono<HelpPageDTO> updateHelpPage(HelpPageDTO dto, ServerWebExchange exchange) {
        return eventSender.send(
                exchange,
                dto,
                AuditEventType.UPDATE_HELP_PAGE,
                HelpPageDTO.class);
    }

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_admin')")
    public Mono<List<HelpPageDTO>> getAllHelpPages(ServerWebExchange exchange) {
        return eventSender.send(
                exchange,
                new ArrayList<>(),
                AuditEventType.FIND_HELP_PAGES,
                List.class).map(e -> (List<HelpPageDTO>) e);
    }
}
