package bg.bulsi.mvr.mpozei.backend.client;

import bg.bulsi.mvr.mpozei.backend.client.config.FeignExternalConfig;
import bg.bulsi.mvr.mpozei.contract.dto.edelivery.*;
import org.springframework.cloud.openfeign.FeignClient;
import org.springframework.http.MediaType;
import org.springframework.web.bind.annotation.*;

import java.io.File;
import java.util.UUID;

@FeignClient(name = "e-delivery-client", url = "${services.e-delivery-base-url}", configuration = FeignExternalConfig.class)
public interface EDeliveryClient {
//    @PostMapping("/integrations/eDelivery/send-message-on-behalf-to-person")
//    void sendEDeliveryNotification(EDeliverySearchProfileDTO dto);

    @PostMapping("/Missev/api/v1/edelivery/create-passive-individual-profile")
    EDeliveryProfileResponse createPassiveIndividualProfile(@RequestBody EDeliveryProfileRequest dto, @RequestParam UUID eIdentityId);

    @GetMapping("/Missev/api/v1/edelivery/search-profile")
    EDeliverySearchProfileDTO searchProfile(@RequestParam UUID EIdentityId, @RequestParam String Identifier, @RequestParam String TargetGroupId);

    @PostMapping("/Missev/api/v1/edelivery/send-message")
    EDeliveryMessageResponse sendMessage(@RequestBody EDeliveryMessageRequest dto, @RequestParam UUID eIdentityId);

    @PostMapping(value = "/Missev/api/v1/edelivery/upload/blobs", consumes = MediaType.MULTIPART_FORM_DATA_VALUE)
    EDeliveryAttachmentResponse uploadFile(@RequestPart("files") File file, @RequestParam UUID eIdentityId);
}