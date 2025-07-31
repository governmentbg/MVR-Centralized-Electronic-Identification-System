package bg.bulsi.mvr.iscei.backend.service;

import java.time.OffsetDateTime;
import java.util.List;
import java.util.Map;
import java.util.TreeMap;
import java.util.UUID;
import java.util.stream.Collectors;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.iscei.backend.config.ApplicationProperties;
import bg.bulsi.mvr.iscei.contract.dto.CertificateLoginResultDto;
import bg.bulsi.mvr.iscei.contract.dto.ExternalProviderLoginResultDto;
import bg.bulsi.mvr.iscei.contract.dto.ReportRequestsCountDto;
import bg.bulsi.mvr.iscei.contract.dto.ReportRequestsTotalDto;
import bg.bulsi.mvr.iscei.contract.dto.ReportDetailedDto;
import bg.bulsi.mvr.iscei.model.AcrScope;
import bg.bulsi.mvr.iscei.model.AuthenticationRequestStatistic;
import bg.bulsi.mvr.iscei.model.AuthenticationResultStatistic;
import bg.bulsi.mvr.iscei.model.repository.jpa.AuthenticationStatisticsRepository;
import jakarta.servlet.http.HttpServletRequest;
import lombok.extern.slf4j.Slf4j;

@Slf4j
@Service
public class AuthenticationStatisticsService {

	@Autowired
	private AuthenticationStatisticsRepository statisticsRepository;

	@Autowired
	private ApplicationProperties applicationProperties;
	
	public void createAuthenticationRequestBaseProfile(String clientId, HttpServletRequest httpRequest) {
		AuthenticationRequestStatistic requestStatistic = new AuthenticationRequestStatistic();
		requestStatistic.setSessionId(UserContextHolder.getFromServletContext().getGlobalCorrelationId().toString());
		requestStatistic.setClientId(clientId);
		requestStatistic.setCreateDate(OffsetDateTime.now());
		requestStatistic.setRequesterIpAddress(httpRequest.getRemoteAddr());
		
		this.statisticsRepository.save(requestStatistic);
	}
    
	public void createAuthenticationResultBaseProfile(String clientId, UUID citizenProfileId, UUID eidentityId, HttpServletRequest httpRequest) {
		AuthenticationResultStatistic resultStatistic = new AuthenticationResultStatistic();
		resultStatistic.setSessionId(UserContextHolder.getFromServletContext().getGlobalCorrelationId().toString());
		resultStatistic.setCitizenProfileId(citizenProfileId);
		resultStatistic.setEidentityId(eidentityId);
		resultStatistic.setClientId(clientId);
		resultStatistic.setCreateDate(OffsetDateTime.now());
		resultStatistic.setRequesterIpAddress(httpRequest.getRemoteAddr());
		resultStatistic.setIsEmployee(false);
		resultStatistic.setLevelOfAssurance(AcrScope.EID_LOA_LOW.getAcrScopeName());
		resultStatistic.setSuccess(true);
	
		this.statisticsRepository.save(resultStatistic);
	}
	
	public void createAuthenticationRequestEid(String clientId, HttpServletRequest httpRequest) {
		AuthenticationRequestStatistic requestStatistic = new AuthenticationRequestStatistic();
		requestStatistic.setSessionId(UserContextHolder.getFromServletContext().getGlobalCorrelationId().toString());
		requestStatistic.setClientId(clientId);
		requestStatistic.setCreateDate(OffsetDateTime.now());
		requestStatistic.setRequesterIpAddress(httpRequest.getRemoteAddr());

		this.statisticsRepository.save(requestStatistic);
	}
    
	public void createAuthenticationResultEid(String clientId, UUID citizenProfileId, ExternalProviderLoginResultDto providerLogin, CertificateLoginResultDto certificateLoginResult, HttpServletRequest httpRequest) {
		AuthenticationResultStatistic resultStatistic = new AuthenticationResultStatistic();
		resultStatistic.setSessionId(UserContextHolder.getFromServletContext().getGlobalCorrelationId().toString());
		resultStatistic.setCitizenProfileId(citizenProfileId);
		resultStatistic.setEidentityId(certificateLoginResult.getEidentityId());
		resultStatistic.setClientId(clientId);
		resultStatistic.setCreateDate(OffsetDateTime.now());
		resultStatistic.setRequesterIpAddress(httpRequest.getRemoteAddr());
		resultStatistic.setSuccess(true);
		resultStatistic.setX509CertificateId(certificateLoginResult.getId());
		resultStatistic.setX509CertificateIssuerDn(certificateLoginResult.getX509CertificateIssuerDn());
		resultStatistic.setX509CertificateSn(certificateLoginResult.getX509CertificateSn());
		
		if(providerLogin != null) {
			resultStatistic.setSystemId(providerLogin.getSystemId().toString());
			resultStatistic.setSystemName(providerLogin.getSystemName());
			resultStatistic.setIsEmployee(true);
		}

		resultStatistic.setLevelOfAssurance(AcrScope.EID_LOA_HIGH.getAcrScopeName());
		
		this.statisticsRepository.save(resultStatistic);
	}
	
	public List<ReportRequestsCountDto> requestsCount(OffsetDateTime createDateFrom, OffsetDateTime createDateTo, String systemType,UUID systemId, String clientId) {
		return this.statisticsRepository.requestsCount(createDateFrom, createDateTo, systemType, systemId, clientId);
	}
	
	public List<ReportDetailedDto> reportDetailed(OffsetDateTime createDateFrom, OffsetDateTime createDateTo, String systemType,UUID systemId,
			String clientId, Boolean isEmployee, Boolean success) {
		return this.statisticsRepository.reportDetailed(createDateFrom, createDateTo, systemType, systemId, clientId, isEmployee, success);
	}
	
	public List<Object[]> reportRequestsTotal(int year) {
		// 1) fetch raw DTOs
		List<ReportRequestsTotalDto> requestsTotal = this.statisticsRepository.reportRequestsTotal(year);
		
        // 2) group into clientId → (month → sum)
        Map<String, Map<Integer, Long>> grouped =
        		requestsTotal.stream()
               .collect(Collectors.groupingBy(
                   ReportRequestsTotalDto::getClientId,           // level 1
                   Collectors.groupingBy(
                       ReportRequestsTotalDto::getMonth,          // level 2
                       Collectors.summingLong(ReportRequestsTotalDto::getCount)
                   )
               ));

        
        // 3) ensure months 1..12 exist with default 0
        return  grouped
                      .entrySet()
                      .stream()
				.map(entry -> {
					String clientId = entry.getKey();

					Map<Integer, Long> monthMap = new TreeMap<>(); //
					for (int m = 1; m <= 12; m++) {
						monthMap.put(m, 0L); // seed with zero
					}

					monthMap.putAll(entry.getValue());

					Object[] row = new Object[1 + monthMap.size()];
					row[0] = this.applicationProperties.getOauthClients().getOrDefault(clientId, clientId);
					for (int i = 1; i <= monthMap.size(); i++) {
						row[i] = monthMap.get(i);
					}
					return row;
				}).toList();
       
	}
	
	public void create(HttpServletRequest httpRequest, CertificateLoginResultDto certificateLogin) {
		
//		AuthenticationResultStatistic requestStatistic = new AuthenticationResultStatistic();
//		requestStatistic.setCitizenProfileId(idAddress);
//		requestStatistic.setEidentityId(certificateLogin.getEidentityId().toString());
//		requestStatistic.setClientId(idAddress);
//		requestStatistic.setCreateDate(LocalDateTime.now());
//		requestStatistic.setRequesterIpAddress(httpRequest.getRemoteAddr());
//		requestStatistic.setRoles(null);
//		requestStatistic.setSuccess(true);
//		requestStatistic.setSystemId(idAddress);
//		requestStatistic.setSystemName(idAddress);
//		requestStatistic.setSystemType(idAddress);
//		requestStatistic.setX509CertificateIssuerDn(certificateLogin.getX509CertificateIssuerDn());
//		requestStatistic.setX509CertificateSn(certificateLogin.getX509CertificateSn());
//	
//		this.statisticRepository.save(requestStatistic);
	}
}
