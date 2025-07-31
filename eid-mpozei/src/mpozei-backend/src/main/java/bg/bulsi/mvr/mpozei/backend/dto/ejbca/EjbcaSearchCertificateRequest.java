package bg.bulsi.mvr.mpozei.backend.dto.ejbca;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.AllArgsConstructor;
import lombok.Data;

import java.util.ArrayList;
import java.util.List;

@Data
public class EjbcaSearchCertificateRequest {
    @JsonProperty("max_number_of_results")
    private Integer maximumNumberOfResults = 399;

    private List<EjbcaSearchCriteria> criteria;

    public void addLikeCriteria(String key, String value) {
        if (this.criteria == null) {
            this.criteria = new ArrayList<>();
        }
        this.criteria.add(new EjbcaSearchCriteria(key, value, "LIKE"));
    }

    @Data
    @AllArgsConstructor
    private static class EjbcaSearchCriteria {
        private String property;
        private String value;
        private String operation;
    }

    @Data
    private static class Pagination {
        @JsonProperty("page_size")
        private Integer pageSize = 1000;
        @JsonProperty("currentPage")
        private Integer currentPage = 0;
    }
}
