package bg.bulsi.mvr.apigateway;

import bg.bulsi.mvr.mpozei.contract.dto.*;

import java.util.HexFormat;
import java.util.UUID;

public class Factory {

    public static DeskApplicationRequest createDeskApplicationRequest() {
        DeskApplicationRequest request = new DeskApplicationRequest();
        request.setDeviceId(UUID.fromString("bc9f97f8-b004-4b61-ac85-7d1a7cb05f14"));
        request.setApplicationType(ApplicationType.ISSUE_EID);
//        request.setEmail("advc@abv.bg");
        request.setCitizenIdentifierType(IdentifierType.LNCh);
        request.setCitizenIdentifierNumber("1234567899");
        request.setLastName("lastName");
        request.setFirstName("firstName");
        request.setSecondName("secondName");
//        request.setPhoneNumber("9876543211");
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
