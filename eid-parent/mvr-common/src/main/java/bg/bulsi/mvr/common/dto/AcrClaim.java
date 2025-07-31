package bg.bulsi.mvr.common.dto;

import com.fasterxml.jackson.annotation.JsonCreator;


/**
 * "acr" claim from JWT values
 */
public enum AcrClaim {
	EID_HIGH("eid_high"), // Base Profile
	EID_SUBSTANTIAL("eid_substantial"), // Login using Mobile Certificate
	EID_LOW("eid_low"); // Login using Chip Card

	private final String acrClaimValue;

	private AcrClaim(String acrClaimValue) {
		this.acrClaimValue = acrClaimValue;
	}

	public String getAcrClaimValue() {
		return acrClaimValue;
	}

}
