package bg.bulsi.mvr.extgateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.raeicei.contract.dto.*;
import bg.bulsi.mvr.raeicei.extgateway.api.v1.EmployeeOfEidManagerApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.util.*;

@Component
@Slf4j
@RequiredArgsConstructor
public class EmployeeOfEidManagerApiDelegateService implements EmployeeOfEidManagerApiDelegate {

    private final EventSender eventSender;

    @Override
    public Mono<List<DeviceDTO>> getDevicesForEmployeeByAdministratorId(ServerWebExchange exchange) {
        Mono<List> result = eventSender.send(
                exchange,
                new HashMap<>(),
                AuditEventType.GET_AEI_DEVICES,
                List.class);
        return result.map(e -> new ArrayList<DeviceDTO>(e));
    }

    @Override
    public Mono<Page<EmployeeDTO>> getAllEmployeesForEidManagerEmployee(ServerWebExchange exchange, Pageable pageable) {
        Mono<Page> result = eventSender.send(
                exchange,
                Map.of("pageable", pageable),
                AuditEventType.GET_ALL_EMPLOYEE_BY_SYSTEM_ID,
                Page.class);
        return result.map(e -> (Page<EmployeeDTO>) e);
    }

    @Override
    public Mono<List<EidManagerFrontOfficeDTO>> getAllEidManagerFrontOfficesForEmployee(ServerWebExchange exchange) {
        Mono<List> result = eventSender.send(
                exchange,
                new HashMap<>(),
                AuditEventType.GET_ALL_EID_MANAGER_FRONT_OFFICES_BY_MANAGER_ID,
                List.class);
        return result.map(e -> new ArrayList<EidManagerFrontOfficeDTO>(e));
    }

    @Override
    public Mono<EidAdministratorAuthorizedDTO> getEidAdministratorWithAuthorizedForEmployeeById(ServerWebExchange exchange) {
        return eventSender.send(exchange, new HashMap<>(), AuditEventType.GET_EID_ADMINISTRATOR_WITH_AUTHORIZED_PERSONS_BY_ID, EidAdministratorAuthorizedDTO.class);
    }

    @Override
    public Mono<EidCenterAuthorizedDTO> getEidCenterWithAuthorizedForEmployeeById(ServerWebExchange exchange) {
        return eventSender.send(exchange, new HashMap<>(), AuditEventType.GET_EID_CENTER_WITH_AUTHORIZED_PERSONS_BY_ID, EidCenterAuthorizedDTO.class);
    }
}
