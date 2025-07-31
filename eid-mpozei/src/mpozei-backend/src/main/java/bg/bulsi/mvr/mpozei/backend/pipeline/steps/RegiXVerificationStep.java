package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.backend.dto.RegixIdentityInfoDTO;
import bg.bulsi.mvr.mpozei.backend.service.RegixService;
import bg.bulsi.mvr.mpozei.contract.dto.PersonalIdentityDocument;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import bg.bulsi.mvr.mpozei.model.pivr.common.Nationality;
import lombok.extern.slf4j.Slf4j;
import org.apache.commons.lang3.StringUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.time.LocalDate;
import java.util.List;

import static bg.bulsi.mvr.common.exception.ErrorCode.*;
import static bg.bulsi.mvr.common.util.ValidationUtil.*;



/**
 * Gets the data of a person from RegiX(using PIVR as proxy).
 * This step is allowed to fail i.e. we continue on error.
 */
@Slf4j
@Component
public class RegiXVerificationStep extends Step<AbstractApplication> {
	@Autowired
	private RegixService regixService;
	
	private static final int LEGAL_ADULT_AGE = 18;

	@Override
    public AbstractApplication process(AbstractApplication application) {
    	Boolean regixAvailability = UserContextHolder.getFromServletContext().getRegixAvailability();
    	
        log.info("Application with id: {} entered RegiXVerificationStep, [regixAvailability={}]", application.getId(), regixAvailability);

        if(Boolean.FALSE.equals(regixAvailability)) {
        	return application;
        }

		PersonalIdentityDocument requestIdDocument = application.getTemporaryData().getPersonalIdentityDocument();
		RegixIdentityInfoDTO dto = regixService.getIdentityInfoFromRegix(application.getCitizenIdentifierNumber(), application.getCitizenIdentifierType(), requestIdDocument.getIdentityNumber());
        
        //Check the citizen Personal ID document
        this.checkPersonalIdDocument(requestIdDocument, dto);

        //Check the citizen names
        this.checkCitizenNamesLatin(application, dto);
        
        //TODO: is this check correct?
        //Check citizen nationality
        this.validateNationality(application, dto.getNationalityList());
        
        //Check if the citizen is of legal age
        this.calculateCitizenAge(application, dto.getBirthDate());
        
        return application;
    }

	private void checkPersonalIdDocument(PersonalIdentityDocument requestIdDocument, RegixIdentityInfoDTO dto) {
        
        assertEquals(dto.getPersonalIdIssueDate(),  requestIdDocument.getIdentityIssueDate(),
				IDENTITY_ISSUE_DATE_IS_DIFFERENT_FROM_EXISTING_ONE);
        
        assertEquals(dto.getPersonalIdValidityToDate(),  requestIdDocument.getIdentityValidityToDate(),
				IDENTITY_VALIDITY_TO_DATE_IS_DIFFERENT_FROM_EXISTING_ONE);

		// премахнато заради различия в начина на изписване на издателя на лични карти
//        assertTrue(StringUtils.equals(dto.getPersonalIdIssuer(), requestIdDocument.getIdentityIssuer())
//        		|| StringUtils.equals(dto.getPersonalIdIssuerLatin(), requestIdDocument.getIdentityIssuer()),
//				IDENTITY_ISSUER_IS_DIFFERENT_FROM_EXISTING_ONE);
        
        assertEquals(dto.getPersonalIdNumber(),  requestIdDocument.getIdentityNumber(),
				IDENTITY_NUMBER_IS_DIFFERENT_FROM_EXISTING_ONE);
        
        assertTrue(StringUtils.equalsIgnoreCase(dto.getPersonalIdDocumentType(), requestIdDocument.getIdentityType())
        		|| StringUtils.equalsIgnoreCase(dto.getPersonalIdDocumentTypeLatin(), requestIdDocument.getIdentityType()),
				IDENTITY_TYPE_IS_DIFFERENT_FROM_EXISTING_ONE);
	}

	private void checkCitizenNamesLatin(AbstractApplication application, RegixIdentityInfoDTO dto) {
		assertEqualsIgnoreCase(application.getFirstNameLatin(), dto.getFirstNameLatin(),
				FIRST_NAME_LATIN_IS_DIFFERENT_FROM_EXISTING_ONE);
        
		assertEqualsIgnoreCase(application.getSecondNameLatin(),  dto.getSecondNameLatin(),
				SECOND_NAME_LATIN_IS_DIFFERENT_FROM_EXISTING_ONE);
        
		assertEqualsIgnoreCase(application.getLastNameLatin(),  dto.getLastNameLatin(),
				LAST_NAME_LATIN_IS_DIFFERENT_FROM_EXISTING_ONE);
	}

	private void calculateCitizenAge(AbstractApplication application, LocalDate birthDate) {
//		OffsetDateTime currentDateTime = OffsetDateTime.now();
//        //TODO: check this calcucation later
//        Period ageDifference = Period.between(birthDate, currentDateTime.toLocalDate());
//        if(ageDifference.getYears() < LEGAL_ADULT_AGE) {
//            application.getParams().setRequireGuardians(true);
//        }
		
        application.getParams().setDateOfBirth(birthDate.toString());
	}

	private void validateNationality(AbstractApplication application, List<Nationality> nationalityList) {
		boolean matchesNationality = false;
        for(Nationality nationality: nationalityList) {
        	if((StringUtils.isNotEmpty(nationality.getNationalityNameLatin()) && nationality.getNationalityNameLatin().equalsIgnoreCase(application.getCitizenship()))
        			|| nationality.getNationalityName().equalsIgnoreCase(application.getCitizenship())) {
        		matchesNationality = true;
        		break;
        	}
        }
        
        assertTrue(matchesNationality, NATIONALITY_IS_DIFFERENT_FROM_EXISTING_ONE);
	}

    @Override
    public PipelineStatus getStatus() {
        return PipelineStatus.REGIX_VERIFICATION;
    }
}
