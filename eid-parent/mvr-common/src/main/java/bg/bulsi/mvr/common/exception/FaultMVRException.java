package bg.bulsi.mvr.common.exception;

import com.fasterxml.jackson.annotation.JsonCreator;
import com.fasterxml.jackson.annotation.JsonProperty;

import java.io.Serial;
import java.util.HashSet;
import java.util.Set;

public class FaultMVRException extends BaseMVRException {
    @Serial
    private static final long serialVersionUID = -8325920526317605802L;

    private final static String TITLE = "Internal Server Error";
    private final static String TYPE = "REASON_UNKNOWN";
    private final static int STATUS = 500;

    public FaultMVRException(String message, ErrorCode errorCode, Set<String> additionalProperties) {
        super(message, errorCode, STATUS, additionalProperties);
    }

    public FaultMVRException(ErrorCode errorCode, Object... params) {
        super(String.format(errorCode.getDetail(), params), errorCode, STATUS, new HashSet<>());
    }

    public FaultMVRException(String message, ErrorCode errorCode) {
        super(message, errorCode, STATUS, new HashSet<>());
    }

    public FaultMVRException(Throwable cause) {
        super(cause);
    }

    @JsonCreator
    public FaultMVRException(@JsonProperty("type") String type,
                                   @JsonProperty("title") String title,
                                   @JsonProperty("detail") String detail,
                                   @JsonProperty("status") int status,
                                   @JsonProperty("additionalProperties") Set<String> additionalProperties){
        super(type, title, detail, status, additionalProperties);
    }
}
