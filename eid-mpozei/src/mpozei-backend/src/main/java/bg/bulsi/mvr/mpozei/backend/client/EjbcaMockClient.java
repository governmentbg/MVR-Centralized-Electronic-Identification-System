package bg.bulsi.mvr.mpozei.backend.client;

import bg.bulsi.mvr.mpozei.backend.client.config.FeignExternalConfig;
import bg.bulsi.mvr.mpozei.backend.dto.ejbca.EjbcaEndEntityRequest;
import org.springframework.cloud.openfeign.FeignClient;

@FeignClient(name = "ejbca-mock-client", url = "${services.ejbca-base-url}", configuration = FeignExternalConfig.class)
public interface EjbcaMockClient {
    //    TODO: 8/15/2023 mocking until api is provided
//    @PostMapping("/v1/endentity")
    default void createEndEntity(EjbcaEndEntityRequest request) {
    }

//    //    TODO: 8/15/2023 mocking until api is provided
////    @PostMapping("/v1/certificate/pkcs10enroll")
//    default EjbcaCertificateDTO enrollForCertificate(EjbcaCertificateEnrollRequest request) {
//        EjbcaCertificateDTO response = new EjbcaCertificateDTO();
//        String certificate = "MIIC9TCCAd2gAwIBAgIUT7HMakuRCF3LRXdFvpAhjJBSDdkwDQYJKoZIhvcNAQELBQAwFjEUMBIGA1UEAwwLZXhhbXBsZS5jb20wIBcNMjQwNDA5MTMyMjIwWhgPMjEyNDAzMTYxMzIyMjBaMBYxFDASBgNVBAMMC2V4YW1wbGUuY29tMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAnF8z9Z/TPc0GtLVXj3KeuNOHkUYap6CceYpLo0TBwqlS48NPxrr4llbonuH5rEAZLBcjv4HQXA7dYRM1uPrhXRsHHeATIQgEF4Wuvh8q07BteOCp+dEerzcOOmveiqWFSS0OqGwWT1JJ37ePQpIud1BeQCHMwO1ExgBqFHPx++oo0tB3ah1ZYhpqiix7Y3OBW3MUp7sqvWa6xfDo5Z7DoOXX24KfpEhoI4OfREMTUNVo/cJWUbSFUgaUjDvc+2Tkv0jPhUjlAMHvOPu5QIV1wALTe8rf1JxLHCoV4gnuI1gCNorU8v6j+ZEirkk1FFeBb0nQz7+c7yN/pHSyC7v4IQIDAQABozkwNzAWBgNVHREEDzANggtleGFtcGxlLmNvbTAdBgNVHQ4EFgQUjMUvoAZmOCileO3oFV4qpiw0B2cwDQYJKoZIhvcNAQELBQADggEBACwPpHOZTXWKz/qCgf70YUxy9LCPZ7X5U+lg14q1n21GL5BXLf23eYhhtkXn1lv50+7fxNM1dy08n4oTYyosU+3CA3ZfDSniuKZ9eR/u1qAKpnBF9o0HYc5eDSYfIkK9L1xI9MpLLzWrrJ0Lhsuxqpi8CJyUz5SfpLdhCQWFUFzrVGHqS7oLSh0irmsBaPWu19ZhMdHe1BuMZHanDuQbbbdrNbhG5M8xPGWRLkmb2yklgmCG1T3ZD6h9n2lt9kYAkkKsw6NDs3ljh9G2QxftYHlV6NoY4TkMPIsMbfeg9HX5Bex2etLfG1VVlr8khlpetX2UQSuyadsLU1n2PMSIcR8=";
//        response.setCertificate(certificate);
//        response.setCertificateChain(List.of(certificate));
//        response.setSerialNumber("16560047501273096490576915830647825293");
//        return response;
//    }

    //    TODO: 8/15/2023 mocking until api is provided
//    @GetMapping("/v1/certificate/search")
//    default EjbcaSearchCertificateResponse searchCertificates(EjbcaSearchCertificateRequest request) {
//        EjbcaCertificateDTO dto = new EjbcaCertificateDTO();
//        String certificate = "MIIC9TCCAd2gAwIBAgIUT7HMakuRCF3LRXdFvpAhjJBSDdkwDQYJKoZIhvcNAQELBQAwFjEUMBIGA1UEAwwLZXhhbXBsZS5jb20wIBcNMjQwNDA5MTMyMjIwWhgPMjEyNDAzMTYxMzIyMjBaMBYxFDASBgNVBAMMC2V4YW1wbGUuY29tMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAnF8z9Z/TPc0GtLVXj3KeuNOHkUYap6CceYpLo0TBwqlS48NPxrr4llbonuH5rEAZLBcjv4HQXA7dYRM1uPrhXRsHHeATIQgEF4Wuvh8q07BteOCp+dEerzcOOmveiqWFSS0OqGwWT1JJ37ePQpIud1BeQCHMwO1ExgBqFHPx++oo0tB3ah1ZYhpqiix7Y3OBW3MUp7sqvWa6xfDo5Z7DoOXX24KfpEhoI4OfREMTUNVo/cJWUbSFUgaUjDvc+2Tkv0jPhUjlAMHvOPu5QIV1wALTe8rf1JxLHCoV4gnuI1gCNorU8v6j+ZEirkk1FFeBb0nQz7+c7yN/pHSyC7v4IQIDAQABozkwNzAWBgNVHREEDzANggtleGFtcGxlLmNvbTAdBgNVHQ4EFgQUjMUvoAZmOCileO3oFV4qpiw0B2cwDQYJKoZIhvcNAQELBQADggEBACwPpHOZTXWKz/qCgf70YUxy9LCPZ7X5U+lg14q1n21GL5BXLf23eYhhtkXn1lv50+7fxNM1dy08n4oTYyosU+3CA3ZfDSniuKZ9eR/u1qAKpnBF9o0HYc5eDSYfIkK9L1xI9MpLLzWrrJ0Lhsuxqpi8CJyUz5SfpLdhCQWFUFzrVGHqS7oLSh0irmsBaPWu19ZhMdHe1BuMZHanDuQbbbdrNbhG5M8xPGWRLkmb2yklgmCG1T3ZD6h9n2lt9kYAkkKsw6NDs3ljh9G2QxftYHlV6NoY4TkMPIsMbfeg9HX5Bex2etLfG1VVlr8khlpetX2UQSuyadsLU1n2PMSIcR8=";
//        dto.setCertificate(certificate);
//        dto.setCertificateChain(List.of(certificate));
//        EjbcaSearchCertificateResponse response = new EjbcaSearchCertificateResponse();
//        response.setCertificates(List.of(dto));
//        return response;
//    }
//
//    //    TODO: 8/15/2023 mocking until api is provided
////    @PutMapping("/v1/certificate/{issuerDn}/{certificateSerialNumber}/revoke")
//    default EjbcaRevocationResponse revokeCertificate(@PathVariable String issuerDn,
//                                                      @PathVariable String certificateSerialNumber,
//                                                      @RequestParam String revocationReason) {
//        EjbcaRevocationResponse response = new EjbcaRevocationResponse();
//        response.setRevocationDate(LocalDateTime.now());
//        response.setInvalidityDate(LocalDateTime.now());
//        response.setIssuerDn(issuerDn);
//        response.setSerialNumber(certificateSerialNumber);
//        response.setRevocationReason(revocationReason);
//        return response;
//    }

}
