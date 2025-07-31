package bg.bulsi.mvr.common.exception;

import com.fasterxml.jackson.annotation.JsonCreator;
import com.fasterxml.jackson.annotation.JsonProperty;
import org.springframework.http.HttpStatus;

import java.io.Serial;
import java.util.HashSet;
import java.util.Set;

public class ValidationMVRException extends BaseMVRException {
    @Serial
    private static final long serialVersionUID = -8740854015617242941L;

    private final static String TITLE = "Validation Exception";
    private final static String TYPE = "PROVIDED_DATA_NOT_VALID";
    private final static int STATUS = HttpStatus.BAD_REQUEST.value();
    
	public ValidationMVRException(ErrorCode errorCode, Object... messageParams) {
		super(String.format(errorCode.getDetail(), messageParams), errorCode, STATUS, new HashSet<>());
	}

	public ValidationMVRException(ErrorCode errorCode, HttpStatus httpStatus, Object... messageParams) {
		super(String.format(errorCode.getDetail(), messageParams), errorCode, httpStatus.value(), new HashSet<>());
	}
	
    public ValidationMVRException(String message, ErrorCode errorCode) {
        super(message, errorCode, STATUS, new HashSet<>());
    }

    public ValidationMVRException(String message, ErrorCode errorCode, Set<String> additionalProperties) {
        super(message, errorCode, STATUS, additionalProperties);
    }

    public ValidationMVRException(ErrorCode errorCode, Set<String> additionalProperties, Object... messageParams) {
        super(String.format(errorCode.getDetail(), messageParams), errorCode, STATUS, additionalProperties);
    }
//
//    public ValidationMVRException(Throwable e) {
//        super(e);
//    }

    @JsonCreator
    public ValidationMVRException(@JsonProperty("type") String type,
                                   @JsonProperty("title") String title,
                                  @JsonProperty("detail") String detail,
                                   @JsonProperty("status") int status,
                                   @JsonProperty("additionalProperties") Set<String> additionalProperties){
        super(type, title, detail, status, additionalProperties);
    }
}
