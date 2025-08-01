package bg.bulsi.mvr.mpozei.backend.service;

import java.util.List;

import bg.bulsi.mvr.mpozei.backend.dto.RegixIdentityInfoDTO;
import bg.bulsi.mvr.mpozei.contract.dto.IdentifierType;

public interface RegixService {
	String BULGARIA_COUNTRY_CODE = "БЛГ";
	String BULGARIA_COUNTRY_CODE_LATIN = "BGR";
	
	List<String> BULGARIAN_NATIONALITY_LIST = List.of("България", "Българско", "Българин", "Българка");
	
	static boolean isBulgarian(String nationality) {
		return BULGARIAN_NATIONALITY_LIST.stream().anyMatch(n -> n.equalsIgnoreCase(nationality));
	}
	
    RegixIdentityInfoDTO getIdentityInfoFromRegix(String citizenIdentifierNumber, IdentifierType type, String personalIdNumber);
}
