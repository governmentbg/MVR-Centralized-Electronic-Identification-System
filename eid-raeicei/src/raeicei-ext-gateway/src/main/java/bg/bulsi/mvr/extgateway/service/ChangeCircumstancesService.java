package bg.bulsi.mvr.extgateway.service;

import bg.bulsi.mvr.audit_logger.dto.EventPayload;
import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.raeicei.contract.dto.*;
import bg.bulsi.mvr.raeicei.extgateway.api.v1.ChangeCircumstancesApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.util.HashMap;
import java.util.Map;
import java.util.UUID;
import java.util.function.Function;

@Component
@Slf4j
@RequiredArgsConstructor
public class ChangeCircumstancesService implements ChangeCircumstancesApiDelegate {

    private final EventSender eventSender;

    @Override
    public Mono<DeviceDTO> createExtDevice(DeviceExtRequestDTO deviceDTO, ServerWebExchange exchange) {
        return eventSender.send(exchange, deviceDTO, AuditEventType.CREATE_DEVICE, DeviceDTO.class);
    }

    @Override
    public Mono<DeviceDTO> updateExtDevice(DeviceExtRequestDTO deviceDTO, ServerWebExchange exchange) {
        return eventSender.send(exchange, deviceDTO, AuditEventType.UPDATE_DEVICE, DeviceDTO.class);
    }

    @Override
    public Mono<HttpResponse> deleteDevice(String id, ServerWebExchange exchange) {
        return eventSender.send(exchange, id, AuditEventType.DELETE_DEVICE, HttpResponse.class);
    }

    @Override
    public Mono<DiscountResponseDTO> createDiscount(DiscountDTO discountDTO, ServerWebExchange exchange) {
        return eventSender.send(exchange, discountDTO, AuditEventType.CREATE_DISCOUNT, DiscountResponseDTO.class);
    }

    @Override
    public Mono<DiscountResponseDTO> updateDiscount(UUID id, DiscountDTO discountDTO, ServerWebExchange exchange) {

        Map<String, Object> messageBody = new HashMap<>();
        messageBody.put("id", id);
        messageBody.put("discountDTO", discountDTO);

        return eventSender.send(exchange, messageBody, AuditEventType.UPDATE_DISCOUNT, DiscountResponseDTO.class);
    }

    @Override
    public Mono<HttpResponse> deleteDiscount(String id, ServerWebExchange exchange) {
        return eventSender.send(exchange, id, AuditEventType.DELETE_DISCOUNT, HttpResponse.class);
    }

    @Override
    public Mono<EidManagerFrontOfficeResponseDTO> createEidManagerFrontOffice(EidManagerFrontOfficeDTO administratorFrontOfficeDTO, ServerWebExchange exchange) {
        return eventSender.send(exchange, administratorFrontOfficeDTO, AuditEventType.CREATE_EID_MANAGER_FRONT_OFFICE, EidManagerFrontOfficeResponseDTO.class);
    }

    @Override
    public Mono<EidManagerFrontOfficeResponseDTO> updateEidManagerFrontOffice(EidManagerFrontOfficeDTO administratorFrontOfficeDTO, ServerWebExchange exchange) {
        return eventSender.send(exchange, administratorFrontOfficeDTO, AuditEventType.UPDATE_EID_MANAGER_FRONT_OFFICE, EidManagerFrontOfficeResponseDTO.class);
    }

    @Override
    public Mono<HttpResponse> deleteEidManagerFrontOffice(String id, ServerWebExchange exchange) {
        return eventSender.send(exchange, id, AuditEventType.DELETE_EID_MANAGER_FRONT_OFFICE, HttpResponse.class);
    }

    @Override
    public Mono<TariffResponseDTO> createTariff(TariffDTO tariffDTO, ServerWebExchange exchange) {

        EventPayload eventPayloadReq = new EventPayload();
        eventPayloadReq.setRequestBody(tariffDTO);

        Function<TariffResponseDTO, EventPayload> auditRespPayload = (appResp -> {
            EventPayload eventPayloadResp = new EventPayload();
            eventPayloadResp.setRequestBody(tariffDTO);

            return eventPayloadResp;
        });

        return eventSender.send(exchange, tariffDTO, AuditEventType.CREATE_TARIFF, TariffResponseDTO.class, eventPayloadReq, auditRespPayload);
    }

    @Override
    public Mono<TariffResponseDTO> updateTariff(UUID id, TariffDTO tariffDTO, ServerWebExchange exchange) {

        Map<String, Object> messageBody = new HashMap<>();
        messageBody.put("id", id);
        messageBody.put("tariffDTO", tariffDTO);

        EventPayload eventPayloadReq = new EventPayload();
        eventPayloadReq.addAdditionalParam("Tariff Id:", id);
        eventPayloadReq.setRequestBody(tariffDTO);

        Function<TariffResponseDTO, EventPayload> auditRespPayload = (appResp -> {
            EventPayload eventPayloadResp = new EventPayload();
            eventPayloadResp.addAdditionalParam("Tariff Id", id);
            eventPayloadResp.setRequestBody(tariffDTO);

            return eventPayloadResp;
        });

        return eventSender.send(exchange, messageBody, AuditEventType.UPDATE_TARIFF, TariffResponseDTO.class, eventPayloadReq, auditRespPayload);
    }

    @Override
    public Mono<HttpResponse> deleteTariff(String id, ServerWebExchange exchange) {
        return eventSender.send(exchange, id, AuditEventType.DELETE_TARIFF, HttpResponse.class);
    }

    @Override
    public Mono<EmployeeDTO> createEmployee(EmployeeDTO employeeDTO, ServerWebExchange exchange) {
        return eventSender.send(exchange, employeeDTO, AuditEventType.CREATE_EMPLOYEE, EmployeeDTO.class);
    }

    @Override
    public Mono<EmployeeDTO> updateEmployee(UUID id, EmployeeDTO employeeDTO, ServerWebExchange exchange) {

        Map<String, Object> messageBody = new HashMap<>();
        messageBody.put("id", id);
        messageBody.put("employeeDTO", employeeDTO);

        return eventSender.send(exchange, messageBody, AuditEventType.UPDATE_EMPLOYEE, EmployeeDTO.class);
    }

    @Override
    public Mono<HttpResponse> deleteEmployee(String id, ServerWebExchange exchange) {
        return eventSender.send(exchange, id, AuditEventType.DELETE_EMPLOYEE, HttpResponse.class);
    }

    @Override
    public Mono<ContactDTO> createAuthorizedPerson(ContactDTO contactDTO, ServerWebExchange exchange) {
        return eventSender.send(exchange, contactDTO, AuditEventType.CREATE_AUTHORIZED_PERSON, ContactDTO.class);
    }

    @Override
    public Mono<ContactDTO> updateAuthorizedPerson(UUID id, ContactDTO contactDTO, ServerWebExchange exchange) {

        Map<String, Object> messageBody = new HashMap<>();
        messageBody.put("id", id);
        messageBody.put("contactDTO", contactDTO);

        return eventSender.send(exchange, messageBody, AuditEventType.UPDATE_AUTHORIZED_PERSON, ContactDTO.class);
    }

    @Override
    public Mono<HttpResponse> deleteAuthorizedPerson(String id, ServerWebExchange exchange) {
        return eventSender.send(exchange, id, AuditEventType.DELETE_AUTHORIZED_PERSON, HttpResponse.class);
    }

    @Override
    public Mono<DocumentNoteDTO> getDocumentsNotesForEidManager(ServerWebExchange exchange) {
        return eventSender.send(exchange, new HashMap<>(), AuditEventType.GET_DOCUMENTS_NOTES_FOR_EID_MANAGER, DocumentNoteDTO.class);
    }
}
