package bg.bulsi.mvr.mpozei.contract.dto;

import lombok.AllArgsConstructor;
import lombok.Data;

import java.io.Serial;
import java.io.Serializable;
import java.util.UUID;

@Data
@AllArgsConstructor
public class DenyApplicationDTO implements Serializable {
    @Serial
    private static final long serialVersionUID = 2144599735308380839L;

    private UUID applicationId;
    private UUID reasonId;
    private String reasonText;
}
