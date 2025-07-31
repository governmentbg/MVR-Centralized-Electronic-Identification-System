package bg.bulsi.mvr.common.exception;

import com.fasterxml.jackson.annotation.JsonCreator;
import com.fasterxml.jackson.annotation.JsonProperty;
import org.springframework.http.HttpStatus;

import java.io.Serial;
import java.util.HashSet;
import java.util.Set;

public class WrongStatusException  extends BaseMVRException {
    @Serial
    private static final long serialVersionUID = 7702616107433435823L;

    private static final String TYPE = "CONFLICT";
    private static final int STATUS = HttpStatus.CONFLICT.value();

    public WrongStatusException(ErrorCode errorCode, Object... messageParams) {
        super(String.format(errorCode.getDetail(), messageParams), errorCode, STATUS, new HashSet<>());
    }

    public WrongStatusException(ErrorCode errorCode, String id, Set<String> additionalProperties) {
        super(errorCode.getDetail() + id, errorCode, STATUS, additionalProperties);
    }

    @JsonCreator
    public WrongStatusException(@JsonProperty("type") String type,
                                   @JsonProperty("title") String title,
                                @JsonProperty("detail") String detail,
                                   @JsonProperty("status") int status,
                                   @JsonProperty("additionalProperties") Set<String> additionalProperties){
        super(type, title, detail, status, additionalProperties);
    }
}
