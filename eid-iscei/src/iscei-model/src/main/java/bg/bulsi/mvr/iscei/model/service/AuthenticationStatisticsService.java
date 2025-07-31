package bg.bulsi.mvr.iscei.model.service;

import java.time.OffsetDateTime;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.Optional;
import java.util.TreeMap;
import java.util.UUID;
import java.util.stream.Collectors;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import bg.bulsi.mvr.iscei.contract.dto.ReportRequestsCountDto;
import bg.bulsi.mvr.iscei.contract.dto.ReportRequestsTotalDto;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.iscei.contract.dto.CertificateLoginResultDto;
import bg.bulsi.mvr.iscei.contract.dto.ExternalProviderLoginResultDto;
//import bg.bulsi.mvr.iscei.gateway.config.ApplicationProperties;
import bg.bulsi.mvr.iscei.contract.dto.ReportDetailedDto;
import bg.bulsi.mvr.iscei.model.AcrScope;
import bg.bulsi.mvr.iscei.model.AuthenticationRequestStatistic;
import bg.bulsi.mvr.iscei.model.AuthenticationResultStatistic;
import bg.bulsi.mvr.iscei.model.AuthenticationStatistic;
import bg.bulsi.mvr.iscei.model.config.ApplicationProperties;
import bg.bulsi.mvr.iscei.model.config.MvrOAuthClient;
import bg.bulsi.mvr.iscei.model.repository.jpa.AuthenticationStatisticsRepository;
import jakarta.servlet.http.HttpServletRequest;

@Service
public class AuthenticationStatisticsService {

	public static final String[] REPORT_REQUESTS_TOTAL_HEADERS ={ "Име на доставчик", "Януари", "Февруари",
			"Март", "Април", "Май", "Юни", "Юли", "Август", "Септември", "Октомври", "Ноември", "Декември"};

	public static final String[] REQUEST_COUNT_HEADERS = {"Идентификатор на клиента", "Идентификатор на доставчик", "Име на доставчик", "Брой заявки" };

	public static final String[] DETAILED_HEADERS = {"Дата на cъздаване", "Идентификатор на доставчик", "Име на доставчик", "IP адрес на заявителя", "Електронна идентичност", "Сериен номер на УЕИ", "Издател на УЕИ", "Идентификатор на УЕИ", "Ниво на осигуреност", "Успех"};
	
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
		requestStatistic.setLevelOfAssurance(AcrScope.EID_LOA_LOW.getAcrScopeName());
		
		this.populateSystemProperties(clientId, requestStatistic);
		
		this.populateSystemProperties(clientId, requestStatistic);
		
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
	
		this.populateSystemProperties(clientId, resultStatistic);
		
		this.statisticsRepository.save(resultStatistic);
	}

	public void createAuthenticationRequestEid(String clientId, UUID citizenProfileId, ExternalProviderLoginResultDto providerLogin, CertificateLoginResultDto certificateLoginResult, HttpServletRequest httpRequest) {
		AuthenticationRequestStatistic requestStatistic = new AuthenticationRequestStatistic();
		requestStatistic.setCitizenProfileId(citizenProfileId);
		requestStatistic.setEidentityId(certificateLoginResult.getEidentityId());
		requestStatistic.setSessionId(UserContextHolder.getFromServletContext().getGlobalCorrelationId().toString());
		requestStatistic.setClientId(clientId);
		requestStatistic.setCreateDate(OffsetDateTime.now());
		requestStatistic.setRequesterIpAddress(httpRequest.getRemoteAddr());
		requestStatistic.setX509CertificateId(certificateLoginResult.getId());
		requestStatistic.setX509CertificateIssuerDn(certificateLoginResult.getX509CertificateIssuerDn());
		requestStatistic.setX509CertificateSn(certificateLoginResult.getX509CertificateSn());
		requestStatistic.setDeviceId(certificateLoginResult.getDeviceId());
		
		if(providerLogin != null) {
			requestStatistic.setSystemId(providerLogin.getSystemId().toString());
			requestStatistic.setSystemName(providerLogin.getSystemName());
			requestStatistic.setSystemType(providerLogin.getSystemType());
			requestStatistic.setIsEmployee(true);
		} else {
			this.populateSystemProperties(clientId, requestStatistic);
		}
		
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
		resultStatistic.setDeviceId(certificateLoginResult.getDeviceId());
		
		if(providerLogin != null) {
			resultStatistic.setSystemId(providerLogin.getSystemId().toString());
			resultStatistic.setSystemName(providerLogin.getSystemName());
			resultStatistic.setSystemType(providerLogin.getSystemType());
			resultStatistic.setIsEmployee(true);
		} else {
			this.populateSystemProperties(clientId, resultStatistic);
		}
		
		resultStatistic.setLevelOfAssurance(AcrScope.EID_LOA_HIGH.getAcrScopeName());
		
		this.statisticsRepository.save(resultStatistic);
	}
	
	public List<ReportRequestsCountDto> requestsCountDtoList(OffsetDateTime createDateFrom, OffsetDateTime createDateTo, String systemType, String systemId, String clientId) {
		return this.statisticsRepository.requestsCount(createDateFrom, createDateTo, systemType, systemId, clientId).get();
	}
	
	public List<String[]> requestsCount(OffsetDateTime createDateFrom, OffsetDateTime createDateTo, String systemType,String systemId, String clientId) {
		Optional <List<ReportRequestsCountDto>> result= this.statisticsRepository.requestsCount(createDateFrom, createDateTo, systemType, systemId, clientId);
		List<String[]> res=new ArrayList<String[]>();
		result.ifPresent((rr)->{
			 res.addAll(rr.stream().map(ReportRequestsCountDto::toArray).collect(Collectors.toList()));
			 });
		 return res;
	}
	
	public List<ReportDetailedDto> reportDetailedDtoList(OffsetDateTime createDateFrom, OffsetDateTime createDateTo, String systemType,String systemId,
			String clientId, Boolean isEmployee, Boolean success) {
		
		return this.statisticsRepository.reportDetailed(createDateFrom, createDateTo, systemType, systemId, clientId, isEmployee, success).get();
	}
	
	public List<String[]> reportDetailed(OffsetDateTime createDateFrom, OffsetDateTime createDateTo, String systemType,String systemId,
			String clientId, Boolean isEmployee, Boolean success) {
		Optional <List<ReportDetailedDto>> result= this.statisticsRepository.reportDetailed(createDateFrom, createDateTo, systemType, systemId, clientId, isEmployee, success);
		List<String[]> res=new ArrayList<String[]>();
		result.ifPresent((rr)->{
			 res.addAll(rr.stream().map(ReportDetailedDto::toArray).collect(Collectors.toList()));
			 });
		return res;
	}
	
	public List<String[]> reportRequestsTotal(int year) {
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

					String[] row = new String[1 + monthMap.size()];
					MvrOAuthClient client = this.applicationProperties.getOauthClients().get(clientId);
					row[0] = client != null ? client.getName() : clientId;
					for (int i = 1; i <= monthMap.size(); i++) {
						row[i] = String.valueOf( monthMap.get(i));
					}
					return row;
				}).toList();
       
	}
	
	private void populateSystemProperties(String clientId, AuthenticationStatistic statistic) {
		MvrOAuthClient mvrClient = (MvrOAuthClient) this.applicationProperties.getOauthClients().getOrDefault(clientId, null);
		if(mvrClient != null) {
			statistic.setSystemId(mvrClient.getSystemId().toString());
			statistic.setSystemName(mvrClient.getSystemName());
		}
	}
	
}
