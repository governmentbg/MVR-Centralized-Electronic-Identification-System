package bg.bulsi.mvr.mpozei.backend.dto.ejbca;

import lombok.Data;

@Data
public class EjbcaEndEntityStatusUpdateRequest {
    private String status;
    private String token = "USERGENERATED";
    private String password;
}
