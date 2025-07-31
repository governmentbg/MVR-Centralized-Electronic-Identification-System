package bg.bulsi.mvr.common.exception;

import com.fasterxml.jackson.annotation.JsonCreator;
import com.fasterxml.jackson.annotation.JsonProperty;

import java.io.Serial;
import java.util.HashSet;
import java.util.Set;

public class EntityNotFoundException extends BaseMVRException {
	@Serial
	private static final long serialVersionUID = -6646186558807967295L;

	private static final String TYPE = "ENTITY_NOT_FOUND";
	private static final int STATUS = 404;

	public EntityNotFoundException(ErrorCode errorCode, String id) {
		super(String.format(errorCode.getDetail(), id), errorCode, STATUS, new HashSet<>());
	}

	public EntityNotFoundException(ErrorCode errorCode, Object... messageParams) {
		super(String.format(errorCode.getDetail(), messageParams), errorCode, STATUS, new HashSet<>());
	}
	
	public EntityNotFoundException(ErrorCode errorCode, String id, Set<String> additionalProperties) {
		super(String.format(errorCode.getDetail(), id), errorCode, STATUS, additionalProperties);
	}

	@JsonCreator
	public EntityNotFoundException(@JsonProperty("type") String type,
							@JsonProperty("title") String title,
								   @JsonProperty("detail") String detail,
							@JsonProperty("status") int status,
							@JsonProperty("additionalProperties") Set<String> additionalProperties) {
		super(type, title, detail, status, additionalProperties);
	}
}
