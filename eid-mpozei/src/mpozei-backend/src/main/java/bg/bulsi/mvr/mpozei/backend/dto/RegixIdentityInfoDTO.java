package bg.bulsi.mvr.mpozei.backend.dto;

import bg.bulsi.mvr.mpozei.model.pivr.common.Nationality;
import lombok.Data;

import java.time.LocalDate;
import java.util.List;

@Data
public class RegixIdentityInfoDTO {
    private LocalDate birthDate;
    private String firstNameLatin;
    private String secondNameLatin;
    private String lastNameLatin;
    private String personalIdNumber;
    private String personalIdDocumentType;
    private String personalIdDocumentTypeLatin;
    private LocalDate personalIdIssueDate;
    private LocalDate personalIdValidityToDate;
    private String personalIdIssuer;
    private String personalIdIssuerLatin;
    private List<Nationality> nationalityList;
}
