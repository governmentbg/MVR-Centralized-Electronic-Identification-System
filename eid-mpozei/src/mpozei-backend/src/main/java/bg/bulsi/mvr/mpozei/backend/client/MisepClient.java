package bg.bulsi.mvr.mpozei.backend.client;

import bg.bulsi.mvr.mpozei.backend.client.config.FeignExternalConfig;
import bg.bulsi.mvr.mpozei.backend.dto.misep.*;
import bg.bulsi.mvr.mpozei.contract.dto.MisepPaymentDTO;
import org.springframework.cloud.openfeign.FeignClient;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.UUID;

@FeignClient(name = "misep-client", url = "${services.misep-base-url}", configuration = FeignExternalConfig.class)
public interface MisepClient {
    @GetMapping("/api/v1/PaymentRequests/{paymentRequestId}/status")
    MisepStatusCode getPaymentStatus(@PathVariable UUID paymentRequestId, @RequestParam UUID citizenProfileId);

    @GetMapping("/api/v1/ePayments/get-clients-by-eik")
    MisepClientResponse getClientsByEik(@RequestParam("eik") String eikNumber);

    @PostMapping("/api/v1/PaymentRequests")
    MisepPaymentResponse createPaymentRequest(@RequestBody MisepPaymentRequest request);

    @GetMapping("/api/v1/PaymentRequests")
    List<MisepPaymentDTO> getAllPaymentRequests(@RequestParam String citizenProfileId);

    @GetMapping("/api/v1/PaymentRequests/{paymentRequestId}/status")
    String getPaymentStatusByPaymentId(@PathVariable String paymentRequestId, @RequestParam String citizenProfileId);
}
