package bg.bulsi.mvr.extgateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.mpozei.contract.dto.FindHelpPagesFilter;
import bg.bulsi.mvr.mpozei.contract.dto.HelpPageDTO;
import bg.bulsi.mvr.mpozei.extgateway.api.v1.HelpPageApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.util.ArrayList;
import java.util.List;

@Component
@Slf4j
@RequiredArgsConstructor
public class HelpPageApiDelegateService implements HelpPageApiDelegate {
    private final EventSender eventSender;

    @Override
    public Mono<Page<HelpPageDTO>> findHelpPages(String keyword, String language, ServerWebExchange exchange, Pageable pageable) {
        return eventSender.send(
                exchange,
                new FindHelpPagesFilter(keyword, language, pageable),
                AuditEventType.FIND_HELP_PAGES,
                Page.class).map(e -> (Page<HelpPageDTO>) e);
    }

    @Override
    public Mono<HelpPageDTO> getHelpPageById(String id, ServerWebExchange exchange) {
        return eventSender.send(
                exchange,
                id,
                AuditEventType.GET_HELP_PAGE_BY_ID,
                HelpPageDTO.class);
    }

    @Override
    public Mono<List<HelpPageDTO>> getHelpPagesByPageName(String pageName, ServerWebExchange exchange) {
        return eventSender.send(
                exchange,
                pageName,
                AuditEventType.GET_HELP_PAGE_BY_PAGE_NAME,
                List.class).map(e -> (List<HelpPageDTO>) e);
    }

    @Override
    public Mono<List<HelpPageDTO>> getAllHelpPages(ServerWebExchange exchange) {
        return eventSender.send(
                exchange,
                new ArrayList<>(),
                AuditEventType.FIND_HELP_PAGES,
                List.class).map(e -> (List<HelpPageDTO>) e);
    }
}
