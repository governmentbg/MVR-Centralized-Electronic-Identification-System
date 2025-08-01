package bg.bulsi.mvr.iscei.contract.dto;

import com.fasterxml.jackson.annotation.JsonCreator;
import com.fasterxml.jackson.annotation.JsonValue;


public enum IdentifierTypeDTO {
	  
	  EGN("EGN"),
	  
	  LNCh("LNCh");
	  
	  private String value;

	  IdentifierTypeDTO(String value) {
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
	  public static IdentifierTypeDTO fromValue(String value) {
	    for (IdentifierTypeDTO b : IdentifierTypeDTO.values()) {
	      if (b.value.equals(value)) {
	        return b;
	      }
	    }
	    throw new IllegalArgumentException("Unexpected value '" + value + "'");
	  }
}
