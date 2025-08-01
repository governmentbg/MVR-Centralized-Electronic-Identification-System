package bg.bulsi.mvr.mpozei.backend.client;

import bg.bulsi.mvr.mpozei.backend.client.config.FeignInternalConfig;
import bg.bulsi.mvr.mpozei.backend.dto.CitizenDataDTO;
import bg.bulsi.mvr.mpozei.backend.dto.EidentityDTO;
import bg.bulsi.mvr.mpozei.backend.dto.FindEidentitiesRequest;
import bg.bulsi.mvr.mpozei.contract.dto.HttpResponse;
import bg.bulsi.mvr.mpozei.contract.dto.IdentifierType;
import bg.bulsi.mvr.mpozei.contract.dto.ReiUpdateCitizenIdentifierDTO;
import org.springframework.cloud.openfeign.FeignClient;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.UUID;

@FeignClient(name = "rei-client", url = "${services.rei-gateway-base-url}", configuration = FeignInternalConfig.class)
public interface ReiClient {
    @PostMapping("/api/v1/eidentity")
    UUID createEidentity(CitizenDataDTO citizenData);

    @GetMapping("/api/v1/eidentity")
    EidentityDTO findEidentityByNumberAndType(@RequestParam String number, @RequestParam IdentifierType type);

    @GetMapping("/api/v1/eidentity/internal")
    EidentityDTO findEidentityByNumberAndTypeInternal(@RequestParam String number, @RequestParam IdentifierType type);

    @GetMapping("/api/v1/eidentity/{eidentityId}")
    EidentityDTO getEidentityById(@PathVariable UUID eidentityId);

    @GetMapping("/api/v1/eidentity/{eidentityId}/internal")
    EidentityDTO getEidentityByIdInternal(@PathVariable UUID eidentityId);

    @PostMapping("/api/v1/eidentity/list")
    List<EidentityDTO> getEidentitiesByNumberAndType(@RequestBody FindEidentitiesRequest request);

    @PutMapping("/api/v1/eidentity/{eidentityId}/status")
    EidentityDTO updateEidentityActive(@PathVariable UUID eidentityId, @RequestParam boolean isActive);

    @PutMapping("/api/v1/eidentity/update-citizen-identifier")
    HttpResponse updateCitizenIdentifier(@RequestBody ReiUpdateCitizenIdentifierDTO dto);
}
