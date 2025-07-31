package bg.bulsi.mvr.common.config.security;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import bg.bulsi.mvr.common.dto.AcrClaim;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.extern.jackson.Jacksonized;

import org.apache.commons.lang3.EnumUtils;
import org.springframework.security.core.GrantedAuthority;
import org.springframework.security.oauth2.server.resource.authentication.JwtAuthenticationToken;

import java.io.Serial;
import java.io.Serializable;
import java.security.Principal;
import java.util.List;
import java.util.UUID;

@Jacksonized
@Builder
@AllArgsConstructor
@Getter
@JsonIgnoreProperties(ignoreUnknown = true)
public class UserContext implements Serializable {
    @Serial
    private static final long serialVersionUID = 8050003558413691800L;
    public static final String USER_CONTEXT_KEY = "user-context";

    private final String citizenIdentifier;
    private final String citizenIdentifierType;
    private final String systemId;
    private final String systemName;
    
    private final String username;
    private final String name;
    private final String email;
    private final String givenName;
    private final String middleName;
    private final String famillyName;
    private final String givenNameCyrillic;
    private final String middleNameCyrillic;
    private final String famillyNameCyrillic;
    private final String requesterUserId;
    private final List<String> authorities;
    private final UUID globalCorrelationId;
    private final String eidAdministratorFrontOfficeId;
    private final String eidAdministratorId;
    private final String citizenProfileId;
    private final String eidentityId;
    private final AcrClaim acrClaim;
    private Boolean regixAvailability;
    private String targetUserId;
    
    public UserContext(Principal principal) {
        JwtAuthenticationToken token = (JwtAuthenticationToken) principal;
        this.username = (String) token.getTokenAttributes().get("preferred_username");
        this.email = (String) token.getTokenAttributes().get("email");
        
        this.givenName = (String) token.getTokenAttributes().get("given_name");
        this.middleName = (String) token.getTokenAttributes().get("middle_name");
        this.famillyName = (String) token.getTokenAttributes().get("family_name");
        this.givenNameCyrillic = (String) token.getTokenAttributes().get("given_name_cyrillic");
        this.middleNameCyrillic = (String) token.getTokenAttributes().get("middle_name_cyrillic");
        this.famillyNameCyrillic = (String) token.getTokenAttributes().get("family_name_cyrillic");
        
        this.citizenIdentifier = (String) token.getTokenAttributes().getOrDefault("citizen_identifier", null);
        this.citizenIdentifierType = (String)token.getTokenAttributes().getOrDefault("citizen_identifier_type", null);
        this.eidAdministratorFrontOfficeId = (String) token.getTokenAttributes().getOrDefault("eidadminfrontofficeid", null);
        this.eidAdministratorId = (String) token.getTokenAttributes().getOrDefault("eidadministratorid", null);
        //this.eidAdministratorId = (String) token.getTokenAttributes().getOrDefault("system_id", null);
        this.citizenProfileId = (String) token.getTokenAttributes().getOrDefault("citizen_profile_id", null);
        this.systemId = (String) token.getTokenAttributes().getOrDefault("system_id", null);
        this.systemName = (String) token.getTokenAttributes().getOrDefault("system_name", null);;
        this.name = (String) token.getTokenAttributes().getOrDefault("name", null);
        this.authorities = token.getAuthorities().stream().map(GrantedAuthority::getAuthority).toList();
        this.globalCorrelationId = UUID.randomUUID();
        
        //Citizens should not have supplierid. EID Admins and m2m bots have supplierid
        //if citizenProfileId == null - tokens come form internal Keycloak
        if(citizenProfileId == null){
	        //this.requesterUserId = (String) token.getTokenAttributes().getOrDefault("userid", null);
        	this.requesterUserId = this.citizenIdentifier;
	        this.acrClaim = null;
	        this.eidentityId = null;
        } else {
	        this.eidentityId = (String) token.getTokenAttributes().getOrDefault("eidenity_id", null);
	        this.requesterUserId = this.citizenProfileId;
	        this.acrClaim = EnumUtils.getEnumIgnoreCase(AcrClaim.class, token.getTokenAttributes().getOrDefault("acr", null).toString());
	        
        }
        
        this.targetUserId = null;
    }

    
    
//  //  @JsonCreator
//    public UserContext(@JsonProperty("username") String username,
//                       @JsonProperty("email") String email,
//                       @JsonProperty("requesterUserId") String requesterUserId,
//                       @JsonProperty("name") String name,
////                       @JsonProperty("requesterEGN") String requesterEGN,
//                       @JsonProperty("citizenIdentifier") String citizenIdentifier,
//                       @JsonProperty("citizenIdentifierType") String citizenIdentifierType,
//                       @JsonProperty("systemId") String systemId,
//                       @JsonProperty("systemName") String systemName,
//                       @JsonProperty("targetUserId") String targetUserId,
//                       @JsonProperty("authorities") List<String> authorities,
//                       @JsonProperty("globalCorrelationId") UUID globalCorrelationId,
//                       @JsonProperty("eidAdministratorFrontOfficeId") String eidAdministratorFrontOfficeId,
//                       @JsonProperty("eidAdministratorId") String eidAdministratorId,
//                       @JsonProperty("regixAvailability") Boolean regixAvailability,
//                       @JsonProperty("citizenProfileId") String citizenProfileId
//    ) {
//        this.username = username;
//        this.name = name;
//        this.email = email;
//        this.requesterUserId = requesterUserId;
////        this.requesterEGN = requesterEGN;
//        this.citizenIdentifier = citizenIdentifier;
//        this.citizenIdentifierType = citizenIdentifierType;
//        this.systemId = systemId;
//        this.systemName = systemName;
//        this.targetUserId = targetUserId;
//        this.authorities = authorities;
//        this.globalCorrelationId = globalCorrelationId;
//        this.eidAdministratorFrontOfficeId = eidAdministratorFrontOfficeId;
//        this.eidAdministratorId = eidAdministratorId;
//        this.regixAvailability = regixAvailability;
//        this.citizenProfileId = citizenProfileId;
//    }

    /**
     * Check if login is email/password
     * 
     * @return
     */
    public boolean isBaseProfileAuth() {
    	return AcrClaim.EID_LOW.equals(this.acrClaim);
    }
    
    /**
     * Check if login is X509 Certificate
     * 
     * @return
     */
    public boolean isEidAuth() {
    	return AcrClaim.EID_SUBSTANTIAL.equals(this.acrClaim) || AcrClaim.EID_HIGH.equals(this.acrClaim);
    }

    public void setTargetUserId(Object targetUserId) {
        this.targetUserId = targetUserId != null ? targetUserId.toString() : null;
    }

    /**
     * @param regixAvailability the regixAvailability to set
     */
    public void setRegixAvailability(Boolean regixAvailability) {
        this.regixAvailability = regixAvailability;
    }
}
