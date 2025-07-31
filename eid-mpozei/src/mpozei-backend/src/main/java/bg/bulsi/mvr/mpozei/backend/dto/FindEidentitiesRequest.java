package bg.bulsi.mvr.mpozei.backend.dto;

import lombok.AllArgsConstructor;
import lombok.Data;

import java.util.List;

@Data
@AllArgsConstructor
public class FindEidentitiesRequest {
    private List<EidentityRequestDTO> eidentities;
}
