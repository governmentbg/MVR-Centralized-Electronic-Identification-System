package bg.bulsi.mvr.mpozei.model.application;

import bg.bulsi.mvr.mpozei.contract.dto.GuardianDetails;
import bg.bulsi.mvr.mpozei.contract.dto.PersonalIdentityDocument;
import lombok.Data;

import java.util.List;

@Data
public class TemporaryData {

	private List<GuardianDetails> guardians;
	
//	private ApplicationExport applicationExportRequest;
	
	private byte[] applicationExportResponse;
	
	private PersonalIdentityDocument personalIdentityDocument;
	
	private String signatureProvider;
}
