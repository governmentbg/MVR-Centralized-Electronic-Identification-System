package bg.bulsi.mvr.mpozei.backend.client;

import bg.bulsi.mvr.mpozei.backend.client.config.FeignExternalConfig;
import bg.bulsi.mvr.mpozei.backend.dto.PunCarrierDTO;
import bg.bulsi.mvr.mpozei.backend.dto.PunCreateCarrierRequest;
import org.springframework.cloud.openfeign.FeignClient;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestParam;

import java.util.List;
import java.util.UUID;

@FeignClient(name = "pun-client", url = "${services.pun-base-url}", configuration = FeignExternalConfig.class)
public interface PunClient {
    @PostMapping("/api/v1/Carriers")
    UUID createPunCarrier(PunCreateCarrierRequest request);

    @GetMapping("/api/v1/Carriers")
    List<PunCarrierDTO> getPunCarrierByEidentityIdAndCertificateId(@RequestParam("EId") UUID eidentityId,
                                                                   @RequestParam("CertificateId") UUID certificateId);

    @GetMapping("/api/v1/Carriers")
    List<PunCarrierDTO> getPunCarriersByEidentityId(@RequestParam("EId") UUID eidentityId);

    @GetMapping("/api/v1/Carriers")
    List<PunCarrierDTO> getPunCarriersBySerialNumber(@RequestParam("SerialNumber") String deviceSerialNumber);

    @GetMapping("/api/v1/Carriers")
    List<PunCarrierDTO> getPunCarriersBySerialNumberAndEidentityId(@RequestParam("SerialNumber") String deviceSerialNumber, @RequestParam("EId") UUID eidentityId);
}
