package bg.bulsi.mvr.mpozei.backend.dto.ejbca;

import bg.bulsi.mvr.mpozei.backend.dto.SearchEndEntityDTO;
import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.Data;

import java.util.List;

@Data
public class EjbcaSearchEndEntityResponse {
    @JsonProperty("end_entities")
    private List<SearchEndEntityDTO> endEntities;
    @JsonProperty("more_results")
    private Boolean moreResults;
}
