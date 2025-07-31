package bg.bulsi.mvr.apigateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.raeicei.contract.dto.*;
import bg.bulsi.mvr.raeicei.gateway.api.v1.ApproveNewCircumstancesApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.UUID;

@Component
@Slf4j
@RequiredArgsConstructor
public class ApproveNewCircumstancesService implements ApproveNewCircumstancesApiDelegate {

    private final EventSender eventSender;

    @Override
    public Mono<List<EidAdministratorDTO>> getAllEidAdministratorsByStatus(EidManagerStatus status, ServerWebExchange exchange) {
        Mono<List> result = eventSender.send(
                exchange,
                status,
                AuditEventType.GET_ALL_EID_ADMINISTRATORS,
                List.class);
        return result.map(e -> new ArrayList<EidAdministratorDTO>(e));
    }

    @Override
    public Mono<EidAdministratorFullDTO> getFullEidAdministratorById(UUID id, Boolean onlyActiveElements, ServerWebExchange exchange) {
        HashMap<String, Object> payload = new HashMap<>(2);
        payload.put("id", id);
        payload.put("onlyActiveElements", onlyActiveElements);
        return eventSender.send(exchange, payload, AuditEventType.GET_FULL_EID_ADMINISTRATOR_BY_ID, EidAdministratorFullDTO.class);
    }

    @Override
    public Mono<HttpResponse> approveEidAdministrator(UUID id, NewCircumstancesStatus newCircumstancesStatus, NoteAndDocumentsDTO noteAndDocumentsDTO, ServerWebExchange exchange) {
        return eventSender.send(exchange, buildPayload(id, newCircumstancesStatus, noteAndDocumentsDTO), AuditEventType.CHANGE_EID_MANAGER_STATUS, HttpResponse.class);
    }

    @Override
    public Mono<List<EidCenterDTO>> getAllEidCentersByStatus(EidManagerStatus status, ServerWebExchange exchange) {
        Mono<List> result = eventSender.send(
                exchange,
                status,
                AuditEventType.GET_ALL_EID_CENTERS,
                List.class);
        return result.map(e -> new ArrayList<EidCenterDTO>(e));
    }

    @Override
    public Mono<EidCenterFullDTO> getFullEidCenterById(UUID id, Boolean onlyActiveElements, ServerWebExchange exchange) {
        HashMap<String, Object> payload = new HashMap<>(2);
        payload.put("id", id);
        payload.put("onlyActiveElements", onlyActiveElements);
        return eventSender.send(exchange, payload, AuditEventType.GET_FULL_EID_CENTER_BY_ID, EidCenterFullDTO.class);
    }

    @Override
    public Mono<HttpResponse> approveEidCenter(UUID id, NewCircumstancesStatus newCircumstancesStatus, NoteAndDocumentsDTO noteAndDocumentsDTO, ServerWebExchange exchange) {
        return eventSender.send(exchange, buildPayload(id, newCircumstancesStatus, noteAndDocumentsDTO), AuditEventType.CHANGE_EID_MANAGER_STATUS, HttpResponse.class);
    }

    @Override
    public Mono<DocumentNoteDTO> getDocumentsNotesForEidManager(ServerWebExchange exchange) {
        return eventSender.send(exchange, new HashMap<>(), AuditEventType.GET_DOCUMENTS_NOTES_FOR_EID_MANAGER, DocumentNoteDTO.class);
    }

    public HashMap<String, Object> buildPayload(UUID id, NewCircumstancesStatus newCircumstancesStatus, NoteAndDocumentsDTO noteAndDocumentsDTO) {
        HashMap<String, Object> payload = new HashMap<>(3);
        payload.put("id", id);
        payload.put("newCircumstancesStatus", newCircumstancesStatus);
        payload.put("noteAndDocumentsDTO", noteAndDocumentsDTO);

        return payload;
    }
}
