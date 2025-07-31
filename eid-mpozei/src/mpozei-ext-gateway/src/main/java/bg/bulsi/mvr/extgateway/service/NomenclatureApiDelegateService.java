package bg.bulsi.mvr.extgateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus;
import bg.bulsi.mvr.mpozei.contract.dto.CertificateStatus;
import bg.bulsi.mvr.mpozei.contract.dto.CertificateStatusExternal;
import bg.bulsi.mvr.mpozei.contract.dto.NomenclatureTypeDTO;
import bg.bulsi.mvr.mpozei.extgateway.api.v1.NomenclatureApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;

@Slf4j
@Service
@RequiredArgsConstructor
public class NomenclatureApiDelegateService implements NomenclatureApiDelegate {
    private final EventSender eventSender;

    @Override
    public Mono<List<NomenclatureTypeDTO>> getAllReasons(ServerWebExchange exchange) {
        Mono<List> result = eventSender.send(
                exchange,
                new HashMap<>(),
                AuditEventType.GET_ALL_REASONS,
                List.class);
        return result.map(e -> new ArrayList<NomenclatureTypeDTO>(e));
    }

    //TODO: check if the other methods {@link NomenclatureApiDelegate} need to be implemented
}
