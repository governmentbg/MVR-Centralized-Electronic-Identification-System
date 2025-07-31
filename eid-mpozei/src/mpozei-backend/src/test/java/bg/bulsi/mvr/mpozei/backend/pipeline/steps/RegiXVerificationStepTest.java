package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.config.security.UserContext;
import bg.bulsi.mvr.common.config.security.UserContextHolder;
import bg.bulsi.mvr.common.exception.BaseMVRException;
import bg.bulsi.mvr.mpozei.backend.BaseTest;
import bg.bulsi.mvr.mpozei.backend.Factory;
import bg.bulsi.mvr.mpozei.backend.client.PivrClient;
import bg.bulsi.mvr.mpozei.contract.dto.IdentifierType;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import bg.bulsi.mvr.mpozei.model.pivr.ForeignIdentityInfoResponseType;
import bg.bulsi.mvr.mpozei.model.pivr.PersonalIdentityInfoResponseType;
import bg.bulsi.mvr.mpozei.model.pivr.RegiXResult;
import bg.bulsi.mvr.mpozei.model.pivr.common.Nationality;
import bg.bulsi.mvr.mpozei.model.pivr.common.ReturnInformation;
import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.Test;
import org.mockito.MockedStatic;
import org.mockito.Mockito;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.mock.mockito.MockBean;

import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.UUID;

import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.*;

public class RegiXVerificationStepTest extends BaseTest {
	@MockBean
	private PivrClient pivrClient;
	
	@Autowired
	private RegiXVerificationStep regiXVerificationStep;

	@Test
	public void process_ShouldSucceed() {
		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
			userContextHolder.when(UserContextHolder::getFromServletContext)
			.thenReturn(emptyUserContext);
			
			AbstractApplication application = Factory.createAbstractApplication2();
			application.setCitizenIdentifierType(IdentifierType.EGN);
			application.getTemporaryData().setPersonalIdentityDocument(Factory.createPersonalIdentityDocument());
			
			when(pivrClient.getPersonalIdentityV2(any(), any()))
			.thenReturn(Factory.createRegiXResultWithPersonalIdentity());

			
			this.regiXVerificationStep.process(application);
		}
	}
	
	@Test
	public void process_RegixAvailabilityFalse_ShouldSkip() {
		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
			userContextHolder.when(UserContextHolder::getFromServletContext)
			.thenReturn(emptyUserContext);
			
			AbstractApplication application = new AbstractApplication();
			
			this.regiXVerificationStep.process(application);
			
			verify(pivrClient, times(0)).getPersonalIdentityV2(any(), any());
			verify(pivrClient, times(0)).getForeignIdentityV2(any(), any());
		}
	}
	
	@Test
	public void process_PivrDataNotFound_ShouldThrow() {
		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
			userContextHolder.when(UserContextHolder::getFromServletContext)
			.thenReturn(emptyUserContext);
			
			//Bulgarian Citizen
			AbstractApplication application = new AbstractApplication();
			application.setCitizenIdentifierType(IdentifierType.EGN);
			application.getTemporaryData().setPersonalIdentityDocument(Factory.createPersonalIdentityDocument());
			
			PersonalIdentityInfoResponseType personalResponseType = new PersonalIdentityInfoResponseType();
			ReturnInformation returnInformation = new ReturnInformation();
			returnInformation.setReturnCode("0100");
			returnInformation.setInfo("Citizen not found");
			personalResponseType.setReturnInformations(returnInformation);
			
			Map<String, Object> response = new HashMap<>();
			response.put("PersonalIdentityInfoResponse", personalResponseType);
			
			RegiXResult regiXResult = new RegiXResult();
			regiXResult.setError(null);
			regiXResult.setHasFailed(false);
			regiXResult.setResponse(response);
			
			when(pivrClient.getPersonalIdentityV2(any(), any()))
			.thenReturn(regiXResult);
			
			BaseMVRException thrown = Assertions.assertThrows(BaseMVRException.class, () -> {
				this.regiXVerificationStep.process(application);
			});
			
			Assertions.assertEquals("Regix could not verify request", thrown.getMessage());

			//Foreign Citizen
			application.setCitizenIdentifierType(IdentifierType.LNCh);
			ForeignIdentityInfoResponseType foreignResponseType = new ForeignIdentityInfoResponseType();
			foreignResponseType.setReturnInformations(returnInformation);
			response.put("ForeignIdentityInfoResponse", foreignResponseType);

			when(pivrClient.getForeignIdentityV2(any(), any()))
			.thenReturn(regiXResult);
			
			thrown = Assertions.assertThrows(BaseMVRException.class, () -> {
				this.regiXVerificationStep.process(application);
			});
			
			Assertions.assertEquals("Regix could not verify request", thrown.getMessage());
		}
	}
	
	
	@Test
	public void process_NationalityLatinNull_ShouldPass() {
		try (MockedStatic<UserContextHolder> userContextHolder = Mockito.mockStatic(UserContextHolder.class)) {
			userContextHolder.when(UserContextHolder::getFromServletContext)
			.thenReturn(emptyUserContext);
			
			AbstractApplication application = Factory.createAbstractApplication2();
			application.setCitizenIdentifierType(IdentifierType.EGN);
			application.getTemporaryData().setPersonalIdentityDocument(Factory.createPersonalIdentityDocument());
			
			RegiXResult regiXResult = Factory.createRegiXResultWithPersonalIdentity();
			PersonalIdentityInfoResponseType personalIdentityResponse = (PersonalIdentityInfoResponseType) regiXResult.getResponse().get("PersonalIdentityInfoResponse");
	        for(Nationality nationality: personalIdentityResponse.getNationalityList()) {
	        	nationality.setNationalityNameLatin(null);
	        }
			
			when(pivrClient.getPersonalIdentityV2(any(), any()))
			.thenReturn(regiXResult);
			
			this.regiXVerificationStep.process(application);
		}
	}
}
