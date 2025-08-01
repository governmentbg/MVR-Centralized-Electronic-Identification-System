package bg.bulsi.mvr.iscei.gateway.client.config;

import com.fasterxml.jackson.annotation.JsonCreator;
import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.Data;

import java.net.URI;
import java.util.List;
import java.util.Set;

@Data
public class InternalProblemDetail {
    private String type;
    private String title;
    private String detail;
    private int status;
    private URI instance;
    private Set<String> errors;

    @JsonCreator
    public InternalProblemDetail(@JsonProperty("type") String type,
                                 @JsonProperty("title") String title,
                                 @JsonProperty("detail") String detail,
                                 @JsonProperty("status") int status,
                                 @JsonProperty("errors") Set<String> errors) {
        this.type = type;
        this.title = title;
        this.detail = detail;
        this.status = status;
        this.errors = errors;
    }
}
