package bg.bulsi.mvr.apigateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.raeicei.contract.dto.DocumentTypeDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DocumentTypeResponseDTO;
import bg.bulsi.mvr.raeicei.contract.dto.HttpResponse;
import bg.bulsi.mvr.raeicei.gateway.api.v1.DocumentTypeApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.util.HashMap;
import java.util.List;

@Component
@Slf4j
@RequiredArgsConstructor
public class DocumentTypeService implements DocumentTypeApiDelegate {

    private final EventSender eventSender;

    @Override
    public Mono<DocumentTypeResponseDTO> createDocumentType(DocumentTypeDTO documentTypeDTO, ServerWebExchange exchange) {
        return eventSender.send(exchange, documentTypeDTO, AuditEventType.CREATE_DOCUMENT_TYPE, DocumentTypeResponseDTO.class);
    }

    @Override
    public Mono<DocumentTypeResponseDTO> updateDocumentType(DocumentTypeResponseDTO documentTypeDTO, ServerWebExchange exchange) {
        return eventSender.send(exchange, documentTypeDTO, AuditEventType.UPDATE_DOCUMENT_TYPE, DocumentTypeResponseDTO.class);
    }

    @Override
    public Mono<HttpResponse> deleteDocumentType(String id, ServerWebExchange exchange) {
        return eventSender.send(exchange, id, AuditEventType.DELETE_DOCUMENT_TYPE, HttpResponse.class);
    }

    @Override
    public Mono<DocumentTypeResponseDTO> documentTypeById(String id, ServerWebExchange exchange) {
        return eventSender.send(exchange, id, AuditEventType.GET_DOCUMENT_TYPE, DocumentTypeResponseDTO.class);
    }

    @Override
    public Mono<List<DocumentTypeResponseDTO>> documentTypeGetAll(ServerWebExchange exchange) {
        Mono<List> result = eventSender.send(
                exchange,
                new HashMap<>(),
                AuditEventType.LIST_DOCUMENT_TYPE,
                List.class);
        return result.map(e -> (List<DocumentTypeResponseDTO>) e);
    }
}
