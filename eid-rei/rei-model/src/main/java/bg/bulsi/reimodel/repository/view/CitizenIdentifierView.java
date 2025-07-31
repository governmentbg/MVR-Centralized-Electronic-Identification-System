package bg.bulsi.reimodel.repository.view;

import bg.bulsi.reimodel.model.IdentifierType;
import com.fasterxml.jackson.annotation.JsonProperty;

import java.util.UUID;

//TODO: use contract?
public interface CitizenIdentifierView {
	UUID getId();

    String getFirstName();

    String getSecondName();

    String getLastName();

    IdentifierType getType();

    @JsonProperty("citizenIdentifierNumber")
    String getNumber();

    @JsonProperty("eidentityId")
    UUID getEidentityId();

    Boolean getActive();
}
