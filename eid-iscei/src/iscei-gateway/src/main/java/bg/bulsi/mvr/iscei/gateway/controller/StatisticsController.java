package bg.bulsi.mvr.iscei.gateway.controller;

import java.io.IOException;
import java.time.OffsetDateTime;
import java.util.ArrayList;
import java.util.List;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import bg.bulsi.mvr.iscei.contract.dto.ReportDetailedDto;
import bg.bulsi.mvr.iscei.contract.dto.ReportRequestsCountDto;
import bg.bulsi.mvr.iscei.model.service.AuthenticationStatisticsService;
import bg.bulsi.mvr.iscei.model.service.CsvService;
import jakarta.servlet.http.HttpServletRequest;
import lombok.extern.slf4j.Slf4j;

@Slf4j
@RestController
@RequestMapping("/api/v1/statistics")
public class StatisticsController {

	@Autowired
	private AuthenticationStatisticsService authenticationStatisticsService;
	
	@Autowired
	private CsvService csvService;
	
	@GetMapping(path = "/report/requests-count",
			produces = MediaType.APPLICATION_JSON_VALUE)
	public List<String[]> reportRequestsCount(
			@RequestParam(required = true) OffsetDateTime createDateFrom,
			@RequestParam(required = true) OffsetDateTime createDateTo,
			@RequestParam(required = false) String systemType,
			@RequestParam(required = false) String systemId,
			@RequestParam(required = false) String clientId,
			HttpServletRequest httpRequest) {

		log.info(".reportRequestsCount()");
		List<String[]> result = new ArrayList<>(1);
		result.add(AuthenticationStatisticsService.REQUEST_COUNT_HEADERS);
		result.addAll(this.authenticationStatisticsService.requestsCount(createDateFrom, createDateTo, systemType, systemId, clientId));

		return result;
	}
	
	@GetMapping(path = "/report/detailed",
			produces = MediaType.APPLICATION_JSON_VALUE)
	public List<String[]> reportDetailed(
			// @DateTimeFormat(iso = DateTimeFormat.ISO.DATE_TIME)
			@RequestParam(required = true) OffsetDateTime createDateFrom,
			@RequestParam(required = true) OffsetDateTime createDateTo,
			@RequestParam(required = false) String systemType,
			@RequestParam(required = false) String systemId,
			@RequestParam(required = false) String clientId,
			@RequestParam(required = false) Boolean isEmployee, 
			@RequestParam(required = false) Boolean success,
			HttpServletRequest httpRequest) {

		log.info(".reportDetailed()");
		List<String[]> result = new ArrayList<>(1);
		result.add(AuthenticationStatisticsService.DETAILED_HEADERS);
		result.addAll(this.authenticationStatisticsService.reportDetailed(createDateFrom, createDateTo, systemType, systemId, clientId, isEmployee, success));
		
		return result;
	}
	
	@GetMapping(path = "/report/requests-total",
			produces = MediaType.APPLICATION_JSON_VALUE)
	public List<String[]> reportRequestsTotal(
			@RequestParam(required = true) int year,
			HttpServletRequest httpRequest) {

		log.info(".reportRequestsTotal()");
		List<String[]> result = new ArrayList<>(1);
		result.add(AuthenticationStatisticsService.REPORT_REQUESTS_TOTAL_HEADERS);
		result.addAll(this.authenticationStatisticsService.reportRequestsTotal(year) );

		return result;
	}
	
	@GetMapping(path = "/report/pui/requests-count",
			produces = MediaType.APPLICATION_JSON_VALUE)
	public List<ReportRequestsCountDto> reportRequestsCountPui(
			// @DateTimeFormat(iso = DateTimeFormat.ISO.DATE_TIME)
			@RequestParam(required = true) OffsetDateTime createDateFrom,
			@RequestParam(required = true) OffsetDateTime createDateTo,
			@RequestParam(required = false) String systemType,
			@RequestParam(required = false) String systemId,
			@RequestParam(required = false) String clientId,
			HttpServletRequest httpRequest
			) {

		log.info(".reportRequestsCountPui()");
		
		return this.authenticationStatisticsService.requestsCountDtoList(createDateFrom, createDateTo, systemType, systemId, clientId);
	}
	
	@GetMapping(path = "/report/pui/detailed",
			produces = MediaType.APPLICATION_JSON_VALUE)
	public List<ReportDetailedDto> reportDetailedPui(
			@RequestParam(required = true) OffsetDateTime createDateFrom,
			@RequestParam(required = true) OffsetDateTime createDateTo,
			@RequestParam(required = false) String systemType,
			@RequestParam(required = false) String systemId,
			@RequestParam(required = false) String clientId,
			@RequestParam(required = false) Boolean isEmployee, 
			@RequestParam(required = false) Boolean success,
			HttpServletRequest httpRequest) {

		log.info(".reportDetailedPui()");
		
		return this.authenticationStatisticsService.reportDetailedDtoList(createDateFrom, createDateTo, systemType, systemId, clientId, isEmployee, success);
	}
	
	@GetMapping(path = "/report/pui/requests-total",
			produces = MediaType.APPLICATION_JSON_VALUE)
	public List<String[]> requestsTotalPui(
			@RequestParam(required = true) int year,
			HttpServletRequest httpRequest) {

		log.info(".reportRequestsTotal()");
		List<String[]> result = new ArrayList<>(1);
		result.add(AuthenticationStatisticsService.REPORT_REQUESTS_TOTAL_HEADERS);
		result.addAll(this.authenticationStatisticsService.reportRequestsTotal(year) );

		return result;
	}
	
	@GetMapping(path = "/report/csv/requests-count", produces = MediaType.APPLICATION_JSON_VALUE)
	public ResponseEntity<Object> reportRequestsCountCsv(
			@RequestParam(required = true) OffsetDateTime createDateFrom,
			@RequestParam(required = true) OffsetDateTime createDateTo,
			@RequestParam(required = false) String systemType, 
			@RequestParam(required = false) String systemId,
			@RequestParam(required = false) String clientId, HttpServletRequest httpRequest) throws IOException {

		log.info(".reportRequestsCountCsv()");

		List<String[]> result = new ArrayList<>(1);
		result.add(AuthenticationStatisticsService.REQUEST_COUNT_HEADERS);
		result.addAll(this.authenticationStatisticsService.requestsCount(createDateFrom, createDateTo, systemType, systemId, clientId));
		
		return this.csvService.generateCsv(result);
	}

	@GetMapping(path = "/report/csv/detailed", produces = MediaType.APPLICATION_JSON_VALUE)
	public ResponseEntity<Object> reportDetailedCsv(
			@RequestParam(required = true) OffsetDateTime createDateFrom,
			@RequestParam(required = true) OffsetDateTime createDateTo,
			@RequestParam(required = false) String systemType,
			@RequestParam(required = false) String systemId,
			@RequestParam(required = false) String clientId, 
			@RequestParam(required = false) Boolean isEmployee,
			@RequestParam(required = false) Boolean success, HttpServletRequest httpRequest) throws IOException {

		log.info(".reportDetailedCsv()");
		
		List<String[]> result = new ArrayList<>(1);
		result.add(AuthenticationStatisticsService.DETAILED_HEADERS);
		result.addAll(this.authenticationStatisticsService.reportDetailed(createDateFrom, createDateTo, systemType, systemId, clientId, isEmployee, success));
		
		return this.csvService.generateCsv(result);
	}

	@GetMapping(path = "/report/csv/requests-total", produces = MediaType.APPLICATION_JSON_VALUE)
	public ResponseEntity<Object> requestsTotalCsv(@RequestParam(required = true) int year, HttpServletRequest httpRequest) throws IOException {
		log.info(".requestsTotalCsv()");

		List<String[]> result = new ArrayList<>(1);
		result.add(AuthenticationStatisticsService.REPORT_REQUESTS_TOTAL_HEADERS);
		result.addAll(this.authenticationStatisticsService.reportRequestsTotal(year) );
		
		return this.csvService.generateCsv(result);
	}
}
