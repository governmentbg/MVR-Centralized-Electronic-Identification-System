package bg.bulsi.mvr.mpozei.contract.dto;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;
import org.springframework.data.domain.Pageable;

import java.io.Serializable;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class FindHelpPagesFilter implements Serializable {
    private String keyword;
    private String language;
    private Pageable pageable;
}
