package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.exception.ValidationMVRException;
import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.backend.client.PivrClient;
import bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import bg.bulsi.mvr.mpozei.model.pivr.CitizenProhibition;
import bg.bulsi.mvr.mpozei.model.pivr.ProhibitionType;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.time.OffsetDateTime;
import java.util.List;

import static bg.bulsi.mvr.common.exception.ErrorCode.CITIZEN_IS_DEAD;
import static bg.bulsi.mvr.common.exception.ErrorCode.CITIZEN_IS_PROHIBITED;
import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType.BASE_PROFILE;
import static bg.bulsi.mvr.mpozei.contract.dto.ApplicationSubmissionType.EID;


/**
 * Checks if the person is under prohibition or death.
 * If this step fail we rollback.
 */
@Slf4j
@Component
@RequiredArgsConstructor
public class PivrVerificationStep extends Step<AbstractApplication> {
	
	@Autowired
	private PivrClient pivrClient;
	
	
	@Override
	public AbstractApplication process(AbstractApplication application) {
        log.info("Application with id: {} entered PivrVerificationStep", application.getId());
        
        OffsetDateTime dateOfDeath = pivrClient.getDateOfDeath(application.getCitizenIdentifierNumber(), application.getCitizenIdentifierType().name()).date();
        if(dateOfDeath != null) {
        	throw new ValidationMVRException(CITIZEN_IS_DEAD);
        }
        
        CitizenProhibition citizenProhibition = pivrClient.getDateOfProhibition(application.getCitizenIdentifierNumber(), application.getCitizenIdentifierType().name());
        if(citizenProhibition.typeOfProhibition() != ProhibitionType.NONE) {
			ApplicationSubmissionType submissionType = application.getSubmissionType();
			if (List.of(BASE_PROFILE, EID).contains(submissionType)) {
				throw new ValidationMVRException(CITIZEN_IS_PROHIBITED);
			}
//            application.getParams().setRequireGuardians(true);
        }
        
		return application;
	}

	@Override
	public PipelineStatus getStatus() {
		return PipelineStatus.PIVR_VERIFICATION;
	}
}
