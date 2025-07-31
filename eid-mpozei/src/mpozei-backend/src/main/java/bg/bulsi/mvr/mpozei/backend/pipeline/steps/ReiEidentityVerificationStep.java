package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.backend.client.ReiClient;
import bg.bulsi.mvr.mpozei.backend.dto.EidentityDTO;
import bg.bulsi.mvr.mpozei.contract.dto.IdentifierType;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;

import static bg.bulsi.mvr.common.exception.ErrorCode.*;
import static bg.bulsi.mvr.common.util.ValidationUtil.assertEquals;
import static bg.bulsi.mvr.common.util.ValidationUtil.assertNotNull;
import static bg.bulsi.mvr.common.util.ValidationUtil.assertEqualsIgnoreCase;

@Slf4j
@Component
@RequiredArgsConstructor
public class ReiEidentityVerificationStep extends Step<AbstractApplication> {

    private final ReiClient reiClient;

    @Override
    public AbstractApplication process(AbstractApplication application) {
        log.info("Application with id: {} entered ReiEidentityVerificationStep", application.getId());
        EidentityDTO eidentityDTO = getActiveCitizenIdentifier(application.getCitizenIdentifierNumber(), application.getCitizenIdentifierType());
        
        assertEqualsIgnoreCase(application.getFirstName(),  eidentityDTO.getFirstName(),
                FIRST_NAME_IS_DIFFERENT_FROM_EXISTING_ONE);

        assertEqualsIgnoreCase(application.getSecondName(),  eidentityDTO.getSecondName(),
                SECOND_NAME_IS_DIFFERENT_FROM_EXISTING_ONE);
        
        assertEqualsIgnoreCase(application.getLastName(),  eidentityDTO.getLastName(),
                LAST_NAME_IS_DIFFERENT_FROM_EXISTING_ONE);

        //TODO: do we need latin names in rei
        assertEquals(application.getCitizenIdentifierNumber(),  eidentityDTO.getCitizenIdentifierNumber(),
                CITIZEN_IDENTIFIER_NUMBER_IS_DIFFERENT_FROM_EXISTING_ONE);
        
        assertEquals(application.getCitizenIdentifierType(),  eidentityDTO.getCitizenIdentifierType(),
                CITIZEN_IDENTIFIER_TYPE_IS_DIFFERENT_FROM_EXISTING_ONE);

        application.setEidentityId(eidentityDTO.getId());

        //TODO: maybe check if CitizenIdentifier is active 
        
        return application;
    }

    @Override
    public PipelineStatus getStatus() {
        return PipelineStatus.REI_EIDENTITY_VERIFICATION;
    }

    private EidentityDTO getActiveCitizenIdentifier(String number, IdentifierType type) {
    	log.info("Sending request to Get EIdentity");
    	EidentityDTO eidentityDTO = reiClient.findEidentityByNumberAndType(number, type);
    	assertNotNull(eidentityDTO, EIDENTITY_NOT_FOUND);
    	log.info("Received success response for Get EIdentity for eidentityId: {}", eidentityDTO.getId());
    	
    	return eidentityDTO;
    }
}
