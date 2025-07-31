package bg.bulsi.mvr.iscei.contract.dto;

import com.fasterxml.jackson.annotation.JsonCreator;
import com.fasterxml.jackson.annotation.JsonValue;

public enum ExternalSystemScope {

	EID_AEI("eid_aei"),
	EID_CEI("eid_cei"),
    EID_DEAU("eid_deau");

	private String value;

	ExternalSystemScope(String value) {
		this.value = value;
	}

	@JsonValue
	public String getValue() {
		return value;
	}

	@Override
	public String toString() {
		return String.valueOf(value);
	}

	@JsonCreator
	public static ExternalSystemScope fromValue(String value) {
		for (ExternalSystemScope b : ExternalSystemScope.values()) {
			if (b.value.equals(value)) {
				return b;
			}
		}
		throw new IllegalArgumentException("Unexpected value '" + value + "'");
	}
}
