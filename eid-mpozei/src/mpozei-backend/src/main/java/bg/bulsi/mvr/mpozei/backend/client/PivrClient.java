package bg.bulsi.mvr.mpozei.backend.client;

import bg.bulsi.mvr.mpozei.backend.client.config.FeignExternalConfig;
import bg.bulsi.mvr.mpozei.backend.dto.DeceasedCitizenDTO;
import bg.bulsi.mvr.mpozei.backend.dto.PivrSignatureValidateRequest;
import bg.bulsi.mvr.mpozei.contract.dto.IdentifierType;
import bg.bulsi.mvr.mpozei.contract.dto.PivrIdchangesResponseDto;
import bg.bulsi.mvr.mpozei.model.pivr.CitizenDateOfDeath;
import bg.bulsi.mvr.mpozei.model.pivr.CitizenProhibition;
import bg.bulsi.mvr.mpozei.model.pivr.RegiXResult;
import org.springframework.cloud.openfeign.FeignClient;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestParam;

import java.util.List;

/**
 * HTTP Client to the Subsystem for integration with external registries (PIVR)
 */
@FeignClient(name = "pivr-client", url = "${services.pivr-base-url}", configuration = FeignExternalConfig.class)
public interface PivrClient {
    
	@GetMapping("/api/v1/DateOfProhibition")
	CitizenProhibition getDateOfProhibition(@RequestParam(name="PersonalId") String citizenIdentifierNumber, @RequestParam(name="UidType") String citizenIdentifierType);
	
	@GetMapping("/api/v1/DateOfDeath")
	CitizenDateOfDeath getDateOfDeath(@RequestParam(name="PersonalId") String citizenIdentifierNumber, @RequestParam(name="UidType") String citizenIdentifierType);
	
	@GetMapping("/api/v1/Registries/mvr/getpersonalidentityv2")
	RegiXResult getPersonalIdentityV2(@RequestParam(name = "EGN", required = true) String citizenIdentifierNumber, @RequestParam(name = "IdentityDocumentNumber", required = true) String personalIdNumber);
	
	@GetMapping("/api/v1/Registries/mvr/getforeignidentityv2")
	RegiXResult getForeignIdentityV2(@RequestParam(name = "IdentifierType", required = true) IdentifierType citizenIdentifierType, @RequestParam(name = "Identifier", required = true) String citizenIdentifierNumber);

	@PostMapping("/api/v1/Verify/signature")
	void validateSignedXml(PivrSignatureValidateRequest request);

	@GetMapping("/api/v1/DateOfDeath/deceasedByPeriod")
	List<DeceasedCitizenDTO> getDeceasedByDateRange(@RequestParam("From") String from, @RequestParam("To") String to);
	
	@GetMapping("/api/v1/IdentityChecks/idchanges")
	List<PivrIdchangesResponseDto> getIdChanges(@RequestParam(name="PersonalId") String citizenIdentifierNumber, @RequestParam(name="UidType") String citizenIdentifierType, @RequestParam("CreatedOnFrom") String createdOnFrom, @RequestParam("CreatedOnTo") String createdOnTo);
}
