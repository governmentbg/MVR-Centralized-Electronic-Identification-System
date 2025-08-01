//package bg.bulsi.mvr.iscei.gateway.service;
//
//import java.time.OffsetDateTime;
//import java.util.List;
//import java.util.Map;
//import java.util.TreeMap;
//import java.util.UUID;
//import java.util.stream.Collectors;
//
//import org.springframework.beans.factory.annotation.Autowired;
//import org.springframework.stereotype.Service;
//
//import bg.bulsi.mvr.iscei.contract.dto.ReportRequestsCountDto;
//import bg.bulsi.mvr.iscei.contract.dto.ReportRequestsTotalDto;
//import bg.bulsi.mvr.iscei.gateway.config.ApplicationProperties;
//import bg.bulsi.mvr.iscei.contract.dto.ReportDetailedDto;
//import bg.bulsi.mvr.iscei.model.repository.jpa.AuthenticationStatisticsRepository;
//
//@Service
//public class AuthenticationStatisticsService {
//
//	@Autowired
//	private AuthenticationStatisticsRepository statisticsRepository;
//
//	@Autowired
//	private ApplicationProperties applicationProperties;
//	
//	public List<ReportRequestsCountDto> requestsCount(OffsetDateTime createDateFrom, OffsetDateTime createDateTo, String systemType,UUID systemId, String clientId) {
//		return this.statisticsRepository.requestsCount(createDateFrom, createDateTo, systemType, systemId, clientId);
//	}
//	
//	public List<ReportDetailedDto> reportDetailed(OffsetDateTime createDateFrom, OffsetDateTime createDateTo, String systemType,UUID systemId,
//			String clientId, Boolean isEmployee, Boolean success) {
//		return this.statisticsRepository.reportDetailed(createDateFrom, createDateTo, systemType, systemId, clientId, isEmployee, success);
//	}
//	
//	public List<Object[]> reportRequestsTotal(int year) {
//		// 1) fetch raw DTOs
//		List<ReportRequestsTotalDto> requestsTotal = this.statisticsRepository.reportRequestsTotal(year);
//		
//        // 2) group into clientId → (month → sum)
//        Map<String, Map<Integer, Long>> grouped =
//        		requestsTotal.stream()
//               .collect(Collectors.groupingBy(
//                   ReportRequestsTotalDto::getClientId,           // level 1
//                   Collectors.groupingBy(
//                       ReportRequestsTotalDto::getMonth,          // level 2
//                       Collectors.summingLong(ReportRequestsTotalDto::getCount)
//                   )
//               ));
//
//        
//        // 3) ensure months 1..12 exist with default 0
//        return  grouped
//                      .entrySet()
//                      .stream()
//				.map(entry -> {
//					String clientId = entry.getKey();
//
//					Map<Integer, Long> monthMap = new TreeMap<>(); //
//					for (int m = 1; m <= 12; m++) {
//						monthMap.put(m, 0L); // seed with zero
//					}
//
//					monthMap.putAll(entry.getValue());
//
//					Object[] row = new Object[1 + monthMap.size()];
//					row[0] = this.applicationProperties.getOauthClients().getOrDefault(clientId, clientId);
//					for (int i = 1; i <= monthMap.size(); i++) {
//						row[i] = monthMap.get(i);
//					}
//					return row;
//				}).toList();
//       
//	}
//	
//}
