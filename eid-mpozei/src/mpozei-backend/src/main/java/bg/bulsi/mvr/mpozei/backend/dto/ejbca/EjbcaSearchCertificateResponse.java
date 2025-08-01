package bg.bulsi.mvr.mpozei.backend.dto.ejbca;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.Data;
import lombok.Getter;

import java.util.List;

@Data
public class EjbcaSearchCertificateResponse {
    @JsonProperty("pagination_summary")
    private PaginationSummary paginationSummary;
    private List<EjbcaSearchCertificateDTO> certificates;

    @Getter
    private static class PaginationSummary {
        @JsonProperty("page_size")
        private Integer pageSize;

        @JsonProperty("current_page")
        private Integer currentPage;

        @JsonProperty("total_certs")
        private Integer totalCerts;
    }
}
