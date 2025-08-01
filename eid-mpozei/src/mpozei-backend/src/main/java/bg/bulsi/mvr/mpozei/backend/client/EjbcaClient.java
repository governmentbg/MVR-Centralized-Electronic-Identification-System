package bg.bulsi.mvr.mpozei.backend.client;

import bg.bulsi.mvr.mpozei.backend.client.config.FeignEjbcaConfig;
import bg.bulsi.mvr.mpozei.backend.dto.ejbca.*;
import org.springframework.cloud.openfeign.FeignClient;
import org.springframework.web.bind.annotation.*;

@FeignClient(name = "ejbca-client", url = "${services.ejbca-base-url}", configuration = FeignEjbcaConfig.class)
public interface EjbcaClient {
    @PostMapping("/ejbca/ejbca-rest-api/v1/endentity")
    void createEndEntity(@RequestBody EjbcaEndEntityRequest request);

    @PostMapping("/ejbca/ejbca-rest-api/v1/certificate/pkcs10enroll")
    EjbcaCertificateDTO enrollForCertificate(EjbcaCertificateEnrollRequest request);

    @PostMapping("/ejbca/ejbca-rest-api/v1/certificate/certificaterequest")
    EjbcaCertificateDTO certificateRequest2Cert(EjbcaCertificateRequestRestRequest request);

    @GetMapping("/ejbca/ejbca-rest-api/v1/certificate/search")
    EjbcaSearchCertificateResponse searchCertificates(EjbcaSearchCertificateRequest request);

    @PutMapping("/ejbca/ejbca-rest-api/v1/certificate/{issuerDn}/{certificateSerialNumber}/revoke")
    EjbcaRevocationResponse revokeCertificate(@PathVariable String issuerDn,
                                              @PathVariable String certificateSerialNumber,
                                              @RequestParam EjbcaRevocationReason reason);

    @PostMapping("/ejbca/ejbca-rest-api/v1/endentity/search")
    EjbcaSearchEndEntityResponse searchEndEntity(@RequestBody EjbcaSearchEndEntityRequest request);

    @PostMapping("/ejbca/ejbca-rest-api/v1/endentity/{endentityName}/setstatus")
    void setEndEntityStatus(@PathVariable String endentityName, EjbcaEndEntityStatusUpdateRequest request);
}
