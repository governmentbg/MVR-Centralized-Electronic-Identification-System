package bg.bulsi.mvr.mpozei.contract.dto;

import lombok.AllArgsConstructor;
import lombok.Data;

import java.io.Serial;
import java.io.Serializable;

@Data
@AllArgsConstructor
public class UpdateProfilePasswordDTO implements Serializable {
    @Serial
    private static final long serialVersionUID = 9161417026050966253L;

    @lombok.ToString.Exclude @lombok.EqualsAndHashCode.Exclude 
    private String oldPassword;
    
    @lombok.ToString.Exclude @lombok.EqualsAndHashCode.Exclude 
    private String newPassword;
}
