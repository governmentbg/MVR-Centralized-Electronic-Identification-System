package bg.bulsi.mvr.mpozei.backend.dto;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.Data;
import org.springframework.format.annotation.DateTimeFormat;

import java.time.OffsetDateTime;

@Data
public class DeceasedCitizenDTO {
    @JsonProperty("personalId")
    private String citizenIdentifierNumber;

    @JsonProperty("date")
    @DateTimeFormat(iso = DateTimeFormat.ISO.DATE_TIME)
    private OffsetDateTime date;

    @JsonProperty("uidType")
    private IdentifierTypeDTO citizenIdentifierType;
}
