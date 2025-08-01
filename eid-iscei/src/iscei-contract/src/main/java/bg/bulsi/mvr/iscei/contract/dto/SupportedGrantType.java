package bg.bulsi.mvr.iscei.contract.dto;

import org.springframework.util.Assert;

public enum SupportedGrantType {
	
	AUTHORIZATION_CODE ("authorization_code"),
	REFRESH_TOKEN ("refresh_token");

	private String value;

	private SupportedGrantType(String value) {
		Assert.hasText(value, "value cannot be empty");
		this.value = value;
	}
	
	public String getValue() {
		return this.value;
	}
}
