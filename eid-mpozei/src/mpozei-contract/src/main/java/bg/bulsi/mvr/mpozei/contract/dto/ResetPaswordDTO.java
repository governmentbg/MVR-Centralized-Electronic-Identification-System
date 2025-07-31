package bg.bulsi.mvr.mpozei.contract.dto;

import lombok.AllArgsConstructor;
import lombok.Data;

import java.io.Serial;
import java.io.Serializable;

@Data
@AllArgsConstructor
public class ResetPaswordDTO implements Serializable {
    @Serial
    private static final long serialVersionUID = 8069351410029824206L;

    @lombok.ToString.Exclude @lombok.EqualsAndHashCode.Exclude 
    private String password;
    private String token;
}
