package bg.bulsi.mvr.iscei.gateway.client;

import java.util.UUID;

import org.springframework.cloud.openfeign.FeignClient;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestParam;

import bg.bulsi.mvr.iscei.contract.dto.IdentifierTypeDTO;
import bg.bulsi.mvr.iscei.contract.dto.ProviderEmployeeInfo;
import bg.bulsi.mvr.iscei.gateway.client.config.FeignInternalConfig;

@FeignClient(name = "pdeau-client", url = "${services.pdeau-gateway-base-url}", configuration = FeignInternalConfig.class)
public interface PdeauClient {

    @GetMapping("/api/v1/Providers/{providerId}/users")
    ProviderEmployeeInfo getEmployeeInfo(@PathVariable UUID providerId, @RequestParam(name = "Uid") String number, @RequestParam(name = "UidType") IdentifierTypeDTO type);
}
