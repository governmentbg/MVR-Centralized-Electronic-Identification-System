package bg.bulsi.mvr.common.exception;

import com.fasterxml.jackson.annotation.JsonCreator;
import com.fasterxml.jackson.annotation.JsonProperty;

import java.io.Serial;
import java.util.HashSet;
import java.util.Set;

public class RegistryMVRException extends BaseMVRException {
	@Serial
	private static final long serialVersionUID = 5916083904678093703L;

	private static final String TYPE = "REGISTRY_ACCESS_EXCEPTION";
	private static final int STATUS = 500;

	public RegistryMVRException(ErrorCode errorCode) {
		super(errorCode.getDetail(), errorCode, STATUS, new HashSet<>());
	}

	public RegistryMVRException(ErrorCode errorCode, Set<String> additionalProperties) {
		super(errorCode.getDetail(), errorCode, STATUS, additionalProperties);
	}

	@JsonCreator
	public RegistryMVRException(@JsonProperty("type") String type,
								   @JsonProperty("title") String title,
								@JsonProperty("detail") String detail,
								   @JsonProperty("status") int status,
								   @JsonProperty("additionalProperties") Set<String> additionalProperties){
		super(type, title, detail, status, additionalProperties);
	}
}
