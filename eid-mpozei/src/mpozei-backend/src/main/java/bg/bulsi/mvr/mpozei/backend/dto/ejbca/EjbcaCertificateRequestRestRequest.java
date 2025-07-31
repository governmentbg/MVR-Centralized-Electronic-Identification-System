package bg.bulsi.mvr.mpozei.backend.dto.ejbca;

import com.fasterxml.jackson.annotation.JsonProperty;

import lombok.Data;

/*"CertificateRequestRestRequest": {
	"type": "object",
	"properties": {
		"certificate_request": {
			"type": "string",
			"example": "-----BEGIN CERTIFICATE REQUEST-----\nMIICh...V8shQ==\n-----END CERTIFICATE REQUEST-----",
			"description": "Certificate request"
		},
		"username": {
			"type": "string",
			"example": "JohnDoe",
			"description": "Username"
		},
		"password": {
			"type": "string",
			"example": "foo123",
			"description": "Password"
		},
		"include_chain": {
			"type": "boolean"
		},
		"certificate_authority_name": {
			"type": "string",
			"example": "ExampleCA",
			"description": "Certificate Authority (CA) name"
		}
	}
}*/
@Data
public class EjbcaCertificateRequestRestRequest {
	
    @JsonProperty("certificate_request")
    private String certificateRequest;
    private String username;
    private String password;

    @JsonProperty("include_chain")
    private boolean includeChain = true;

    @JsonProperty("certificate_authority_name")
    private String certificateAuthorityName;
}
