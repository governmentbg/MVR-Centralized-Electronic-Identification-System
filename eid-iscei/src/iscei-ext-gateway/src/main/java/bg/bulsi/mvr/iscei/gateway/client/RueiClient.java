package bg.bulsi.mvr.iscei.gateway.client;

import java.util.UUID;

import org.springframework.cloud.openfeign.FeignClient;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestParam;

import bg.bulsi.mvr.iscei.contract.dto.CitizenCertificateDetailsDTO;
import bg.bulsi.mvr.iscei.contract.dto.CitizenProfileAttachDTO;
import bg.bulsi.mvr.iscei.contract.dto.CitizenProfileDTO;
import bg.bulsi.mvr.iscei.gateway.client.config.FeignInternalConfig;

@FeignClient(name = "ruei-client", url = "${services.ruei-gateway-base-url}", configuration = FeignInternalConfig.class)
public interface RueiClient {

    @GetMapping("/api/v1/certificate-registry/eidentities/{eidentityId}/citizen-profile")
    CitizenProfileDTO getCitizenProfileByEidentityId(@PathVariable UUID eidentityId);

    @GetMapping("/api/v1/certificate-registry/citizens/profile/{citizenProfileId}")
    CitizenProfileDTO getCitizenProfileById(@PathVariable UUID citizenProfileId);

    @GetMapping("/api/v1/certificate-registry/citizens/profile")
    CitizenProfileDTO getCitizenProfileByEmail(@RequestParam String email);
    
    @GetMapping("/api/v1/certificate-registry/certificates")
    CitizenCertificateDetailsDTO getCitizenCertificateByIssuerAndSN(@RequestParam String issuer, @RequestParam String serialNumber);
    
    @PostMapping("/api/v1/certificate-registry/citizens/attach")
    CitizenProfileDTO attachCitizenProfile(CitizenProfileAttachDTO dto);
}