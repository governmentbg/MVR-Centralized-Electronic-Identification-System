package bg.bulsi.mvr.mpozei.model.pivr;

import java.time.OffsetDateTime;

public record CitizenProhibition(OffsetDateTime date, ProhibitionType typeOfProhibition, String descriptionOfProhibition) {
}
