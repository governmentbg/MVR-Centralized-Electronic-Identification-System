package bg.bulsi.mvr.extgateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.raeicei.contract.dto.DocumentDTO;
import bg.bulsi.mvr.raeicei.contract.dto.DocumentResponseDTO;
import bg.bulsi.mvr.raeicei.contract.dto.HttpResponse;
import bg.bulsi.mvr.raeicei.extgateway.api.v1.FileUploadApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.UUID;

@Component
@Slf4j
@RequiredArgsConstructor
public class FileUploadService implements FileUploadApiDelegate {

    private final EventSender eventSender;

//    @Override
//    public Mono<HttpResponse> deleteDocument(String id, ServerWebExchange exchange) {
//        return eventSender.send(exchange, id, AuditEventType.DELETE_DOCUMENT, HttpResponse.class);
//    }

    @Override
    public Mono<DocumentResponseDTO> downloadDocumentById(String id, ServerWebExchange exchange) {
        return eventSender.send(exchange, id, AuditEventType.GET_DOCUMENT_BY_ID, DocumentResponseDTO.class);
    }

    @Override
    public Mono<List<DocumentDTO>> uploadDocumentForApplication(UUID id, List<DocumentDTO> documents, ServerWebExchange exchange) {

        Map<String, Object> messageBody = new HashMap<>();
        messageBody.put("id", id);
        messageBody.put("documents", documents);

        return eventSender.send(exchange, messageBody, AuditEventType.CREATE_EXT_DOCUMENT, List.class).map(e -> (List<DocumentDTO>) e);
    }

    @Override
    public Mono<List<DocumentDTO>> uploadDocumentForEidManager(List<DocumentDTO> documents, ServerWebExchange exchange) {
        return eventSender.send(exchange, documents, AuditEventType.CREATE_EXT_DOCUMENT, List.class).map(e -> (List<DocumentDTO>) e);
    }
}
