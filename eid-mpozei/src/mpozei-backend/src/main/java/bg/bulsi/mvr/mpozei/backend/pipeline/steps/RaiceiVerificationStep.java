package bg.bulsi.mvr.mpozei.backend.pipeline.steps;

import org.springframework.stereotype.Component;

import bg.bulsi.mvr.common.pipeline.PipelineStatus;
import bg.bulsi.mvr.common.pipeline.Step;
import bg.bulsi.mvr.mpozei.model.application.AbstractApplication;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;

@Slf4j
@Component
@RequiredArgsConstructor
public class RaiceiVerificationStep extends Step<AbstractApplication>{
	@Override
	public AbstractApplication process(AbstractApplication application) {
        log.info("Application with id: {} entered PivrVerificationStep", application.getId());

		// TODO 26/11/2023 Implement me
		return null;
	}

	@Override
	public PipelineStatus getStatus() {
		// TODO Auto-generated method stub
		return null;
	}

}
