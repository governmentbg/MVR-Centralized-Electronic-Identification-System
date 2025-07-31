package bg.bulsi.mvr.iscei.contract.dto;

import com.fasterxml.jackson.annotation.JsonValue;
import com.fasterxml.jackson.annotation.JsonCreator;

/**
 * Gets or Sets ProfileStatus
 */

public enum ProfileStatusDTO {
  
  ENABLED("ENABLED"),
  
  DISABLED("DISABLED");

  private String value;

  ProfileStatusDTO(String value) {
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
  public static ProfileStatusDTO fromValue(String value) {
    for (ProfileStatusDTO b : ProfileStatusDTO.values()) {
      if (b.value.equals(value)) {
        return b;
      }
    }
    throw new IllegalArgumentException("Unexpected value '" + value + "'");
  }
}

