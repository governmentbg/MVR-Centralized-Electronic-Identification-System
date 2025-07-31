package bg.bulsi.mvr.mpozei.backend.dto.ejbca;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.AllArgsConstructor;
import lombok.Data;

import java.util.ArrayList;
import java.util.List;

@Data
public class EjbcaSearchEndEntityRequest {
    public static final String QUERY = "QUERY";
    @JsonProperty("max_number_of_results")
    private Integer maxNumberOfResults = 100;

    @JsonProperty("current_page")
    private Integer currentPage = 0;

    private List<EjbcaSearchCriteria> criteria;

    public void addEqualsCriteria(String key, String value) {
        if (this.criteria == null) {
            this.criteria = new ArrayList<>();
        }
        this.criteria.add(new EjbcaSearchCriteria(key, value, "EQUAL"));
    }

    @Data
    @AllArgsConstructor
    private static class EjbcaSearchCriteria {
        private String property;
        private String value;
        private String operation;
    }
}
