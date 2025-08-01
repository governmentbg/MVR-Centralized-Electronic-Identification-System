package bg.bulsi.mvr.apigateway.service;

import bg.bulsi.mvr.audit_logger.model.AuditEventType;
import bg.bulsi.mvr.common.rabbitmq.producer.EventSender;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationDTO;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationFilter;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationReportByOperatorsDTO;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationType;
import bg.bulsi.mvr.mpozei.contract.dto.report.ReportByOfficesRequest;
import bg.bulsi.mvr.mpozei.contract.dto.report.ReportByOperatorsRequest;
import bg.bulsi.mvr.mpozei.contract.dto.report.ReportByRegionRequest;
import bg.bulsi.mvr.mpozei.contract.dto.report.ReportTotalRequest;
import bg.bulsi.mvr.mpozei.gateway.api.v1.ReportApiDelegate;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;

import org.springframework.http.HttpHeaders;
import org.springframework.http.server.reactive.ServerHttpResponse;
import org.springframework.stereotype.Component;
import org.springframework.web.server.ServerWebExchange;
import reactor.core.publisher.Mono;

import java.time.LocalDate;
import java.time.OffsetDateTime;
import java.util.List;
import java.util.UUID;

@Component
@Slf4j
@RequiredArgsConstructor
public class ReportDelegateService implements ReportApiDelegate {
	private final EventSender eventSender;

	@Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_super_admin')")
	public Mono<List<Object>> getApplicationsReportByAdministrator(UUID eidAdministrator, LocalDate from, LocalDate to,
			ServerWebExchange exchange) {
		return eventSender
				.send(exchange, new ReportByOfficesRequest(eidAdministrator, from, to),
						AuditEventType.APPLICATION_REPORT_BY_ADMINISTRATOR, List.class)
				.map(e -> List.copyOf((List<List<String>>) e));
	}

	@Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_super_admin')")
	public Mono<List<Object>> getApplicationsReportByRegion(LocalDate from, LocalDate to, ServerWebExchange exchange) {
		Mono<List> list = eventSender.send(exchange, new ReportByRegionRequest(from, to),
				AuditEventType.APPLICATION_REPORT_BY_REGION, List.class);
		return list.map(e -> (List<Object>) e);
	}

	@Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_super_admin')")
	public Mono<List<Object>> getApplicationsReportTotal(LocalDate from, LocalDate to, ServerWebExchange exchange) {
		Mono<List> list = eventSender.send(exchange, new ReportTotalRequest(from, to),
				AuditEventType.APPLICATION_REPORT_TOTAL, List.class);
		return list.map(e -> List.copyOf((List<List<String>>) e));
	}

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_super_admin')")
    public Mono<List<ApplicationReportByOperatorsDTO>> getJsonApplicationsReportByOperators(List<String> operators, LocalDate from, LocalDate to, ServerWebExchange exchange) {
        return eventSender
                .send(exchange, new ReportByOperatorsRequest(operators, from, to),
                        AuditEventType.APPLICATION_REPORT_BY_OPERATORS, List.class)
                .map(e -> (List<ApplicationReportByOperatorsDTO>) e);
    }

    @Override
//    @PreAuthorize("@authzService.hasAnyRoleReactive('ROLE_AEI_super_admin')")
    public Mono<String> getCsvApplicationsReportByOperators(List<String> operators, LocalDate from, LocalDate to, ServerWebExchange exchange) {
    	ServerHttpResponse httpResponse = exchange.getResponse();
    	httpResponse.getHeaders().add(HttpHeaders.CONTENT_DISPOSITION, "attachment; filename=data.csv");
    	
    	return eventSender
                .send(exchange, new ReportByOperatorsRequest(operators, from, to),
                        AuditEventType.APPLICATION_REPORT_BY_OPERATORS, String.class)
                .map(e -> e);
    }
    
    @Override
    public Mono<String> getCsvFindApplications(UUID eidentityId, List<ApplicationStatus> statuses, List<ApplicationSubmissionType> submissionTypes, String id, String applicationNumber, List<UUID> deviceIds, UUID administratorId, List<UUID> eidAdministratorFrontOfficeId, List<ApplicationType> applicationType, OffsetDateTime createdDateFrom, OffsetDateTime createdDateTo, ServerWebExchange exchange) {
    	  
    	ServerHttpResponse httpResponse = exchange.getResponse();
    	httpResponse.getHeaders().add(HttpHeaders.CONTENT_DISPOSITION, "attachment; filename=data.csv");

        ApplicationFilter filter = new ApplicationFilter(eidentityId, id, applicationNumber, createdDateFrom, createdDateTo, deviceIds, administratorId, eidAdministratorFrontOfficeId, statuses, submissionTypes, applicationType, null);
    	
    	return eventSender.send(exchange, filter, AuditEventType.APPLICATION_REPORT_BY_OPERATORS, String.class);
    }
}
