package bg.bulsi.mvr.apigateway.service;

import bg.bulsi.mvr.audit_logger.dto.EventPayload;
import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.raeicei.contract.dto.ApplicationStatus;
import bg.bulsi.mvr.raeicei.contract.dto.DocumentNoteDTO;
import bg.bulsi.mvr.raeicei.contract.dto.HttpResponse;
import bg.bulsi.mvr.raeicei.contract.dto.NoteAndDocumentsDTO;
import bg.bulsi.mvr.raeicei.gateway.api.v1.ChangeApplicationStatusApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.util.HashMap;
import java.util.UUID;
import java.util.function.Function;

@Component
@Slf4j
@RequiredArgsConstructor
public class ChangeApplicationStatusService implements ChangeApplicationStatusApiDelegate {

    private final EventSender eventSender;

    @Override
    public Mono<HttpResponse> changeApplicationStatus(UUID id, ApplicationStatus applicationStatus, NoteAndDocumentsDTO noteAndDocumentsDTO, String code, ServerWebExchange exchange) {

        EventPayload eventPayloadReq = new EventPayload();
        eventPayloadReq.setApplicationId(id.toString());
        eventPayloadReq.setApplicationStatus(applicationStatus.name());

        Function<HttpResponse, EventPayload> auditRespPayload = (appResp -> {
            EventPayload eventPayloadResp = new EventPayload();
            eventPayloadResp.setApplicationId(id.toString());
            eventPayloadResp.setApplicationStatus(applicationStatus.name());

            return eventPayloadResp;
        });

        return eventSender.send(exchange, buildPayload(id, applicationStatus, code, noteAndDocumentsDTO), AuditEventType.CHANGE_APPLICATION_STATUS, HttpResponse.class, eventPayloadReq, auditRespPayload);
    }

    @Override
    public Mono<DocumentNoteDTO> getDocumentsNotesForApplication(UUID id, ServerWebExchange exchange) {
        return eventSender.send(exchange, id, AuditEventType.GET_DOCUMENTS_NOTES_FOR_APPLICATION, DocumentNoteDTO.class);
    }

    public HashMap<String, Object> buildPayload(UUID id, ApplicationStatus applicationStatus, String code, NoteAndDocumentsDTO noteAndDocumentsDTO) {
        HashMap<String, Object> payload = new HashMap<>(4);
        payload.put("id", id);
        payload.put("applicationStatus", applicationStatus);
        payload.put("code", code);
        payload.put("noteAndDocumentsDTO", noteAndDocumentsDTO);
        return payload;
    }
}
