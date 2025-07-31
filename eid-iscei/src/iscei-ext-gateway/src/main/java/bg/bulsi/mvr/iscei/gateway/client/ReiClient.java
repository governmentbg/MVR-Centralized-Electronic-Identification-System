package bg.bulsi.mvr.iscei.gateway.client;

import java.util.UUID;

import org.springframework.cloud.openfeign.FeignClient;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestParam;

import bg.bulsi.mvr.iscei.contract.dto.EidentityDTO;
import bg.bulsi.mvr.iscei.contract.dto.IdentifierTypeDTO;
import bg.bulsi.mvr.iscei.gateway.client.config.FeignInternalConfig;

@FeignClient(name = "rei-client", url = "${services.rei-gateway-base-url}", configuration = FeignInternalConfig.class)
public interface ReiClient {
    @GetMapping("/api/v1/eidentity")
    EidentityDTO findEidentityByNumberAndType(@RequestParam String number, @RequestParam IdentifierTypeDTO type);

    @GetMapping("/api/v1/eidentity/{eidentityId}")
    EidentityDTO getEidentityById(@PathVariable UUID eidentityId);
}
