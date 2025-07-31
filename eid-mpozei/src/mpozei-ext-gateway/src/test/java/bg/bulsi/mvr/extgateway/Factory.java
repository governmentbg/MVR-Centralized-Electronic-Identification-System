package bg.bulsi.mvr.extgateway;

import bg.bulsi.mvr.mpozei.contract.dto.*;

import java.util.HexFormat;
import java.util.UUID;

public class Factory {

    public static ApplicationXmlRequest createApplicationRequest() {
        ApplicationXmlRequest request = new ApplicationXmlRequest();
        request.setDeviceId(UUID.fromString("bc9f97f8-b004-4b61-ac85-7d1a7cb05f14"));
        request.setApplicationType(ApplicationType.ISSUE_EID);
        request.setCitizenIdentifierType(IdentifierType.LNCh);
        request.setCitizenIdentifierNumber("1234567899");
        request.setEidAdministratorId(UUID.fromString("194a90a0-3b9d-47f5-865a-ad8bcf2c3acc"));
        request.setLastName("lastName");
        request.setFirstName("firstName");
        request.setSecondName("secondName");
        return request;
    }

    public static byte[] createByteArray() {
        return HexFormat.of().parseHex("1A3F");
    }

    public static ApplicationResponse createApplicationResponse() {
        ApplicationResponse response = new ApplicationResponse();
        response.setStatus(ApplicationStatus.SIGNED);
        response.setId(UUID.randomUUID());
        response.setEidAdministratorName("MVR");
        return response;
    }

    public static CertificateRequest createCertificateRequest() {
        CertificateRequest request = new CertificateRequest();
        request.setCertificateSigningRequest("csr");
        request.setApplicationId(UUID.randomUUID());
        request.setCertificateAuthorityName("CA_NAME");
        return request;
    }

    public static CertificateResponse createCertificateResponse() {
        CertificateResponse response = new CertificateResponse();
        response.setSerialNumber("serial");
        return response;
    }
}
