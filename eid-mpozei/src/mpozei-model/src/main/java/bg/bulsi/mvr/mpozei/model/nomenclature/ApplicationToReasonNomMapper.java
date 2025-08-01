package bg.bulsi.mvr.mpozei.model.nomenclature;

import org.springframework.stereotype.Component;

import bg.bulsi.mvr.mpozei.contract.dto.ApplicationStatus;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;

/**
 * Gets the ReasonNomenclatureType Name by the provided application
 */
@Component
public class ApplicationToReasonNomMapper {

	public String getReasonNomTypeName(AbstractApplication application) {
		String reasonNomType = switch (application.getApplicationType()){
		case STOP_EID -> "STOP_REASON_TYPE";
		case REVOKE_EID -> "REVOKE_REASON_TYPE";
		default -> null;
		};
		
		if(application.getStatus() == ApplicationStatus.DENIED) {
			reasonNomType = "DENIED_REASON_TYPE";
		}
		
		return reasonNomType;
	}
}
