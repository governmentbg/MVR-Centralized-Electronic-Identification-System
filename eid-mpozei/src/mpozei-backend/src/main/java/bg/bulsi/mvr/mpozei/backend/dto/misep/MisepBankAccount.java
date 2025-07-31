package bg.bulsi.mvr.mpozei.backend.dto.misep;

import lombok.Data;

@Data
public class MisepBankAccount {
    private String name;
    private String bank;
    private String bic;
    private String iban;
}
