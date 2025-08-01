package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import bg.bulsi.mvr.common.exception.BaseMVRException;
import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.mpozei.backend.BaseTest;
import bg.bulsi.mvr.mpozei.backend.Factory;
import bg.bulsi.mvr.mpozei.backend.client.PivrClient;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import bg.bulsi.mvr.mpozei.model.pivr.CitizenDateOfDeath;
import bg.bulsi.mvr.mpozei.model.pivr.CitizenProhibition;
import bg.bulsi.mvr.mpozei.model.pivr.ProhibitionType;
import bg.bulsi.mvr.mpozei.model.repository.ApplicationRepository;
import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.mock.mockito.MockBean;

import java.time.OffsetDateTime;

import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.when;

public class PivrVerificationStepTest extends BaseTest {

	@MockBean
	private PivrClient pivrClient;
	
	@Autowired
	private PivrVerificationStep pivrVerificationStep;

	@Autowired
	private ApplicationRepository<AbstractApplication> applicationRepository;
	
	@Test
	public void checkStepStatus() {
		Assertions.assertEquals(PipelineStatus.PIVR_VERIFICATION, this.pivrVerificationStep.getStatus());
	}
	
	@Test
	public void processPivrVerificationStep_ShouldPass() {
		when(pivrClient.getDateOfDeath(any(), any())).thenReturn(new CitizenDateOfDeath(null));
		
		when(pivrClient.getDateOfProhibition(any(), any())).thenReturn(new CitizenProhibition(null, ProhibitionType.NONE, null));

		AbstractApplication application = this.pivrVerificationStep.process(Factory.createAbstractApplication());
		
		Assertions.assertFalse(application.getParams().getRequireGuardians());
	}
	
	@Test
	public void personIsDead_ShouldThrow() {
		when(pivrClient.getDateOfDeath(any(), any())).thenReturn(new CitizenDateOfDeath(OffsetDateTime.now()));
		
		BaseMVRException thrown = Assertions.assertThrows(BaseMVRException.class, () -> {
			this.pivrVerificationStep.process(Factory.createAbstractApplication());
		});
		
		Assertions.assertEquals("The citizen is dead", thrown.getMessage());
	}
	
	@Test
	public void personIsUnderProhibition_ShouldRequireGuardians() {
		when(pivrClient.getDateOfDeath(any(), any())).thenReturn(new CitizenDateOfDeath(null));
		
		when(pivrClient.getDateOfProhibition(any(), any())).thenReturn(new CitizenProhibition(null, ProhibitionType.PARTIAL, null));

		AbstractApplication application = this.pivrVerificationStep.process(Factory.createAbstractApplication());
		
		Assertions.assertTrue(application.getParams().getRequireGuardians());
	}
}
