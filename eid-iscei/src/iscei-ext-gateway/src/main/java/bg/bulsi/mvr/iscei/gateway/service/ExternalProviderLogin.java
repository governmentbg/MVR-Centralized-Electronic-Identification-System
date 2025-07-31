package bg.bulsi.mvr.iscei.gateway.service;

import java.util.HashSet;
import java.util.Map;
import java.util.Set;
import java.util.UUID;

import org.apache.commons.codec.digest.DigestUtils;
import org.apache.commons.lang3.EnumUtils;
import org.apache.commons.lang3.StringUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import org.springframework.util.MultiValueMap;

import bg.bulsi.mvr.common.exception.ErrorCode;
import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.iscei.contract.dto.ExternalProviderLoginResultDto;
import bg.bulsi.mvr.iscei.contract.dto.ExternalSystemScope;
import bg.bulsi.mvr.iscei.contract.dto.IdentifierTypeDTO;
import bg.bulsi.mvr.iscei.contract.dto.ProviderEmployeeInfo;
import bg.bulsi.mvr.iscei.gateway.client.PdeauClient;
import bg.bulsi.mvr.iscei.gateway.client.RaeiceiClient;

/**
 * This class checks if the Citizen logs in as External System/Provider Administrator or Employee
 */
@Service
public class ExternalProviderLogin {

	public static final String EID_EXT_SYS_ADMIN = "eid_ext_sys_admin";

	@Autowired
	private RaeiceiClient raeiceiClient;
	
	@Autowired
	private PdeauClient pdeauClient;
	
	public ExternalProviderLoginResultDto verifyLogin(String number, IdentifierTypeDTO type, Set<String> scope, Map<String, String> requestParams, MultiValueMap<String, String> requestBody) {
		Set<ExternalSystemScope> systemScopeSet = new HashSet<>();
		for(String currentScope: scope) {
			if(EnumUtils.isValidEnumIgnoreCase(ExternalSystemScope.class, currentScope)) {
				systemScopeSet.add(ExternalSystemScope.fromValue(currentScope));
			}
		}
		
		//Ordinary Citizen Login, Skipping logic here
		if(systemScopeSet.isEmpty()) {
			return null;
		}
		
		//Only 1 {@link ExternalSystemScope} can be requested
		if(systemScopeSet.size() > 1) {
			throw new ValidationMVRException(ErrorCode.INVALID_SCOPE_PARAM);
		}
		
		UUID systemId = UUID.fromString(requestParams.get("system_id"));

		ProviderEmployeeInfo employeeInfo = null;
		ExternalSystemScope systemScope = systemScopeSet.iterator().next();
		if(ExternalSystemScope.EID_DEAU == systemScope) {
			employeeInfo = this.pdeauClient.getEmployeeInfo(systemId, number, type);
		} else if (ExternalSystemScope.EID_CEI == systemScope) {
			employeeInfo = this.raeiceiClient.getCeiEmployeeInfo(systemId, number, type);
		} else if (ExternalSystemScope.EID_AEI == systemScope) {
			employeeInfo = this.raeiceiClient.getAeiEmployeeInfo(systemId, number, type);
		}
		
		if(Boolean.TRUE.equals(employeeInfo.getIsAdministrator())) {
			scope.add(EID_EXT_SYS_ADMIN);
		}
		
		String sectorId = DigestUtils.sha3_256Hex(StringUtils.join(employeeInfo.getProviderId(), ":", employeeInfo.getUidType().name(), ":", employeeInfo.getUid()));
		requestBody.add("sector_id", sectorId);
		requestBody.add("system_id", employeeInfo.getProviderId().toString());
		requestBody.add("system_name", employeeInfo.getProviderName());
		
		return ExternalProviderLoginResultDto.builder()
				.sectorId(sectorId)
				.systemId(employeeInfo.getProviderId())
				.systemName(employeeInfo.getProviderName())
				.systemType(systemScope.name())
				.build();
	}
	
//	private String mapSystemScopeToSystemType(ExternalSystemScope systemScope) {
//		return switch(systemScope) {
//	    case EID_DEAU-> "Д;
//	    case EID_CEI -> 1;
//	    case EID_AEI, > 2;
//	    default -> 0; 
//	};
//	}
}
