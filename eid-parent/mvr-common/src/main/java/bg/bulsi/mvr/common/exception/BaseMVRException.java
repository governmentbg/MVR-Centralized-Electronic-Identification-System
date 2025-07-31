package bg.bulsi.mvr.common.exception;

import com.fasterxml.jackson.annotation.JsonCreator;
import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.ToString;

import java.io.Serial;
import java.util.HashSet;
import java.util.Set;

@NoArgsConstructor
@ToString
@Getter
public class BaseMVRException extends RuntimeException {
    @Serial
    private static final long serialVersionUID = -9012660901039234849L;

    private String type;
    private String title;
    private int status;
    private Set<String> additionalProperties = new HashSet<>();

    public BaseMVRException(String message, ErrorCode errorCode, int status, Set<String> properties) {
        super(message);
        this.title = errorCode.getTitle();
        this.status = status;
        if (errorCode.isInResponse()) {
            properties.add(errorCode.name());
        }
        this.additionalProperties = properties;
    }

    @JsonCreator
    public BaseMVRException(@JsonProperty("type") String type,
                            @JsonProperty("title") String title,
                            @JsonProperty("detail") String detail,
                            @JsonProperty("status") int status,
                            @JsonProperty("additionalProperties") Set<String> additionalProperties){
        super(detail);
        this.type = type;
        this.title = title;
        this.status = status;
        this.additionalProperties = additionalProperties;
    }

    public BaseMVRException(Throwable cause) {
        super(cause);
    }
}
